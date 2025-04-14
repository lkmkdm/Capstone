using System.Collections;
using UnityEngine;
using Google;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GoogleAuthentication : MonoBehaviour
{
    public string webClientId = "481102876130-ci87efh89dkoodbp5fiemqd250lrplgs.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;
    private FirebaseAuth auth;
    private FirebaseFirestore db;

    void Awake()
    {
        // Firebase 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp firebaseApp = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseFirestore.DefaultInstance;
        });

        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true,
            UseGameSignIn = false,
            RequestEmail = true,
        };
    }

    public void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
            OnAuthenticationFinished, TaskScheduler.Default);
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.LogError("Google Sign-in Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.LogError("Unexpected sign-in exception: " + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Google Sign-in was canceled.");
        }
        else
        {
            AuthenticateWithFirebase(task.Result);
        }
    }

    // Firebase 인증 함수
    private void AuthenticateWithFirebase(GoogleSignInUser googleUser)
    {
        string idToken = googleUser.IdToken;
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Firebase Sign-in failed: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;
            Debug.Log("Firebase login success: " + newUser.Email);

            // Firestore에서 설문 결과 확인
            CheckSurveyResult(newUser.Email);
        });
    }

    private void CheckSurveyResult(string userEmail)
    {
        DocumentReference docRef = db
            .Collection("users")
            .Document(userEmail)
            .Collection("personal_information")
            .Document("info");

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to get Firestore document: " + task.Exception);
                return;
            }

            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists && snapshot.ContainsField("adhd_test") && snapshot.GetValue<bool>("adhd_test"))
            {
                Debug.Log("설문 완료 사용자입니다. MainScreen으로 이동합니다.");
                SceneManager.LoadScene("MainScreen");
            }
            else
            {
                Debug.Log("설문 미완료 사용자입니다. AdhdTest로 이동합니다.");
                SceneManager.LoadScene("AdhdTest");
            }
        });
    }

}

