using UnityEngine;

public sealed class GameLoop : MonoBehaviour
{
    public InputService inputService;
    public CameraRig cameraRig;
    public WeaponController weaponController;

    public EnemySystem enemySystem;
    public EnemySpawner enemySpawner;
    public PoolManager pool;
    public WaveDirector waveDirector;

    public int stressSpawnCount = 20;

    [Header("Pool Keys")]
    public string enemyKey = "Zombie";

    private void Start()
    {
        // Spawn initial wave
        if (enemySpawner != null && enemySystem != null && enemySystem.playerTarget != null)
        {
            enemySpawner.enemyKey = enemyKey;
            enemySpawner.SpawnWave(enemySpawner.initialWaveCount, enemySystem.playerTarget);
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        float t = Time.time;

        inputService.UpdateInput();

        cameraRig.ApplyLook(inputService.LookDelta);
        cameraRig.Tick(dt);

        weaponController.Tick(t, dt, inputService.FireHeld, inputService.ReloadPressed);

        // Enemy tick + recycle
        enemySystem.Tick(t, dt, (view) =>
        {
            if (view != null)
                pool.Release(enemyKey, view.gameObject);
        });
        waveDirector.TickWave();

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.F2))
        {
            // Spawn zombie stress-test
            if (enemySpawner != null && enemySystem != null && enemySystem.playerTarget != null)
                enemySpawner.SpawnWave(stressSpawnCount, enemySystem.playerTarget);
        }
#endif

    }
}
