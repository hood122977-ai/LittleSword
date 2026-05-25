namespace LittleSword.Interfaces
{
    // 생명(체력) 관련 동작 표준화 
    // 데미지 피해 받고 필요시 사망처리 (TakeDamage, DIe 호출)
    public interface IDamageable
    {
        // 객체 사망 여부 
        bool IsDead { get; }

        // 현재 HP 반환, HP 증감 관리
        int CurrentHP { get; }

        // 피해량 만큼 체력 감소
        // 구현체 피해 적용후 체력(CurrentHp) 0 이하일 경우 Die 호출 권장
        void TakeDamage(int damage);

        // 사망 처리 로직 
        // 중복 호출에 대해 방어적 처리 
        void Die();
    }
}
