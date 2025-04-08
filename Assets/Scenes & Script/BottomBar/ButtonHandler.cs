using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public void OnHomeButtonClick() { PanelSlider.Instance?.MoveToPanel(0); }
    public void OnGamesButtonClick() { PanelSlider.Instance?.MoveToPanel(1); }
    public void OnUserButtonClick() { PanelSlider.Instance?.MoveToPanel(2); }
    public void OnSettingsButtonClick() { PanelSlider.Instance?.MoveToPanel(3); }
}
