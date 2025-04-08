using UnityEngine;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private FirebaseFirestore db;
    public Dictionary<string, float> AbilityData { get; private set; } = new Dictionary<string, float>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지
            db = FirebaseFirestore.DefaultInstance;
            FetchDataFromFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    async void FetchDataFromFirebase()
    {
        DocumentReference docRef = db.Collection("users").Document("testuser@example.com").Collection("ability").Document("current");
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            AbilityData["memory"] = snapshot.GetValue<float>("memory");
            AbilityData["concentration"] = snapshot.GetValue<float>("concentration");
            AbilityData["processingspeed"] = snapshot.GetValue<float>("processingspeed");
            AbilityData["impulsiveness"] = snapshot.GetValue<float>("impulsiveness");
            AbilityData["accuracy"] = snapshot.GetValue<float>("accuracy");

            Debug.Log("Firebase 데이터 로드 완료!");
            foreach (var entry in AbilityData)
            {
                Debug.Log($"{entry.Key}: {entry.Value}");
            }
        }
        else
        {
            Debug.LogError("Firebase 데이터 없음!");
        }
    }

}
