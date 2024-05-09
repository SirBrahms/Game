namespace Game.Enemies;
class Tree : IEnemy {
public string Name { get; } = "Tree";
public string ImagePath { get; } = "img/tree.png";
public int Health { get; set; } = 100;
public int MaxHealth { get; } = 100;
public int Level { get; } = 1;
public List<string> Noise { get; } = new List<string>() {
"tree",
"whoosh",
"tree noises",
};
public Dictionary<string, float> Attacks { get; } = new Dictionary<string, float>() {
{"Leaf", 1},
{"Falling Branch", 50},
};
public string Info { get; } = "A tree, what did you expect?";
}
