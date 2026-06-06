using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("生命值")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("死亡设置")]
    public bool hideOnDeath = true;
    public float deathDelay = 0.1f;

    [Header("死亡反馈")]
    public bool showDeathEffect = true;
    public float deathEffectHeight = 1.0f;

    private bool isDead = false;

    void OnEnable()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;

        CancelInvoke();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;

        Debug.Log($"{name} 受到伤害：{damage}，剩余血量：{currentHealth}");

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

        Debug.Log($"{name} 已死亡");

        if (showDeathEffect)
        {
            CreateDeathEffect();
        }

        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnEnemyKilled();
        }

        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.OnEnemyDead();
        }

        if (hideOnDeath)
        {
            Invoke(nameof(HideEnemy), deathDelay);
        }
    }

    void HideEnemy()
    {
        gameObject.SetActive(false);
    }

    public bool IsDead()
    {
        return isDead;
    }

    void CreateDeathEffect()
    {
        GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        effect.name = "EnemyDeathEffect";
        effect.transform.position = transform.position + Vector3.up * deathEffectHeight;

        Collider col = effect.GetComponent<Collider>();
        if (col != null)
        {
            Destroy(col);
        }

        effect.AddComponent<EnemyDeathEffect>();
    }
}