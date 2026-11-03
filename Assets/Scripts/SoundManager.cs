using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    // ===== 全局访问 =====
    public static SoundManager Instance { get; private set; }

    // ===== 音效资源 =====
    [SerializeField]
    private AudioClip playerShotClip;

    [SerializeField]
    private AudioClip enemyShotClip;

    [SerializeField]
    private AudioClip explosionClip;

    [SerializeField]
    private AudioClip bombClip;

    // ===== 组件引用 =====
    private AudioSource audioSource;

    // ===== Unity 生命周期函数 =====
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    // ===== 对外播放接口 =====
    public void PlayPlayerShot()
    {
        PlaySound(playerShotClip);
    }

    public void PlayEnemyShot()
    {
        PlaySound(enemyShotClip);
    }

    public void PlayExplosion()
    {
        PlaySound(explosionClip);
    }

    public void PlayBomb()
    {
        PlaySound(bombClip);
    }

    // ===== 共用播放逻辑 =====
    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;

        audioSource.PlayOneShot(clip);
    }
}
