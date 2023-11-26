public class MeleeEnemy : EnemyController
{
    protected override void Start()
    {
        base.Start();
        CreateMeleeWeapon();
    }
}
