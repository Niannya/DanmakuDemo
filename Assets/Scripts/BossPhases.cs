using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyShooter))]
public class BossPhases : MonoBehaviour
{
    // ===== 第二阶段设置 =====
    [SerializeField, Range(0.1f, 0.9f)]
    private float phaseTwoHealthRatio = 0.5f;

    [SerializeField]
    private float phaseTwoFireInterval = 0.4f;

    [SerializeField, Min(1)]
    private int phaseTwoBulletCount = 28;

    [SerializeField]
    private float phaseTwoRotationPerShot = 15f;

    [SerializeField]
    private Color phaseTwoColor = new Color(1f, 0.4f, 0.4f);

    // ===== 第三阶段设置 =====
    [SerializeField, Range(0.05f, 0.8f)]
    private float phaseThreeHealthRatio = 0.25f;

    [SerializeField]
    private float phaseThreeFireInterval = 0.35f;

    [SerializeField, Min(1)]
    private int phaseThreeBulletCount = 9;

    [SerializeField, Range(0f, 180f)]
    private float phaseThreeFanAngle = 120f;

    [SerializeField]
    private Color phaseThreeColor = new Color(0.7f, 0.3f, 1f);

    // ===== 组件引用 =====
    private EnemyHealth health;
    private EnemyShooter shooter;
    private SpriteRenderer spriteRenderer;

    // ===== 运行时状态 =====
    private int currentPhase = 1;

    // ===== Unity 生命周期函数 =====
    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        shooter = GetComponent<EnemyShooter>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health.Died += OnBossDefeated;
    }

    private void Update()
    {
        float healthRatio =
            (float)health.CurrentHealth / health.MaxHealth;

        if (currentPhase < 3 &&
            healthRatio <= phaseThreeHealthRatio)
        {
            StartPhaseThree();
            return;
        }

        if (currentPhase < 2 &&
            healthRatio <= phaseTwoHealthRatio)
        {
            StartPhaseTwo();
        }
    }

    private void OnDestroy()
    {
        if (health != null) health.Died -= OnBossDefeated;
    }

    // ===== 阶段切换 =====
    // 生命值降到阈值后，只切换一次 Boss 的颜色和弹幕设置。
    private void StartPhaseTwo()
    {
        currentPhase = 2;

        shooter.SetCircleSettings(
            phaseTwoFireInterval,
            phaseTwoBulletCount,
            phaseTwoRotationPerShot
        );

        if (spriteRenderer != null)
        {
            spriteRenderer.color = phaseTwoColor;
        }

        Debug.Log("Boss 进入第二阶段");
    }

    private void StartPhaseThree()
    {
        currentPhase = 3;

        shooter.SetAimedFanSettings(
            phaseThreeFireInterval,
            phaseThreeBulletCount,
            phaseThreeFanAngle
        );

        if (spriteRenderer != null)
        {
            spriteRenderer.color = phaseThreeColor;
        }

        Debug.Log("Boss 进入第三阶段");
    }

    // ===== Boss 击败处理 =====
    private void OnBossDefeated()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null) gameManager.GameClear();
    }
}
