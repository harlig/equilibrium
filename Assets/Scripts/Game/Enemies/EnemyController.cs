using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private enum MoveDirection
    {
        TOWARDS_START_POSITION,
        TOWARDS_PATROL_POSITION
    }

    [SerializeField]
    private TextMeshPro hpTextElement;

    [SerializeField]
    private ProjectileBehavior projectile;

    // TODO use this
    // [SerializeField]
    // private Transform launchOffset;

    private const float MOVEMENT_SPEED = 1.0f;
    private const int MAX_HP = 10;

    private float hpRemaining;

    private Vector2 spawnPosition;

    private bool _isPatrolling;

    void Awake()
    {
        Debug.Log("Starting enemy");
        hpRemaining = MAX_HP;
        hpTextElement.text = $"{hpRemaining}";
    }

    public Vector2 GetPositionAsVector2()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public void StartPatrolling(Vector2 patrolPosition)
    {
        spawnPosition = GetPositionAsVector2();
        Debug.LogFormat(
            "spawn position of enemy: {0}; and this is where enemy is going: {1}",
            spawnPosition,
            patrolPosition
        );

        StartCoroutine(MoveObstacleOnPatrolCourse(MoveDirection.TOWARDS_PATROL_POSITION));

        IEnumerator MoveObstacleOnPatrolCourse(MoveDirection moveDirection)
        {
            // stop patrolling, bro is dead
            if (hpRemaining <= 0)
            {
                yield break;
            }

            Vector2 endPosition;
            if (moveDirection == MoveDirection.TOWARDS_START_POSITION)
            {
                endPosition = spawnPosition;
            }
            else if (moveDirection == MoveDirection.TOWARDS_PATROL_POSITION)
            {
                endPosition = patrolPosition;
            }
            else
            {
                throw new Exception("Unexpected move direction!");
            }

            _isPatrolling = true;

            // handle getting to the end of my patrol
            StartCoroutine(ChangePatrolDirectionWhenDonePatrolling(moveDirection));

            Vector2 curPosition = GetPositionAsVector2();
            MoveTowardsPosition(out float xDiff, out float yDiff, curPosition, endPosition);

            while (xDiff != 0 || yDiff != 0)
            {
                yield return new WaitForSeconds(0.13f);

                curPosition = GetPositionAsVector2();
                MoveTowardsPosition(out xDiff, out yDiff, curPosition, endPosition);
            }
            _isPatrolling = false;

            // end of patrol actions

            // fire projectile
            Debug.Log("Creating new projectil!");
            var newProjectile = Instantiate(
                projectile,
                transform.localToWorldMatrix.GetPosition(),
                Quaternion.identity
            );
            var randomX = UnityEngine.Random.Range(-1.0f, 1.0f);
            var randomY = UnityEngine.Random.Range(-1.0f, 1.0f);
            Vector2 launchDirection = new Vector2(randomX, randomY).normalized;

            // Calculate the rotation in 2D space to align with the launch direction and adjust by 90 degrees to handle long projectile
            float angle = Mathf.Atan2(launchDirection.y, launchDirection.x) * Mathf.Rad2Deg - 90;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            newProjectile.transform.rotation = rotation;

            newProjectile.MoveInDirection(launchDirection);

            // lose hp at end of patrol
            hpRemaining -= 1.0f;
            hpTextElement.text = $"{hpRemaining}";
        }

        IEnumerator ChangePatrolDirectionWhenDonePatrolling(MoveDirection oldMoveDirection)
        {
            while (_isPatrolling)
            {
                // can modify to determine how long should rest at the final position before switching directions
                yield return new WaitForSeconds(0.5f);
            }

            // I think this is okay because this execution of the method will end after the StartCoroutine call
            // and a new execution begins, but the old garbage is freed. if obstacle movement is slow this could be why though
            if (oldMoveDirection == MoveDirection.TOWARDS_START_POSITION)
            {
                StartCoroutine(MoveObstacleOnPatrolCourse(MoveDirection.TOWARDS_PATROL_POSITION));
            }
            else if (oldMoveDirection == MoveDirection.TOWARDS_PATROL_POSITION)
            {
                StartCoroutine(MoveObstacleOnPatrolCourse(MoveDirection.TOWARDS_START_POSITION));
            }
            else
            {
                throw new Exception("Unexpected move direction!");
            }
        }
    }

    // there be dragons in this math, it's probably not great lol
    private void MoveTowardsPosition(
        out float xDiff,
        out float yDiff,
        Vector2 curPosition,
        Vector2 endPosition
    )
    {
        xDiff = endPosition.x - curPosition.x;
        yDiff = endPosition.y - curPosition.y;

        if (xDiff == 0 && yDiff == 0)
            return;

        float totalDistanceToMove = Math.Abs(xDiff) + Math.Abs(yDiff);
        float moveRatioX = xDiff / totalDistanceToMove;
        float moveRatioY = yDiff / totalDistanceToMove;

        float moveDistanceX = moveRatioX * MOVEMENT_SPEED;
        float moveDistanceY = moveRatioY * MOVEMENT_SPEED;

        // we want to ensure that we can get back to a diff of exactly 0, so if the diff is smaller let's just take that
        float movementX = Math.Abs(moveDistanceX) > Math.Abs(xDiff) ? xDiff : moveDistanceX;
        float movementY = Math.Abs(moveDistanceY) > Math.Abs(yDiff) ? yDiff : moveDistanceY;

        // yikes!! gonna need some smoother movement
        transform.Translate(new Vector2(movementX, movementY));
    }
}
