using Game.Enemies;
using Game.Items;

namespace Game.Types;

static class RoomManager
{
    #pragma warning disable // Init must be called anyway
    public static Room CurrentRoom { get; set; }
    #pragma warning restore

    public static void Init()
    {
        CurrentRoom = CreateRandomStandardRoom();
    }

    public static Room CreateRandomStandardRoom()
    {
        return new Room(() => {}, () => {}, () => {}) { Enemies = GetRandomEnemies() };
    }

    // move to enemy manager
    private static List<IEnemy> GetRandomEnemies()
    {
        List<IEnemy> EnemiesRet = new List<IEnemy>();
        Random r = new Random();
        for (int i = 0; i > r.Next(1, 5); i++)
        {
            var Enemies = EnemyManager.GetAllEnemies();
            var CurrentEnemyType = Enemies[r.Next(Enemies.Count - 1)];

            var EnemyInstance = (IEnemy?)Activator.CreateInstance(CurrentEnemyType) ?? throw new Exception("Somehow got Null: GetRandomEnemy");
            EnemiesRet.Add(EnemyInstance);
        }

        return EnemiesRet;
    }

    
}