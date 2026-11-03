using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    // ===== 全局访问 =====
    public static BulletPool Instance { get; private set; }

    // ===== 子弹 Prefab =====
    [SerializeField]
    private PlayerBullet playerBulletPrefab;

    [SerializeField]
    private EnemyBullet enemyBulletPrefab;

    // ===== 预生成数量 =====
    [SerializeField, Min(0)]
    private int initialPlayerBulletCount = 50;

    [SerializeField, Min(0)]
    private int initialEnemyBulletCount = 200;

    // ===== 可用对象队列 =====
    private readonly Queue<PlayerBullet> playerBullets =
        new Queue<PlayerBullet>();

    private readonly Queue<EnemyBullet> enemyBullets =
        new Queue<EnemyBullet>();

    // ===== Unity 生命周期函数 =====
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        PreparePlayerBullets();
        PrepareEnemyBullets();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // ===== 玩家子弹 =====
    public PlayerBullet GetPlayerBullet(Vector3 position)
    {
        if (playerBulletPrefab == null) return null;

        PlayerBullet bullet = playerBullets.Count > 0
            ? playerBullets.Dequeue()
            : CreatePlayerBullet();

        bullet.transform.SetPositionAndRotation(
            position,
            Quaternion.identity
        );
        bullet.gameObject.SetActive(true);

        return bullet;
    }

    public void ReturnPlayerBullet(PlayerBullet bullet)
    {
        if (bullet == null || !bullet.gameObject.activeSelf) return;

        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(transform);
        playerBullets.Enqueue(bullet);
    }

    // ===== 敌人子弹 =====
    public EnemyBullet GetEnemyBullet(Vector3 position)
    {
        if (enemyBulletPrefab == null) return null;

        EnemyBullet bullet = enemyBullets.Count > 0
            ? enemyBullets.Dequeue()
            : CreateEnemyBullet();

        bullet.transform.SetPositionAndRotation(
            position,
            Quaternion.identity
        );
        bullet.gameObject.SetActive(true);

        return bullet;
    }

    public void ReturnEnemyBullet(EnemyBullet bullet)
    {
        if (bullet == null || !bullet.gameObject.activeSelf) return;

        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(transform);
        enemyBullets.Enqueue(bullet);
    }

    // ===== 对象预生成 =====
    private void PreparePlayerBullets()
    {
        if (playerBulletPrefab == null) return;

        for (int i = 0; i < initialPlayerBulletCount; i++)
        {
            PlayerBullet bullet = CreatePlayerBullet();
            playerBullets.Enqueue(bullet);
        }
    }

    private void PrepareEnemyBullets()
    {
        if (enemyBulletPrefab == null) return;

        for (int i = 0; i < initialEnemyBulletCount; i++)
        {
            EnemyBullet bullet = CreateEnemyBullet();
            enemyBullets.Enqueue(bullet);
        }
    }

    private PlayerBullet CreatePlayerBullet()
    {
        PlayerBullet bullet = Instantiate(
            playerBulletPrefab,
            transform
        );
        bullet.gameObject.SetActive(false);
        return bullet;
    }

    private EnemyBullet CreateEnemyBullet()
    {
        EnemyBullet bullet = Instantiate(
            enemyBulletPrefab,
            transform
        );
        bullet.gameObject.SetActive(false);
        return bullet;
    }
}
