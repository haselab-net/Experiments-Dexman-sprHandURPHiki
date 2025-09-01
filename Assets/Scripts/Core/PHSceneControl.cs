using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SprCs;
using System.IO;
//Control or get some parameters in PHScene Springhead
//Must attached with PHsceneBehavior
public class PHSceneControl : MonoBehaviour
{
    PHSceneIf phScene;
    public bool isContactForceOn = false;
    public static bool isPressureOn = true, isVibrationOn = true, isTengentOn = true, isLocalDirectionFeedabck = true;
    // Add separate control for X, Y, Z tangent force components
    public static bool isTengentXOn = true, isTengentYOn = true, isTengentZOn = true;
    public static bool isCollisionOn = true;
    public bool publicisPressureOn = true, publicisVibrationOn = true, publicisTengentOn = true, publicisLocalDirectionFeedabck = true; //localFeedback 指给手指全局反馈还是局部反馈
    // Add corresponding public variables for inspector control
    public bool publicisTengentXOn = true, publicisTengentYOn = true, publicisTengentZOn = true;
    public bool publicisCollisionOn = true;
    public GameObject debugPoint;
    public PHSolidBehaviour indexBone, thumbBone;

    public DrawDataLine drawLine;
    private AudioSource[] audioSources;

    public static rbContactValue indexContact, thumbContact;
    // Start is called before the first frame update
    void Start()
    {
        indexContact = new rbContactValue();
        thumbContact = new rbContactValue();
        phScene = this.GetComponent<PHSceneBehaviour>().phScene;
        audioSources = GetComponents<AudioSource>();
        indexContact.resetValues();
        thumbContact.resetValues();
        
    }
    
    List<double> channel1Data = new List<double>();
    List<double> channel2Data = new List<double>();
    string filePath = "Assets/data.csv";
    // Update is called once per frame
    void Update(){
        isPressureOn = publicisPressureOn;
        isVibrationOn = publicisVibrationOn;
        isTengentOn = publicisTengentOn;
        isLocalDirectionFeedabck = publicisLocalDirectionFeedabck;
        // Sync the new public and static variables
        isTengentXOn = publicisTengentXOn;
        isTengentYOn = publicisTengentYOn;
        isTengentZOn = publicisTengentZOn;
        isCollisionOn = publicisCollisionOn;
    }
    void FixedUpdate()
    {   
        indexContact.resetValues();
        thumbContact.resetValues();
        Vec3d f = new Vec3d();
        Vec3d to = new Vec3d();
        Posed pose = new Posed();
        Vec3d fsum = new Vec3d();
        f = new Vec3d(0, 0, 0);
        to = new Vec3d(0, 0, 0);

        float relativeLinearSpeed = 0;
        for (int i = 0; i < phScene.NContacts(); i++){
                
                PHContactPointIf contact = phScene.GetContact(i);
                
                if(contact.GetSocketSolid() == indexBone.phSolid || contact.GetPlugSolid() == indexBone.phSolid){
                    contact.GetRelativeVelocity(f, to);
                    relativeLinearSpeed = (float)(f.x + f.y + f.z);
                    indexContact.relativeAngularVelocity = (float)(to.x + to.y + to.z);
                    indexContact.relativeLinearVelocity = relativeLinearSpeed;
                    indexContact.isStaticFriction = contact.IsStaticFriction();
                    
                    contact.GetConstraintForce(f, to);
                    if(contact.GetSocketSolid() == indexBone.phSolid){
                        indexContact.contactName = contact.GetPlugSolid().GetName();
                        
                        fsum -= pose.Ori() * f;
                        //tsum -= pose.Pos() % pose.Ori() * f;//扭矩
                    }
                    if(contact.GetPlugSolid() == indexBone.phSolid){
                        indexContact.contactName = contact.GetSocketSolid().GetName();
                        contact.GetPlugPose(pose);
                        fsum += pose.Ori() * f;
                        //tsum += pose.Pos() % pose.Ori() * f;
                    }
                    indexContact.sumForce = (Mathf.Abs((float)fsum.x) + Mathf.Abs((float)fsum.y) + Mathf.Abs((float)fsum.z));

                }
                else if(contact.GetSocketSolid() == thumbBone.phSolid || contact.GetPlugSolid() == thumbBone.phSolid){
                    contact.GetRelativeVelocity(f, to);
                    relativeLinearSpeed = (float)(f.x + f.y + f.z);
                    thumbContact.relativeLinearVelocity = (float)(f.x + f.y + f.z);
                    thumbContact.relativeAngularVelocity = (float)(to.x + to.y + to.z);
                    thumbContact.isStaticFriction = contact.IsStaticFriction();


                    contact.GetConstraintForce(f, to);
                    if(contact.GetSocketSolid() == thumbBone.phSolid){
                        thumbContact.contactName = contact.GetPlugSolid().GetName();
                        contact.GetSocketPose(pose);
                        fsum -= pose.Ori() * f;
                        //tsum -= pose.Pos() % pose.Ori() * f;//扭矩
                    }
                    if(contact.GetPlugSolid() == thumbBone.phSolid){
                        thumbContact.contactName = contact.GetSocketSolid().GetName();
                        contact.GetPlugPose(pose);
                        fsum += pose.Ori() * f;
                        //tsum += pose.Pos() % pose.Ori() * f;
                    }
                    thumbContact.sumForce = (Mathf.Abs((float)fsum.x) + Mathf.Abs((float)fsum.y) + Mathf.Abs((float)fsum.z));
                }
                
                
        }
        if(drawLine != null){
                    
               drawLine.waitingVector = new Vector3(indexContact.isStaticFriction ? 5 : 0, 0, 0); //画是否是静摩擦
        }

        if(isContactForceOn == true){
            for (int i = 0; i < phScene.NContacts(); i++){
                PHContactPointIf contact = phScene.GetContact(i);
                
                GameObject oneCube = Instantiate(debugPoint);
                oneCube.transform.position = new Vector3((float)contact.GetSocketSolid().GetPose().px, (float)contact.GetSocketSolid().GetPose().py, (float)contact.GetSocketSolid().GetPose().pz);
                Destroy(oneCube, 0.05f);

                GameObject oneCube1 = Instantiate(debugPoint);
                oneCube1.transform.position = new Vector3((float)contact.GetPlugSolid().GetPose().px, (float)contact.GetPlugSolid().GetPose().py, (float)contact.GetPlugSolid().GetPose().pz);
                Destroy(oneCube1, 0.05f);

            }
        }
        
    }

    public class rbContactValue{
        public float relativeLinearVelocity = 0;
        public float relativeAngularVelocity = 0;
        public Vec3d fsum;
        public Posed pose;
        public float sumForce = 0;
        public float secondSumForce = 0;
        public string contactName = "";
        public bool isStaticFriction = false;

        public Quaternion contactRotation;

        public void resetValues(){
            relativeLinearVelocity = 0;
            relativeAngularVelocity = 0;
            sumForce = 0;
            contactName = "";
            isStaticFriction = false;
            contactRotation = new Quaternion(0, 0, 0, 0);
        }
    }
}

                    // using (StreamWriter streamWriter = new StreamWriter(filePath, true))
                    // {
                    //     hasAim = 1;
                    //     streamWriter.WriteLine(string.Format("{0},{1}", f.x + f.y + f.z, to.x + to.y + to.z));
                    // }