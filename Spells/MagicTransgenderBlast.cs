using Game.Enemies;

namespace Game.Spells;

public class MagicTransgenderBlast : Spell
{
    public override string Name { get; } = "Magical Transgender Blast";
    public override string Description { get; } = "The Magical Transgender Blast";
    public override int Cost { get; } = 100;
    public override void Action()
    {
        base.Action();
        if (!(EnemyManager.CurrentEnemy is null))
        {
            if (EnemyManager.DoDamage(100))
                return;
            EnemyManager.DoEnemyTurn();
        }
        return;
    }
}