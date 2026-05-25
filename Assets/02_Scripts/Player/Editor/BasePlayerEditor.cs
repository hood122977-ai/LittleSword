using UnityEngine;
using LittleSword.Player;
using UnityEditor;


[CustomEditor(typeof(Warrior))]
public class BasePlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BasePlayer basePlayer = (BasePlayer)target;

        // 기본 인스펙터 
        DrawDefaultInspector();

        // PlayStats 필드
        basePlayer.playerStats.maxHP = EditorGUILayout.IntField("MaxHp", basePlayer.playerStats.maxHP);

        // 현재 HP 필드
        EditorGUILayout.LabelField("Current HP", basePlayer.CurrentHP.ToString());

        // 버튼 생성
        if (GUILayout.Button("피격"))
        {
            basePlayer.TakeDamage(10); // 10의 데미지를 입음
        }

        // 버튼 생성
        if (GUILayout.Button("초기화"))
        {
            // 현재 HP를 최대 HP로 초기화
            basePlayer.CurrentHP = basePlayer.playerStats.maxHP;
        }
    }
}
