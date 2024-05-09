namespace Game.Effects;

public interface IEffect
{
    public string Name { get; }
    public string Description { get; }
    public void Tick();
}