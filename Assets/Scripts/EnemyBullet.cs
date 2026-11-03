using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    // ===== 移动设置 =====
    [SerializeField] private float speed = 4f;

    [SerializeField]
    private float lifetime = 8f;

    // ===== 组件引用 =====
    private Rigidbody2D rb;

    // ===== Unity 生命周期函数 =====
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        rb.velocity = Vector2.zero;
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

    // ===== 碰撞处理 =====
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerLives player = other.GetComponent<PlayerLives>();
        if (player == null) return;

        player.LoseLife();
        Release();
    }

    // ===== 对外接口 =====
    // 为刚生成的子弹设置移动方向。
    public void SetDirection(Vector2 direction)
    {
        Vector2 moveDirection = direction.normalized;

        rb.velocity = moveDirection * speed;
        transform.up = -moveDirection;
    }

    // ===== 回收到对象池 =====
    public void Release()
    {
        if (!gameObject.activeSelf) return;

        if (BulletPool.Instance != null)
        {
            BulletPool.Instance.ReturnEnemyBullet(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
