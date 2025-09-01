using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FrictionController : MonoBehaviour
{
    [System.Serializable]
    public struct KeyObjectPair
    {
        public KeyCode key;
        public GameObject targetObject;
        [HideInInspector]
        public Vector3 initialPosition;
        [HideInInspector]
        public Color color;
    }

    [Tooltip("Define the GameObjects to control with number keys 1-9")]
    public List<KeyObjectPair> keyObjectMap = new List<KeyObjectPair>()
    {
        new KeyObjectPair { key = KeyCode.Alpha1, targetObject = null },
        new KeyObjectPair { key = KeyCode.Alpha2, targetObject = null },
        new KeyObjectPair { key = KeyCode.Alpha3, targetObject = null },
        new KeyObjectPair { key = KeyCode.Alpha4, targetObject = null },
        new KeyObjectPair { key = KeyCode.Alpha5, targetObject = null },
        new KeyObjectPair { key = KeyCode.Alpha6, targetObject = null },
        new KeyObjectPair { key = KeyCode.Alpha7, targetObject = null },
        new KeyObjectPair { key = KeyCode.Alpha8, targetObject = null },
        new KeyObjectPair { key = KeyCode.Alpha9, targetObject = null }
    };

    [Header("Quiz Settings")]
    [Tooltip("Enable quiz mode for ranking objects by friction")]
    public bool quizMode = false;
    
    [System.Serializable]
    public class FrictionRanking
    {
        public KeyCode key;
        public string rankingLabel; // "High Friction", "Medium Friction", "Low Friction"
        [HideInInspector]
        public int userGuess; // 1, 2, or 3 - corresponding to the keys
    }
    
    public List<FrictionRanking> frictionRankings = new List<FrictionRanking>()
    {
        new FrictionRanking { key = KeyCode.Alpha1, rankingLabel = "High Friction" },
        new FrictionRanking { key = KeyCode.Alpha2, rankingLabel = "Medium Friction" },
        new FrictionRanking { key = KeyCode.Alpha3, rankingLabel = "Low Friction" }
    };
    
    [Tooltip("Directory to save quiz results")]
    public string saveDirectory = "QuizResults";
    
    [Tooltip("Reference to PHSceneControl to record haptic feedback settings")]
    public PHSceneControl phSceneControl;

    // Position to move inactive objects
    private readonly Vector3 inactivePosition = new Vector3(10f, 10f, 10f);
    
    // The currently active object index
    private int activeIndex = -1;
    
    // Material to display the color (optional, can be null)
    public Renderer colorRenderer;
    
    // For quiz results
    private bool resultsSaved = false;

    void Start()
    {
        // Store all game objects that are not null
        List<GameObject> validObjects = new List<GameObject>();
        foreach (var pair in keyObjectMap)
        {
            if (pair.targetObject != null)
            {
                validObjects.Add(pair.targetObject);
            }
        }

        // Check if we have any valid objects
        if (validObjects.Count == 0)
        {
            Debug.LogWarning("No GameObjects assigned to the FrictionController.");
            return;
        }

        // Store all used keys
        List<KeyCode> keys = keyObjectMap.Select(p => p.key).ToList();

        // Clear the current keyObjectMap
        keyObjectMap.Clear();

        // Shuffle the objects
        for (int i = 0; i < validObjects.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, validObjects.Count);
            GameObject temp = validObjects[i];
            validObjects[i] = validObjects[randomIndex];
            validObjects[randomIndex] = temp;
        }

        // Reassign objects to keys randomly
        for (int i = 0; i < Mathf.Min(validObjects.Count, keys.Count); i++)
        {
            KeyObjectPair newPair = new KeyObjectPair
            {
                key = keys[i],
                targetObject = validObjects[i],
                initialPosition = validObjects[i].transform.position,
                color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value)
            };

            // Apply color to the object if it has a renderer
            Renderer renderer = newPair.targetObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = newPair.color;
            }

            // Move to inactive position
            newPair.targetObject.transform.position = inactivePosition;

            // Add to map
            keyObjectMap.Add(newPair);
            
            Debug.Log($"Randomly assigned key {newPair.key} to control {newPair.targetObject.name} with color {newPair.color}");
        }
        
        // Create save directory if it doesn't exist
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }
    }

    void Update()
    {
        // Reset random objects when T key is pressed instead of reloading the scene
        if (Input.GetKeyDown(KeyCode.T))
        {
            ResetRandomization();
            return;
        }

        for (int i = 0; i < keyObjectMap.Count; i++)
        {
            var pair = keyObjectMap[i];
            if (Input.GetKeyDown(pair.key) && pair.targetObject != null)
            {
                // Move all objects to inactive position
                MoveAllToInactivePosition();
                
                // Move only the selected object to its initial position
                pair.targetObject.transform.position = pair.initialPosition;
                activeIndex = i;
                
                // Apply the color if a global renderer is assigned
                if (colorRenderer != null)
                {
                    colorRenderer.material.color = pair.color;
                }
                
                Debug.Log($"Moved {pair.targetObject.name} to initial position using key {pair.key} with color {pair.color}");
            }
        }
    }

    private void MoveAllToInactivePosition()
    {
        foreach (var pair in keyObjectMap)
        {
            if (pair.targetObject != null)
            {
                pair.targetObject.transform.position = inactivePosition;
            }
        }
    }
    
