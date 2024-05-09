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
}