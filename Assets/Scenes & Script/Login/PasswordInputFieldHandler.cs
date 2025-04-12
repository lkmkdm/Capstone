using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class PasswordInputFieldHandler : MonoBehaviour
{
    [Header("필수 연결")]
    public TMP_InputField inputField;

    [Header("선택 사항")]
    public Button toggleVisibilityButton;
    public Sprite showIcon;
    public Sprite hideIcon;

    private bool isVisible = false;

    void Start()
    {
        inputField.contentType = TMP_InputField.ContentType.Password;
        inputField.onValidateInput += ValidateChar;
        inputField.ForceLabelUpdate();

        if (toggleVisibilityButton != null)
        {
            toggleVisibilityButton.onClick.AddListener(ToggleVisibility);
            UpdateToggleIcon();
        }
    }

    // 입력 전 유효성 검사: 허용 문자만 입력 (한글 입력 불가)
    private char ValidateChar(string text, int charIndex, char addedChar)
    {
        if (Regex.IsMatch(addedChar.ToString(), @"[a-zA-Z0-9!@#$%^&*()_+\-={}\[\]:;'""\\|,.<>/?]"))
        {
            return addedChar;
        }
        return '\0'; // 입력 무시
    }

    // 눈 버튼 눌러서 표시/숨김 전환
    void ToggleVisibility()
    {
        isVisible = !isVisible;

        inputField.contentType = isVisible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        inputField.ForceLabelUpdate();
        inputField.caretPosition = inputField.text.Length;

        UpdateToggleIcon();
    }

    void UpdateToggleIcon()
    {
        if (toggleVisibilityButton != null && showIcon != null && hideIcon != null)
        {
            var iconImage = toggleVisibilityButton.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = isVisible ? hideIcon : showIcon;
            }
        }
    }

    void LateUpdate()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (inputField.isFocused)
            Input.imeCompositionMode = IMECompositionMode.Off;
#endif
    }

    // 비밀번호 가져오기
    public string GetPassword()
    {
        return inputField.text;
    }
}
