namespace Game.Enemies;

public class Bird : IEnemy
{
    public string Name { get; } = "Birdy";
    public string ImagePath { get; } = "img/bird.png";
    public float Health { get; set; } = 60;
    public float MaxHealth { get; } = 60;
    public int Level { get; } = 1;
    public List<string> Noise { get; } = new List<string>()
    {
        "Piep!",
        "Pieeeeeeeep!",
        "AAAAAAAAAAAAAAAAAAAAAA"
    };
    public Dictionary<string, float> Attacks  { get; } = new Dictionary<string, float>()
    {
        {"Scream",  10},
        {"Poke", 15},
        {"Fire Breath", 70}
    };

    public string Info { get; } = "Test Enemy";
}