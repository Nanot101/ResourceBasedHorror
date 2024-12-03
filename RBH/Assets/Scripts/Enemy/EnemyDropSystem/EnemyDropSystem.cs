using System.Collections.Generic;
using UnityEngine;

public class EnemyDropSystem : MonoBehaviour
{
    [SerializeField]
    private float dropForce = 100.0f;

    [SerializeField]
    private EnemyDropItem dropItemPrefab;

    private readonly List<EnemyDropSO> enemyDropSOs = new();

    void Start()
    {
        Debug.Assert(dropItemPrefab != null, $"{nameof(dropItemPrefab)} is required for {nameof(EnemyDropSystem)}", this);
    }

    public void SetDrops(IEnumerable<EnemyDropSO> drops)
    {
        enemyDropSOs.Clear();
        enemyDropSOs.AddRange(drops);
    }

    public void DropItems()
    {
        if (enemyDropSOs.Count == 0)
        {
            return;
        }

        var dropDirection = GetRandomInitialDropDirection();
        var directionAngleStep = 360.0f / enemyDropSOs.Count;

        foreach (var dropSO in enemyDropSOs)
        {
            DropItem(dropSO, dropDirection);

            AddDirectionStep();
        }

        void AddDirectionStep()
        {
            var randomStep = directionAngleStep + Random.Range(-10.0f, 10.0f);

            dropDirection = Quaternion.AngleAxis(randomStep, Vector3.forward) * dropDirection;
        }
    }

    private void DropItem(EnemyDropSO dropSO, Vector2 dropDir)
    {
        var dropItem = Instantiate(dropItemPrefab, transform.position, transform.rotation);

        dropItem.SetDropSO(dropSO);

        var randomDropForce = dropForce + Random.Range(-15.0f, 15.0f);
        dropItem.AddDropForce(dropDir * randomDropForce);
    }

    private Vector2 GetRandomInitialDropDirection()
    {
        var randomDirection = RandomInsideUnitCircleNormailzed();

        while (randomDirection.magnitude == 0.0f)
        {
            randomDirection = RandomInsideUnitCircleNormailzed();
        }

        return randomDirection;

        static Vector2 RandomInsideUnitCircleNormailzed() => Random.insideUnitCircle.normalized;
    }
}
