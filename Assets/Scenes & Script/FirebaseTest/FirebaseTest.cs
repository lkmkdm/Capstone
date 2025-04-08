using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FirebaseTest : MonoBehaviour
{
    FirebaseFirestore db;

    void Start()
    {
        // Firestore 인스턴스 가져오기
        db = FirebaseFirestore.DefaultInstance;

        // 버튼을 찾아 클릭 이벤트 연결
        Button btn = GameObject.Find("SaveButton").GetComponent<Button>();
        btn.onClick.AddListener(SaveDataToFirestore);
    }

    async void SaveDataToFirestore()
    {
        string userEmail = "testuser@example.com"; // 테스트용 이메일

        // Firestore users 컬렉션에 문서 생성
        DocumentReference userDocRef = db.Collection("users").Document(userEmail);

        // 🔹 personal_information/info 문서 저장 (유저 기본 정보)
        Dictionary<string, object> personalInfo = new Dictionary<string, object>
        {
            { "age", 25 },
            { "difficulty", "hard" },
            { "gender", "male" },
            { "name", "TurboMaximus" },
            { "userid", "user123" }
        };
        await userDocRef.Collection("personal_information").Document("info").SetAsync(personalInfo);

        // 🔹 personal_information/original_ability 문서 저장 (변경되지 않는 능력치)
        Dictionary<string, object> originalAbility = new Dictionary<string, object>
        {
            { "original_accuracy", 80 },
            { "original_concentration", 85 },
            { "original_impulsiveness", 65 },
            { "original_memory", 78 },
            { "original_processingspeed", 72 }
        };
        await userDocRef.Collection("personal_information").Document("original_ability").SetAsync(originalAbility);

        // 🔹 ability 문서 저장 (게임 진행에 따라 업데이트될 값)
        Dictionary<string, object> ability = new Dictionary<string, object>
        {
            { "accuracy", 85 },
            { "concentration", 90 },
            { "impulsiveness", 70 },
            { "memory", 80 },
            { "processingspeed", 75 }
        };
        await userDocRef.Collection("ability").Document("current").SetAsync(ability);

        Debug.Log("🔥 Firestore에 데이터 저장 완료!");
    }
}
