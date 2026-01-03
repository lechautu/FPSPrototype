using UnityEngine;

public sealed class CombatSystem : MonoBehaviour
{
    public LayerMask hitMask = ~0;
    public int maxHits = 8;

    private RaycastHit[] _hits;

    private void Awake()
    {
        _hits = new RaycastHit[Mathf.Max(1, maxHits)];
    }

    public HitResult ShootHitscan(Ray ray, float range)
    {
        int hitCount = Physics.RaycastNonAlloc(ray, _hits, range, hitMask, QueryTriggerInteraction.Ignore);
        if (hitCount <= 0) return HitResult.NoHit;

        int best = -1;
        float bestDist = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            float d = _hits[i].distance;
            if (d < bestDist)
            {
                bestDist = d;
                best = i;
            }
        }

        var h = _hits[best];
        int enemyId = -1;
        bool headshot = false;

        // EnemyView placed on root of pooled enemy
        var view = h.collider.GetComponentInParent<EnemyView>();
        if (view != null)
        {
            enemyId = view.EnemyId;
            headshot = h.collider.CompareTag("Head");
        }

        return new HitResult(true, h.point, h.normal, h.collider, enemyId, headshot);
    }
}
