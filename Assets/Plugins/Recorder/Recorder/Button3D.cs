using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static objectRecordState;
using System.Text;
using System.IO;

public class Button3D : MonoBehaviour
{
    //public GameObject buttonSquare;   // 内接的白色正方形对象
    public List<GameObject> objectsToRecord = new List<GameObject>(); // 在Inspector中设置的游戏对象列表
    private Dictionary<int, List<ObjectState>> recordedStates = new Dictionary<int, List<ObjectState>>();
    private bool isRecording = false; // 录像状态
    private int frameNum = 0;

    

    private void Start()
    {
        // 初始化按钮状态
        //buttonSquare.SetActive(false); // 初始时隐藏内接正方形
        ToggleRecordingState();
    }

    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;

        //     if (Physics.Raycast(ray, out hit))
        //     {
        //         // 检查射线是否击中了指定的按钮对象
        //         if (hit.transform.gameObject == this.gameObject)
        //         {
        //             ToggleRecordingState(); // 切换录像状态
        //         }
        //     }
        // }

        // 如果正在录制，实时记录每个对象的状态
        if (isRecording)
        {
            RecordCurrentStates();
        }
    }

    void FixedUpdate(){
        frameNum++;
    }

    private void ToggleRecordingState()
    {
        frameNum = 0;
        isRecording = !isRecording; // 切换录像状态

        //buttonSquare.SetActive(isRecording); // 根据状态显示或隐藏按钮内部正方形

        if (!isRecording)
        {
            // 停止录制时保存记录
            SaveRecording();
        }
    }

    private void RecordCurrentStates()
    {
        foreach (var obj in objectsToRecord)
        {
            if (obj != null && obj.GetComponent<MeshFilter>() != null)
            {
                Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
                string meshName = mesh != null ? mesh.name : "Unknown";
                int objectId = obj.GetInstanceID(); // 获取新的 objectID

                if (!recordedStates.ContainsKey(objectId))
                {
                    recordedStates[objectId] = new List<ObjectState>();
                    Transform currentTransform = obj.transform;
                    recordedStates[objectId].Add(new ObjectState(currentTransform, mesh, frameNum, meshName, objectId)); // 记录初始状态
                }
                else
                {
                    var lastStateIndex = recordedStates[objectId].Count - 1;
                    ObjectState lastState = recordedStates[objectId][lastStateIndex];
                    Transform currentTransform = obj.transform;

                    if (HasSignificantChange(lastState, currentTransform))
                    {
                        recordedStates[objectId].Add(new ObjectState(currentTransform, mesh, frameNum, meshName, lastState.objectID));
                    }
                }
            }
        }
    }

    private void SaveRecording()
    {
        // 检查是否有数据要保存
        if (recordedStates.Count > 0)
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("Frame,ObjectID,Mesh,PositionX,PositionY,PositionZ,RotationX,RotationY,RotationZ,RotationW,ScaleX,ScaleY,ScaleZ");

            foreach (var pair in recordedStates)
            {
                List<ObjectState> states = pair.Value;
                foreach (var state in states)
                {
                    csv.AppendLine($"{state.frameNum},{state.objectID},{state.meshName},{state.position.x},{state.position.y},{state.position.z}," +
                                $"{state.rotation.x},{state.rotation.y},{state.rotation.z},{state.rotation.w}," +
                                $"{state.scale.x},{state.scale.y},{state.scale.z}");
                }
            }

            string timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            string filePath = Path.Combine(Application.dataPath, timestamp + ".csv");
            File.WriteAllText(filePath, csv.ToString());
            Debug.Log($"Recording saved to {filePath}");

            recordedStates.Clear();
        }
        else
        {
            Debug.Log("No recorded data to save.");
        }
    }


    void OnApplicationQuit()
    {
        SaveRecording();
    }




    private bool HasSignificantChange(ObjectState lastState, Transform currentTransform)
    {
        float positionThreshold = 0.001f; // 位置变化的阈值
        float rotationThreshold = 0.001f; // 旋转变化的阈值
        float scaleThreshold = 0.001f;    // 缩放变化的阈值

        bool positionChanged = Vector3.Distance(lastState.position, currentTransform.position) > positionThreshold;
        bool rotationChanged = Quaternion.Angle(lastState.rotation, currentTransform.rotation) > rotationThreshold;
        bool scaleChanged = Vector3.Distance(lastState.scale, currentTransform.localScale) > scaleThreshold;

        return positionChanged || rotationChanged || scaleChanged;
    }

    //以下是为了SprHandURP改的

    // public void ReplaceRecordedObject(GameObject oldObject, GameObject newObject)
    // {
    //     // 更新 objectsToRecord 列表
    //     int index = objectsToRecord.IndexOf(oldObject);
    //     if (index != -1)
    //     {
    //         objectsToRecord[index] = newObject;
    //     }

    //     // 更新 recordedStates 字典
    //     if (recordedStates.ContainsKey(oldObject))
    //     {
    //         // 获取旧GameObject对应的List<ObjectState>
    //         List<ObjectState> states = recordedStates[oldObject];

    //         // 移除旧的键值对
    //         recordedStates.Remove(oldObject);

    //         // 添加新的键值对，这里不需要检查是否已存在，因为oldObject已被移除
    //         recordedStates[newObject] = states;
    //     }
    // }


    private static int nextID = 0;

    public static int GetNextID() //不用系统自带的ID了，不然删除没法继承
    {
        return nextID++;
    }

}