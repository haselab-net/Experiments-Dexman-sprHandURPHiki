using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//GetValueFromPHSceneControl, and UseVibrationControl send waveform to Audio
public class GenerateAudioSignal : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource[] As;
    VibrationControl VC;
    void Start()
    {
        VC = this.GetComponent<VibrationControl>();
        As = gameObject.GetComponents<AudioSource>();
        // clip1 = AudioClip.Create("NewClip", (int)(44100 * 0.1f), 1, 44100, false);
        // As = this.GetComponent<AudioSource>();   
        // audioData = new float[(int)(44100 * 0.1f)];
        // for (int i = 0; i < audioData.Length; i++)
        // {
        //     audioData[i] = Mathf.Sin(i * 2 * Mathf.PI * 440 / 44100);//2pi f t
        // }
        // clip1.SetData(audioData, 0);
        // As.clip = clip1;
        // As.Play();
        // As.loop = true;
    }

    public float indexSumforce, thumbSumforce;
    private float previousIndexSumForce,  previousThumbSumForce;
    private bool previousIndexIsStaticFriction,  previousThumbIsStaticFriction;
    void Update()
    {

        indexSumforce = PHSceneControl.indexContact.sumForce;
        if (indexSumforce != 0 && previousIndexSumForce == 0) {
            
            var mappedValue = 1.5f * Mathf.InverseLerp(10, 3500, Mathf.Abs(indexSumforce)) ;
            float[] result;
            if(PHSceneControl.indexContact.contactName.Contains("glass"))//如果对方名字里有材料名字
                result =  VC.GenerateCollideWaveform(7, 160, mappedValue, 750, 1721);//Glass
            else if(PHSceneControl.indexContact.contactName.Contains("steel"))
                result =  VC.GenerateCollideWaveform(29, 160, mappedValue,1692, 1682);//steel
            else
                result =  VC.GenerateCollideWaveform(0.14f, 100, mappedValue,309, 67);//wood
            //var result =  VC.GenerateCollideWaveform(7, 20, mappedValue, 750, 1721);//Glass
            //var result =  VC.GenerateCollideWaveform(0.14f, 100, mappedValue,309, 67);//wood
            //var result =  VC.GenerateCollideWaveform(29, 50, mappedValue, 1692, 1682);//Steel
            VC.PlayCollide(result, As[0]);
        }
        previousIndexSumForce = indexSumforce;
        
        if(PHSceneControl.indexContact.isStaticFriction == false && previousIndexIsStaticFriction == true){//如果是从1 静摩擦跳变到0 for stick slip
            if(PHSceneControl.indexContact.sumForce > 0)
                //VC.PlayCollide(VC.GenerateCollideWaveform(7, 160, PHSceneControl.indexContact.relativeLinearVelocity, 750, 1721), As[0]);
                VC.PlayCollide(VC.GenerateStickSlip(0.8f, 0.6f, 0.0016f, 1000, 1, 0.0005f * PHSceneControl.indexContact.sumForce, PHSceneControl.indexContact.relativeLinearVelocity * 0.0001f, 0.018f), As[0]);
        }
        previousIndexIsStaticFriction = PHSceneControl.indexContact.isStaticFriction;

        if(PHSceneControl.thumbContact.isStaticFriction == false && previousThumbIsStaticFriction == true){//如果是从1 静摩擦跳变到0
            if(PHSceneControl.thumbContact.sumForce > 0)
                //VC.PlayCollide(VC.GenerateCollideWaveform(7, 160, PHSceneControl.indexContact.relativeLinearVelocity, 750, 1721), As[0]);
                VC.PlayCollide(VC.GenerateStickSlip(0.8f, 0.6f, 0.0016f, 1000, 1, 0.0005f * PHSceneControl.thumbContact.sumForce, PHSceneControl.indexContact.relativeLinearVelocity * 0.0001f, 0.018f), As[1]);
        }
        previousThumbIsStaticFriction = PHSceneControl.thumbContact.isStaticFriction;

        float[] indexSliding;//简单滑动摩擦
        indexSliding = VC.GenerateSineWave(240, 0.02f * PHSceneControl.indexContact.relativeLinearVelocity * PHSceneControl.indexContact.relativeAngularVelocity + 0.03f * PHSceneControl.indexContact.relativeLinearVelocity);
        VC.PlayTexture(indexSliding, As[2]);



        float[] thumbSliding;//简单滑动摩擦
        thumbSliding = VC.GenerateSineWave(240, 0.02f * PHSceneControl.thumbContact.relativeLinearVelocity * PHSceneControl.thumbContact.relativeAngularVelocity + 0.03f * PHSceneControl.thumbContact.relativeLinearVelocity);
        VC.PlayTexture(thumbSliding, As[3]);



        thumbSumforce = PHSceneControl.thumbContact.sumForce;
        if (thumbSumforce != 0 && previousThumbSumForce == 0) {
            var mappedValue = 1.5f *Mathf.InverseLerp(10, 3500, Mathf.Abs(thumbSumforce));
            float[] result;
            if(PHSceneControl.thumbContact.contactName.Contains("glass"))//如果对方名字里有材料名字
                result =  VC.GenerateCollideWaveform(7, 200, mappedValue, 750, 1721);//Glass
            else if(PHSceneControl.thumbContact.contactName.Contains("steel"))
                result =  VC.GenerateCollideWaveform(29, 200, mappedValue,1692, 1682);//steel
            else
                result =  VC.GenerateCollideWaveform(0.14f, 120, mappedValue,309, 67);//wood
            VC.PlayCollide(result, As[1]);
        }
        previousThumbSumForce = thumbSumforce;
        
    }
}
