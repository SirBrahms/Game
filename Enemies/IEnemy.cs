namespace Game.Enemies;

public interface IEnemy
{
    public string Name { get; }
    public string ImagePath { get; }
    public int Health { get; set; }
    public int MaxHealth { get; }
    public int Level { get; }
    public List<string> Noise { get; }
    public Dictionary<string, float> Attacks { get; }
    public string Info { get; }
}