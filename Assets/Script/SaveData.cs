using System;
using Firebase.Firestore;
using UnityEngine;

[FirestoreData]
public class SaveData
{
    private string userName = "TurboMaximus";
    private int health = 100;
    private float difficultyModifier = 1.3f;

    [FirestoreProperty]
    public string UserName
    {
        get => userName;
        set => userName = value;
    }

    [FirestoreProperty]
    public int Health
    {
        get => health;
        set => health = value;
    }

    [FirestoreProperty]
    public float DifficultyModifier
    {
        get => difficultyModifier;
        set => difficultyModifier = value;
    }
}
