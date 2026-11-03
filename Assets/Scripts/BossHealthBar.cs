using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class BossHealthBar : MonoBehaviour
{
    // ===== 组件引用 =====
    private Slider slider;

    // ===== 运行时状态 =====
    private EnemyHealth bossHealth;

    // ===== Unity 生命周期函数 =====
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (bossHealth == null)
        {
            gameObject.SetActive(false);
            return;
        }

        slider.value = bossHealth.CurrentHealth;
    }

    // ===== 对外接口 =====
    // 开始显示并持续读取指定 Boss 的生命值。
    public void Show(EnemyHealth health)
    {
        bossHealth = health;

        slider.minValue = 0;
        slider.maxValue = bossHealth.MaxHealth;
        slider.value = bossHealth.CurrentHealth;

        gameObject.SetActive(true);
    }
}
