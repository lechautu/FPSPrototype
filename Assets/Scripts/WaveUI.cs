using UnityEngine;
using UnityEngine.UI;

public sealed class WaveUI : MonoBehaviour
{
    public WaveDirector waveDirector;
    public EnemySystem enemySystem;
    public Text label;

    private void Update()
    {
        if (!label || !waveDirector || !enemySystem) return;
        label.text = $"Wave {waveDirector.WaveIndex} | Alive {enemySystem.AliveCount}";
    }
}
