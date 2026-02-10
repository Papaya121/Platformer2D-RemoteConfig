public class EnemyHealthController : HealthController
{
    protected override void Die()
    {
        AudioManager.PlaySound("EnemyDie");
        Destroy(gameObject);
    }

    protected override void OnDamage()
    {
        AudioManager.PlaySound("EnemyHit");
    }
}