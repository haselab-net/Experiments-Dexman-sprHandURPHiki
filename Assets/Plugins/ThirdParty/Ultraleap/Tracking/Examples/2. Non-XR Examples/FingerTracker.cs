using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FingerTracker : MonoBehaviour
{
    private List<string> fingers = new List<string> { "Thumb", "Index", "Middle", "Ring", "Pinky" };
    private Dictionary<string, Vector3> previousPositions;
    private Dictionary<string, Vector3> velocities;
    private Dictionary<string, float> confidenceScores;

    public List<GameObject> fingerRigidBody = new List<GameObject>();
    public GameObject thumbNextRigidBody;
    public GameObject middleNextRigidBody;
    private Dictionary<string, Color> fingerColors;

    private const float VelocityWeight = 0.3f;
    private const float PositionWeight = 0.4f;
    private const float GeometryWeight = 0.3f;
    private const float ConfidenceThreshold = 0.7f;
    private const float VelocitySmoothingFactor = 0.8f;

    void Start()
    {
        InitializeTracking();
    }

    void InitializeTracking()
    {
        previousPositions = new Dictionary<string, Vector3>();
        velocities = new Dictionary<string, Vector3>();
        confidenceScores = new Dictionary<string, float>();
        fingerColors = new Dictionary<string, Color>();

        for (int i = 0; i < fingers.Count; i++)
        {
            string finger = fingers[i];
            if (i < fingerRigidBody.Count && fingerRigidBody[i] != null)
            {
                previousPositions[finger] = fingerRigidBody[i].transform.position;
            }
            else
            {
                previousPositions[finger] = Vector3.zero;
                Debug.LogWarning($"FingerRigidBody for {finger} is not assigned.");
            }
            velocities[finger] = Vector3.zero;
            confidenceScores[finger] = 1f;
            fingerColors[finger] = GetColorForFinger(i);
        }

        if (thumbNextRigidBody == null)
        {
            Debug.LogWarning("ThumbNextRigidBody is not assigned.");
        }
        if (middleNextRigidBody == null)
        {
            Debug.LogWarning("MiddleNextRigidBody is not assigned.");
        }
    }

    Color GetColorForFinger(int index)
    {
        Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta };
        return colors[index % colors.Length];
    }

    void Update()
    {
        List<Vector3> framePositions = GetCurrentFramePositions();

        if (framePositions.Count == 5)
        {
            Dictionary<string, Vector3> matchedPositions = MatchFingersToMarkers(framePositions);
            UpdateTracking(matchedPositions);
            VisualizeFingers(matchedPositions);
        }
        else
        {
            Debug.Log("Marker lost, frame skipped or handled.");
        }
    }

    List<Vector3> GetCurrentFramePositions()
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (var rigidBody in fingerRigidBody)
        {
            if (rigidBody != null)
            {
                positions.Add(rigidBody.transform.position);
            }
            else
            {
                positions.Add(Vector3.zero);
                Debug.LogWarning("A FingerRigidBody is not assigned.");
            }
        }
        return positions;
    }

    Dictionary<string, Vector3> MatchFingersToMarkers(List<Vector3> framePositions)
    {
        int n = fingers.Count;
        float[,] costMatrix = new float[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < framePositions.Count; j++)
            {
                float positionCost = Vector3.Distance(previousPositions[fingers[i]], framePositions[j]);
                float velocityCost = Vector3.Distance(velocities[fingers[i]], (framePositions[j] - previousPositions[fingers[i]]) / Time.deltaTime);
                float geometryCost = Vector3.Distance(GetKnucklePosition(fingers[i]), framePositions[j]);

                costMatrix[i, j] = PositionWeight * positionCost + 
                                   VelocityWeight * velocityCost + 
                                   GeometryWeight * geometryCost;
            }
        }

        int[] assignment = HungarianAlgorithm.Compute(costMatrix);
        Dictionary<string, Vector3> matchedPositions = new Dictionary<string, Vector3>();

        for (int i = 0; i < n; i++)
        {
            if (assignment[i] != -1)
            {
                matchedPositions[fingers[i]] = framePositions[assignment[i]];
            }
        }

        return matchedPositions;
    }

    void UpdateTracking(Dictionary<string, Vector3> matchedPositions)
    {
        foreach (var finger in fingers)
        {
            if (matchedPositions.ContainsKey(finger))
            {
                Vector3 newPosition = matchedPositions[finger];
                Vector3 newVelocity = (newPosition - previousPositions[finger]) / Time.deltaTime;
                
                velocities[finger] = Vector3.Lerp(velocities[finger], newVelocity, VelocitySmoothingFactor);
                
                float confidence = CalculateConfidence(finger, newPosition);
                confidenceScores[finger] = Mathf.Lerp(confidenceScores[finger], confidence, 0.2f);

                if (confidenceScores[finger] > ConfidenceThreshold)
                {
                    previousPositions[finger] = newPosition;
                }
            }
        }
    }

    float CalculateConfidence(string finger, Vector3 newPosition)
    {
        float distanceFromPrevious = Vector3.Distance(previousPositions[finger], newPosition);
        float distanceFromKnuckle = Vector3.Distance(GetKnucklePosition(finger), newPosition);
        float expectedFingerLength = GetExpectedFingerLength(finger);

        float confidenceScore = 1f;
        confidenceScore *= Mathf.Clamp01(1f - distanceFromPrevious / 0.1f); // Assume max movement of 10cm per frame
        confidenceScore *= Mathf.Clamp01(1f - Mathf.Abs(distanceFromKnuckle - expectedFingerLength) / 0.05f); // Allow 5cm error

        return confidenceScore;
    }

    Vector3 GetKnucklePosition(string finger)
    {
        switch (finger)
        {
            case "Thumb":
                return thumbNextRigidBody != null ? thumbNextRigidBody.transform.position : EstimateKnucklePosition(finger);
            case "Middle":
                return middleNextRigidBody != null ? middleNextRigidBody.transform.position : EstimateKnucklePosition(finger);
            default:
                return EstimateKnucklePosition(finger);
        }
    }

    Vector3 EstimateKnucklePosition(string finger)
    {
        int index = fingers.IndexOf(finger);
        if (index < 0 || index >= fingerRigidBody.Count || fingerRigidBody[index] == null)
        {
            return Vector3.zero;
        }

        Vector3 fingerTip = fingerRigidBody[index].transform.position;
        Vector3 palmCenter = CalculatePalmCenter();
        return Vector3.Lerp(fingerTip, palmCenter, 0.3f);
    }

    Vector3 CalculatePalmCenter()
    {
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (var rigidBody in fingerRigidBody)
        {
            if (rigidBody != null)
            {
                sum += rigidBody.transform.position;
                count++;
            }
        }
        return count > 0 ? sum / count : Vector3.zero;
    }

    float GetExpectedFingerLength(string finger)
    {
        float defaultLength = 0.1f; // 假设默认手指长度为10cm
        switch (finger)
        {
            case "Thumb":
                return thumbNextRigidBody != null && fingerRigidBody.Count > 0 && fingerRigidBody[0] != null
                    ? Vector3.Distance(fingerRigidBody[0].transform.position, thumbNextRigidBody.transform.position)
                    : defaultLength;
            case "Middle":
                return middleNextRigidBody != null && fingerRigidBody.Count > 2 && fingerRigidBody[2] != null
                    ? Vector3.Distance(fingerRigidBody[2].transform.position, middleNextRigidBody.transform.position)
                    : defaultLength;
            default:
                return defaultLength;
        }
    }

    void VisualizeFingers(Dictionary<string, Vector3> matchedPositions)
    {
        foreach (var finger in matchedPositions)
        {
            if (confidenceScores[finger.Key] > ConfidenceThreshold)
            {
                CreateMarkerSphere(finger.Value, fingerColors[finger.Key]);
            }
        }
    }

    void CreateMarkerSphere(Vector3 position, Color color)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        Renderer renderer = sphere.GetComponent<Renderer>();
        renderer.material.color = color;
        Destroy(sphere, 0.1f);
    }
}


