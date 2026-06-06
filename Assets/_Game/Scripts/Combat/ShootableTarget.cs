using UnityEngine;

public class ShootableTarget : MonoBehaviour, IDamageable
{
    public float health = 100f;

    public void TakeDamage(float damage)
    {
        health -= damage;

        Debug.Log(name + " 受到伤害：" + damage + "，剩余血量：" + health);

        if (health <= 0f)
        {
            Debug.Log(name + " 被击毁。");
            gameObject.SetActive(false);
        }
    }
}