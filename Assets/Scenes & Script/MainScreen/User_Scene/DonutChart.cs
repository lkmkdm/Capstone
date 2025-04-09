using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DonutChart : MaskableGraphic
{
    public float[] values;
    public Color[] colors = { new Color(0.796f, 0.945f, 0.961f), new Color(0.651f, 0.890f, 0.914f), new Color(0.890f, 0.992f, 0.992f), new Color(0.443f, 0.788f, 0.808f) };
    public float thickness = 120f;
    private float total = 100f;
    public Sprite[] icons;
    public Vector2[] iconSizes = new Vector2[4] {new Vector2(75, 72) , new Vector2(70, 70), new Vector2(66, 78) , new Vector2(80, 68)};
    private Image[] iconImages = new Image[4];
    private float animationProgress = 0f;
    private float animationSpeed = 2f; // 조절 가능


    protected override void Start()
    {
        Invoke("UpdateChartData", 1f);
        CreateIcons();
    }
    public void StartChartAnimation()
    {
        animationProgress = 0f;
        StartCoroutine(AnimateChart());
    }

    private IEnumerator AnimateChart()
    {
        while (animationProgress < 1f)
        {
            animationProgress += Time.deltaTime * animationSpeed;
            SetVerticesDirty(); // 다시 그리기
            yield return null;
        }
        animationProgress = 1f;
        SetVerticesDirty(); // 최종 업데이트
    }


    void CreateIcons()
    {
        ClearIcons();

        if (icons == null || icons.Length < 4)
        {
            Debug.LogError("아이콘 배열이 비어 있습니다! Inspector에서 설정하세요.");
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            GameObject iconObj = new GameObject("Icon" + i);
            iconObj.transform.SetParent(transform, false);

            Image img = iconObj.AddComponent<Image>();
            img.sprite = icons[i];
            img.rectTransform.sizeDelta = iconSizes[i];

            iconImages[i] = img;
        }
    }

    void ClearIcons()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("Icon"))
            {
                DestroyImmediate(child.gameObject); // 즉시 삭제
            }
        }
    }

    public void SetIconSize(int index, float width, float height)
    {
        if (index < 0 || index >= iconSizes.Length) return;
        iconSizes[index] = new Vector2(width, height);
        if (iconImages[index] != null)
        {
            iconImages[index].rectTransform.sizeDelta = iconSizes[index];
        }
    }

    void UpdateChartData()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance가 null입니다!(실행하면 없어짐 ㄱㅊㄱㅊ)");
            return;
        }

        var data = GameManager.Instance.AbilityData;

        if (data != null && data.Count > 0)
        {
            float sum = data["memory"] + data["concentration"] + data["processingspeed"] + data["impulsiveness"];
            if (sum > 0)
            {
                var sortedData = new[]
                {
                    new {
                        Value = (data["memory"] / sum) * total,
                        Color = colors[0],
                        Icon = icons[0],
                        IconSize = iconSizes[0]
                    },
                    new {
                        Value = (data["concentration"] / sum) * total,
                        Color = colors[1],
                        Icon = icons[1],
                        IconSize = iconSizes[1]
                    },
                    new {
                        Value = (data["processingspeed"] / sum) * total,
                        Color = colors[2],
                        Icon = icons[2],
                        IconSize = iconSizes[2]
                    },
                    new {
                        Value = (data["impulsiveness"] / sum) * total,
                        Color = colors[3],
                        Icon = icons[3],
                        IconSize = iconSizes[3]
                    }
                }
                .OrderByDescending(x => x.Value)
                .ToArray();

                values = sortedData.Select(x => x.Value).ToArray();
                colors = sortedData.Select(x => x.Color).ToArray();
                icons = sortedData.Select(x => x.Icon).ToArray();
                iconSizes = sortedData.Select(x => x.IconSize).ToArray();

                ClearIcons();
                CreateIcons();
            }
            else
            {
                values = new float[] { 25f, 25f, 25f, 25f };
                colors = new[] { colors[0], colors[1], colors[2], colors[3] };
            }

            Debug.Log("차트 데이터 업데이트 완료!");
            StartChartAnimation(); // **애니메이션 시작**
        }
        else
        {
            Debug.LogError("GameManager에서 데이터 없음!");
        }
    }


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (values == null || values.Length == 0)
            return;

        vh.Clear();
        float angleOffset = 225f;
        float radius = rectTransform.rect.width / 2;
        float iconRadius = radius - (thickness / 2);

        for (int i = 0; i < values.Length; i++)
        {
            float angle = values[i] / total * 360f * animationProgress;

            DrawSegment(vh, radius, thickness, angleOffset, -angle, colors[i]);

            float midAngle = angleOffset + (-angle / 2f); // sweepAngle은 음수니까 중심각은 offset + 반절
            float rad = Mathf.Deg2Rad * midAngle;

            Vector3 iconPos = new Vector3(Mathf.Cos(rad) * iconRadius, Mathf.Sin(rad) * iconRadius, 0);
            if (iconImages[i] != null)
            {
                iconImages[i].rectTransform.anchoredPosition = iconPos;
                iconImages[i].rectTransform.sizeDelta = iconSizes[i];
            }

            angleOffset -= angle;
        }
    }

    


    void DrawSegment(VertexHelper vh, float radius, float thickness, float startAngle, float sweepAngle, Color color)
    {
        int segments = 20;
        float angleStep = sweepAngle / segments;
        int vertexStart = vh.currentVertCount;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * (startAngle + i * angleStep);
            Vector3 outer = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            Vector3 inner = new Vector3(Mathf.Cos(angle) * (radius - thickness), Mathf.Sin(angle) * (radius - thickness));

            vertex.position = outer;
            vh.AddVert(vertex);

            vertex.position = inner;
            vh.AddVert(vertex);
        }

        for (int i = 0; i < segments; i++)
        {
            int index = vertexStart + i * 2;
            vh.AddTriangle(index, index + 1, index + 3);
            vh.AddTriangle(index, index + 3, index + 2);
        }
    }
}