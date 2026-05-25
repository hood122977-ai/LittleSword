using UnityEngine; // 델리게이트(Action) 사용을 위한 네임스페이스 
using System; // Vector2 등 Unity 타입 사용

namespace LittleSword.InputSystem
{
    // 입력 관련 이벤트를 외부에 노출하기 위한 인터페이스
    public interface IInputEvents
    {
        // 이동 입력 발생 시 전달되는 이벤트
        // 페이로드: Vector2 (x = 수평, Y = 수직)
        event Action<Vector2> OnMove;

        // 공격 입력 발생 시 전달되는 이벤트 (페이로드 없음)
        event Action OnAttack;
    }

}
