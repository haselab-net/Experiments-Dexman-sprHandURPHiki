using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SprCs;

public class fragileGlass : MonoBehaviour
{
    PHSolidBehaviour mySolid;
    PHSceneIf phScene;
    PHSceneControl.rbContactValue myContact;
    Vector3 initPosition;
    Quaterniond initRotation;

    public float n = 50f;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach(GameObject obj in allObjects)
        {

            PHSceneBehaviour phb = obj.GetComponent<PHSceneBehaviour>();
            if(phb != null)
            {
                phScene = phb.phScene;
            }
        }

        initRotation = new Quaterniond();
        initPosition = this.transform.position;
        initRotation.x = this.transform.rotation.x;
        initRotation.y = this.transform.rotation.y;
        initRotation.z = this.transform.rotation.z;
        initRotation.w = this.transform.rotation.w;

        mySolid = this.GetComponent<PHSolidBehaviour>();
        myContact = new PHSceneControl.rbContactValue();
        myContact.resetValues();
    }

    // Update is called once per frame
    void Update()
    {
         GetSumForce(mySolid.phSolid, myContact);
         if(myContact.sumForce > n)
            mySolid.UpdatePosition(initPosition, initRotation);
         //print(myContact.sumForce);
    }

    public PHSceneControl.rbContactValue GetSumForce(PHSolidIf phsolid, PHSceneControl.rbContactValue myContact){
        myContact.resetValues();
        Vec3d f = new Vec3d(0, 0, 0);
        Vec3d to = new Vec3d(0, 0, 0);
        Posed pose = new Posed();
        Vec3d fsum = new Vec3d(0, 0, 0);

        float relativeLinearSpeed = 0, secondObjectRelativeLinearSpeed = 0;
        for (int i = 0; i < phScene.NContacts(); i++){
                
                PHContactPointIf contact = phScene.GetContact(i);
                contact.GetRelativeVelocity(f, to);
                if(contact.GetSocketSolid() == phsolid || contact.GetPlugSolid() == phsolid){ //if one of the contact is the input solid

                    string secondObjectnameToFind = ""; //get secoond object haptics
                    if(contact.GetSocketSolid().GetName().Contains("Aim"))
                        secondObjectnameToFind = contact.GetSocketSolid().GetName().Substring(3);
                    else if(contact.GetPlugSolid().GetName().Contains("Aim"))
                        secondObjectnameToFind = contact.GetPlugSolid().GetName().Substring(3);
                    GameObject foundObject = GameObject.Find(secondObjectnameToFind);
                    if(foundObject != null){ //handle the second object 
                        secondObjectRelativeLinearSpeed = foundObject.GetComponent<AimObjectGetHaptic>().myContact.relativeLinearVelocity;
                        myContact.secondSumForce = foundObject.GetComponent<AimObjectGetHaptic>().myContact.sumForce;
                        //print(myContact.secondSumForce);
                    }
                        

                    //print(contact.GetSocketSolid().PHSolidBehaviour.gameObject.Name); // .GetComponent<objectGetForce>().myContact.sumForce
                    // contact.GetRelativeVelocity(f, to); //获取不到对应的gameObject, 作废
                    relativeLinearSpeed = (float)(f.x + f.y + f.z);
                    myContact.relativeAngularVelocity = (float)(to.x + to.y + to.z);
                    myContact.relativeLinearVelocity = relativeLinearSpeed;// + secondObjectRelativeLinearSpeed * 1; //第二个物体的速度也加上去
                    myContact.isStaticFriction = contact.IsStaticFriction();
                    
                    contact.GetConstraintForce(f, to);
                    if(contact.GetSocketSolid() == phsolid){
                        myContact.contactName = contact.GetPlugSolid().GetName();
                        
                        fsum -= pose.Ori() * f;
                        //tsum -= pose.Pos() % pose.Ori() * f;//扭矩
                    }
                    if(contact.GetPlugSolid() == phsolid){
                        myContact.contactName = contact.GetSocketSolid().GetName();
                        contact.GetPlugPose(pose);
                        fsum += pose.Ori() * f;
                        //tsum += pose.Pos() % pose.Ori() * f;
                    }
                    

                }
                
                
        }
        myContact.sumForce = (Mathf.Abs((float)fsum.x) + Mathf.Abs((float)fsum.y) + Mathf.Abs((float)fsum.z));
        return myContact;
    }
}
