namespace Game.Spells;

public class MagicTransgenderBlast : Spell
{
    public override string Name { get; } = "Magical Transgender Blast";
    public override string Description { get; } = "The Magical Transgender Blast";
    public override int Cost { get; } = 100;
    public override void Action()
    {
        base.Action();
        if (!(GameActions.CurrentEnemy is null))
        {
            if (GameActions.DoDamage(100))
                return;
            GameActions.DoEnemyTurn();
        }
        return;
    }
}