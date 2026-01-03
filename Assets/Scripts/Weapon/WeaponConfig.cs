using UnityEngine;

[CreateAssetMenu(menuName = "FPS/WeaponConfig")]
public sealed class WeaponConfig : ScriptableObject
{
    public string weaponName = "Rifle";
    public float fireRate = 10f;          // shots per second
    public int clipSize = 30;
    public float reloadTime = 1.6f;

    public float damage = 20f;
    public float range = 80f;

    [Header("Recoil")]
    public float recoilKickUp = 0.7f;
    public float recoilKickSide = 0.15f;

    [Header("Spread (optional)")]
    public float spreadDegrees = 0.6f;
}
