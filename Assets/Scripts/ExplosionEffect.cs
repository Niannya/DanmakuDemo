using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ExplosionEffect : MonoBehaviour
{
    // ===== 生命周期设置 =====
    // 当前爆炸动画约为 0.417 秒，稍微多留一点时间再销毁对象。
    [SerializeField, Min(0f)]
    private float lifetime = 0.45f;

    // ===== Unity 生命周期函数 =====
    private void Awake()
    {
        // 游戏结束时 Time.timeScale 会变成 0，使用非缩放时间仍可播放动画。
        Animator animator = GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    private IEnumerator Start()
    {
        SoundManager.Instance?.PlayExplosion();

        // 等待真实时间，不受暂停或慢动作影响。
        yield return new WaitForSecondsRealtime(lifetime);
        Destroy(gameObject);
    }
}
