using System.Reflection;
using System.Threading.Tasks.Dataflow;
using Game.Types;

namespace Game.Enemies;

public static class EnemyManager
{
    public static IEnemy? CurrentEnemy { get; set; }
    public static bool EnemyIsAlive { get; set; } = true;
    public static int CurrentEnemyInRoom { get; set; } = 0;

    public static void Init()
    {
        //GameActions.ShowData(RoomManager.CurrentRoom.Enemies.Count.ToString(), EnemyIsAlive.ToString());
        //CreateEnemy();
    }

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

    public static List<Type> GetRandomEnemies()
    {
        List<Type> EnemiesRet = new List<Type>();
        Random r = new Random();
        for (int i = 0; i < r.Next(3, 7); i++)
        {
            var Enemies = GetAllEnemies();
            var CurrentEnemyType = Enemies[r.Next(Enemies.Count - 1)];

            //var EnemyInstance = (IEnemy?)Activator.CreateInstance(CurrentEnemyType) ?? throw new Exception("Somehow got Null: GetRandomEnemy");
            EnemiesRet.Add(CurrentEnemyType);
        }

        return EnemiesRet;
    }

    // Params: None
    // Returns: Nothing
    // Function that makes a random Enemy appear on Screen and initializes the players health
    public static void CreateEnemy()
    {
        Random r = new Random();
        if ((CurrentEnemy is null || !EnemyIsAlive) /*&& RoomManager.CurrentRoom.Enemies.Count != CurrentEnemyInRoom*/)
        {

            Type CurrentEnemyType = RoomManager.CurrentRoom.Enemies[CurrentEnemyInRoom];

            var EnemyInstance = (IEnemy?)Activator.CreateInstance(CurrentEnemyType) ?? throw new Exception("Somehow got Null: CreateEnemy");
            CurrentEnemy = EnemyInstance;
            
            // ---
            CurrentEnemy.Health = CurrentEnemy.MaxHealth;

            EnemyIsAlive = true;

            GameActions.Graphics.DrawImage(Path.GetFullPath(CurrentEnemy.ImagePath));
            GameViewSetup.FightDisplayGraph.SetNeedsDisplay();

            GameViewSetup.LabelHP.Text = $"{Player.CurrentHP} / {Player.MaxHP}♥";
            GameActions.UpdateMPLabel();

            //GameViewSetup.FightTextContainer.Text = "";
            GameActions.Write("");
            GameActions.Write($"{CurrentEnemy.Name} Appears!");
            GameActions.Write($"HP: {CurrentEnemy.Health} / {CurrentEnemy.MaxHealth}♥");
            GameActions.Write($"{CurrentEnemy.Noise[r.Next(CurrentEnemy.Noise.Count)]}");
            GameActions.Write("");
        }
    }

    // Params: None
    // Returns: Nothing
    // Function that Handles Fighting the current Enemy
    public static void FightCurrentEnemy()
    {
        if (CurrentEnemy is null)
            return;
        
        Random r = new Random();

        // Player Turn
        var Damage = r.Next(60);
        if (DoDamage(Damage))
            return;
        
        // Enemy Turn
        DoEnemyTurn();
    }

    // params: Damage to be dealt
    // returns: void
    // Does Damage and advances to enemy turn
    public static void TakeDamageTurn(float Damage)
    {
        if (!(CurrentEnemy is null))
        {
            if (DoDamage(Damage))
                return;
            DoEnemyTurn();
        }
        return;
    }

    // params: Damage to be dealt
    // returns: a bool specifying wether the enemy has been killed (true = killed)
    // Does the specifyied damage to the current enemy and updates the textview
    public static bool DoDamage(float Damage)
    {
        if (!(CurrentEnemy is null))
        {
            CurrentEnemy.Health -= Damage;
            GameActions.Write($"You dealt {Damage} Damage");
            if (CurrentEnemy.Health <= 0)
            {
                WinBattle();
                return true;
            }
            else
            {
                GameActions.Write($"HP: {CurrentEnemy.Health} / {CurrentEnemy.MaxHealth}♥");
                GameActions.Write("");
                GameViewSetup.FightTextContainer.MoveEnd();
                
                return false;
            }
        }
        return false;
    }

    // params: None
    // returns: nothing
    // Does the enemies turn by selecting a random attack and using it
    public static void DoEnemyTurn()
    {
        if (CurrentEnemy is null)
            return;
        var EnemyDmg = CurrentEnemy.Attacks.GetRandomKVP();
        Player.CurrentHP -= EnemyDmg.Value;
        GameActions.Write($"{CurrentEnemy.Name} used {EnemyDmg.Key}, dealing {EnemyDmg.Value} Damage");
        GameActions.Write("");
        GameViewSetup.FightTextContainer.MoveEnd();
        if (Player.CurrentHP < 0)
        {
            Player.CurrentHP = 0;
            GameActions.ShowError("You Died!"); // Add death-screen later
        }

        GameViewSetup.LabelHP.Text = $"{Player.CurrentHP} / {Player.MaxHP}♥";
        GameActions.UpdateMPLabel();
    }

    // params: None
    // returns: Nothing
    // Triggers when the player wins a fight and handles XP/Level-Up logic
    public static void WinBattle()
    {
        CurrentEnemyInRoom++;
        EnemyIsAlive = false;
        GameActions.Write("You Won!");
        
        var Gained = Player.HandleXP(CurrentEnemy ?? throw new Exception("This isn't supposed to happen"));
        GameActions.Write($"You got {Gained} ⬙exp");
        
        if (RoomManager.CurrentRoom.Enemies.Count != CurrentEnemyInRoom)
            CreateEnemy();
        else
            RoomManager.RoomChange();
    }
}