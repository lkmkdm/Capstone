using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;

public class RadarChart : MonoBehaviour
{
    public float[] values = new float[5]; // 5개 지표
    private float[] currentValues = new float[5]; // 애니메이션용 현재 값
    public float maxValue = 100f; // 최대값
    public float graphSize = 5f; // 그래프 크기
    public Color lineColor = Color.black; // 선 색상
    public float lineWidth = 0.05f; // 선 두께
    public Color fillColor = new Color(1f, 0.5f, 0.5f, 0.5f); // 내부 색상

    private LineRenderer lineRenderer;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private FirebaseFirestore db;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        DrawGraph();
        LoadDataFromFirebase();
    }

    void DrawGraph()
    {
        if (lineRenderer == null) lineRenderer = gameObject.AddComponent<LineRenderer>();
        if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
        if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = fillColor;

        lineRenderer.loop = true;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = false;

        UpdateGraph();
    }

    void UpdateGraph()
    {
        Vector3[] vertices = new Vector3[5];
        float angleOffset = 72f; // 360도 / 5 (오각형)
        float rotationOffset = 18f; // 꼭짓점을 위로 맞추기 위해 18도 회전

        for (int i = 0; i < 5; i++)
        {
            float angle = Mathf.Deg2Rad * (angleOffset * i + rotationOffset);
            float normalizedValue = currentValues[i] / maxValue;
            float radius = graphSize * normalizedValue;
            vertices[i] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        }

        // 외곽선 업데이트
        lineRenderer.positionCount = 6;
        lineRenderer.SetPositions(new Vector3[] { vertices[0], vertices[1], vertices[2], vertices[3], vertices[4], vertices[0] });

        // 내부 색칠 업데이트
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] { Vector3.zero, vertices[0], vertices[1], vertices[2], vertices[3], vertices[4] };
        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5, 0, 5, 1 };
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    void LoadDataFromFirebase()
    {
        db.Collection("users").Document("testuser@example.com").Collection("ability").Document("current")
        .GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                DocumentSnapshot snapshot = task.Result;
                Dictionary<string, object> data = snapshot.ToDictionary();

                // 목표 값 설정
                values[0] = GetValueFromDict(data, "memory");
                values[1] = GetValueFromDict(data, "concentration");
                values[2] = GetValueFromDict(data, "processingspeed"); // 공백 제거
                values[3] = GetValueFromDict(data, "impulsiveness");
                values[4] = GetValueFromDict(data, "accuracy");

                // 애니메이션 시작
                StartCoroutine(AnimateGraph());
            }
            else
            {
                Debug.LogError("Firebase 데이터 불러오기 실패");
            }
        });
    }


    IEnumerator AnimateGraph()
    {
        float duration = 1.0f;
        float elapsed = 0f;
        float[] startValues = new float[5];

        for (int i = 0; i < 5; i++)
            startValues[i] = 0f; // 0부터 시작

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            for (int i = 0; i < 5; i++)
            {
                currentValues[i] = Mathf.Lerp(startValues[i], values[i], elapsed / duration);
            }
            UpdateGraph();
            yield return null;
        }

        // 최종 값 설정
        for (int i = 0; i < 5; i++)
            currentValues[i] = values[i];

        UpdateGraph();
    }

    float GetValueFromDict(Dictionary<string, object> data, string key)
    {
        if (data.ContainsKey(key) && data[key] is long)
        {
            return (long)data[key];
        }
        if (data.ContainsKey(key) && data[key] is double)
        {
            return (float)(double)data[key];
        }
        return 0f;
    }
}
