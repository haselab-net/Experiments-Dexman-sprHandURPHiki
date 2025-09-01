using UnityEngine;

public class FixedUpdateLogger : MonoBehaviour
{
    private float fixedUpdateTime = 0f;
    private int fixedUpdateCount = 0;
    private float displayRefreshRate = 0.2f; // 更新显示频率，每秒更新一次
    private float timer = 0f;

    void FixedUpdate()
    {
        fixedUpdateTime += Time.fixedDeltaTime;
        fixedUpdateCount++;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= displayRefreshRate)
        {
            float rate = fixedUpdateCount / fixedUpdateTime;
            Debug.Log($"FixedUpdate Rate: {rate:F2} calls per second");

            // 重置计数器和计时器
            fixedUpdateTime = 0f;
            fixedUpdateCount = 0;
            timer = 0f;
        }
    }
}
