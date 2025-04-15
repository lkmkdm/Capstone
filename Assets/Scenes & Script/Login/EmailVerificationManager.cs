using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class EmailVerificationManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject RegisterPanel;
    public GameObject VerificationPanel;
    public GameObject CompletePanel;

    [Header("UI Elements")]
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_InputField nameInputField;
    public TMP_Text timerText;
    public TMP_Text UserEmailText;
    public TMP_Text UserNameText;
    public Button verifyButton;
    public Button gotoTestButton;
    public Button BackPanelButton;

    [Header("Validator")]
    public RegisterManager validator;

    private FirebaseAuth auth;
    private FirebaseUser currentUser;

    private float timerDuration = 3600f; // 1시간
    private bool timerRunning = false;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        RegisterPanel.SetActive(true);
        VerificationPanel.SetActive(false);
        CompletePanel.SetActive(false);

        if (BackPanelButton != null)
        {
            BackPanelButton.onClick.AddListener(OnClickBackPanel);
        }
    }

    public void OnClickNext()
    {
        Debug.Log("✅ [OnClickNext] 호출됨");

        validator.ValidateFields();

        if (!validator.AllFieldsValid())
        {
            Debug.Log("❌ [OnClickNext] 필드 유효하지 않음. 중단");
            return;
        }

        // ✅ 로딩창 표시
        if (LoadingScreenController.Instance != null)
            LoadingScreenController.Instance.loadingPanel.SetActive(true);

        // ✅ 회원가입 처리
        string email = emailInputField.text;
        string password = passwordInputField.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                currentUser = task.Result.User;
                Debug.Log("✅ 회원가입 성공");

                currentUser.SendEmailVerificationAsync().ContinueWithOnMainThread(sendTask =>
                {
                    if (sendTask.IsCompleted && !sendTask.IsFaulted)
                    {
                        UserEmailText.text = emailInputField.text;

                        RegisterPanel.SetActive(false);
                        VerificationPanel.SetActive(true);

                        DateTime startTime = DateTime.Now;
                        PlayerPrefs.SetString("email_verification_start_time", startTime.ToBinary().ToString());
                        timerRunning = true;
                    }
                    else
                    {
                        Debug.LogError("❌ 이메일 발송 실패: " + sendTask.Exception);
                    }

                    // ✅ 로딩창 숨기기
                    if (LoadingScreenController.Instance != null)
                        LoadingScreenController.Instance.loadingPanel.SetActive(false);
                });
            }
            else
            {
                Debug.LogError("❌ 회원가입 실패: " + task.Exception);

                // ✅ 실패 시에도 로딩창 닫기
                if (LoadingScreenController.Instance != null)
                    LoadingScreenController.Instance.loadingPanel.SetActive(false);
            }
        });
    }

    void Update()
    {
        if (timerRunning)
        {
            long binaryTime = Convert.ToInt64(PlayerPrefs.GetString("email_verification_start_time"));
            DateTime start = DateTime.FromBinary(binaryTime);
            float remaining = timerDuration - (float)(DateTime.Now - start).TotalSeconds;

            if (remaining > 0)
            {
                TimeSpan remainSpan = TimeSpan.FromSeconds(remaining);
                timerText.text = $"남은 시간: {remainSpan.Minutes:D2}:{remainSpan.Seconds:D2}";
            }
            else
            {
                timerRunning = false;
                timerText.text = "시간 초과. 계정이 삭제됩니다.";
                CheckAndDeleteUser();
            }
        }
    }

    public void OnClickVerify()
    {
        if (currentUser != null)
        {
            if (LoadingScreenController.Instance != null)
                LoadingScreenController.Instance.loadingPanel.SetActive(true);

            currentUser.ReloadAsync().ContinueWithOnMainThread(task =>
            {
                if (LoadingScreenController.Instance != null)
                    LoadingScreenController.Instance.loadingPanel.SetActive(false);

                if (task.IsCompleted && currentUser.IsEmailVerified)
                {
                    Debug.Log("✅ 이메일 인증 완료! 사용자 유지");
                    timerRunning = false;
                    PlayerPrefs.DeleteKey("email_verification_start_time");

                    UserNameText.text = $"환영합니다! {nameInputField.text}님!";
                    VerificationPanel.SetActive(false);
                    CompletePanel.SetActive(true);
                }
                else
                {
                    Debug.Log("⏳ 아직 인증되지 않았습니다.");
                }
            });
        }
    }

    void CheckAndDeleteUser()
    {
        if (currentUser != null)
        {
            currentUser.ReloadAsync().ContinueWith(task =>
            {
                if (!currentUser.IsEmailVerified)
                {
                    currentUser.DeleteAsync().ContinueWith(deleteTask =>
                    {
                        Debug.Log("🗑️ 계정 삭제 완료");
                        PlayerPrefs.DeleteKey("email_verification_start_time");
                        RegisterPanel.SetActive(true);
                        VerificationPanel.SetActive(false);
                    });
                }
            });
        }
    }

    public void OnClickGotoTest()
    {
        SceneManager.LoadScene("AdhdTest");
    }

    public void OnClickBackPanel()
    {
        RegisterPanel.SetActive(true);
        VerificationPanel.SetActive(false);
    }
}