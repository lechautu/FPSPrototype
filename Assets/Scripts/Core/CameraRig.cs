using UnityEngine;

public sealed class CameraRig : MonoBehaviour
{
    public Transform yawPivot;
    public Transform pitchPivot;
    public Camera cam;

    [Header("Clamp")]
    public float pitchMin = -55f;
    public float pitchMax = 55f;

    [Header("Recoil")]
    public float recoilReturnSpeed = 18f;
    public float recoilDamping = 22f;

    private float _yaw;
    private float _pitch;

    private Vector2 _recoil;
    private Vector2 _recoilVel;

    public void ApplyLook(Vector2 lookDelta)
    {
        _yaw += lookDelta.x;
        _pitch -= lookDelta.y;
        _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);
    }

    public void AddRecoil(float kickUp, float kickSide)
    {
        _recoil += new Vector2(kickSide, kickUp);
    }

    public void Tick(float dt)
    {
        // Recoil spring back
        _recoil = Vector2.SmoothDamp(_recoil, Vector2.zero, ref _recoilVel, 1f / recoilDamping, Mathf.Infinity, dt);
        Vector2 applied = Vector2.Lerp(_recoil, Vector2.zero, recoilReturnSpeed * dt);

        yawPivot.localRotation = Quaternion.Euler(0f, _yaw + applied.x, 0f);
        pitchPivot.localRotation = Quaternion.Euler(_pitch - applied.y, 0f, 0f);
    }

    public Ray GetCenterRay()
    {
        return cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }
}
