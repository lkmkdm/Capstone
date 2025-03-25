using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;

public class RadarChart : MonoBehaviour
{
    public float[] values = new float[5]; // 5개 지표
    public float maxValue = 100f; // 최대값
    public float graphSize = 5f; // 그래프 크기
    public Color lineColor = Color.black; // 선 색상
    public float lineWidth = 0.05f; // 선 두께
    public Color fillColor = new Color(1f, 0.5f, 0.5f, 0.5f); // 내부 색상

    public float pointSize = 0.1f; // 점 크기
    public Color pointColor = Color.red; // 점 색상

    private LineRenderer lineRenderer;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private FirebaseFirestore db;
    private GameObject[] points = new GameObject[5]; // 꼭짓점 점들

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        DrawGraph();
        LoadDataFromFirebase();
    }

    void Update()
    {
        UpdateGraph();
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

        // 꼭짓점 점 생성
        for (int i = 0; i < 5; i++)
        {
            if (points[i] == null)
            {
                points[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                points[i].transform.SetParent(transform);
                points[i].transform.localScale = Vector3.one * pointSize;
                points[i].GetComponent<Renderer>().material.color = pointColor;
            }
        }

        UpdateGraph();
    }

    void UpdateGraph()
    {
        Vector3[] vertices = new Vector3[5];
        float angleOffset = 72f; // 360도 / 5 (오각형)
        float rotationOffset = 18f; // 꼭짓점을 위로 맞추기 위해 -18도 회전

        for (int i = 0; i < 5; i++)
        {
            float angle = Mathf.Deg2Rad * (angleOffset * i + rotationOffset);
            float normalizedValue = values[i] / maxValue;
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

        // 점 위치 및 스타일 업데이트
        for (int i = 0; i < 5; i++)
        {
            points[i].transform.localPosition = vertices[i];
            points[i].transform.localScale = Vector3.one * pointSize;
            points[i].GetComponent<Renderer>().material.color = pointColor;
        }
    }

    void LoadDataFromFirebase()
    {
        db.Collection("example").Document("ability").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                DocumentSnapshot snapshot = task.Result;
                Dictionary<string, object> data = snapshot.ToDictionary();

                // Firebase에서 데이터 가져와서 values 배열에 저장
                values[0] = GetValueFromDict(data, "memory");
                values[1] = GetValueFromDict(data, "concentration");
                values[2] = GetValueFromDict(data, "processing speed");
                values[3] = GetValueFromDict(data, "impulsiveness");
                values[4] = GetValueFromDict(data, "accuracy");

                UpdateGraph();
            }
            else
            {
                Debug.LogError("Firebase 데이터 불러오기 실패");
            }
        });
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
