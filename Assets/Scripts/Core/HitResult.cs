using UnityEngine;

public readonly struct HitResult
{
    public readonly bool HasHit;
    public readonly Vector3 Point;
    public readonly Vector3 Normal;
    public readonly Collider Collider;
    public readonly int EnemyId;      // -1 if not an enemy
    public readonly bool IsHeadshot;

    public HitResult(bool hasHit, Vector3 point, Vector3 normal, Collider collider, int enemyId, bool isHeadshot)
    {
        HasHit = hasHit;
        Point = point;
        Normal = normal;
        Collider = collider;
        EnemyId = enemyId;
        IsHeadshot = isHeadshot;
    }

    public static HitResult NoHit => new HitResult(false, Vector3.zero, Vector3.up, null, -1, false);
}
