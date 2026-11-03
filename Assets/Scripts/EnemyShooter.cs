using System.Collections;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    // 在 Inspector 中为敌人选择使用哪一种射击方式。
    private enum ShotPattern
    {
        Circle,
        Aimed,
        AimedFan
    }

    // ===== 通用射击设置 =====
    [SerializeField]
    private ShotPattern shotPattern;

    [SerializeField]
    private EnemyBullet bulletPrefab;

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private float fireInterval = 2f;

    // ===== 圆形弹幕设置 =====
    [SerializeField, Min(1)]
    private int bulletCount = 12;

    [SerializeField]
    private float rotationPerShot = 10f;

    // ===== 自机狙连射设置 =====
    [SerializeField, Min(1)]
    private int burstCount = 5;

    [SerializeField, Min(0.01f)]
    private float burstShotInterval = 0.12f;

    // ===== 自机狙扇形弹设置 =====
    [SerializeField, Min(1)]
    private int fanBulletCount = 5;

    [SerializeField, Range(0f, 180f)]
    private float fanAngle = 60f;

    // ===== 运行时引用与状态 =====
    private Transform player;
    private float nextFireTime;
    private float currentRotation;
    private bool isBursting;

    // ===== Unity 生命周期函数 =====
    private void Start()
    {
        PlayerController playerController =
            FindObjectOfType<PlayerController>();

        if (playerController != null)
        {
            player = playerController.transform;
        }
    }

    private void Update()
    {
        if (!GameManager.IsGameplayActive) return;

        if (Time.time < nextFireTime || isBursting) return;

        switch (shotPattern)
        {
            case ShotPattern.Circle:
                ShootCircle();
                break;
            case ShotPattern.Aimed:
                StartCoroutine(ShootAimedBurst());
                break;
            case ShotPattern.AimedFan:
                ShootAimedFan();
                break;
            default:
                break;
        }

        nextFireTime =
            Time.time + fireInterval;
    }

    // ===== 圆形弹幕 =====
    private void ShootCircle()
    {
        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle =
                angleStep * i + currentRotation;

            float radians =
                angle * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(
                Mathf.Cos(radians),
                Mathf.Sin(radians)
            );

            CreateBullet(direction);
        }

        SoundManager.Instance?.PlayEnemyShot();

        currentRotation += rotationPerShot;
        currentRotation %= 360f;
    }

    // ===== 自机狙连射 =====
    private IEnumerator ShootAimedBurst()
    {
        isBursting = true;

        for (int i = 0; i < burstCount; i++)
        {
            if (player == null) break;


            Vector2 direction =
                player.position - firePoint.position;

            CreateBullet(direction);
            SoundManager.Instance?.PlayEnemyShot();

            yield return new WaitForSeconds(
                burstShotInterval
            );
        }

        isBursting = false;
    }

    // ===== 自机狙扇形弹 =====
    private void ShootAimedFan()
    {
        if (player == null)
        {
            return;
        }

        Vector2 aimedDirection =
            player.position - firePoint.position;

        float centerAngle =
            Mathf.Atan2(aimedDirection.y, aimedDirection.x) *
            Mathf.Rad2Deg;

        if (fanBulletCount == 1)
        {
            CreateBullet(aimedDirection);
            SoundManager.Instance?.PlayEnemyShot();
            return;
        }

        float startAngle = centerAngle - fanAngle * 0.5f;
        float angleStep = fanAngle / (fanBulletCount - 1);

        for (int i = 0; i < fanBulletCount; i++)
        {
            float angle = startAngle + angleStep * i;
            float radians = angle * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(
                Mathf.Cos(radians),
                Mathf.Sin(radians)
            );

            CreateBullet(direction);
        }

        SoundManager.Instance?.PlayEnemyShot();
    }

    // ===== 子弹生成 =====
    private void CreateBullet(Vector2 direction)
    {
        EnemyBullet bullet = null;

        if (BulletPool.Instance != null)
        {
            bullet = BulletPool.Instance.GetEnemyBullet(
                firePoint.position
            );
        }

        if (bullet == null)
        {
            bullet = Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.identity
            );
        }

        bullet.SetDirection(direction);
    }

    // ===== 对外接口 =====
    // 供 Boss 阶段脚本在运行时切换圆形弹幕参数。
    public void SetCircleSettings(float interval, int count, float rotation)
    {
        StopAllCoroutines();
        isBursting = false;

        shotPattern = ShotPattern.Circle;
        fireInterval = interval;
        bulletCount = Mathf.Max(1, count);
        rotationPerShot = rotation;

        nextFireTime = Time.time + fireInterval;
    }

    // 供 Boss 阶段脚本在运行时切换自机狙扇形弹参数。
    public void SetAimedFanSettings(float interval, int count, float angle)
    {
        StopAllCoroutines();
        isBursting = false;

        shotPattern = ShotPattern.AimedFan;
        fireInterval = Mathf.Max(0.01f, interval);
        fanBulletCount = Mathf.Max(1, count);
        fanAngle = Mathf.Clamp(angle, 0f, 180f);

        nextFireTime = Time.time + fireInterval;
    }
}
