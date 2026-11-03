using UnityEngine;

[RequireComponent(typeof(EnemyShooter))]
public class EnemyMovement : MonoBehaviour
{
    // 在 Inspector 中选择敌人到达目标位置后的移动方式。
    private enum MovementPattern
    {
        Stop,
        HorizontalPatrol
    }

    // ===== 进入场景设置 =====
    [SerializeField]
    private MovementPattern movementPattern;

    [SerializeField]
    private float moveSpeed = 1.5f;

    [SerializeField]
    private float stopY = 2.5f;

    // ===== 左右移动设置 =====
    [SerializeField]
    private float horizontalSpeed = 2f;

    [SerializeField, Min(0f)]
    private float patrolDistance = 2f;

    // ===== 组件引用 =====
    private EnemyShooter shooter;

    // ===== 运行时状态 =====
    private bool hasReachedPosition;
    private float patrolCenterX;
    private float horizontalDirection = 1f;

    // ===== Unity 生命周期函数 =====
    private void Awake()
    {
        shooter = GetComponent<EnemyShooter>();
        shooter.enabled = false;
    }

    private void Update()
    {
        if (!hasReachedPosition)
        {
            MoveIntoPosition();
            return;
        }

        if (movementPattern == MovementPattern.HorizontalPatrol)
        {
            MoveHorizontally();
        }
    }

    // ===== 进入场景 =====
    private void MoveIntoPosition()
    {
        transform.position +=
            Vector3.down * moveSpeed * Time.deltaTime;

        if (transform.position.y > stopY)
        {
            return;
        }

        Vector3 position = transform.position;
        position.y = stopY;
        transform.position = position;

        patrolCenterX = position.x;
        hasReachedPosition = true;
        shooter.enabled = true;
    }

    // ===== 左右往返 =====
    private void MoveHorizontally()
    {
        Vector3 position = transform.position;

        position.x +=
            horizontalDirection * horizontalSpeed * Time.deltaTime;

        float leftBoundary = patrolCenterX - patrolDistance;
        float rightBoundary = patrolCenterX + patrolDistance;

        if (position.x >= rightBoundary)
        {
            position.x = rightBoundary;
            horizontalDirection = -1f;
        }
        else if (position.x <= leftBoundary)
        {
            position.x = leftBoundary;
            horizontalDirection = 1f;
        }

        transform.position = position;
    }
}
