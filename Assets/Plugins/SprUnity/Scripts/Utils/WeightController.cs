using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SprUnity; // Assuming PHSolidBehaviour is in this namespace
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WeightController : MonoBehaviour
{
    [Tooltip("The PHSolidBehaviour component to control.")]
    public PHSolidBehaviour targetSolid;

    [System.Serializable]
    public struct KeyWeightPair
    {
        public KeyCode key;
        public float weightValue;
        [HideInInspector]
        public Color color;
    }

    [Tooltip("Define the weight values corresponding to number keys 1-9.")]
    public List<KeyWeightPair> keyWeightMap = new List<KeyWeightPair>()
    {
        new KeyWeightPair { key = KeyCode.Alpha1, weightValue = 1.0f },
        new KeyWeightPair { key = KeyCode.Alpha2, weightValue = 2.0f },
        new KeyWeightPair { key = KeyCode.Alpha3, weightValue = 3.0f },
        new KeyWeightPair { key = KeyCode.Alpha4, weightValue = 4.0f },
        new KeyWeightPair { key = KeyCode.Alpha5, weightValue = 5.0f },
        new KeyWeightPair { key = KeyCode.Alpha6, weightValue = 6.0f },
        new KeyWeightPair { key = KeyCode.Alpha7, weightValue = 7.0f },
        new KeyWeightPair { key = KeyCode.Alpha8, weightValue = 8.0f },
        new KeyWeightPair { key = KeyCode.Alpha9, weightValue = 9.0f }
    };

    [Header("Quiz Settings")]
    [Tooltip("Enable quiz mode for ranking objects by weight")]
    public bool quizMode = false;
    
    [System.Serializable]
    public class WeightRanking
    {
        public KeyCode key;
        public string rankingLabel; // "Heavy", "Medium", "Light"
        [HideInInspector]
        public int userGuess; // 1, 2, or 3 - corresponding to the keys
    }
    
    public List<WeightRanking> weightRankings = new List<WeightRanking>()
    {
        new WeightRanking { key = KeyCode.Alpha1, rankingLabel = "Heavy" },
        new WeightRanking { key = KeyCode.Alpha2, rankingLabel = "Medium" },
        new WeightRanking { key = KeyCode.Alpha3, rankingLabel = "Light" }
    };
    
    [Tooltip("Directory to save quiz results")]
    public string saveDirectory = "QuizResults";
    
    [Tooltip("Reference to PHSceneControl to record haptic feedback settings")]
    public PHSceneControl phSceneControl;
    
    // Cached renderer component from the target solid
    private Renderer targetRenderer;

    void Start()
    {
        // Store all keys and weight values from the inspector
        List<KeyCode> keys = keyWeightMap.Select(p => p.key).ToList();
        List<float> weights = keyWeightMap.Select(p => p.weightValue).ToList();

        // Clear the current keyWeightMap
        keyWeightMap.Clear();

        // Shuffle the weights
        for (int i = 0; i < weights.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, weights.Count);
            float temp = weights[i];
            weights[i] = weights[randomIndex];
            weights[randomIndex] = temp;
        }

        // Reassign weights to keys randomly
        for (int i = 0; i < Mathf.Min(weights.Count, keys.Count); i++)
        {
            KeyWeightPair newPair = new KeyWeightPair
            {
                key = keys[i],
                weightValue = weights[i],
                color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value)
            };

            // Add to map
            keyWeightMap.Add(newPair);
            
            Debug.Log($"Randomly assigned key {newPair.key} to weight value {newPair.weightValue} with color {newPair.color}");
        }
        
        // Cache the renderer component if available
        if (targetSolid != null)
        {
            targetRenderer = targetSolid.GetComponent<Renderer>();
            if (targetRenderer == null)
            {
                // Try to find renderer in children if not on the main object
                targetRenderer = targetSolid.GetComponentInChildren<Renderer>();
            }
        }
        
        // Create save directory if it doesn't exist
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }
    }

    void Update()
    {
        // Reset random weights when T key is pressed instead of reloading the scene
        if (Input.GetKeyDown(KeyCode.T))
        {
            ResetRandomization();
            return;
        }

        if (targetSolid == null)
        {
            return;
        }

        // If renderer not cached yet (in case targetSolid was assigned after Start)
        if (targetRenderer == null)
        {
            targetRenderer = targetSolid.GetComponent<Renderer>();
            if (targetRenderer == null)
            {
                targetRenderer = targetSolid.GetComponentInChildren<Renderer>();
            }
        }

        if (!quizMode)
        {
            // Original functionality - change weight on key press
            foreach (var pair in keyWeightMap)
            {
                if (Input.GetKeyDown(pair.key))
                {
                    // Set weight
                    targetSolid.controlledWeight = pair.weightValue;
                    // Optionally trigger OnValidate or a custom update method if needed
                    targetSolid.OnValidate();
                    
                    // Apply the color directly to the target object's renderer
                    if (targetRenderer != null)
                    {
                        targetRenderer.material.color = pair.color;
                    }
                    
                    Debug.Log($"Set {targetSolid.gameObject.name}'s controlledWeight to {pair.weightValue} and color to {pair.color} using key {pair.key}");
                }
            }
        }
        else
        {
            // Quiz mode functionality - allow pressing 1, 2, 3 to test weights
            for (int i = 0; i < 3; i++)
            {
                KeyCode key = KeyCode.Alpha1 + i;
                if (Input.GetKeyDown(key))
                {
                    // Find the weight pair for this key
                    var pair = keyWeightMap.FirstOrDefault(p => p.key == key);
                    if (pair.key != KeyCode.None)
                    {
                        // Set weight
                        targetSolid.controlledWeight = pair.weightValue;
                        // Update physics
                        targetSolid.OnValidate();
                        
                        // Apply the color directly to the target object's renderer
                        if (targetRenderer != null)
                        {
                            targetRenderer.material.color = pair.color;
                        }
                        
                        Debug.Log($"测试 按键 {i+1}: 设置 {targetSolid.gameObject.name} 的重量为 {pair.weightValue}");
                    }
                }
            }
        }
    }
    
