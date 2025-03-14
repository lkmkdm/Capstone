using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;

public class FirebaseAuthManager : MonoBehaviour
{

    private FirebaseAuth auth; // 로그인 회원가입 등에 사용
    private FirebaseUser user; // 인증이 완료된 유저 정보

    public InputField email;
    public InputField password;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void Create()
    {
        auth.CreateUserWithEmailAndPasswordAsync(email.text, password.text).ContinueWith(task => 
        {
            if (task.IsCanceled)
            {
                Debug.LogError("회원가입 취소");
                return;
            }
            if (task.IsFaulted) 
            {
                // 회원가입 실패 이유 => 이메일이 비정상 / 비밀번호가 너무 간단 / 이미 가입된 이메일 등등...
                Debug.LogError("회원가입 실패");
                return;
            }

            AuthResult authresult = task.Result;
            FirebaseUser user = authresult.User;
            Debug.LogError("회원가입 완료");
        });
    }

    public void Login()
    {
        auth.SignInWithEmailAndPasswordAsync(email.text, password.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("로그인 취소");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("로그인 실패");
                return;
            }

            AuthResult authresult = task.Result;
            FirebaseUser user = authresult.User;
            Debug.LogError("로그인 완료");
        });
    }

    public void LogOut()
    {
        auth.SignOut();
        Debug.Log("로그아웃");
    }
}
