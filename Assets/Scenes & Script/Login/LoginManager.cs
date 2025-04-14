using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using Firebase.Firestore;

public class LoginManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_Text statusText;

    private FirebaseAuth auth;
    private FirebaseFirestore db;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
        statusText.text = "";
    }

    public void OnClickLogin()
    {
        // TMP_InputField가 최신 입력을 반영하도록 강제 업데이트(모바일 환경 버그 수정)
        emailInputField.ForceLabelUpdate();
        passwordInputField.ForceLabelUpdate();

        string email = emailInputField.text.Trim();
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            statusText.text = "이메일과 비밀번호를 모두 입력해주세요.";
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                FirebaseUser user = task.Result.User;
                Debug.Log("로그인 성공: " + user.Email);

                // Firestore에서 adhd_test 필드 확인
                DocumentReference docRef = db.Collection("users").Document(user.Email)
                    .Collection("personal_information").Document("info");

                docRef.GetSnapshotAsync().ContinueWithOnMainThread(snapshotTask =>
                {
                    if (snapshotTask.Result.Exists)
                    {
                        var doc = snapshotTask.Result;
                        
                        if (doc.ContainsField("adhd_test") && doc.GetValue<bool>("adhd_test") == true) // 설문 완료 시 true
                        {
                            SceneManager.LoadScene("MainScreen");
                        }
                        else
                        {
                            SceneManager.LoadScene("AdhdTest");
                        }
                    }
                    else
                    {
                        // 문서가 없으면 설문 씬으로 이동
                        SceneManager.LoadScene("AdhdTest");
                    }
                });
            }
            else
            {
                Debug.LogError("로그인 실패: " + task.Exception);
                statusText.text = "로그인에 실패했습니다. 다시 시도해주세요.";
            }
        });
    }
}
