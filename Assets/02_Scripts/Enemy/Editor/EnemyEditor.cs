using UnityEngine;
using LittleSword.Enemy;
using LittleSword.Enemy.FSM;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Enemy))]
public class EnemyEditor : Editor
{
    // 커스텀 인스펙터를 통해 런타임에 상태 전환을 빠르게 태스트
    // 에디터에서 기본 인스펙터 그대로 렌더링, 플레이 중 상태 전환 버튼 활성화
    public override void OnInspectorGUI()
    {
        Enemy enemy = (Enemy)target;

        // 기본 인스펙터 내용 그리기 (serialized 필드 등 디본 UI 유저)
        DrawDefaultInspector();

        EditorGUILayout.Space(10); // 섹션 간 여백 추가

        // 아래 GUI는 플레이 모드에서만 활성화되도록 설정
        GUI.enabled = Application.isPlaying;

        // 현재 상태 이름을 표시함 (읽기 전용)
        EditorGUILayout.LabelField("현재 상태", enemy.CurrentStateName);

        EditorGUILayout.BeginHorizontal(); // 버튼을 한 줄에 배치
                        

        // Idle 상태 버튼: 클릭 시 Enemy의 제네릭 상태 전환 호출
        if (GUILayout.Button("Idle 상태"))
        {
            enemy.ChangeState<Idlestate>();
        }

        // Chase 상태 버튼
        if (GUILayout.Button("Chase 상태"))
        {
            enemy.ChangeState<ChaseState>();

        }

        // Attack 상태 버튼
        if (GUILayout.Button("Attact 상태"))
        {
            enemy.ChangeState<AttackState>();

        }

        EditorGUILayout.EndHorizontal();

        // GUI 상태 복원: 버튼 비활성화 해제
        GUI.enabled = true;

    }
}