#if UNITY_EDITOR
    // Custom inspector functionality
    [CustomEditor(typeof(WeightController))]
    public class WeightControllerEditor : Editor
    {
        private bool showRankings = true;
        
        public override void OnInspectorGUI()
        {
            WeightController controller = (WeightController)target;
            
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
                for (int i = 0; i < controller.weightRankings.Count; i++)
                {
                    WeightRanking ranking = controller.weightRankings[i];
                    EditorGUILayout.BeginHorizontal();
                    
                    EditorGUILayout.LabelField(ranking.rankingLabel, GUILayout.Width(70));
                    
                    string[] options = {"Unsure", "Key 1", "Key 2", "Key 3"};
                    int selected = controller.weightRankings[i].userGuess;
                    
                    int newSelection = EditorGUILayout.Popup(selected, options, GUILayout.Width(100));
                    if (newSelection != selected)
                    {
                        controller.weightRankings[i].userGuess = newSelection;
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
        string filePath = Path.Combine(saveDirectory, $"quiz_results_{timestamp}.csv");
        
        // Sort weight pairs by weight to determine correct order
        var sortedWeights = keyWeightMap
            .Where(p => p.key == KeyCode.Alpha1 || p.key == KeyCode.Alpha2 || p.key == KeyCode.Alpha3)
            .OrderByDescending(p => p.weightValue)
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
            foreach (var ranking in weightRankings)
            {
                // Use "blank" for unselected entries (userGuess == 0)
                string guessValue = ranking.userGuess == 0 ? "blank" : ranking.userGuess.ToString();
                writer.WriteLine($"{ranking.rankingLabel},{guessValue},");
            }
            
            writer.WriteLine();
            
            // Write correct answers
            writer.WriteLine("CORRECT_ANSWERS,Key,Weight");
            string[] rankLabels = {"Heavy", "Medium", "Light"};
            
            for (int i = 0; i < Math.Min(sortedWeights.Count, 3); i++)
            {
                string keyNumber = sortedWeights[i].key.ToString().Replace("Alpha", "");
                writer.WriteLine($"{rankLabels[i]},{keyNumber},{sortedWeights[i].weightValue}");
            }
            
            // Calculate score
            int correctAnswers = 0;
            for (int i = 0; i < weightRankings.Count; i++)
            {
                var ranking = weightRankings[i];
                string keyNumber = sortedWeights[i].key.ToString().Replace("Alpha", "");
                
                if (ranking.userGuess == int.Parse(keyNumber))
                    correctAnswers++;
            }
            
            writer.WriteLine();
            writer.WriteLine($"Score,{correctAnswers}/{weightRankings.Count},");
            
            // Record PHSceneControl settings
            writer.WriteLine();
            writer.WriteLine("HAPTIC_SETTINGS,Value,");
            
            // Access static variables directly
            writer.WriteLine($"isPressureOn,{PHSceneControl.isPressureOn},");
            writer.WriteLine($"isVibrationOn,{PHSceneControl.isVibrationOn},");
            writer.WriteLine($"isTengentOn,{PHSceneControl.isTengentOn},");
            writer.WriteLine($"isLocalDirectionFeedabck,{PHSceneControl.isLocalDirectionFeedabck},");
            writer.WriteLine($"isTengentXOn,{PHSceneControl.isTengentXOn},");
            writer.WriteLine($"isTengentYOn,{PHSceneControl.isTengentYOn},");
            writer.WriteLine($"isTengentZOn,{PHSceneControl.isTengentZOn},");
            
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
        
        Debug.Log($"Quiz results saved to {filePath}");
    }

    // New method to reset randomization without reloading the scene
    private void ResetRandomization()
    {
        // Store all keys and weight values from the current map
        List<KeyCode> keys = keyWeightMap.Select(p => p.key).ToList();
        List<float> weights = keyWeightMap.Select(p => p.weightValue).ToList();

        // Clear the current keyWeightMap
        keyWeightMap.Clear();

        // Shuffle the weights
        for (int i = 0; i < weights.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, weights.Count);
            float temp = weights[i];
            weights[i] = weights[randomIndex];
            weights[randomIndex] = temp;
        }

        // Reassign weights to keys randomly
        for (int i = 0; i < Mathf.Min(weights.Count, keys.Count); i++)
        {
            KeyWeightPair newPair = new KeyWeightPair
            {
                key = keys[i],
                weightValue = weights[i],
                color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value)
            };

            // Add to map
            keyWeightMap.Add(newPair);
            
            Debug.Log($"Randomly reassigned key {newPair.key} to weight value {newPair.weightValue} with color {newPair.color}");
        }
        
        // Reset the material color of the target if it has a renderer
        if (targetSolid != null && targetRenderer != null)
        {
            // Reset to a neutral color or the first color in the map
            if (keyWeightMap.Count > 0)
            {
                targetRenderer.material.color = keyWeightMap[0].color;
                targetSolid.controlledWeight = keyWeightMap[0].weightValue;
                targetSolid.OnValidate();
            }
        }
        
        Debug.Log("Randomization reset completed");
    }
} 