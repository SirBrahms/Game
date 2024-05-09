using Game.Enemies;

namespace Game.Spells;

public class TestSpell : Spell
{
    public override string Name { get; } = "Test Spell";
    public override string Description { get; } = "A spell used to test the spell function";
    public override int Cost { get; } = 10;
    public override void Action()
    {
        base.Action();
        EnemyManager.TakeDamageTurn(10);
        return;
    }
}