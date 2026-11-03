using UnityEngine;

public class PowerItem : MonoBehaviour
{
    // ===== 移动与生命周期设置 =====
    [SerializeField]
    private float fallSpeed = 2f;

    [SerializeField]
    private float destroyY = -6f;

    // ===== Unity 生命周期函数 =====
    private void Update()
    {
        transform.position +=
            Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }

    // ===== 拾取处理 =====
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player =
            other.GetComponent<PlayerController>();

        if (player == null)
        {
            return;
        }

        player.AddPower();
        Destroy(gameObject);
    }
}
