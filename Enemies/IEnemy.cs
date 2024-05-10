namespace Game.Enemies;

public interface IEnemy
{
    public string Name { get; }
    public string ImagePath { get; }
    public float Health { get; set; }
    public float MaxHealth { get; }
    public int Level { get; }
    public List<string> Noise { get; }
    public Dictionary<string, float> Attacks { get; }
    public string Info { get; }
}