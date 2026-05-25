using UnityEngine;
using LittleSword.Enemy.FSM;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using LittleSword.Enemy.Stats;
using System.Linq;
using LittleSword.Interfaces;
using LittleSword.Player;

namespace LittleSword.Enemy
{
    // 적 게임 오브젝ㅇ트에 붙여 사용하는 컨텍스트 컴포넌트
    // 내부적으로 state메췽을 보유하며 상태 전환/갱신을 위임함
    public class Enemy : MonoBehaviour, IDamageable
    {
        // 상태머신 입스턴스
        private StateMachine stateMachine;
        // 외부에서 상태머신에 접근할 수 있도록 프로퍼티로 노출
        public StateMachine StateMachine => stateMachine;

        // 상태 이름(에디터용)
        public string CurrentStateName => stateMachine?.currentState?.GetType().Name ?? "None";


        // 상태를 저장할 딕션너리 선언
        //private Dictionary<Type, IState> states;
        protected Dictionary<Type, IState> states;

        // 컴포넌트 캐싱 변수: GetComponent 호출 비용을 줄이기 위해 런타임 참조를 보관함
        [NonSerialized] public Rigidbody2D rigidbody;
        [NonSerialized] public SpriteRenderer spriteRenderer;
        [NonSerialized] public Animator animator;

        // 애니메이션 파라미터 해시: 문자열 대시 해시를 사용하여 SetTrigger/SetBool 호출 성능을 개선함
        public static readonly int hashIsRun = Animator.StringToHash("IsRun");
        public static readonly int hashAttack = Animator.StringToHash("Attack");
        public static readonly int hashDie = Animator.StringToHash("Die");
        public static readonly int hashHit = Animator.StringToHash("Hit");

        // State 
        // 적의 스탯을 ScriptableObject로 관리
        // [SerializeField] private EnemyStats enemyStats;
        [SerializeField] protected EnemyStats enemyStats;

        //추적 대상
        [SerializeField] private Transform target;
        public Transform Target => target; // 외부에서 타겟에 접근할 수 있도록 프로퍼티로 노출

        public bool IsDead => CurrentHP <= 0; // 사망 여부(읽기 전용). true면 Die()가 호출되어 처리된 상태임
        public int CurrentHP { get; private set; } // 현재 HP(읽기 전용). TakeDamage 등으로 내부에서 관리되어야 함s


        public LayerMask playerLayer;

        #region 상태관련 메서드
        // 제네릭 상태 전환 메서드: 타입 T(IState 구현)를 지정하여 미리 생성된 상태 인스턴스로 전환을 시도함
        // - T는 IState를 구현해야 함 (where T : IState)
        // - 내부 states 딕셔너리에서 typeof(T)로 상태 인스턴스를 조회함
        // - 조회에 성공하면 stateMachine.ChangeState로 전환을 위임함
        // - 조회 실패 시 아무 동작도 하지 않음(필요 시 로그 또는 예외 처리로 변경 가능)
        public void ChangeState<T>() where T : IState
        {
            // 사망 상태에서는 다른 상태로 전환하지 않도록 방어적 처리 
            // 사망 상태에ㅅ는 DieState로만 전환 허용, 그 외 상태로의 전환은 무시
            if (IsDead && typeof(T) != typeof(DieState)) return;

            if (states.TryGetValue(typeof(T), out IState newState))
            {
                // 상태 머신에 새 상태로 전환을 요청함 (현재 상태의 Exit, 새 상태의 Enter가 실행됨)
                stateMachine.ChangeState(newState);
            }
        }

        // 외부에서 상태 전환을 요청할 수 있도록 체인지스탯를 노출함
        // public void ChangeState(IState newState)
        //{
        // stateMachine.ChangeState(newState);
        //}

        // 주인공을 검출하는 메서드 : 검출 성공시 true 반환
        public bool DetectPlayer()
        {
            // 반경(추적거리) 내 해당 레이어의 모든 콜라이더를 수집
            Collider2D[] colliders =
                Physics2D.OverlapCircleAll(transform.position, enemyStats.chaseDistance, playerLayer);

            // 수집된 콜라이더가 하나 이상인지 확인
            if (colliders.Length > 0)
            {
                // 가장 가까운 콜라이더의 Transform을 target에 할당하기 위한 체인 시작
                target = colliders
                     .OrderBy(c => (transform.position - c.transform.position).sqrMagnitude)
                     .Where(c => c.GetComponent<BasePlayer>()?.IsDead == false) // 사망하지 않은 "Player" 만 
                     .First()
                     .transform;

                return true;

            }
            target = null; // 검출 실패시 target을 null로 초기화 
            return false; // 검출 실패 반환
        }

