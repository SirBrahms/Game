namespace Game.Spells;

public class Blast : Spell
{
    public override string Name { get; } = "Blast";
    public override string Description { get; } = "The Magical Blast";
    public override int Cost { get; } = 50;
    public override void Action()
    {
        base.Action();
        if (!(GameActions.CurrentEnemy is null))
        {
            if (GameActions.DoDamage(50))
                return;
            GameActions.DoEnemyTurn();
        }
        return;
    }
}