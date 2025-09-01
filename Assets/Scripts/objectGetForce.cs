using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SprCs;

//物体用于获取自身受到的力，这里还会刨除手指施加的，用于给手指施加捏着的物体碰撞的力
public class objectGetForce : MonoBehaviour
{
    //如果是组合的物体，一个组合放一个这个脚本就可以，然后public获取到每个子物体的PHsolid把结果加在一起就可以
    // Start is called before the first frame update
    public PHSceneControl.rbContactValue myContact;
    PHSolidBehaviour mySolid;
    PHSceneIf phScene;
    void Start()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        mySolid = this.GetComponent<PHSolidBehaviour>();

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
         
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetSumForceExceptFinger(mySolid.phSolid, myContact);
        if ( DetectForce0ToNon0(myContact.sumForce))
        {

        }
    }

    public PHSceneControl.rbContactValue GetSumForceExceptFinger(PHSolidIf phsolid, PHSceneControl.rbContactValue myContact){ //去掉跟手指尖有关系的力
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
                   if(contact.GetSocketSolid().GetName().Contains("fingerCapsule") || contact.GetPlugSolid().GetName().Contains("fingerCapsule")){ //if the other contact is the finger
                        continue;
                        //把碰到的手指编号拉清单， 然后后面遍历清单 按名字搜索对应的capsule 直接发送haptic--算了 不行
                    }
                    // contact.GetRelativeVelocity(f, to);
                    // relativeLinearSpeed = (float)(f.x + f.y + f.z);
                    // myContact.relativeAngularVelocity = (float)(to.x + to.y + to.z);
                    // myContact.relativeLinearVelocity = relativeLinearSpeed;
                    // myContact.isStaticFriction = contact.IsStaticFriction();
                    
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
        //print(myContact.sumForce);
        return myContact;
    }
    
    private float lastFrameForce = 0f;
    bool DetectForce0ToNon0(float currentForce)
    {
        bool hasChanged = false;

        if (lastFrameForce == 0 && currentForce > 0)
        {
            hasChanged = true;
            //print(currentForce);
        }

        lastFrameForce = currentForce;
        return hasChanged;
    }
}