        // 추적 로직(깃허브용: NavMeshPlus, A* Pathfinding Project 등으로 구현할 수 있음)
        // 이번에는 플레이어를 직선으로 따라가는 간단한 로직으로 구현함
        public void MoveToPlayer()
        {
            if (target == null) return;

            SetFacing();

            // 이동방향 계산
            Vector2 direction = (target.position - transform.position).normalized;
            // Target 의 위치에 따라서 스프라이트 Flip
            // spriteRenderer.flipX = direction.x < 0;
            rigidbody.linearVelocity = direction * enemyStats.moveSpeed;
        }

        public void SetFacing()
        {
            if (spriteRenderer == null) return; // SpriteRenderer가 없으면 처리 중지(안전 검사)
            Vector2 dir = target.position - transform.position;  // 타겟 방향 벡터 계산(타겟 위치 - 자기 위치)
            spriteRenderer.flipX = dir.x < 0; // 방향벡터의 x < 0이면 왼쪽을 바라보도록 flip 설정
        }


        // 정지 로직
        public void StopMoving()
        {
            rigidbody.linearVelocity = Vector2.zero;

        }

        // 공격 사정거리 이내에 있는지 확인
        //public bool IsInAttackRange()
        //{
        //    // 타겟이 없으면 공격 불가
        //    if (target == null) return false;

        //    // 현재 오브젝트 위치와 타겟 위치 사이의 거리 제곱계산
        //    // sqrMagnitude를 사용하면 제곱근 연산을 피할 수 있어 성능이 향상됨 
        //    float targetDistance = (transform.position - transform.position).sqrMagnitude;

        //    // 거리 제곱이 공격 가능 거리 제곱보다 작거나 같으면 공격 범위 내에 있음
        //    return targetDistance <= enemyStats.attackDistance * enemyStats.attackDistance;
        //}

        public bool IsInAttackRange()
        {
            if (target == null) return false;

            float targetDistance = (transform.position - target.position).sqrMagnitude; // ⭐ 수정

            return targetDistance <= enemyStats.attackDistance * enemyStats.attackDistance;
        }


        #endregion

        #region 유니티 이벤트

        private void Awake()
        {
            // 상태 딕셔너리 초기화: 상태 타입별로 미리 생성한 IState 인스턴스를 보관함
            //InisState();
            InitStates();

            // 런타임에 사용할 컴포넌트들을 한 번만 조회하여 캐싱함(GetComponent 비용 절감 목적)
            InitComponents();
        }

        private void Start()
        {
            // 상태머신 초기화: this를 컨텍스트로 주입함
            stateMachine = new StateMachine(this);

            // 초기 상태 설정: 상태 머신을 시작 상태인 IdleState로 전환하여
            // 해당 상태의 Enter(enemy)가 호출되도록 함
            ChangeState<Idlestate>();
        }

        private void Update()
        {
            // 매 프레임 상태머신의 현재 산태를 갱신함
            stateMachine.Update();

            // 개발/테스트용 키 입력에 따라 상태 전환하는 코드 실행
            // TestFSM();
        }
        #endregion

        #region 초기화
        // 상태를 저장할 딕셔너리 선언 초기값 설정
        // 상태 타입 별로 미리 생성한 IState 인스턴스를 보관
        // 키: 상태 타입
        // 값: 해당 상태 구현체의 인스턴스
        // 현재는 즉시 생성(eager) 방식임(필요 시 지연 생성으로 변경 가능)
        protected virtual void InitStates() // ⭐ virtual 추가, 이름 수정
        {
            states = new Dictionary<Type, IState>
            {
                [typeof(Idlestate)] = new Idlestate(enemyStats.detecInterval),
                [typeof(ChaseState)] = new ChaseState(enemyStats.detecInterval),
                [typeof(AttackState)] = new AttackState(enemyStats.attackCooldown),
                [typeof(DieState)] = new DieState()
            };
        }

