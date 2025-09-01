using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SprCs;
//为了碰地面触觉
public class AimObjectGetHaptic : MonoBehaviour
{
    // Start is called before the first frame update
    PHSolidBehaviour mySolid;
    public PHSceneControl.rbContactValue myContact;
    PHSceneIf phScene;

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

        myContact = new PHSceneControl.rbContactValue();
        myContact.resetValues();
        mySolid = this.GetComponent<PHSolidBehaviour>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetSumForce(mySolid.phSolid, myContact);
    }

    public PHSceneControl.rbContactValue GetSumForce(PHSolidIf phsolid, PHSceneControl.rbContactValue myContact){
        myContact.resetValues();
        Vec3d f = new Vec3d(0, 0, 0);
        Vec3d to = new Vec3d(0, 0, 0);
        Posed pose = new Posed();
        Vec3d fsum = new Vec3d(0, 0, 0);

        float relativeLinearSpeed = 0;
        for (int i = 0; i < phScene.NContacts(); i++){
                
                PHContactPointIf contact = phScene.GetContact(i);
                contact.GetRelativeVelocity(f, to);
                if(contact.GetSocketSolid() == phsolid || contact.GetPlugSolid() == phsolid){ //if one of the contact is the input solid

                    PHSolidIf secondObjectnameToFind; //get secoond object haptics
                    if(contact.GetSocketSolid().GetName().Contains("Floor"))
                        phsolid = contact.GetSocketSolid();
                    else if(contact.GetPlugSolid().GetName().Contains("Floor"))
                        phsolid = contact.GetPlugSolid();
                    else
                        continue;
                    
                    relativeLinearSpeed = (float)(f.x + f.y + f.z);
                    myContact.relativeAngularVelocity = (float)(to.x + to.y + to.z);
                    myContact.relativeLinearVelocity = relativeLinearSpeed;
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


