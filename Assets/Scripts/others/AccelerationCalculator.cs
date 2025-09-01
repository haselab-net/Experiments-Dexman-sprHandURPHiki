using UnityEngine;

public class AccelerationCalculator : MonoBehaviour
{
    public GameObject targetObject; // 要追踪的对象

    private Vector3 previousPosition;
    private Vector3 currentVelocity;
    private Vector3 previousVelocity;
    private Vector3 acceleration;

    public DrawHapticsWave drawHapticsWave; //if you want to display this force, assign this

    void Start()
    {
        if (targetObject != null)
        {
            previousPosition = targetObject.transform.position;
        }
    }

    void Update()
    {
        if (targetObject != null)
        {
            // 计算当前速度
            currentVelocity = (targetObject.transform.position - previousPosition) / Time.deltaTime;
            // 计算加速度
            acceleration = (currentVelocity - previousVelocity) / Time.deltaTime;

            // 更新用于下一次计算的值
            previousPosition = targetObject.transform.position;
            previousVelocity = currentVelocity;

            // 可以在控制台输出加速度进行测试
            //Debug.Log("Acceleration: " + (acceleration.y ) * 10);
            //drawHapticsWave.AddNewValue((acceleration.y) * 10);
        }
    }
}
