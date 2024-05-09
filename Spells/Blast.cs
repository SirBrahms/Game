using Game.Enemies;

namespace Game.Spells;

public class Blast : Spell
{
    public override string Name { get; } = "Blast";
    public override string Description { get; } = "The Magical Blast";
    public override int Cost { get; } = 50;
    public override void Action()
    {
        base.Action();
        if (!(EnemyManager.CurrentEnemy is null))
        {
            if (EnemyManager.DoDamage(50))
                return;
            EnemyManager.DoEnemyTurn();
        }
        return;
    }
}