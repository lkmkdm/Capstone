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
        // TMP_InputField�� �ֽ� �Է��� �ݿ��ϵ��� ���� ������Ʈ(����� ȯ�� ���� ����)
        emailInputField.ForceLabelUpdate();
        passwordInputField.ForceLabelUpdate();

        string email = emailInputField.text.Trim();
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            statusText.text = "�̸��ϰ� ��й�ȣ�� ��� �Է����ּ���.";
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                FirebaseUser user = task.Result.User;
                Debug.Log("�α��� ����: " + user.Email);

                // Firestore���� adhd_test �ʵ� Ȯ��
                DocumentReference docRef = db.Collection("users").Document(user.Email)
                    .Collection("personal_information").Document("info");

                docRef.GetSnapshotAsync().ContinueWithOnMainThread(snapshotTask =>
                {
                    if (snapshotTask.Result.Exists)
                    {
                        var doc = snapshotTask.Result;
                        
                        if (doc.ContainsField("adhd_test") && doc.GetValue<bool>("adhd_test") == true) // ���� �Ϸ� �� true
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
                        // ������ ������ ���� ������ �̵�
                        SceneManager.LoadScene("AdhdTest");
                    }
                });
            }
            else
            {
                Debug.LogError("�α��� ����: " + task.Exception);
                statusText.text = "�α��ο� �����߽��ϴ�. �ٽ� �õ����ּ���.";
            }
        });
    }
}
