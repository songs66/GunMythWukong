using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("玩家生命值")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth < 0f)
        {
            currentHealth = 0f;
        }

        if (DamageFlashUI.Instance != null)
        {
            DamageFlashUI.Instance.ShowDamageFlash();
        }

        Debug.Log($"玩家受到伤害：{damage}，剩余生命值：{currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        Debug.Log("玩家死亡，任务失败。");

        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnPlayerDead();
        }
    }

    public bool IsDead()
    {
        return isDead;
    }
}