using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    // ===== 生命值与奖励设置 =====
    [SerializeField, Min(0.01f)]
    private float health = 3f;
    [SerializeField]
    private int scoreValue = 100;

    // ===== 道具掉落设置 =====
    [SerializeField]
    private GameObject powerItemPrefab;

    [SerializeField, Range(0f, 1f)]
    private float powerItemDropChance = 0.3f;

    // ===== 死亡特效设置 =====
    [SerializeField]
    private GameObject explosionPrefab;

    // ===== 对外状态与事件 =====
    public float CurrentHealth => health;
    public float MaxHealth => maxHealth;
    public event Action Died;

    // ===== 运行时状态 =====
    private float maxHealth;
    private bool isDead;

    // ===== Unity 生命周期函数 =====
    private void Awake()
    {
        maxHealth = health;
    }

    // ===== 碰撞处理 =====
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerBullet bullet = other.GetComponent<PlayerBullet>();

        if (bullet != null)
        {
            float damage = bullet.Damage;
            bullet.Release();
            TakeDamage(damage);
            return;
        }

        PlayerLives player =
            other.GetComponent<PlayerLives>();

        if (player != null)
        {
            player.LoseLife();
        }
    }

    // ===== 受伤与死亡 =====
    // 所有伤害来源统一调用此方法，确保死亡只在一个地方处理。
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            isDead = true;

            GameManager gameManager = FindObjectOfType<GameManager>();

            if (gameManager != null) gameManager.AddScore(scoreValue);

            TryDropPowerItem();
            SpawnDeathEffect();
            Died?.Invoke();

            Destroy(gameObject);
        }
    }

    // ===== 死亡特效 =====
    private void SpawnDeathEffect()
    {
        if (explosionPrefab == null) return;

        Instantiate(
            explosionPrefab,
            transform.position,
            Quaternion.identity
        );
    }

    // ===== 道具掉落 =====
    private void TryDropPowerItem()
    {
        if (powerItemPrefab == null) return;

        if (UnityEngine.Random.value >
            powerItemDropChance)
        {
            return;
        }

        Instantiate(
            powerItemPrefab,
            transform.position,
            Quaternion.identity
        );
    }
}
