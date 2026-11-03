using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerLives : MonoBehaviour
{
    // ===== 残机与复活设置 =====
    [SerializeField, Min(1)]
    private int startingLives = 3;

    [SerializeField]
    private float respawnDelay = 1f;

    [SerializeField]
    private float invincibilityDuration = 2f;

    // ===== 被击特效设置 =====
    [SerializeField]
    private GameObject explosionPrefab;

    // ===== Bomb 设置 =====
    [SerializeField, Min(0)]
    private int startingBombs = 3;

    [SerializeField]
    private float bombInvincibilityDuration = 1.5f;

    [SerializeField]
    private TMP_Text bombsText;

    [SerializeField, Min(1)]
    private int bombDamage = 10;

    // ===== 场景与界面引用 =====
    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private TMP_Text livesText;

    // ===== 对外状态 =====
    public int CurrentLives => currentLives;

    // ===== 运行时状态 =====
    private int currentLives;
    private int currentBombs;
    private bool isRecovering;
    private Vector2 respawnPosition;

    // ===== 组件引用 =====
    private PlayerController controller;
    private Rigidbody2D rb;
    private Collider2D hitbox;
    private SpriteRenderer spriteRenderer;

    // ===== Unity 生命周期函数 =====
    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentLives = startingLives;
        respawnPosition = transform.position;

        currentBombs = startingBombs;

        UpdateLivesText();
        UpdateBombsText();
    }

    private void Update()
    {
        if (!GameManager.IsGameplayActive) return;

        if (Input.GetKeyDown(KeyCode.X))
        {
            UseBomb();
        }
    }

    // ===== 失去残机与复活 =====
    public void LoseLife()
    {
        if (isRecovering) return;
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        isRecovering = true;
        currentLives--;
        UpdateLivesText();
        ClearEnemyBullet();
        SpawnHitEffect();

        controller.enabled = false;
        rb.velocity = Vector2.zero;
        hitbox.enabled = false;
        spriteRenderer.enabled = false;

        if(currentLives <= 0)
        {
            gameManager.GameOver();
            yield break;
        }

        controller.LosePower();

        currentBombs = startingBombs;
        UpdateBombsText();

        yield return new WaitForSeconds(respawnDelay);

        // 先移动刚体，并等待物理系统把新位置同步到 Transform。
        // 如果立刻显示图片，玩家可能会在死亡地点短暂闪现一帧。
        rb.position = respawnPosition;
        yield return new WaitForFixedUpdate();

        spriteRenderer.enabled = true;
        controller.enabled = true;

        yield return new WaitForSeconds(invincibilityDuration);

        hitbox.enabled = true;
        isRecovering = false;
    }

    // ===== 被击特效 =====
    private void SpawnHitEffect()
    {
        if (explosionPrefab == null) return;

        Instantiate(
            explosionPrefab,
            transform.position,
            Quaternion.identity
        );
    }

    private void UpdateLivesText()
    {
        livesText.text = "LIVES X " + currentLives;
    }

    // ===== Bomb 使用 =====
    private void UseBomb()
    {
        if (currentBombs <= 0 || isRecovering)
        {
            return;
        }

        currentBombs--;
        UpdateBombsText();

        SoundManager.Instance?.PlayBomb();

        ClearEnemyBullet();
        DamageAllEnemies();

        StartCoroutine(BombRoutine());
    }

    private IEnumerator BombRoutine()
    {
        isRecovering = true;
        hitbox.enabled = false;

        yield return new WaitForSeconds(
            bombInvincibilityDuration
        );

        hitbox.enabled = true;
        isRecovering = false;
    }

    private void DamageAllEnemies()
    {
        EnemyHealth[] enemies =
            FindObjectsOfType<EnemyHealth>();

        foreach (var enemy in enemies)
        {
            enemy.TakeDamage(bombDamage);
        }
    }

    private void UpdateBombsText()
    {
        bombsText.text =
            "BOMBS X " + currentBombs;
    }

    // ===== 共用的敌弹清除功能 =====
    private void ClearEnemyBullet()
    {
        EnemyBullet[] bullets = FindObjectsOfType<EnemyBullet>();

        foreach (var bullet in bullets)
        {
            bullet.Release();
        }
    }
}
