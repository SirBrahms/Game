using System.Reflection;

namespace Game.Enemies;

public static class EnemyManager
{
    // Params: None
    // Returns: List of all instantiable enemy types
    // Function that searches for all types that implement the IEnemy interface (setup function)
    public static List<Type> GetAllEnemies()
    {
        var Enemies = new List<Type>();
        var InterfaceType = typeof(IEnemy);
        var asm = Assembly.GetAssembly(InterfaceType) ?? throw new Exception("Somehow got null");
        Enemies = asm.GetTypes()
            .Where(p => InterfaceType.IsAssignableFrom(p)).ToList();

        Enemies.Remove(InterfaceType);

        return Enemies;
    }

    public static List<IEnemy> GetRandomEnemies()
    {
        List<IEnemy> EnemiesRet = new List<IEnemy>();
        Random r = new Random();
        for (int i = 0; i > r.Next(1, 5); i++)
        {
            var Enemies = GetAllEnemies();
            var CurrentEnemyType = Enemies[r.Next(Enemies.Count - 1)];

            var EnemyInstance = (IEnemy?)Activator.CreateInstance(CurrentEnemyType) ?? throw new Exception("Somehow got Null: GetRandomEnemy");
            EnemiesRet.Add(EnemyInstance);
        }

        return EnemiesRet;
    }
}