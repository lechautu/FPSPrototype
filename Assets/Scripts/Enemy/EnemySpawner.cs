using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    public PoolManager pool;
    public EnemySystem enemySystem;

    [Header("Pool Key")]
    public string enemyKey = "Zombie";

    [Header("Spawn")]
    public int initialWaveCount = 40;
    public float spawnRadius = 14f;
    public float spawnArcDegrees = 90f;

    [Header("Enemy Stats")]
    public float hp = 60f;
    public float speed = 1.2f;
    public float attackRange = 1.1f;
    public float attackCooldown = 1.0f;

    public void SpawnWave(int count, Transform around)
    {
        for (int i = 0; i < count; i++)
        {
            var view = pool.Get<EnemyView>(enemyKey);

            Vector3 spawnPos = SampleArcSpawn(around.position, around.forward, spawnRadius, spawnArcDegrees);
            enemySystem.RegisterEnemy(view, spawnPos, hp, speed, attackRange, attackCooldown);
        }
    }

    private static Vector3 SampleArcSpawn(Vector3 center, Vector3 forward, float radius, float arcDeg)
    {
        float half = arcDeg * 0.5f;
        float ang = Random.Range(-half, half);
        Vector3 dir = Quaternion.Euler(0f, ang, 0f) * forward;
        return center + dir.normalized * radius;
    }
}
