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
        return new Room(() => {}, () => {}, () => {}) { Enemies = EnemyManager.GetRandomEnemies() };
    }

    

    
}