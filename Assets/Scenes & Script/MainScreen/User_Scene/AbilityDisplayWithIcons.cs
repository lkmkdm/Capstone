using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class AbilityDisplayWithIcons : MonoBehaviour
{
    [System.Serializable]
    public class AbilitySlot
    {
        public Image icon;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI percentText;
    }

    public List<AbilitySlot> slots = new List<AbilitySlot>();
    public Sprite memoryIcon, concentrationIcon, speedIcon, impulsivenessIcon;
    public TextMeshProUGUI totalScoreText; // 총합 텍스트 추가

    private Dictionary<string, string> fieldNames = new Dictionary<string, string>
    {
        { "memory", "기억력" },
        { "concentration", "집중력" },
        { "processingspeed", "처리 속도" },
        { "impulsiveness", "충동성" }
    };

    private Dictionary<string, Sprite> iconMap;

    void Awake()
    {
        // 아이콘 매핑
        iconMap = new Dictionary<string, Sprite>
        {
            { "memory", memoryIcon },
            { "concentration", concentrationIcon },
            { "processingspeed", speedIcon },
            { "impulsiveness", impulsivenessIcon }
        };
    }

    void OnEnable()
    {
        InvokeRepeating(nameof(CheckAndDisplay), 0f, 1f);
    }

    void OnDisable()
    {
        CancelInvoke(nameof(CheckAndDisplay));
    }

    void CheckAndDisplay()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance가 null입니다!");
            return;
        }

        var data = GameManager.Instance.AbilityData;
        if (data != null && data.Count > 0)
        {
            DisplayData(data);
            CancelInvoke(nameof(CheckAndDisplay));
        }
    }

    void DisplayData(Dictionary<string, float> data)
{
    // accuracy 제외하고 필터링
    var filteredData = data
        .Where(entry => fieldNames.ContainsKey(entry.Key))
        .ToDictionary(entry => entry.Key, entry => entry.Value);

    float total = filteredData.Values.Sum();
    var sorted = filteredData.OrderByDescending(entry => entry.Value).ToList();

    for (int i = 0; i < slots.Count && i < sorted.Count; i++)
    {
        string key = sorted[i].Key;
        float value = sorted[i].Value;

        string fieldName = fieldNames[key];
        slots[i].nameText.text = $"{fieldName}<size=25><color=#767688>({value})</color></size>";
        slots[i].percentText.text = $"{(value / total * 100):F1}%";
        slots[i].icon.sprite = iconMap[key];
    }

    // 총합 텍스트 업데이트
    if (totalScoreText != null)
    {
        totalScoreText.text = $"{total:F0}\n<b><size=30><color=#767688>총점</color></size></b>";
    }
}

}
