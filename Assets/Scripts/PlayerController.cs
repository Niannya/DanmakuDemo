using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ===== 移动与低速模式设置 =====
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float focusSpeed = 5f;
    [SerializeField] private Vector2 minBounds = new Vector2(-8f, -4.5f);
    [SerializeField] private Vector2 maxBounds = new Vector2(8f, 4.5f);
    [SerializeField] private GameObject hitBoxIndicator;

    // ===== 射击设置 =====
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    // ===== 各位置子弹伤害 =====
    [SerializeField, Min(0f)]
    private float centerShotDamage = 1f;

    [SerializeField, Min(0f)]
    private float sideShotDamage = 0.25f;

    [SerializeField, Min(0f)]
    private float outerShotDamage = 0.15f;

    // ===== 火力等级设置与界面 =====
    [SerializeField, Min(1)]
    private int maxPowerLevel = 3;

    [SerializeField]
    private float sideShotOffset = 0.35f;

    [SerializeField]
    private TMP_Text powerText;

    // ===== 组件引用 =====
    private Rigidbody2D rb;

    // ===== 运行时状态 =====
    private Vector2 moveInput;
    private float currentSpeed;
    private float nextFireTime;
    private int powerLevel = 1;
    private bool isFocusing;

    // ===== Unity 生命周期函数 =====
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdatePowerText();
    }

    private void Update()
    {
        if (!GameManager.IsGameplayActive)
        {
            moveInput = Vector2.zero;
            hitBoxIndicator.SetActive(false);
            return;
        }

        isFocusing = Input.GetKey(KeyCode.LeftShift);
        hitBoxIndicator.SetActive(isFocusing);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Z) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        moveInput = new Vector2(horizontal, vertical).normalized;
    }

    private void FixedUpdate()
    {
        currentSpeed = isFocusing ? focusSpeed : moveSpeed;
        Vector2 nextPosition = rb.position + moveInput * currentSpeed * Time.fixedDeltaTime;
        nextPosition.x = Mathf.Clamp(nextPosition.x, minBounds.x, maxBounds.x);
        nextPosition.y = Mathf.Clamp(nextPosition.y, minBounds.y, maxBounds.y);
        rb.MovePosition(nextPosition);
    }

    private void OnDisable()
    {
        moveInput = Vector2.zero;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        if (hitBoxIndicator != null)
        {
            hitBoxIndicator.SetActive(false);
        }
    }

    // ===== 射击 =====
    private void Shoot()
    {
        Vector3 center = firePoint.position;

        CreateBullet(center, centerShotDamage);

        if (powerLevel >= 2)
        {
            CreateBullet(
                center + Vector3.left * sideShotOffset,
                sideShotDamage
            );

            CreateBullet(
                center + Vector3.right * sideShotOffset,
                sideShotDamage
            );
        }

        if (powerLevel >= 3)
        {
            CreateBullet(
                center + Vector3.left *
                sideShotOffset * 2f,
                outerShotDamage
            );

            CreateBullet(
                center + Vector3.right *
                sideShotOffset * 2f,
                outerShotDamage
            );
        }

        // 一轮可能生成多颗子弹，但只播放一次射击音效。
        SoundManager.Instance?.PlayPlayerShot();
    }

    private void CreateBullet(Vector3 position, float damage)
    {
        PlayerBullet bullet = null;

        if (BulletPool.Instance != null)
        {
            bullet = BulletPool.Instance.GetPlayerBullet(position);
        }

        if (bullet == null)
        {
            GameObject bulletObject = Instantiate(
                bulletPrefab,
                position,
                Quaternion.identity
            );

            bullet = bulletObject.GetComponent<PlayerBullet>();
        }

        if (bullet != null)
        {
            bullet.SetDamage(damage);
        }
    }

    // ===== 火力等级 =====
    public void AddPower()
    {
        powerLevel = Mathf.Min(
            maxPowerLevel,
            powerLevel + 1
        );

        UpdatePowerText();
    }

    public void LosePower()
    {
        powerLevel = Mathf.Max(1, powerLevel - 1);
        UpdatePowerText();
    }

    private void UpdatePowerText()
    {
        if (powerText == null)
        {
            return;
        }
        powerText.text = $"POWER {powerLevel} / {maxPowerLevel}";
    }
}
