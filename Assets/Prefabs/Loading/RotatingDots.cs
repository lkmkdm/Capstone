using UnityEngine;

public class RotatingDots : MonoBehaviour
{
    [Tooltip("회전 속도 (양수 = 시계 방향, 음수 = 반시계 방향)")]
    public float rotationSpeed = -100f;

    void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
