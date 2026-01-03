using UnityEngine;

public sealed class EnemyView : MonoBehaviour
{
    public int EnemyId { get; private set; } = -1;

    [Header("Optional")]
    public Animator animator;
    public static bool EnableAnimator = true;

    public void Bind(int enemyId)
    {
        EnemyId = enemyId;
        gameObject.SetActive(true);
        if (animator && EnableAnimator) animator.Play("Walk", 0, 0f);
    }


    public void PlayHit()
    {
        if (!EnableAnimator) return;
        if (animator) animator.SetTrigger("Hit");
    }

    public void PlayDie()
    {
        if (!EnableAnimator) return;
        if (animator) animator.SetTrigger("Die");
    }

    public void Unbind()
    {
        EnemyId = -1;
    }
}
