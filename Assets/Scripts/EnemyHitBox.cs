using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    [Header("References")]
    public Enemy enemy;
    public EnemyRanged rangedEnemy;
    public Boss1 boss;


    [Header("Damage")]
    public Collider2D damageCollider;



    public bool TryTakeDamage(
    float damage,
    Collider2D hitCollider)
    {
        if (hitCollider != damageCollider)
            return false;

        if (Player.Instance == null)
            return false;

        Vector2 knockbackDirection =
            (
                (Vector2)transform.position -
                (Vector2)Player.Instance.transform.position
            ).normalized;


        if (enemy != null)
        {
            if (enemy.Health <= 0f)
                return false;


            if (DamageDealt.Instance != null)
                DamageDealt.Instance.AddDamage(damage);

            enemy.TakeDamage(damage);

            enemy.ApplyKnockback(
                knockbackDirection
            );

            return true;
        }



        if (rangedEnemy != null)
        {
            if (rangedEnemy.Health <= 0f)
                return false;

            // Count the FULL damage dealt.
            if (DamageDealt.Instance != null)
                DamageDealt.Instance.AddDamage(damage);

            rangedEnemy.TakeDamage(damage);

            rangedEnemy.ApplyKnockback(
                knockbackDirection
            );

            return true;
        }


        if (boss != null)
        {
            if (boss.Health <= 0f)
                return false;

            // Count the FULL damage dealt.
            if (DamageDealt.Instance != null)
                DamageDealt.Instance.AddDamage(damage);

            boss.TakeDamage(damage);

            boss.ApplyKnockback(
                knockbackDirection
            );

            return true;
        }


        return false;
    }



    public void TakeDamage(
        float damage,
        Collider2D hitCollider)
    {
        TryTakeDamage(
            damage,
            hitCollider
        );
    }
}

