using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;

[RequireComponent(typeof(LineRenderer), typeof(MeshFilter), typeof(MeshRenderer))]
public class RadarChart : MonoBehaviour
{
    FirebaseFirestore db;
    FirebaseAuth auth;

    private string userEmail;
    private string userID;
    private string userName;

    public float[] values = new float[5]; // 5개 지표
    private float[] currentValues = new float[5]; // 애니메이션용 현재 값
    public float maxValue = 1f; // 최대값

    private LineRenderer lineRenderer;  // 선을 그릴 LineRenderer
    private Vector3[] baseVertices = new Vector3[5];  // 기본 오각형 꼭짓점 좌표

    private MeshFilter meshFilter;
    private Mesh mesh;
    private Material meshMaterial;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;

        //테스트용(테스트 계정 사용 시 바로 밑에 if else 문 주석처리 필수)
        //userEmail = "testuser@example.com";

        //테스트용 사용 시 주석 처리하기
        if (auth.CurrentUser != null)
        {
            userEmail = auth.CurrentUser.Email;
            userID = auth.CurrentUser.UserId;
            userName = auth.CurrentUser.DisplayName;
        }
        else
        {
            Debug.LogError("Firebase Authentication: 로그인된 사용자가 없습니다!");
        }
        //테스트용 사용 시 여기까지 주석 처리

        // LineRenderer 및 MeshFilter 초기화
        lineRenderer = GetComponent<LineRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }

        lineRenderer.positionCount = 6;
        lineRenderer.startWidth = 5f;
        lineRenderer.endWidth = 5f;
        lineRenderer.useWorldSpace = false;

        Color semiTransparentCyan = new Color(1f, 0.5f, 0.5f, 0.3f);
        lineRenderer.startColor = semiTransparentCyan;
        lineRenderer.endColor = semiTransparentCyan;

        lineRenderer.material = new Material(Shader.Find("Custom/TransparentShader"));

        meshMaterial = new Material(Shader.Find("Custom/TransparentShader"));
        meshMaterial.color = semiTransparentCyan;

        meshRenderer.material = meshMaterial;

        mesh = new Mesh();
        meshFilter.mesh = mesh;

        SetupBaseVertices();
        LoadDataFromFirebase();
    }

    void SetupBaseVertices()
    {
        float angle = Mathf.PI * 2f / 5f;  // 360도 / 5 (오각형)

        for (int i = 0; i < 5; i++)
        {
            float x = Mathf.Sin(i * angle); // X축 방향
            float y = Mathf.Cos(i * angle); // Y축 방향
            baseVertices[i] = new Vector3(x, y, 0);
        }
    }

    void LoadDataFromFirebase()
    {
        if (string.IsNullOrEmpty(userEmail))
        {
            Debug.LogError("유저 이메일이 설정되지 않음");
            return;
        }

        db.Collection("users").Document(userEmail)
          .Collection("personal_information").Document("original_ability")
          .GetSnapshotAsync().ContinueWithOnMainThread(task =>
          {
              if (task.IsCompleted && task.Result.Exists)
              {
                  DocumentSnapshot snapshot = task.Result;
                  Dictionary<string, object> data = snapshot.ToDictionary();

                  values[0] = GetValueFromDict(data, "original_concentration");
                  values[1] = GetValueFromDict(data, "original_impulsiveness");
                  values[2] = GetValueFromDict(data, "original_memory");
                  values[3] = GetValueFromDict(data, "original_potential");
                  values[4] = GetValueFromDict(data, "original_processingspeed");

                  StartCoroutine(AnimateGraph());
              }
              else
              {
                  Debug.LogError("Firebase 데이터 불러오기 실패 또는 문서 없음");
              }
          });
    }

    IEnumerator AnimateGraph()
    {
        maxValue = 100f;

        float duration = 3.0f;
        float elapsed = 0f;
        float[] startValues = new float[5];

        for (int i = 0; i < 5; i++)
            startValues[i] = 0f;

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

        for (int i = 0; i < 5; i++)
            currentValues[i] = values[i];

        UpdateGraph();
    }

    void UpdateGraph()
    {
        Vector3[] newVertices = new Vector3[6];

        float chartRadius = 270f;  // 오각형 외곽 크기에 맞게 조절..이게 최선임(Pentagon들 Pos Y 값은 25 근처로 설정)

        for (int i = 0; i < 5; i++)
        {
            float scale = currentValues[i] / maxValue;
            newVertices[i] = baseVertices[i] * scale * chartRadius;
        }

        newVertices[5] = newVertices[0]; // 처음 점으로 닫기
        lineRenderer.SetPositions(newVertices);

        // 내부를 채우기 위한 Mesh 설정
        Vector3[] meshVertices = new Vector3[6]; // 중앙점 + 5개 꼭짓점
        int[] triangles = new int[15]; // 삼각형 5개 (3개씩)

        meshVertices[0] = Vector3.zero; // 중앙점
        for (int i = 0; i < 5; i++)
        {
            meshVertices[i + 1] = newVertices[i];

            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 1) % 5 + 1;
        }

        // 마지막 삼각형이 제대로 연결되도록 수정
        triangles[12] = 0;
        triangles[13] = 5;
        triangles[14] = 1;

        mesh.Clear();
        mesh.vertices = meshVertices;
        mesh.triangles = triangles;

        // 내부 색상
        meshFilter.GetComponent<MeshRenderer>().material = meshMaterial;
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