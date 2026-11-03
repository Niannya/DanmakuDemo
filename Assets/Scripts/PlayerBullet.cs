using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    // ===== 移动设置 =====
    [SerializeField] private float speed = 10f;

    [SerializeField]
    private float lifetime = 5f;

    // ===== 伤害状态 =====
    public float Damage => damage;

    private float damage = 1f;

    // ===== 组件引用 =====
    private Rigidbody2D rb;

    // ===== Unity 生命周期函数 =====
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        rb.velocity = Vector2.up * speed;
        Invoke(nameof(Release), lifetime);
    }

    private void OnDisable()
    {
        CancelInvoke();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    // ===== 回收到对象池 =====
    public void Release()
    {
        if (!gameObject.activeSelf) return;

        if (BulletPool.Instance != null)
        {
            BulletPool.Instance.ReturnPlayerBullet(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 每次发射时设置本次子弹的伤害，适用于对象池重复使用。
    public void SetDamage(float amount)
    {
        damage = Mathf.Max(0f, amount);
    }
}
