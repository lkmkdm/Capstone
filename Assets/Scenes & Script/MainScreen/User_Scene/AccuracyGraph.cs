using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[ExecuteAlways]
public class AccuracyGraph : MaskableGraphic
{
    public float accuracyValue = 0f; // 0~100 기준
    public float maxAngle = 270f; // 225 ~ -270 도
    public float animationSpeed = 2f;

    private float currentValue = 0f;
    private float animationProgress = 0f;

    // 네 가지 색상 구간 추가
    public Color lowColor = new Color32(0xE3, 0xFD, 0xFD, 0xFF);      // #E3FDFD
    public Color midLowColor = new Color32(0xCB, 0xF1, 0xF5, 0xFF);   // #CBF1F5
    public Color midHighColor = new Color32(0xA6, 0xE3, 0xE9, 0xFF);  // #A6E3E9
    public Color highColor = new Color32(0x71, 0xC9, 0xCE, 0xFF);     // #71C9CE

    public float thickness = 25f;

    [SerializeField] private TextMeshProUGUI percentText; // Inspector에서 연결할 TMP 객체

    protected override void Start()
    {
        base.Start();
        Invoke("LoadAccuracyData", 1f); // GameManager 준비 대기
    }

    void LoadAccuracyData()
    {
        if (GameManager.Instance != null && GameManager.Instance.AbilityData != null)
        {
            accuracyValue = GameManager.Instance.AbilityData["accuracy"];
            StartCoroutine(AnimateToValue(accuracyValue));
        }
        else
        {
            Debug.LogError("GameManager 또는 데이터 없음");
        }
    }

    IEnumerator AnimateToValue(float target)
    {
        float start = currentValue;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * animationSpeed;
            currentValue = Mathf.Lerp(start, target, t);
            UpdatePercentText();
            SetVerticesDirty();
            yield return null;
        }

        currentValue = target;
        UpdatePercentText();
        SetVerticesDirty();
    }

    void UpdatePercentText()
    {
        if (percentText != null)
        {
            percentText.richText = true;
            percentText.text = string.Format("{0}<size=30><color=#767688>%</color></size>", Mathf.RoundToInt(currentValue));
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        float radius = rectTransform.rect.width / 2f;
        Vector2 center = rectTransform.rect.center;

        // 배경 반원 (#767688)
        Color backgroundColor = new Color32(0x76, 0x76, 0x88, 0xFF);
        DrawArc(vh, center, radius, 225f, -270f, backgroundColor, true);

        // 전경 반원 (컬러 - 현재 값까지)
        float sweep = Mathf.Clamp(currentValue, 0f, 100f) / 100f * -270f;
        Color valColor = GetGradientColor(currentValue);
        DrawArc(vh, center, radius, 225f, sweep, valColor, true);
    }

    Color GetGradientColor(float value)
    {
        if (value < 25f)
        {
            return Color.Lerp(lowColor, midLowColor, value / 25f);
        }
        else if (value < 50f)
        {
            return Color.Lerp(midLowColor, midHighColor, (value - 25f) / 25f);
        }
        else if (value < 75f)
        {
            return Color.Lerp(midHighColor, highColor, (value - 50f) / 25f);
        }
        else
        {
            return highColor;
        }
    }

    void DrawArc(VertexHelper vh, Vector2 center, float radius, float startAngle, float sweepAngle, Color color, bool roundCap = false)
    {
        int segments = 50;
        float angleStep = sweepAngle / segments;
        int vertexStart = vh.currentVertCount;

        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        Vector2? firstCenter = null;
        Vector2? lastCenter = null;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * (startAngle + i * angleStep);
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 outer = center + direction * radius;
            Vector2 inner = center + direction * (radius - thickness);

            if (i == 0) firstCenter = (outer + inner) / 2f;
            if (i == segments) lastCenter = (outer + inner) / 2f;

            vert.position = outer;
            vh.AddVert(vert);
            vert.position = inner;
            vh.AddVert(vert);
        }

        for (int i = 0; i < segments; i++)
        {
            int idx = vertexStart + i * 2;
            vh.AddTriangle(idx, idx + 1, idx + 3);
            vh.AddTriangle(idx, idx + 3, idx + 2);
        }

        if (roundCap && firstCenter.HasValue && lastCenter.HasValue)
        {
            float capRadius = thickness / 2f;
            DrawCircleCap(vh, firstCenter.Value, capRadius, color);
            DrawCircleCap(vh, lastCenter.Value, capRadius, color);
        }
    }

    void DrawCircleCap(VertexHelper vh, Vector2 center, float radius, Color color)
    {
        int segments = 10;
        float angleStep = 360f / segments;

        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        int startIndex = vh.currentVertCount;

        vert.position = center;
        vh.AddVert(vert);

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * (i * angleStep);
            Vector2 point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            vert.position = point;
            vh.AddVert(vert);
        }

        for (int i = 1; i <= segments; i++)
        {
            vh.AddTriangle(startIndex, startIndex + i, startIndex + i + 1);
        }
    }
}