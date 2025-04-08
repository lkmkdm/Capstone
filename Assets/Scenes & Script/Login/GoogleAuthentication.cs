using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Google;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Net;
using Firebase;
using Firebase.Extensions;
using Firebase.Auth; // Firebase ������ ���� ���ӽ����̽� �߰�

public class GoogleAuthentication : MonoBehaviour
{
    public string imageURL;
    public TMP_Text userNameTxt, userEmailTxt;
    public Image profilePic;
    public GameObject loginPanel, profilePanel;
    private GoogleSignInConfiguration configuration;
    public string webClientId = "481102876130-ci87efh89dkoodbp5fiemqd250lrplgs.apps.googleusercontent.com";

    private FirebaseAuth auth; // Firebase ���� ��ü

    void Awake()
    {
        // Firebase �ʱ�ȭ
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp firebaseApp = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
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
            using (IEnumerator<System.Exception> IEnumerator =
                task.Exception.InnerExceptions.GetEnumerator())
            {
                if (IEnumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                        (GoogleSignIn.SignInException)IEnumerator.Current;
                    Debug.LogError("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.LogError("Got unexpected exception!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Cancelled");
        }
        else
        {
            StartCoroutine(UpdateUI(task.Result));

            // Firebase ����
            AuthenticateWithFirebase(task.Result);
        }
    }

    IEnumerator UpdateUI(GoogleSignInUser user)
    {
        Debug.Log("Welcome: " +  user.DisplayName + "!");

        userNameTxt.text = user.DisplayName;
        userEmailTxt.text = user.Email;
        imageURL = user.ImageUrl.ToString();

        loginPanel.SetActive(false);
        yield return null;
        profilePanel.SetActive(true);

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageURL);
        yield return request.SendWebRequest();
        Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(request);
        Rect rect = new Rect(0, 0, downloadedTexture.width, downloadedTexture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        profilePic.sprite = Sprite.Create(downloadedTexture, rect, pivot);
    }

    // Firebase ���� �Լ�
    private void AuthenticateWithFirebase(GoogleSignInUser googleUser)
    {
        string idToken = googleUser.IdToken;

        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Sign-in with Firebase was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("Sign-in with Firebase failed: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;
            Debug.Log("User signed in successfully: " + newUser.DisplayName);
        });
    }


    public void OnSignOut()
    {
        userNameTxt.text = "";
        userEmailTxt.text = "";

        imageURL = "";
        loginPanel.SetActive(true);
        profilePanel.SetActive(false);
        Debug.Log("Calling SignOut");

        // Firebase���� �α׾ƿ�
        auth.SignOut();

        // Google �α��ο��� �α׾ƿ�
        GoogleSignIn.DefaultInstance.SignOut();
    }

}

