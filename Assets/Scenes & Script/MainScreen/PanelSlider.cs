using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PanelSlider : MonoBehaviour
{
    public static PanelSlider Instance { get; private set; } // 싱글톤 인스턴스
    public RectTransform wholePannel;
    private float panelWidth = 1080f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(SetInitialPosition());
    }

    private IEnumerator SetInitialPosition()
    {
        yield return null; // 한 프레임 대기 후 실행
        wholePannel.anchoredPosition = new Vector2(1620, 0);
        // Debug.Log($"초기 위치 설정 완료: {wholePannel.anchoredPosition.x}, {wholePannel.anchoredPosition.y}");
    }

    public void MoveToPanel(int panelIndex)
{
    float targetX = 1620 - (panelWidth * panelIndex);  // 기준점(1620)에서 빼기
    Vector2 targetPos = new Vector2(targetX, wholePannel.anchoredPosition.y);

    //Debug.Log($"현재 wholePannel 위치: {wholePannel.anchoredPosition.x}, {wholePannel.anchoredPosition.y}");
    //Debug.Log($"이동할 위치: {targetPos.x}, {targetPos.y}");

    wholePannel.DOAnchorPos(targetPos, 0.3f).SetEase(Ease.OutCubic);
}

}