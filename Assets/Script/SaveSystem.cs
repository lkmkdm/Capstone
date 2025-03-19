using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private FirebaseFirestore firestore;
    public void Awake()
    {
        firestore = FirebaseFirestore.DefaultInstance;
    }

    public void SaveToCloud()
    {
        SaveData saveData = new();
        firestore.Document($"save_data/0").SetAsync(saveData);
    }

    public async void LoadFromCloud()
    {
        var snapshot = await firestore.Document($"save_data/0").GetSnapshotAsync();

        if(snapshot.Exists)
        {
            var data = snapshot.ConvertTo<SaveData>();
            Debug.Log($"userName: {data.UserName} "
                + $"health: {data.Health} "
                + $"difficultyModifier: {data.DifficultyModifier}");
        }
    }
}
