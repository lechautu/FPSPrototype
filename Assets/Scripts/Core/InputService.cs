using UnityEngine;

public sealed class InputService : MonoBehaviour
{
    [Header("Look")]
    public float sensitivityX = 0.12f;
    public float sensitivityY = 0.12f;

    public Vector2 LookDelta { get; private set; }
    public bool FireHeld { get; private set; }
    public bool ReloadPressed { get; private set; }
    public void UI_FireDown() => _uiFireHeld = true;
    public void UI_FireUp() => _uiFireHeld = false;
    private bool _uiFireHeld;


    private Vector2 _lastPointer;

    public void UpdateInput()
    {
        ReloadPressed = false;

#if UNITY_EDITOR || UNITY_STANDALONE
        // Right mouse drag to look
        if (Input.GetMouseButtonDown(1))
            _lastPointer = Input.mousePosition;

        if (Input.GetMouseButton(1))
        {
            Vector2 p = Input.mousePosition;
            Vector2 delta = p - _lastPointer;
            _lastPointer = p;
            LookDelta = new Vector2(delta.x * sensitivityX, delta.y * sensitivityY);
        }
        else
        {
            LookDelta = Vector2.zero;
        }

        FireHeld = Input.GetMouseButton(0);
        if (Input.GetKeyDown(KeyCode.R)) ReloadPressed = true;
#else
        // Touch: 1 finger drag for look, 2nd finger tap for fire (simple)
        LookDelta = Vector2.zero;
        FireHeld = false;

        if (Input.touchCount > 0)
        {
            var t0 = Input.GetTouch(0);
            if (t0.phase == TouchPhase.Moved)
                LookDelta = t0.deltaPosition * new Vector2(sensitivityX, sensitivityY);

            // if (Input.touchCount >= 2)
            //     FireHeld = true;
        }
        FireHeld = _uiFireHeld || (Input.touchCount >= 2);

        // Provide a UI button to call Reload() in mobile; keep here for completeness:
        // ReloadPressed can be set by UI event.
#endif
    }

    // Hook this to UI button on mobile
    public void UI_Reload() => ReloadPressed = true;
}
