namespace Game.Spells;

public class Spell
{
    public virtual string Name { get; } = "New Spell";
    public virtual string Description { get; } = string.Empty;
    public virtual bool Cutscene { get; } = false;
    public virtual string CutscenePath { get; } = string.Empty;
    public virtual int Cost { get; } = 1;
    public virtual int SlotCost { get; } = 1;
    public virtual void Action()
    {
        Player.CurrentMP -= Cost;
        GameActions.UpdateMPLabel();
    }
}