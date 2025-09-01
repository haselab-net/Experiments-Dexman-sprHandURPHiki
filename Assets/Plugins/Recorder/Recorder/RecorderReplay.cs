using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;

public class RecorderReplay : MonoBehaviour
{
    private Dictionary<int, GameObject> objectMap = new Dictionary<int, GameObject>();
    private List<List<string>> csvData = new List<List<string>>();
    private int currentFrame = 0;
    private bool isReplaying = false;
    public Slider progressSlider; // 指向进度条滑块的引用
    public Text timeText; // 指向显示时间的文本元素的引用

    public GameObject fileListPanel; // 指向你的Panel的引用
    public GameObject fileButtonPrefab; // 预制件按钮的引用
    public Button playPauseButton; // 播放/暂停按钮引用
    private bool isPaused = false; // 暂停状态标志
    void Start()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*.csv");
        foreach (string file in files)
        {
            GameObject button = Instantiate(fileButtonPrefab, fileListPanel.transform);
            button.GetComponentInChildren<Text>().text = Path.GetFileName(file);
            button.GetComponent<Button>().onClick.AddListener(() => ReadCsvFile(file));
        }

        
        
    }

    void FixedUpdate()
    {
        if (isReplaying && !isPaused && currentFrame < csvData.Count)
        {
            UpdateObjectsToFrame(currentFrame);
            if (progressSlider != null)
            {
                progressSlider.value = currentFrame;
                UpdateTimeText(currentFrame);
            }
            currentFrame++;
        }
    }

    public void StartReplay()
    {
        isReplaying = true;
        currentFrame = 0;
    }

    private void UpdateObjectsToFrame(int frame)
    {
        var frameData = csvData[frame];

        foreach (var line in frameData)
        {
            var data = line.Split(',');
            int objectID = int.Parse(data[1]);
            string meshType = data[2];

            if (!objectMap.ContainsKey(objectID))
            {
                Debug.Log("Creating new object for ID: " + objectID + " with mesh type: " + meshType);
                GameObject obj = CreateObjectByMeshType(meshType);
                if (obj != null)
                {
                    objectMap[objectID] = obj;
                }
                else
                {
                    Debug.LogError("Failed to create object for mesh type: " + meshType);
                    continue;
                }
            }

            GameObject objectToUpdate = objectMap[objectID];
            Vector3 position = new Vector3(float.Parse(data[3]), float.Parse(data[4]), float.Parse(data[5]));
            Quaternion rotation = new Quaternion(float.Parse(data[6]), float.Parse(data[7]), float.Parse(data[8]), float.Parse(data[9]));
            Vector3 scale = new Vector3(float.Parse(data[10]), float.Parse(data[11]), float.Parse(data[12]));

            objectToUpdate.transform.position = position;
            objectToUpdate.transform.rotation = rotation;
            objectToUpdate.transform.localScale = scale;
        }
    }


    private GameObject CreateObjectByMeshType(string meshType)
    {
        GameObject obj;

        // 移除 " Instance" 后缀，如果有的话
        string cleanMeshType = meshType.Replace(" Instance", "");

        switch (cleanMeshType)
        {
            case "Cube":
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;
            case "Sphere":
                obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            case "Capsule":
                obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                break;
            case "Cylinder":
                obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                break;
            case "Plane":
                obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
                break;
            case "Quad":
                obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                break;
            case "Combined Mesh (root: scene) 2": //special case for SprHandURP
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;
            // 这里可以继续添加其他Unity支持的原始体
            default:
                Debug.LogError("Unknown mesh type: " + cleanMeshType);
                return null;
        }

        // 返回创建的对象
        return obj;
    }



    private void ReadCsvFile(string filePath)
    {

        fileListPanel.SetActive(false);
        print(filePath);
        string[] lines = File.ReadAllLines(filePath);

        // 创建一个临时列表来存储所有行的数据
        List<(int frame, string line)> tempData = new List<(int frame, string line)>();

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            string line = lines[i];
            int frame = int.Parse(line.Split(',')[0]); // 获取每行的Frame值
            tempData.Add((frame, line));
        }

        // 按Frame值对所有数据进行排序
        var sortedData = tempData.OrderBy(data => data.frame).ToList();

        // 重组csvData，以按Frame顺序存储数据
        csvData.Clear();
        foreach (var data in sortedData)
        {
            // 确保csvData有足够的元素
            while (csvData.Count <= data.frame)
            {
                csvData.Add(new List<string>());
            }
            csvData[data.frame].Add(data.line);
        }

        StartReplay();

        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = csvData.Count;
            progressSlider.onValueChanged.AddListener(HandleSliderValueChanged);
        }
        playPauseButton.onClick.AddListener(TogglePlayPause);
    }

    private void HandleSliderValueChanged(float value)
    {
        int newFrame = (int)value;

        if (currentFrame != newFrame)
        {
            currentFrame = newFrame;
            UpdateObjectsToFrame(currentFrame);
            UpdateTimeText(currentFrame);
        }
    }


    private void UpdateTimeText(int frame)
    {
        // 将帧数转换为实际时间（秒）
        float timeInSeconds = frame * Time.fixedDeltaTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);

        // 格式化时间字符串
        string timeString = string.Format("{0:D2}:{1:D2}:{2:D3}",
            timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);

        timeText.text = "Time: " + timeString;
    }

    private void TogglePlayPause()
    {
        isPaused = !isPaused;
        playPauseButton.GetComponentInChildren<Text>().text = isPaused ? "Play" : "Pause";
    }



}
