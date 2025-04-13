using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Text.RegularExpressions;

public class RegisterManager : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite errorSprite;
    public Sprite defaultSprite;

    [Header("Name Panel")]
    public Image nameImage;
    public TMP_InputField nameInputField;
    public TMP_Text nameText;
    public GameObject nameErrorImage;
    public TMP_Text nameErrorText;

    [Header("Email Panel")]
    public Image emailImage;
    public TMP_InputField emailInputField;
    public TMP_Text emailText;
    public GameObject emailErrorImage;
    public TMP_Text emailErrorText;

    [Header("Password Panel")]
    public Image passwordImage;
    public TMP_InputField passwordInputField;
    public TMP_Text passwordText;
    public GameObject passwordErrorImage;
    public TMP_Text passwordErrorText;

    [Header("Confirm Password Panel")]
    public Image confirmImage;
    public TMP_InputField confirmInputField;
    public TMP_Text confirmText;
    public GameObject confirmErrorImage;
    public TMP_Text confirmErrorText;

    [Header("Text Color")]
    public Color defaultTextColor = Color.black;
    public Color errorTextColor = Color.red;

    private bool focusSet = false;

    public void ValidateFields()
    {
        focusSet = false;

        if (string.IsNullOrWhiteSpace(nameInputField.text))
        {
            ApplyErrorState(nameImage, nameText, nameErrorImage, nameInputField);
            nameErrorText.text = "이름을 입력해주세요.";
            ClearErrorState(emailImage, emailText, emailErrorImage);
            ClearErrorState(passwordImage, passwordText, passwordErrorImage);
            ClearErrorState(confirmImage, confirmText, confirmErrorImage);
            return;
        }

        if (string.IsNullOrWhiteSpace(emailInputField.text))
        {
            ClearErrorState(nameImage, nameText, nameErrorImage);
            ApplyErrorState(emailImage, emailText, emailErrorImage, emailInputField);
            emailErrorText.text = "이메일을 입력해주세요.";
            ClearErrorState(passwordImage, passwordText, passwordErrorImage);
            ClearErrorState(confirmImage, confirmText, confirmErrorImage);
            return;
        }
        else if (!IsValidEmail(emailInputField.text))
        {
            ClearErrorState(nameImage, nameText, nameErrorImage);
            ApplyErrorState(emailImage, emailText, emailErrorImage, emailInputField);
            emailErrorText.text = "이메일 형식이 올바르지 않습니다.\n@ 기호와 .이 포함된 도메인을 사용해주세요.";
            ClearErrorState(passwordImage, passwordText, passwordErrorImage);
            ClearErrorState(confirmImage, confirmText, confirmErrorImage);
            return;
        }

        if (string.IsNullOrWhiteSpace(passwordInputField.text))
        {
            ClearErrorState(nameImage, nameText, nameErrorImage);
            ClearErrorState(emailImage, emailText, emailErrorImage);
            ApplyErrorState(passwordImage, passwordText, passwordErrorImage, passwordInputField);
            passwordErrorText.text = "비밀번호를 입력해주세요.";
            ClearErrorState(confirmImage, confirmText, confirmErrorImage);
            return;
        }
        else if (passwordInputField.text.Length < 8 || passwordInputField.text.Length > 16)
        {
            ClearErrorState(nameImage, nameText, nameErrorImage);
            ClearErrorState(emailImage, emailText, emailErrorImage);
            ApplyErrorState(passwordImage, passwordText, passwordErrorImage, passwordInputField);
            passwordErrorText.text = "비밀번호는 8자 이상 16자 이하로 입력해주세요.";
            ClearErrorState(confirmImage, confirmText, confirmErrorImage);
            return;
        }
        else if (HasRepeatedCharacters(passwordInputField.text))
        {
            ClearErrorState(nameImage, nameText, nameErrorImage);
            ClearErrorState(emailImage, emailText, emailErrorImage);
            ApplyErrorState(passwordImage, passwordText, passwordErrorImage, passwordInputField);
            passwordErrorText.text = "동일한 문자를 3번 이상 복당할 수 없습니다.";
            ClearErrorState(confirmImage, confirmText, confirmErrorImage);
            return;
        }

        if (string.IsNullOrWhiteSpace(confirmInputField.text))
        {
            ClearErrorState(nameImage, nameText, nameErrorImage);
            ClearErrorState(emailImage, emailText, emailErrorImage);
            ClearErrorState(passwordImage, passwordText, passwordErrorImage);
            ApplyErrorState(confirmImage, confirmText, confirmErrorImage, confirmInputField);
            confirmErrorText.text = "비밀번호 확인을 입력해주세요.";
            return;
        }
        else if (passwordInputField.text != confirmInputField.text)
        {
            ClearErrorState(nameImage, nameText, nameErrorImage);
            ClearErrorState(emailImage, emailText, emailErrorImage);
            ClearErrorState(passwordImage, passwordText, passwordErrorImage);
            ApplyErrorState(confirmImage, confirmText, confirmErrorImage, confirmInputField);
            confirmErrorText.text = "비밀번호가 일치하지 않습니다.";
            return;
        }

        ClearErrorState(nameImage, nameText, nameErrorImage);
        ClearErrorState(emailImage, emailText, emailErrorImage);
        ClearErrorState(passwordImage, passwordText, passwordErrorImage);
        ClearErrorState(confirmImage, confirmText, confirmErrorImage);

        Debug.Log("모든 필드가 유효합니다. ✅");
    }

    public bool AllFieldsValid()
    {
        return !nameErrorImage.activeSelf &&
               !emailErrorImage.activeSelf &&
               !passwordErrorImage.activeSelf &&
               !confirmErrorImage.activeSelf;
    }

    void ApplyErrorState(Image img, TMP_Text txt, GameObject errorObj, TMP_InputField input)
    {
        if (errorSprite != null) img.sprite = errorSprite;
        txt.color = errorTextColor;
        errorObj.SetActive(true);

        if (!focusSet)
        {
            StartCoroutine(DelayedFocus(input));
            focusSet = true;
        }
    }

    void ClearErrorState(Image img, TMP_Text txt, GameObject errorObj)
    {
        txt.color = defaultTextColor;
        errorObj.SetActive(false);

        if (defaultSprite != null) img.sprite = defaultSprite;
    }

    IEnumerator DelayedFocus(TMP_InputField input)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        yield return null;
        input.ActivateInputField();
    }

    bool IsValidEmail(string email)
    {
        // 엄격한 정규식 검사: @, 도메인, . 포함 + 특수 문자 제한
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }

    bool HasRepeatedCharacters(string input)
    {
        for (int i = 0; i < input.Length - 2; i++)
        {
            char a = input[i];
            char b = input[i + 1];
            char c = input[i + 2];

            if (a == b && b == c)
                return true;
        }
        return false;
    }
}