using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SprCs;
using SprUnity;

[DefaultExecutionOrder(0)]
public class FWHapticTest : MonoBehaviour {
    // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
    // メンバ変数

    public static FWHapticApp app = null;

    public PHSceneBehaviour phSceneBehaviour;

    // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
    // Privateメンバ

    // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
    // MonoBehaviourのメソッド

    void Start () {
        app = new FWHapticApp();

        app.CreateSdk();
        app.GetSdk().CreateScene();
        app.SetPHScene(phSceneBehaviour.phScene);

        app.CreateTimers();

        //

        // phSceneBehaviour.enableStep = false;



        // 

        app.GetTimer(0).SetInterval(10);
        app.GetTimer(1).SetResolution(1);
        app.GetTimer(1).SetInterval(1);
        app.StartTimers();
        
    }

    void FixedUpdate() {
        app.GetSdk().GetScene(0).GetPHScene().GetHapticEngine().StepPhysicsSimulation();
        app.TimerFunc(1);
    }
}
