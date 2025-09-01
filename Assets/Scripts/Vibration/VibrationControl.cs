using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Generate Waveform and play
public class VibrationControl : MonoBehaviour
{
    public int sampleRate = 50000;
    public float collisionAudioDuration = 0.05f;
    private float[] collisionAudioData;

    public float[] GenerateCollideWaveform(float L, float D, float v, float B, float freq)
    {
        var dataNum = (int)(sampleRate * collisionAudioDuration);
        float[] waveform = new float[dataNum];
        for (int i = 0; i < dataNum; i++)
        {
            float t = (float)i / sampleRate;
            waveform[i] = L * D * v * Mathf.Exp(-B * t) * Mathf.Sin(2 * Mathf.PI * freq * t) * Mathf.Sin(2 * Mathf.PI * 240 * t);
            
        }
        return waveform;
    }

    public float[] GenerateStickSlip(float miuS, float miuK, float m, float k, float c, float W, float V, float totalScale)//其实也就是个碰撞 一样的
    {
        var dataNum = (int)(sampleRate * collisionAudioDuration);
        float[] waveform = new float[dataNum];
        for (int i = 0; i < dataNum; i++)
        {
            float t = (float)i / sampleRate;

            float deltaMiu = miuS - miuK;
            float p = Mathf.Sqrt(k / m);
            float ksi = c / 2 * Mathf.Sqrt(m * k);
            float omega = p * Mathf.Sqrt(1 - ksi * ksi);
            float C1 = deltaMiu * W / k;
            float C2 = c * miuS * W / 2 * omega * m + V / omega;

            waveform[i] = totalScale * Mathf.Exp(-p*ksi*t) * ((-omega * C1 - p*ksi * C2) * Mathf.Sin(omega * t) + (omega * C2 - p * ksi * C1) * Mathf.Cos(omega * t) + p * ksi * miuK * W / k);
            
        }
        return waveform;
    }

    public float[] GenerateSineWave(float freq, float Amplitute)
    {
        var dataNum = (int)(sampleRate * collisionAudioDuration);
        float[] waveform = new float[dataNum];
        for (int i = 0; i < dataNum; i++)
        {
            float t = (float)i / sampleRate;
            waveform[i] = Amplitute * Mathf.Sin(2 * Mathf.PI * freq* t);
            
        }
        return waveform;
    }

    public void PlayCollide(float[] waveform, AudioSource audioSource)
    {
        // audioSource.clip = AudioClip.Create("Collision Sound", waveform.Length, 1, sampleRate, false);
        // audioSource.clip.SetData(waveform, 0);
        AudioClip clip = AudioClip.Create("Collision Sound", waveform.Length, 1, sampleRate, false); 
        clip.SetData(waveform, 0);
        audioSource.PlayOneShot(clip);
        // Destroy(audioSource.clip, collisionAudioDuration);
    }

    public void PlayTexture(float[] waveform, AudioSource audioSource)
    {
        audioSource.clip = AudioClip.Create("Collision Sound", waveform.Length, 1, sampleRate, false);
        audioSource.clip.SetData(waveform, 0);
        audioSource.Play();
        Destroy(audioSource.clip, collisionAudioDuration);
    }


}
