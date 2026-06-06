using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("目标")]
    public Transform player;
    public PlayerHealth playerHealth;

    [Header("追击参数")]
    public float detectRange = 50f;
    public float attackRange = 2.5f;
    public float moveSpeed = 3.2f;
    public float rotateSpeed = 8f;

    [Header("近战攻击")]
    public float attackDamage = 10f;
    public float attackInterval = 1.0f;

    [Header("重力")]
    public float gravity = -9.81f;

    private CharacterController controller;
    private EnemyHealth enemyHealth;
    private float nextAttackTime = 0f;
    private float verticalVelocity = 0f;
    private bool isDead = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Start()
    {
        FindPlayerIfNeeded();
    }

    void OnEnable()
    {
        isDead = false;
        nextAttackTime = 0f;
        verticalVelocity = 0f;

        FindPlayerIfNeeded();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        if (enemyHealth != null && enemyHealth.IsDead())
        {
            return;
        }

        FindPlayerIfNeeded();

        if (player == null || playerHealth == null)
        {
            return;
        }

        HandleAI();
    }

    void FindPlayerIfNeeded()
    {
        if (player != null && playerHealth != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            return;
        }

        player = playerObject.transform;

        // 优先在 Player 根对象上找 PlayerHealth
        playerHealth = playerObject.GetComponent<PlayerHealth>();

        // 如果没找到，再向父物体找
        if (playerHealth == null)
        {
            playerHealth = playerObject.GetComponentInParent<PlayerHealth>();
        }

        // 如果还没找到，再向子物体找
        if (playerHealth == null)
        {
            playerHealth = playerObject.GetComponentInChildren<PlayerHealth>();
        }
    }

    void HandleAI()
    {
        Vector3 toPlayer = player.position - transform.position;

        // 只计算水平距离，避免高度差影响攻击判断
        toPlayer.y = 0f;

        float distance = toPlayer.magnitude;

        if (distance > detectRange)
        {
            ApplyGravityOnly();
            return;
        }

        RotateToPlayer(toPlayer);

        if (distance > attackRange)
        {
            ChasePlayer(toPlayer);
        }
        else
        {
            ApplyGravityOnly();
            TryAttackPlayer();
        }
    }

    void RotateToPlayer(Vector3 toPlayer)
    {
        if (toPlayer.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime
        );
    }

    void ChasePlayer(Vector3 toPlayer)
    {
        Vector3 moveDirection = toPlayer.normalized;
        Vector3 moveVelocity = moveDirection * moveSpeed;

        if (controller != null)
        {
            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            verticalVelocity += gravity * Time.deltaTime;
            moveVelocity.y = verticalVelocity;

            controller.Move(moveVelocity * Time.deltaTime);
        }
        else
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    void ApplyGravityOnly()
    {
        if (controller == null)
        {
            return;
        }

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        controller.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
    }

    void TryAttackPlayer()
    {
        if (Time.time < nextAttackTime)
        {
            return;
        }

        nextAttackTime = Time.time + attackInterval;

        if (playerHealth != null)
        {
            Debug.Log($"{name} 近战攻击玩家，造成 {attackDamage} 点伤害。");
            playerHealth.TakeDamage(attackDamage);
        }
        else
        {
            Debug.LogWarning($"{name} 没有找到 PlayerHealth，无法造成伤害。");
        }
    }

    public void OnEnemyDead()
    {
        isDead = true;
    }
}