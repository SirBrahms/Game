namespace Game.Enemies;

class Bloy : IEnemy
{
    public string Name { get; } = "Bloy";

    public string ImagePath { get; } = "img/bloy.png";

    public float Health { get; set; } = 95;

    public float MaxHealth { get; } = 95;

    public int Level { get; } = 1;

    public List<string> Noise { get; } = new List<string>() 
    {
        "Glare",
        "Blink",
        "..."
    };

    public Dictionary<string, float> Attacks { get; } = new Dictionary<string, float>
    {
        {"Intense Stare", 20},
        {"Headbutt", 10},
        {"Kick", 15}
    };

    public string Info { get; } = "An awkward one. Quite the common critter.";
}