using UnityEngine; // 유니티 핵심 타입 참조 InputHandler등
using LittleSword.InputSystem; // 인풋 처리 관련 타입 참조 MonoBehaviour등
using Logger = LittleSword.Common.Logger; // 개발용 조건부 로깅 유틸리티를 Logger이름으로 사용
using LittleSword.Player.Controller; // 플레이어 컨트롤러 관련 타입 참조
using LittleSword.Interfaces; //플레이어 스탯 관련 타입 참조
using System;

namespace LittleSword.Player // 플레이어 도메인
{
    // BasePlayer: 기본 플레이어 컴포넌트
    // - MonoBehaviour 상속으로 Unity 컴포넌트로 동작합니다.
    // - IDamageable 구현으로 피해 처리(TakeDamage, Die)를 제공합니다.
    // RequireComponent 속성은 에디터/런타임에서 해당 컴포넌트가 반드시 존재하도록 보장합니다:
    // InputHandler -> 입력 이벤트 제공
    // Rigidbody2D -> 물리 기반 이동/충돌 처리
    // CapsuleCollider2D -> 플레이어 충돌 영역

    [RequireComponent(typeof(InputHandler))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]

    public class BasePlayer : MonoBehaviour, IDamageable // 기본 플레이어 동작 담당 컴포넌트 
    {
        // Controllers
        private InputHandler inputHandler; // 입력 처리용 핸들러 컴포넌트 참조

        // 이동 제어용 컨트롤러 참조
        protected MovementController movementController;

        // 애니 제어용 컨트롤러 참조
        public AnimationController animationController;

        // Components
        protected Rigidbody2D rigidbody;
        protected SpriteRenderer spriteRenderer;
        protected Animator animator;
        protected Collider2D collider;

        //플레이어 스탯
        //[SerializeField] protected PlayerStats playerStats;
        public PlayerStats playerStats;

        // 현재 체력이 0 이하이면 사망 상태로 간주함
        public bool IsDead => CurrentHP <= 0;
        
        // 현재 체력을 보관하는 자동 프로퍼티. 초기값은 외부에서 할당하거나 Awake 등에서 설정해야 함
        public int CurrentHP {  get; set; }
        public event Action OnDie;


        #region 1. 유니티 이벤트
        protected void Awake() // 오브젝트 초기화 시점 호출
        {
            InitComponents(); // 컴포넌트 초기화 수행
            InitControllers(); // 컨트롤러(컴포넌트) 초기화 수행
        }

        //컨트롤러 활성화 시점 이벤트 구독 등록
        protected void OnEnable()
        {
            inputHandler.OnMove += Move; // 이동 입력 이벤트 Move 메서드에 연결
            inputHandler.OnAttack += Attack; // 공격 입력 이벤트 Attack 메서드에 연결
        }

        protected void OnDisable()
        {
            inputHandler.OnMove -= Move; // 이동 구독 해제
            inputHandler.OnAttack -= Attack; // 공격 구독 해제
        }
        #endregion

        #region 2. 초기화
        private void InitControllers() // 컴포넌트 참조를 획득하는 초기화 메서드
        {
            // 동일 게임오브젝트에서 InputHandler 컴포넌트 가져오기
            inputHandler = GetComponent<InputHandler>();

            // 이동 컨트롤러 인스턴스 
            movementController = new MovementController(rigidbody, spriteRenderer);

            // 애니 컨트롤러 인스턴스 생성
            animationController = new AnimationController(animator);
        }

        // 동일한 게임오브젝트 에서 리지드바디2D 컴포넌트를 조회하여 리지드파디에 할당함
        private void InitComponents()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            collider = GetComponent<Collider2D>();

            // 2D 횡스크롤 게임에서는 중력 효과를 제거하여 수평 이동만 가능하도록 설정
            rigidbody.gravityScale = 0;
            // rigidbody2D의 회전을 고정하여 물리적 충돌 시 회전하지 않도록 설정
            rigidbody.freezeRotation = true;


            CurrentHP = playerStats.maxHP; // 초기 체력을 최대 체력으로 설정
        }
        #endregion

        #region 3. 공통 메서드
        protected virtual void Move(Vector2 direction)
        {
            // 이동로직
            //Logger.Log($"이동 + {direction}"); // 개발/애디터 빌드 로그

            // 이동 처리
            movementController.Move(direction, playerStats.moveSpeed);

            // 조건이 true 면 이동 중인 상태
            animationController.Move(direction != Vector2.zero);

            // rigidbody.linearVelocity <= '유니티 6'로 넘어오면서 'Velocity'에서 'linearVelocity'로 변경 됨
            //rigidbody.linearVelocity = direction * 3.0f;
        }

        //  공격 입력 처리 메서드
        protected virtual void Attack()
        {
            // 공격 로직
            //Logger.Log("공격"); // 개발/애디터 빌드 로그

            // 공격 로직 구현
            animationController.Attack();
        }
        #endregion

        #region 4. 피격관련 메서드 
        // 지정된 피해만큼 체력을 감소시키고, 사망 여부에 따라 처리함
        public void TakeDamage(int damage)
        {
            if (IsDead) return; // 이미 사망이면 동작 수행 안함

            // CurrentHp는 0 미만으로 떨어지지 않도록 Mathf.Max로 클램프함
            CurrentHP = Mathf.Max(0, CurrentHP - damage);

            // 피해 적용 후 체력 0이면 Die 호출 아니면 데미지 애니메이션 호출
            if (IsDead)
            {
                Die(); // Die 호출
            }
            else
            {
                animationController.Hit(); // 피켝 애니메이션 호출
            }
        }

        // 사망 처리 수행
        public void Die()
        {
            OnDie?.Invoke(); // ⭐ 추가

            animationController.Die(); // 사망 애니메이션 호출

            // 사망 상태에서는 입력 처리 컴포넌트를 비활성화하여 이동과 공격을 막음
            inputHandler.enabled = false;

            // 사망시 이동 공격 없도록 콜라이더 비활성화
            collider.enabled = false;

            // 물리 시뮬레이션 중지 사망후 움직이지 못하게함
            rigidbody.linearVelocity = Vector3.zero;
        }

        #endregion
    }
}


