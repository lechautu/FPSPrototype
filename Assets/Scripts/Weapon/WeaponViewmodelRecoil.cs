using UnityEngine;

public sealed class WeaponViewmodelRecoil : MonoBehaviour
{
    [Header("Kick (fast)")]
    public float kickPosZ = 0.06f;          // local meters (scale depends on your gun size)
    public float kickPosY = 0.01f;
    public float kickRotX = 8f;             // degrees (up)
    public float kickRotZ = 2f;             // roll
    public float kickSpeed = 120f;          // higher = snappier kick

    [Header("Return (slower)")]
    public float returnSpeed = 22f;         // settle back
    [Range(0f, 1f)] public float damping = 0.78f;

    [Header("Auto-fire accumulation limits")]
    public float maxPosZ = 0.12f;
    public float maxRotX = 18f;

    private Vector3 _baseLocalPos;
    private Quaternion _baseLocalRot;

    private Vector3 _targetPosOffset;
    private Vector3 _currentPosOffset;
    private Vector3 _posVel;

    private Vector3 _targetRotOffset;       // Euler offsets
    private Vector3 _currentRotOffset;
    private Vector3 _rotVel;

    private void Awake()
    {
        _baseLocalPos = transform.localPosition;
        _baseLocalRot = transform.localRotation;
    }

    // Call per shot
    public void AddRecoil(float strength = 1f)
    {
        // Kick back (negative z), slight up
        _targetPosOffset.z = Mathf.Clamp(_targetPosOffset.z - kickPosZ * strength, -maxPosZ, 0f);
        _targetPosOffset.y = Mathf.Clamp(_targetPosOffset.y + kickPosY * strength, 0f, 0.05f);

        // Rotate up + small roll (gun "jolts")
        _targetRotOffset.x = Mathf.Clamp(_targetRotOffset.x + kickRotX * strength, 0f, maxRotX);
        _targetRotOffset.z += Random.Range(-kickRotZ, kickRotZ) * strength;
    }

    private void LateUpdate()
    {
        float dt = Time.deltaTime;

        // 1) Snap current towards target quickly (kick)
        _currentPosOffset = Vector3.SmoothDamp(_currentPosOffset, _targetPosOffset, ref _posVel, 0f, kickSpeed, dt);
        _currentRotOffset = Vector3.SmoothDamp(_currentRotOffset, _targetRotOffset, ref _rotVel, 0f, kickSpeed, dt);

        // 2) Target returns to zero more slowly (settle)
        _targetPosOffset = Vector3.Lerp(_targetPosOffset, Vector3.zero, (1f - damping) * returnSpeed * dt);
        _targetRotOffset = Vector3.Lerp(_targetRotOffset, Vector3.zero, (1f - damping) * returnSpeed * dt);

        // Apply
        transform.localPosition = _baseLocalPos + _currentPosOffset;
        transform.localRotation = _baseLocalRot * Quaternion.Euler(_currentRotOffset);
    }

    // Optional: call on reload to quickly reset
    public void ResetRecoil()
    {
        _targetPosOffset = _currentPosOffset = Vector3.zero;
        _targetRotOffset = _currentRotOffset = Vector3.zero;
        _posVel = _rotVel = Vector3.zero;
        transform.localPosition = _baseLocalPos;
        transform.localRotation = _baseLocalRot;
    }
}
