using System.Collections.Generic;
using UnityEngine;

public sealed class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public sealed class PoolEntry
    {
        public string key;
        public GameObject prefab;
        public int prewarm = 50;
    }

    public PoolEntry[] pools;

    private readonly Dictionary<string, Queue<GameObject>> _store = new();
    private readonly Dictionary<string, GameObject> _prefabs = new();

    private void Awake()
    {
        foreach (var p in pools)
        {
            if (p.prefab == null || string.IsNullOrEmpty(p.key)) continue;
            _prefabs[p.key] = p.prefab;
            var q = new Queue<GameObject>(p.prewarm);
            _store[p.key] = q;

            for (int i = 0; i < p.prewarm; i++)
            {
                var go = Instantiate(p.prefab);
                go.SetActive(false);
                q.Enqueue(go);
            }
        }
    }

    public T Get<T>(string key) where T : Component
    {
        GameObject go = GetGO(key);
        return go.GetComponent<T>();
    }

    public GameObject GetGO(string key)
    {
        if (!_store.TryGetValue(key, out var q))
        {
            q = new Queue<GameObject>();
            _store[key] = q;
        }

        if (q.Count > 0)
        {
            var go = q.Dequeue();
            go.SetActive(true);
            return go;
        }

        if (_prefabs.TryGetValue(key, out var prefab))
        {
            var go = Instantiate(prefab);
            go.SetActive(true);
            return go;
        }

        Debug.LogError($"Pool key not found: {key}");
        return null;
    }

    public void Release(string key, GameObject go)
    {
        if (go == null) return;
        go.SetActive(false);

        if (!_store.TryGetValue(key, out var q))
        {
            q = new Queue<GameObject>();
            _store[key] = q;
        }
        q.Enqueue(go);
    }
}
