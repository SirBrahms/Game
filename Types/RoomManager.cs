using Game.Enemies;
using Game.Items;
using Terminal.Gui;

namespace Game.Types;

static class RoomManager
{
    #pragma warning disable // Init must be called anyway
    public static Room CurrentRoom { get; set; }
    #pragma warning restore
    public static int RoomAmount { get; private set; }
    public static List<Room> NextRooms { get; set; } = new List<Room>(); // Only contains as many elements as indicated by room amount
    
    private static Random Rand = new Random();

    public static void Init()
    {
        CurrentRoom = CreateRandomStandardRoom();
    }

    public static Room CreateRandomStandardRoom()
    {
        return new Room(() => {}, () => {}, () => {}) { Enemies = EnemyManager.GetRandomEnemies() };
    }

    public static void RoomChange()
    {
        RoomAmount = Rand.Next(1, 5);

        GameActions.Write();
        if (Player.CurrentHP < Player.MaxHP * 0.5)
            GameActions.Write($"You stumble out of the room, finding yourself in front of {RoomAmount} doors");
        else
            GameActions.Write($"You exit the room and find yourself in front of {RoomAmount} doors");
        GameActions.Write($"Which one will you choose?");

        for (int i = 0; i < RoomAmount; i++)
        {
            NextRooms.Add(CreateRandomStandardRoom());
        }

        GameViewSetup.SetupAfterFightChoices(Enumerable.Range(1, RoomAmount).Select(x => x.ToString()).ToArray());
    }

    public static void LoadRoomFromSelection(object Sender)
    {
        EButton SenderBtn = (EButton)Sender;
        int Index = int.Parse(SenderBtn.Data.ToString() ?? throw new Exception("unregistered button: LoadFromSelection"));
        if (Index > RoomAmount)
            throw new ArgumentOutOfRangeException(nameof(Index));
        
        GameActions.Clear();
        CurrentRoom = NextRooms[Index];
        GameViewSetup.SetupViewInventory();
        EnemyManager.CurrentEnemyInRoom = 0;
        EnemyManager.CreateEnemy();
    }

    

    
}