using UnityEngine;
using UnityEngine.UI;

public sealed class WeaponHUD : MonoBehaviour
{
    public WeaponController weapon;

    [Header("Ammo UI")]
    public TMPro.TextMeshProUGUI ammoText;

    [Header("Reload UI")]
    public GameObject reloadGroup;   // to show/hide whole reload UI
    public Image reloadFill;         // Image type Filled
    public TMPro.TextMeshProUGUI reloadText;          // optional text, e.g. "Reloading 0.8s"

    private void Awake()
    {
        if (reloadGroup != null) reloadGroup.SetActive(false);
        if (reloadFill != null) reloadFill.fillAmount = 0f;
    }

    private void Update()
    {
        if (weapon == null) return;

        // Ammo
        if (ammoText != null)
        {
            int clip = weapon.ClipSize;
            int ammo = weapon.AmmoInClip;
            ammoText.text = $"Ammo: {ammo}/{clip}";
        }

        // Reload
        bool reloading = weapon.IsReloading;

        if (reloadGroup != null)
            reloadGroup.SetActive(reloading);

        if (reloading)
        {
            if (reloadFill != null)
                reloadFill.fillAmount = weapon.ReloadProgress01;

            if (reloadText != null)
                reloadText.text = $"Reloading {weapon.ReloadRemaining:0.0}s";
        }
        else
        {
            if (reloadFill != null)
                reloadFill.fillAmount = 0f;

            if (reloadText != null)
                reloadText.text = string.Empty;
        }

        if (!weapon.IsReloading && weapon.AmmoInClip <= 0)
        {
            if (reloadGroup != null) reloadGroup.SetActive(true);
            if (reloadText != null) reloadText.text = "EMPTY - Reload";
        }
    }
}
