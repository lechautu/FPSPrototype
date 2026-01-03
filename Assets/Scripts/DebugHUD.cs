using UnityEngine;

public sealed class DebugHUD : MonoBehaviour
{
    public EnemySystem enemySystem;

    private float _acc;
    private int _frames;
    private float _fps;

    private void Update()
    {
        _acc += Time.unscaledDeltaTime;
        _frames++;

        if (_acc >= 0.5f)
        {
            _fps = _frames / _acc;
            _acc = 0f;
            _frames = 0;
        }

        if (Input.GetKeyDown(KeyCode.F1))
            EnemyView.EnableAnimator = !EnemyView.EnableAnimator;

    }

    private void OnGUI()
    {
        int w = 280;
        int h = 90;
        GUI.Box(new Rect(10, 10, w, h), "Debug");
        GUI.Label(new Rect(20, 35, w, 20), $"FPS: {_fps:0.0}");
        // Nếu muốn, bạn có thể expose enemy count trong EnemySystem.
        // GUI.Label(new Rect(20, 55, w, 20), $"Enemies: {enemySystem.AliveCount}");
        GUI.Label(new Rect(20, 55, w, 20), $"GC: {System.GC.GetTotalMemory(false) / (1024 * 1024)} MB");        
    }
}
