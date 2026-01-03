using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public sealed class VFXSystem : MonoBehaviour
{
    public PoolManager pool;

    [Header("Pool Keys")]
    public string impactKey = "Impact";
    public string bloodKey = "Blood";

    [Header("Lifetime")]
    public float impactLifetime = 1.2f;
    public float bloodLifetime = 1.0f;

    [Header("Gun Muzzle Flash")]
    public ParticleSystem muzzleFlash;
    public Light muzzleFlashLight;

    public void PlayImpact(Vector3 point, Vector3 normal)
    {
        if (IsNearCorner(point, normal))
        {
            return;
        }

        var go = pool.GetGO(impactKey);
        if (go == null) return;

        Quaternion rot = Quaternion.LookRotation(-normal, Vector3.up);

        Vector3 pos = point + normal * 0.02f;

        go.transform.SetPositionAndRotation(pos, rot);

        var projector = go.GetComponent<DecalProjector>();
        if (projector != null)
        {
            Vector3 size = projector.size;
            size.z = Mathf.Clamp(size.z, 0.04f, 0.12f); 
            projector.size = size;

            projector.pivot = new Vector3(0f, 0f, size.z * 0.5f);
        }

        StartCoroutine(ReleaseAfter(impactKey, go, impactLifetime));
    }

    private bool IsNearCorner(Vector3 point, Vector3 normal, float probeDist = 0.06f)
    {
        Vector3 t = Vector3.Cross(normal, Vector3.up);
        if (t.sqrMagnitude < 1e-4f) t = Vector3.Cross(normal, Vector3.right);
        t.Normalize();
        Vector3 b = Vector3.Cross(normal, t);

        Vector3 origin = point + normal * 0.02f;

        return Physics.Raycast(origin, t, probeDist, ~0, QueryTriggerInteraction.Ignore) ||
               Physics.Raycast(origin, -t, probeDist, ~0, QueryTriggerInteraction.Ignore) ||
               Physics.Raycast(origin, b, probeDist, ~0, QueryTriggerInteraction.Ignore) ||
               Physics.Raycast(origin, -b, probeDist, ~0, QueryTriggerInteraction.Ignore);
    }

    public void PlayBlood(Vector3 point, Vector3 normal)
    {
        var go = pool.GetGO(bloodKey);
        if (go == null) return;
        go.transform.SetPositionAndRotation(point, Quaternion.LookRotation(normal));
        StartCoroutine(ReleaseAfter(bloodKey, go, bloodLifetime));
    }

    public void PlayMuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = true;
            CancelInvoke(nameof(DisableMuzzleFlashLight));
            Invoke(nameof(DisableMuzzleFlashLight), 0.05f);
        }
    }

    private void DisableMuzzleFlashLight()
    {
        muzzleFlashLight.enabled = false;
    }

    private System.Collections.IEnumerator ReleaseAfter(string key, GameObject go, float t)
    {
        yield return new WaitForSeconds(t);
        pool.Release(key, go);
    }
}