public static class HungarianAlgorithm
{
    public static int[] Compute(float[,] costMatrix)
    {
        int n = costMatrix.GetLength(0);
        int m = costMatrix.GetLength(1);

        int[] lx = new int[n];
        int[] ly = new int[m];
        int[] xy = new int[n];
        int[] yx = new int[m];

        for (int i = 0; i < n; i++)
        {
            lx[i] = int.MinValue;
            for (int j = 0; j < m; j++)
            {
                lx[i] = Math.Max(lx[i], (int)costMatrix[i, j]);
            }
        }

        for (int i = 0; i < n; i++) xy[i] = -1;
        for (int i = 0; i < m; i++) yx[i] = -1;

        for (int size = 0; size < n; size++)
        {
            int[] queue = new int[n];
            int[] pred = new int[m];
            int[] x = new int[m];
            bool[] S = new bool[n];
            bool[] T = new bool[m];

            int queueStart = 0, queueEnd = 0;
            for (int i = 0; i < n; i++)
            {
                if (xy[i] == -1)
                {
                    queue[queueEnd++] = i;
                    S[i] = true;
                    break;
                }
            }

            for (int i = 0; i < m; i++)
                x[i] = lx[queue[0]] + ly[i] - (int)costMatrix[queue[0], i];

            int y = -1;
            while (y == -1)
            {
                while (queueStart < queueEnd && y == -1)
                {
                    int q = queue[queueStart++];
                    for (int i = 0; i < m; i++)
                    {
                        if (!T[i])
                        {
                            if (x[i] == 0)
                            {
                                pred[i] = q;
                                if (yx[i] == -1)
                                {
                                    y = i;
                                    break;
                                }
                                queue[queueEnd++] = yx[i];
                                S[yx[i]] = true;
                                T[i] = true;
                            }
                            else if (x[i] > 0)
                            {
                                x[i]--;
                                if (x[i] == 0)
                                {
                                    pred[i] = q;
                                    if (yx[i] == -1)
                                    {
                                        y = i;
                                        break;
                                    }
                                    queue[queueEnd++] = yx[i];
                                    S[yx[i]] = true;
                                    T[i] = true;
                                }
                            }
                        }
                    }
                }
                if (y == -1)
                {
                    int delta = int.MaxValue;
                    for (int i = 0; i < n; i++)
                        if (S[i])
                            for (int j = 0; j < m; j++)
                                if (!T[j])
                                    delta = Math.Min(delta, x[j]);
                    for (int i = 0; i < n; i++)
                        if (S[i])
                            lx[i] -= delta;
                    for (int j = 0; j < m; j++)
                        if (T[j])
                            ly[j] += delta;
                        else
                            x[j] -= delta;
                    queueStart = queueEnd = 0;
                    for (int i = 0; i < n; i++)
                        if (S[i] && xy[i] == -1)
                        {
                            queue[queueEnd++] = i;
                            break;
                        }
                }
            }

            while (y != -1)
            {
                int prv = pred[y];
                int nxt = xy[prv];
                xy[prv] = y;
                yx[y] = prv;
                y = nxt;
            }
        }

        return xy;
    }
}