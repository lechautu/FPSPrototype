using UnityEngine;

public sealed class WaveDirector : MonoBehaviour
{
    public EnemySystem enemySystem;
    public EnemySpawner spawner;

    [Header("Wave")]
    public int startCount = 30;
    public int addPerWave = 10;
    public float timeBetweenWaves = 2.0f;

    [Header("Scaling (applied each new wave)")]
    public float hpMulPerWave = 1.10f;
    public float speedMulPerWave = 1.03f;

    public int WaveIndex { get; private set; }
    public int CurrentWaveCount { get; private set; }

    private bool _waiting;
    private float _nextWaveAt;

    private void Start()
    {
        WaveIndex = 0;
        SpawnWaveNow();
    }

    public void TickWave()
    {
        if (_waiting)
        {
            if (Time.time >= _nextWaveAt)
            {
                _waiting = false;
                SpawnWaveNow();
            }
            return;
        }

        if (enemySystem != null && enemySystem.AliveCount <= 0)
        {
            _waiting = true;
            _nextWaveAt = Time.time + timeBetweenWaves;
        }
    }

    private void SpawnWaveNow()
    {
        if (enemySystem == null || spawner == null || enemySystem.playerTarget == null) return;

        WaveIndex++;

        CurrentWaveCount = startCount + (WaveIndex - 1) * addPerWave;

        if (WaveIndex > 1)
        {
            spawner.hp *= hpMulPerWave;
            spawner.speed *= speedMulPerWave;
        }

        spawner.SpawnWave(CurrentWaveCount, enemySystem.playerTarget);
    }
}
