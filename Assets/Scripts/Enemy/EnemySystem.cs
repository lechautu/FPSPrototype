using System;
using UnityEngine;

public sealed class EnemySystem : MonoBehaviour
{
    private struct EnemyEntity
    {
        public int id;
        public bool alive;
        public float hp;

        public Vector3 pos;
        public float speed;

        public float attackRange;
        public float attackCooldown;
        public float nextAttackTime;

        public EnemyState state;

        public Transform tr;
        public EnemyView view;
        public float dieRecycleTime;
        public float dieRecycleAt;

        public Vector3 velocity;
    }

    private enum EnemyState { Walk, Attack, Die }

    [Header("Target")]
    public Transform playerTarget;
    public float stopDistance = 1.2f;
    public int AliveCount { get; private set; }

    public int tickChunkSize = 64;
    private int _tickCursor = 0;

    // Simple storage (good enough for prototype)
    private EnemyEntity[] _enemies = new EnemyEntity[256];
    private int _count = 0;

    public void ClearAll(Action<EnemyView> releaseView)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_enemies[i].alive && _enemies[i].view != null)
            {
                _enemies[i].view.Unbind();
                releaseView?.Invoke(_enemies[i].view);
            }
        }
        _count = 0;
    }

    public int RegisterEnemy(EnemyView view, Vector3 spawnPos, float hp, float speed, float attackRange, float attackCooldown)
    {
        int id = _count;
        EnsureCapacity(id + 1);
        AliveCount++;


        _enemies[id] = new EnemyEntity
        {
            id = id,
            alive = true,
            hp = hp,
            pos = spawnPos,
            speed = speed,
            attackRange = attackRange,
            attackCooldown = attackCooldown,
            nextAttackTime = 0f,
            state = EnemyState.Walk,
            tr = view.transform,
            view = view,
            dieRecycleTime = 1.8f,
            dieRecycleAt = 0f
        };

        view.Bind(id);
        view.transform.position = spawnPos;

        _count++;
        return id;
    }

    public void Tick(float timeNow, float dt, Action<EnemyView> releaseView)
    {
        if (playerTarget == null) return;

        Vector3 playerPos = playerTarget.position;

        int processed = 0;
        int i = _tickCursor;

        while (processed < tickChunkSize && processed < _count)
        {
            if (i >= _count) i = 0;

            ref EnemyEntity e = ref _enemies[i];
            if (e.alive)
            {
                switch (e.state)
                {
                    case EnemyState.Walk:
                        {
                            e.velocity = Vector3.Lerp(e.velocity, Vector3.zero, 10f * dt);
                            e.pos += e.velocity * dt;

                            Vector3 toPlayer = (playerPos - e.pos);
                            float dist = toPlayer.magnitude;

                            if (dist <= Mathf.Max(stopDistance, e.attackRange))
                            {
                                e.state = EnemyState.Attack;
                                break;
                            }

                            Vector3 dir = toPlayer / Mathf.Max(0.001f, dist);
                            e.pos += dir * (e.speed * dt);
                            e.tr.position = e.pos;

                            Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
                            e.tr.rotation = Quaternion.Slerp(e.tr.rotation, lookRot, 10f * dt);

                            break;
                        }
                    case EnemyState.Attack:
                        {
                            Vector3 toPlayer = (playerPos - e.pos);
                            float dist = toPlayer.magnitude;

                            if (dist > Mathf.Max(stopDistance, e.attackRange) + 0.2f)
                            {
                                e.state = EnemyState.Walk;
                                break;
                            }

                            if (timeNow >= e.nextAttackTime)
                            {
                                e.nextAttackTime = timeNow + e.attackCooldown;
                            }
                            break;
                        }
                    case EnemyState.Die:
                        {
                            if (timeNow >= e.dieRecycleAt)
                            {
                                e.alive = false;
                                AliveCount = Mathf.Max(0, AliveCount - 1);
                                e.view.Unbind();
                                releaseView?.Invoke(e.view);
                                e.view = null;
                            }
                            break;
                        }
                }
            }

            processed++;
            i++;
        }

        _tickCursor = i;
    }

    public void ApplyDamage(int enemyId, float damage, HitResult hit)
    {
        if (enemyId < 0 || enemyId >= _count) return;
        ref EnemyEntity e = ref _enemies[enemyId];
        if (!e.alive) return;

        e.hp -= damage;
        if (playerTarget != null)
        {
            Vector3 knockDir = (e.pos - playerTarget.position);
            knockDir.y = 0f;
            if (knockDir.sqrMagnitude > 0.0001f)
            {
                knockDir.Normalize();
                e.velocity += knockDir * 0.9f; // tune 0.6 - 1.2
            }
        }

        e.view?.PlayHit();

        if (e.hp <= 0f)
        {
            e.state = EnemyState.Die;
            e.view?.PlayDie();
            e.dieRecycleAt = Time.time + e.dieRecycleTime;
        }

    }

    private void EnsureCapacity(int needed)
    {
        if (needed <= _enemies.Length) return;
        int newSize = _enemies.Length;
        while (newSize < needed) newSize *= 2;
        Array.Resize(ref _enemies, newSize);
    }
}
