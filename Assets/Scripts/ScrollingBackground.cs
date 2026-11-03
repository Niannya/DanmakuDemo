using UnityEngine;
using UnityEngine.Tilemaps;

public class ScrollingBackground : MonoBehaviour
{
    // ===== 背景引用 =====
    [SerializeField]
    private Tilemap backgroundA;

    [SerializeField]
    private Tilemap backgroundB;

    // ===== 滚动设置 =====
    [SerializeField]
    private float scrollSpeed = 1f;

    [SerializeField, Min(0f)]
    private float seamOverlap = 0.02f;

    // ===== 运行时尺寸 =====
    private float backgroundHeight;
    private float backgroundSpacing;

    // ===== Unity 生命周期函数 =====
    private void Start()
    {
        backgroundA.CompressBounds();

        backgroundHeight =
            backgroundA.localBounds.size.y;

        // 让两张背景稍微重叠，避免小数像素取整时露出黑色接缝。
        backgroundSpacing = Mathf.Max(
            0.01f,
            backgroundHeight - seamOverlap
        );

        backgroundA.transform.localPosition =
            Vector3.zero;

        backgroundB.transform.localPosition =
            Vector3.up * backgroundSpacing;
    }

    private void Update()
    {
        MoveBackground(backgroundA.transform);
        MoveBackground(backgroundB.transform);
    }

    // ===== 背景循环衔接 =====
    private void MoveBackground(Transform background)
    {
        background.localPosition +=
            Vector3.down *
            scrollSpeed *
            Time.deltaTime;

        if (background.localPosition.y <=
            -backgroundSpacing)
        {
            background.localPosition +=
                Vector3.up *
                backgroundSpacing *
                2f;
        }
    }
}
