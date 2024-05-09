using System.Reflection;
using System.Transactions;
using Game.Enemies;

namespace Game.Types;

static class RoomMAnager
{
    public static Room? CurrentRoom { get; set; }

    public static Room CreateRandomStandardRoom()
    {
        return new Room(() => {}, () => {}, () => {}) { Enemies = GetRandomEnemies() };
    }

    private static List<IEnemy> GetRandomEnemies()
    {
        List<IEnemy> EnemiesRet = new List<IEnemy>();
        Random r = new Random();
        for (int i = 0; i > r.Next(1, 5); i++)
        {
            var Enemies = EnemyManager.GetAllEnemies();
            var CurrentEnemyType = Enemies[r.Next()];

            var EnemyInstance = (IEnemy?)Activator.CreateInstance(CurrentEnemyType) ?? throw new Exception("Somehow got Null: GetRandomEnemy");
            EnemiesRet.Add(EnemyInstance);
        }

        return EnemiesRet;
    }

    
}