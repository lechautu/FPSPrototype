using UnityEngine;

public sealed class WeaponController : MonoBehaviour
{
    public WeaponConfig config;
    public CameraRig cameraRig;
    public CombatSystem combatSystem;
    public EnemySystem enemySystem;
    public VFXSystem vfxSystem;
    public HitMarkerUI hitMarkerUI;
    public WeaponViewmodelRecoil viewmodelRecoil;


    public AudioSource fireAudioSource;


    [Header("Optional")]
    public float headshotMultiplier = 2f;

    private int _ammoInClip;
    private float _nextShotTime;
    private bool _reloading;
    private float _reloadEndTime;

    public int AmmoInClip => _ammoInClip;
    public int ClipSize => config != null ? config.clipSize : 0;
    public bool IsReloading => _reloading;
    public float ReloadDuration => config != null ? config.reloadTime : 0f;

    public float ReloadRemaining
    {
        get
        {
            if (!_reloading) return 0f;
            return Mathf.Max(0f, _reloadEndTime - Time.time);
        }
    }

    public float ReloadProgress01
    {
        get
        {
            if (!_reloading) return 0f;
            float dur = ReloadDuration;
            if (dur <= 0.0001f) return 1f;
            return 1f - Mathf.Clamp01(ReloadRemaining / dur);
        }
    }

    private void Awake()
    {
        if (config == null) Debug.LogError("WeaponConfig not assigned");
        _ammoInClip = config != null ? config.clipSize : 0;
    }

    public void Tick(float timeNow, float dt, bool fireHeld, bool reloadPressed)
    {
        if (config == null) return;

        if (_reloading)
        {
            if (timeNow >= _reloadEndTime)
            {
                _reloading = false;
                _ammoInClip = config.clipSize;
            }
            return;
        }

        if (reloadPressed && _ammoInClip < config.clipSize)
        {
            StartReload(timeNow);
            return;
        }

        if (!fireHeld) return;

        if (timeNow < _nextShotTime) return;
        if (_ammoInClip <= 0)
        {
            StartReload(timeNow);
            return;
        }

        Fire(timeNow);
    }

    private void StartReload(float timeNow)
    {
        _reloading = true;
        _reloadEndTime = timeNow + config.reloadTime;
    }

    private void Fire(float timeNow)
    {
        vfxSystem?.PlayMuzzleFlash();
        fireAudioSource?.Play();
        
        float strength = Mathf.Clamp(config.fireRate / 10f, 0.8f, 1.2f);
        viewmodelRecoil?.AddRecoil(strength);

        _ammoInClip--;
        _nextShotTime = timeNow + (1f / Mathf.Max(0.01f, config.fireRate));

        // Recoil
        cameraRig.AddRecoil(config.recoilKickUp, Random.Range(-config.recoilKickSide, config.recoilKickSide));

        // Spread (very simple)
        Ray ray = cameraRig.GetCenterRay();
        ray.direction = ApplySpread(ray.direction, config.spreadDegrees);

        HitResult hit = combatSystem.ShootHitscan(ray, config.range);
        if (hit.HasHit)
        {
            if (hit.EnemyId >= 0)
            {
                float dmg = config.damage * (hit.IsHeadshot ? headshotMultiplier : 1f);
                enemySystem.ApplyDamage(hit.EnemyId, dmg, hit);

                vfxSystem?.PlayBlood(hit.Point, hit.Normal);
                hitMarkerUI?.Flash();
            }
            else
            {
                vfxSystem?.PlayImpact(hit.Point, hit.Normal);
            }
            if (hit.IsHeadshot)
                cameraRig.AddRecoil(config.recoilKickUp * 0.6f, 0f);
        }
        

        // Hook: VFXSystem / audio can be added later.
    }

    private static Vector3 ApplySpread(Vector3 dir, float degrees)
    {
        if (degrees <= 0.001f) return dir;
        float yaw = Random.Range(-degrees, degrees);
        float pitch = Random.Range(-degrees, degrees);
        return (Quaternion.Euler(pitch, yaw, 0f) * dir).normalized;
    }
}