        // 컴포넌트 캐싱: 런타임에 GetComponent로 필요한 컴포넌트를 조회하여 필드에 저장함
        // (GetComponent 호출 비용을 줄이기 위해 Awake/초기화 시 한 번만 호출하는 것이 권장됨)

        private void InitComponents()
        {
            // 리지드바디2d 죄회 및 캐싱 - 물리 기반 이동/정지 제어 사용
            // 없으면 null 가능성있음 __RequireComponent__  추가 또는 null 검사 권장
            rigidbody = GetComponent<Rigidbody2D>();

            // 중력 효과 제거 (2D 플랫폼 게임에서 적이 공중에 떠 있도록 하는 경우 등)
            rigidbody.gravityScale = 0;

            // 회전 고정 (2D 게임에서 적이 회전하지 않도록 하는 경우 등)
            rigidbody.freezeRotation = true;

            //SpriteRenderer 조회및 캐싱 - 스프라이트 반향 전환 등 시각적 제어에 사용
            spriteRenderer = GetComponent<SpriteRenderer>();

            //Animator 조회및 캐싱 - 애니메이션 파라미터/트리거 제어에 사용
            animator = GetComponent<Animator>();

            CurrentHP = enemyStats.maxHP; // 초기 체력을 최대 체력으로 설정
        }

        #endregion

        #region 테스토 
        private void TestFSM()
        {
            // 숫자 1키를 누르면 Idle 상태로 전환
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                ChangeState<Idlestate>();

            }

            // 숫자 2키를 누르면 Idle 상태로 전환
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                ChangeState<ChaseState>();

            }

            // 숫자 3키를 누르면 Idle 상태로 전환
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                ChangeState<AttackState>();

            }
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmos()
        {
            // 기즈모 색상 설정(파랑) - 추적 범위용
            Gizmos.color = Color.blue;
            // Scene 뷰에 추적 범위를 와이어 구로 표시 
            Gizmos.DrawWireSphere(transform.position, enemyStats.chaseDistance);

            // 기즈모 색상 설정(빨강) - 공격 거리용
            Gizmos.color = Color.red;
            // 씬 뷰에 공격 가능 범위를 와이어 구로 표시 
            Gizmos.DrawWireSphere(transform.position, enemyStats.attackDistance);
        }
        #endregion

        #region 애니메이션 이벤트
        // 공격 애니메이션 이벤트에서 호출할 수 있는 메서드 예시
        public void OnAttackAnimationEvent()
        {
            if (target == null) return;

            target.GetComponent<IDamageable>()?.TakeDamage(enemyStats.attackDamage);
        }
        #endregion

        #region 데미지 처리 인터페이스 구현
        public void TakeDamage(int damage)
        {
            if (IsDead) return; // 이미 사망한 상태면 추가 피해 처리 중지(방어적 처리)

            // 데미지 처리
            CurrentHP -= damage; // 현재 HP에서 데미지만큼 감소

            if (IsDead)
            {
                Die(); // 사망 처리 호출
            }
            else
            {
                // 피격 애니메이션 트리거 설정(Hit)
                animator.SetTrigger(hashHit);
            }
        }

        public void Die()
        {
            // 사망 애니메이션 트리거 설정 (Die)
            //animator.SetTrigger(hashDie);

            // DieState로 상태 전환하여 사망 처리 로직 실행
            // DieState에서는 다른 상태로 전환 되면 안됨
            ChangeState<DieState>();
        }

        #endregion

        // 추적 범위 체크
        public bool IsInChaseRange()
        {
            if (target == null) return false;
            float dist = (transform.position - target.position).sqrMagnitude;
            return dist <= enemyStats.chaseDistance * enemyStats.chaseDistance;
        }

        // 플레이어 반대 방향으로 이동
        public void MoveAwayFromPlayer()
        {
            if (target == null) return;
            Vector2 direction = (transform.position - target.position).normalized;
            spriteRenderer.flipX = direction.x > 0; // 반대 방향이라 부호 반대
            rigidbody.linearVelocity = direction * enemyStats.moveSpeed;
        }
    }
}
