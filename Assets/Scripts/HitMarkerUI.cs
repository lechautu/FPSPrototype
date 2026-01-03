using UnityEngine;
using UnityEngine.UI;

public sealed class HitMarkerUI : MonoBehaviour
{
    public CanvasGroup hitMarker;
    public float flashTime = 0.06f;

    private float _until;

    private void Awake()
    {
        if (hitMarker) SetAlpha(0f);
    }

    public void Flash()
    {
        _until = Time.time + flashTime;
        if (hitMarker) SetAlpha(1f);
    }

    private void Update()
    {
        if (!hitMarker) return;
        if (Time.time >= _until) SetAlpha(0f);
    }

    private void SetAlpha(float a)
    {
        hitMarker.alpha = a;
    }
}
