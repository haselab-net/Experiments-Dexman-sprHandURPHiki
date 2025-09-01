using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectRecordState : MonoBehaviour
{
    [System.Serializable]
    public class ObjectState
    {
        public int frameNum;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public string meshName;
        public int objectID;

        public ObjectState(Transform transform, Mesh mesh, int frameNum, string meshName, int objectID)
        {
            this.frameNum = frameNum;
            this.position = transform.position;
            this.rotation = transform.rotation;
            this.scale = transform.localScale;
            this.meshName = meshName;
            this.objectID = objectID;
        }
    }

}