#if UNITY_EDITOR
    // Custom inspector functionality
    [CustomEditor(typeof(FrictionController))]
    public class FrictionControllerEditor : Editor
    {
        private bool showRankings = true;
        
        public override void OnInspectorGUI()
        {
            FrictionController controller = (FrictionController)target;
            
            // Draw default inspector elements
            DrawDefaultInspector();
            
            // Only show quiz UI when quiz mode is enabled
            if (!controller.quizMode)
                return;
                
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Quiz Rankings", EditorStyles.boldLabel);
            
            showRankings = EditorGUILayout.Foldout(showRankings, "User Guesses");
            
            if (showRankings)
            {
                // Display each ranking option
                for (int i = 0; i < controller.frictionRankings.Count; i++)
                {
                    FrictionRanking ranking = controller.frictionRankings[i];
                    EditorGUILayout.BeginHorizontal();
                    
                    EditorGUILayout.LabelField(ranking.rankingLabel, GUILayout.Width(100));
                    
                    string[] options = {"Unsure", "Key 1", "Key 2", "Key 3"};
                    int selected = controller.frictionRankings[i].userGuess;
                    
                    int newSelection = EditorGUILayout.Popup(selected, options, GUILayout.Width(100));
                    if (newSelection != selected)
                    {
                        controller.frictionRankings[i].userGuess = newSelection;
                        EditorUtility.SetDirty(controller);
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
            
            EditorGUILayout.Space(10);
            
            // Submit button
            if (GUILayout.Button("Submit Answer", GUILayout.Height(30)))
            {
                controller.SaveResults();
                EditorUtility.DisplayDialog("Success", "Your answers have been saved!", "OK");
            }
        }
    }
#endif

    private bool ValidateAnswers()
    {
        // Always return true, allowing incomplete answers
        return true;
    }
    
    private void SaveResults()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filePath = Path.Combine(saveDirectory, $"friction_quiz_results_{timestamp}.csv");
        
        // Get objects for keys 1, 2, 3 for ranking
        var rankableObjects = keyObjectMap
            .Where(p => p.key == KeyCode.Alpha1 || p.key == KeyCode.Alpha2 || p.key == KeyCode.Alpha3)
            .OrderBy(p => p.key)
            .ToList();
            
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write CSV header
            writer.WriteLine("Type,Value,Details");
            
            // Write timestamp
            writer.WriteLine($"Timestamp,{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},");
            writer.WriteLine();
            
            // Write user answers
            writer.WriteLine("USER_ANSWERS,Key,");
            foreach (var ranking in frictionRankings)
            {
                // Use "blank" for unselected entries (userGuess == 0)
                string guessValue = ranking.userGuess == 0 ? "blank" : ranking.userGuess.ToString();
                writer.WriteLine($"{ranking.rankingLabel},{guessValue},");
            }
            
            writer.WriteLine();
            
            // Write object information
            writer.WriteLine("OBJECT_INFO,Key,Name");
            for (int i = 0; i < Math.Min(rankableObjects.Count, 3); i++)
            {
                string keyNumber = rankableObjects[i].key.ToString().Replace("Alpha", "");
                writer.WriteLine($"Object {keyNumber},{keyNumber},{(rankableObjects[i].targetObject != null ? rankableObjects[i].targetObject.name : "None")}");
            }
            
            writer.WriteLine();
            
            // Record PHSceneControl settings if available
            if (typeof(PHSceneControl) != null)
            {
                writer.WriteLine("HAPTIC_SETTINGS,Value,");
                
                try
                {
                    // Access static variables through reflection to avoid direct reference
                    var type = Type.GetType("PHSceneControl, Assembly-CSharp");
                    if (type != null)
                    {
                        var isPressureOn = type.GetField("isPressureOn").GetValue(null);
                        var isVibrationOn = type.GetField("isVibrationOn").GetValue(null);
                        var isTengentOn = type.GetField("isTengentOn").GetValue(null);
                        var isLocalDirectionFeedabck = type.GetField("isLocalDirectionFeedabck").GetValue(null);
                        var isTengentXOn = type.GetField("isTengentXOn").GetValue(null);
                        var isTengentYOn = type.GetField("isTengentYOn").GetValue(null);
                        var isTengentZOn = type.GetField("isTengentZOn").GetValue(null);
                        
                        writer.WriteLine($"isPressureOn,{isPressureOn},");
                        writer.WriteLine($"isVibrationOn,{isVibrationOn},");
                        writer.WriteLine($"isTengentOn,{isTengentOn},");
                        writer.WriteLine($"isLocalDirectionFeedabck,{isLocalDirectionFeedabck},");
                        writer.WriteLine($"isTengentXOn,{isTengentXOn},");
                        writer.WriteLine($"isTengentYOn,{isTengentYOn},");
                        writer.WriteLine($"isTengentZOn,{isTengentZOn},");
                    }
                    else
                    {
                        // Direct reference if available
                        writer.WriteLine($"isPressureOn,{PHSceneControl.isPressureOn},");
                        writer.WriteLine($"isVibrationOn,{PHSceneControl.isVibrationOn},");
                        writer.WriteLine($"isTengentOn,{PHSceneControl.isTengentOn},");
                        writer.WriteLine($"isLocalDirectionFeedabck,{PHSceneControl.isLocalDirectionFeedabck},");
                        writer.WriteLine($"isTengentXOn,{PHSceneControl.isTengentXOn},");
                        writer.WriteLine($"isTengentYOn,{PHSceneControl.isTengentYOn},");
                        writer.WriteLine($"isTengentZOn,{PHSceneControl.isTengentZOn},");
                    }
                }
                catch (Exception ex)
                {
                    writer.WriteLine($"Error reading haptic settings,{ex.Message},");
                }
            }
            
            // If there's a reference to PHSceneControl, get the public variables too
            if (phSceneControl != null)
            {
                writer.WriteLine();
                writer.WriteLine("PUBLIC_HAPTIC_SETTINGS,Value,");
                writer.WriteLine($"publicisPressureOn,{phSceneControl.publicisPressureOn},");
                writer.WriteLine($"publicisVibrationOn,{phSceneControl.publicisVibrationOn},");
                writer.WriteLine($"publicisTengentOn,{phSceneControl.publicisTengentOn},");
                writer.WriteLine($"publicisLocalDirectionFeedabck,{phSceneControl.publicisLocalDirectionFeedabck},");
                writer.WriteLine($"publicisTengentXOn,{phSceneControl.publicisTengentXOn},");
                writer.WriteLine($"publicisTengentYOn,{phSceneControl.publicisTengentYOn},");
                writer.WriteLine($"publicisTengentZOn,{phSceneControl.publicisTengentZOn},");
            }
        }
        
        Debug.Log($"Friction quiz results saved to {filePath}");
    }

    // New method to reset randomization without reloading the scene
    private void ResetRandomization()
    {
        // Store all game objects that are not null and their initial positions
        List<GameObject> validObjects = new List<GameObject>();
        List<Vector3> initialPositions = new List<Vector3>();
        
        foreach (var pair in keyObjectMap)
        {
            if (pair.targetObject != null)
            {
                validObjects.Add(pair.targetObject);
                initialPositions.Add(pair.initialPosition);
            }
        }

        // Check if we have any valid objects
        if (validObjects.Count == 0)
        {
            Debug.LogWarning("No GameObjects assigned to the FrictionController.");
            return;
        }

        // Store all used keys
        List<KeyCode> keys = keyObjectMap.Select(p => p.key).ToList();

        // Clear the current keyObjectMap
        keyObjectMap.Clear();

        // Shuffle the objects
        for (int i = 0; i < validObjects.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, validObjects.Count);
            
            // Swap objects
            GameObject tempObj = validObjects[i];
            validObjects[i] = validObjects[randomIndex];
            validObjects[randomIndex] = tempObj;
            
            // Swap initial positions too
            Vector3 tempPos = initialPositions[i];
            initialPositions[i] = initialPositions[randomIndex];
            initialPositions[randomIndex] = tempPos;
        }

        // Reassign objects to keys randomly
        for (int i = 0; i < Mathf.Min(validObjects.Count, keys.Count); i++)
        {
            KeyObjectPair newPair = new KeyObjectPair
            {
                key = keys[i],
                targetObject = validObjects[i],
                initialPosition = initialPositions[i],
                color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value)
            };

            // Apply color to the object if it has a renderer
            Renderer renderer = newPair.targetObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = newPair.color;
            }

            // Move to inactive position
            newPair.targetObject.transform.position = inactivePosition;

            // Add to map
            keyObjectMap.Add(newPair);
            
            Debug.Log($"Randomly reassigned key {newPair.key} to control {newPair.targetObject.name} with color {newPair.color}");
        }
        
        // Reset the active object
        activeIndex = -1;
        
        // Reset the global color renderer if assigned
        if (colorRenderer != null && keyObjectMap.Count > 0)
        {
            colorRenderer.material.color = keyObjectMap[0].color;
        }
        
        Debug.Log("Friction randomization reset completed");
    }
} 