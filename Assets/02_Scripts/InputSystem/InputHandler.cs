using UnityEngine; // Unity 핵심 타입(Vector2, MonoBehaviour 등)
using System; // 기본 System 네임스페이스
using UnityEngine.InputSystem; // 새 Input System 타임들
using Logger = LittleSword.Common.Logger; // 개발용 로깅 유틸리티 (조건부 컴파일)

namespace LittleSword.InputSystem
{
    // 입력 이벤트를 외부에 노출하는 핸들러 컴포넌트 
    public class InputHandler : MonoBehaviour, IInputEvents 
    {
        #region
        // 이동 입력을 전달하는 이벤트 (백터 페이로드)
        public event Action<Vector2> OnMove;

        // 공격 입력을 전달하는 이벤트 (페이로드 없음)
        public event Action OnAttack;

        // InputAction을 담는 래퍼 인스턴스
        private InputSystem_Actions inputActions;

        // 이동 액션 참조
        private InputAction moveAction;

        // 공격 액션 참조
        private InputAction attackAction;
        #endregion

        private void Awake()
        {
            // 액션 맵 인스턴스 생성
            inputActions = new InputSystem_Actions();

            // Player 액션맵에서 Move 액션 가져오기
            moveAction = inputActions.Player.Move;

            // Player 액션맵에서 Attack 액션 가져오기
            attackAction = inputActions.Player.Attack;
        }

        private void OnEnable()
        {
            // 액션 시스템 활성화
            inputActions.Enable();

            // 이동 입력 이벤트에 핸들러 연결
            moveAction.performed += HandleMove;

            //이동 입력이 취소(버튼/스틱) 될때 HandleMove를 호출 정지 
            moveAction.canceled += HandleMove;

            //공격 액션이 HandleAttack 콜백 호출 등록
            attackAction.performed += HandleAttack;

        }

        private void OnDisable()
        {
            //액션 시스템 비활성화
            inputActions.Disable();

            // 이동 입력 이벤트에서 핸들러 해제
            moveAction.performed -= HandleMove;

            //이동 액션 HandleMove 구독을 해제하여 중복 호출과 메무리 누수를 방지함
            moveAction.canceled -= HandleMove;

            // 공격 액션이 'performed' 상태가 될 때 HandleAttack 콜백을 호출하도록 등록합니다.
            attackAction.performed -= HandleAttack;
        }

        // 이동 입력이 발생했을 때 호출되는 콜백
        private void HandleMove(InputAction.CallbackContext ctx)
        {
            // 개발/에디터에서만 출력(Conditional).
            // ctx.ReadValue 호출 비용/자주 호출시 캐싱 고려
            //Logger.Log($"Move:{ctx.ReadValue<Vector2>()}");

            // 현재 이동 벡터를 로그로 출력
            //Debug.Log($"Move:{ctx.ReadValue<Vector2>()}");

            // 외부 구독자에게 이동 값 전달 (null 체크 후 호출)
            OnMove?.Invoke(ctx.ReadValue<Vector2>());
        }

        // 공격 입력이 발생했을 때 실행되는 콜백
        private void HandleAttack(InputAction.CallbackContext obj)
        {
            //Logger.Log("Attack");

            // 외부 구독자 공격 알리는 이벤트 호출(null 체크 포함)
            OnAttack?.Invoke();
        }
    }
}
