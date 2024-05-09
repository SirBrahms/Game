namespace Game;
using System.Text;
using System.Reflection;
using Terminal.Gui;
using Game.Enemies;
using Game.Items;
using Game.Spells;
using NStack;

static class GameActions 
{
    #region Data

    public static Dictionary<string, string> SpellOverview = new Dictionary<string, string>()
    {
        /*{"Conjure Grass", $"This Magical Spell Creates an equally magical patch of grass in front of its user."},
        {"Create Lamp", $"A lamp will appear out of thin air in front of you. You won't know what kind of Lamp, but it'll  be a lamp."},
        {"Summon Pen", $"This spell summons a pen straight from the ninth circle of hell."}*/
    };

    public static GraphView FightDisplayGraph = new GraphView()
    {
        Height = Dim.Fill(),
        Width = Dim.Fill(),
    };
    public static Graphics Graphics = new Graphics(FightDisplayGraph);

    // Fight
    public static IEnemy? CurrentEnemy;
    private static bool EnemyIsAlive = true;
    private static List<Type> Enemies = new List<Type>();
    private static List<Type> Items = new List<Type>();
    public static List<Type> Spells = new List<Type>();
    // Deck
    public static List<Type> Deck = new List<Type>();
    public static int DeckSize = 2;
    public static int CurrentlySelectedSpell = 0;

    // Display
    private static List<Action> ViewSetupFunctions = new List<Action>()
    {
        () => GameViewSetup.SetupDashboard(),
        () => GameViewSetup.SetupSpell(),
        () => GameViewSetup.SetupFight(),
        () => GameViewSetup.SetupApprentices()
    };

    #endregion

    // Params: None
    // Returns: Nothing
    // Does Nothing
    public static void DoNothing() { return; }

    // Params: None
    // Returns:  0 if successful, 0< if not successful
    // Initializes important Data
    public static int Init()
    {
        try
        {
            Enemies = EnemyManager.GetAllEnemies();
            GetAllItems();
            GetAllSpells();
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
            return 1;
        }
        return 0;
    }

    // Params: Event Args, passed by raised Event
    // Returns: Nothing
    // Changes the content of the Spell description Label, depending on the Selected Spell
    public static void HandleSpellSelection(ListViewItemEventArgs e)
    {
        GameViewSetup.InvisibleViews.ForEach(v => v.Visible = true);
        GameViewSetup.ViewSpellText.Text = SpellOverview.ElementAt(GameViewSetup.ListSpells.SelectedItem).Value;
        GameViewSetup.LabelDeckSpace.Text = CreateSpellInstance(e.Item).Cost.ToString();
        CurrentlySelectedSpell = e.Item;
    }

    #region MessageBoxes
    // Params: None
    // Returns: True when the selected result of the message box equaled "yes"
    // Spawns a message box and returns true if "Yes" was clicked
    public static bool Logout() 
    {
        var ret = MessageBox.Query(54, 7, "Logout?", "Do you wish to quit this Application?", "Yes", "No");
        return ret == 0;
    }

    // Params: None
    // Returns: Nothing
    // Spawns a message box displaying a short bit of information about the game
    public static void ShowAbout() 
    {
        MessageBox.Query(54, 7, "About", @"Wonderful Game About -~*Wizards*~-", "Ok");
    }

    // Params: Data to display (params)
    // Returns: Nothing
    // Spawns a message box that's displaying given information
    public static void ShowData(params string[] Data) 
    {
        StringBuilder sb = new StringBuilder();
        int i = 0;
        foreach (var s in Data)
        {
            sb.Append($"[{i}]: {Data[i]}\n");
            i++;
        }
        var Final = sb.ToString();
        MessageBox.Query(54, 10, "Data", Final, "Ok");
    }

    // Params: Error message to display
    // Returns: Nothing
    // Spawns a message box that's displaying a given error
    public static void ShowError(string Msg) 
    {
        MessageBox.ErrorQuery(54, 12, ":( An Error Occurred :(", Msg, "Ok");
    }

    // Params: None
    // Returns: Nothing
    // Spawns a message box displaying a short message to test functionality
    public static void ShowTest() 
    {
        MessageBox.Query(54, 7, "TestMsg", "it worked!", "Ok");
    }
    #endregion

    #region Fight
    // Params: None
    // Returns: Nothing
    // Function that searches for all types that implement the IEnemy interface (setup function)
    /*
    public static void GetAllEnemies()
    {
        var InterfaceType = typeof(IEnemy);
        var asm = Assembly.GetAssembly(InterfaceType) ?? throw new Exception("Somehow got null");
        Enemies = asm.GetTypes()
            .Where(p => InterfaceType.IsAssignableFrom(p)).ToList();

        Enemies.Remove(InterfaceType);
    }
    */

    // Params: None
    // Returns: Nothing
    // Function that searches for all types that implement the IItem interface (setup function)
    public static void GetAllItems()
    {
        var InterfaceType = typeof(IItem);
        var asm = Assembly.GetAssembly(InterfaceType) ?? throw new Exception("Somehow got null");
        Items = asm.GetTypes()
            .Where(p => InterfaceType.IsAssignableFrom(p)).ToList();

        Items.Remove(InterfaceType);
    }

    public static void GetAllSpells()
    {
        var ClassType = typeof(Spell);
        var asm = Assembly.GetAssembly(ClassType) ?? throw new Exception("Somehow got null");
        Spells = asm.GetTypes()
            .Where(p => ClassType.IsAssignableFrom(p)).ToList();

        Spells.Remove(ClassType);
        ListSpells();
    }

    // Params: None
    // Returns: Nothing
    // Function that makes a random Enemy appear on Screen and initializes the players health
    public static void CreateEnemy()
    {
        Random r = new Random();
        if (CurrentEnemy is null || !EnemyIsAlive)
        {
            Type CurrentEnemyType = Enemies[r.Next(Enemies.Count)];

            var EnemyInstance = (IEnemy?)Activator.CreateInstance(CurrentEnemyType) ?? throw new Exception("Somehow got Null: CreateEnemy");
            CurrentEnemy = EnemyInstance;
        }
        EnemyIsAlive = true;

        Graphics.DrawImage(Path.GetFullPath(CurrentEnemy.ImagePath));
        FightDisplayGraph.SetNeedsDisplay();

        GameViewSetup.LabelHP.Text = $"{Player.CurrentHP} / {Player.MaxHP}♥";
        UpdateMPLabel();

        //GameViewSetup.FightTextContainer.Text = "";
        Write("");
        Write($"{CurrentEnemy.Name} Appears!");
        Write($"HP: {CurrentEnemy.Health} / {CurrentEnemy.MaxHealth}♥");
        Write($"{CurrentEnemy.Noise[r.Next(CurrentEnemy.Noise.Count)]}");
        Write("");
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
            CurrentEnemy.Health -= (int)Damage;
            Write($"You dealt {Damage} Damage");
            if (Damage > CurrentEnemy.Health)
            {
                WinBattle();
                return true;
            }
            else
            {
                Write($"HP: {CurrentEnemy.Health} / {CurrentEnemy.MaxHealth}♥");
                Write("");
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
        Write($"{CurrentEnemy.Name} used {EnemyDmg.Key}, dealing {EnemyDmg.Value} Damage");
        Write("");
        GameViewSetup.FightTextContainer.MoveEnd();
        if (Player.CurrentHP < 0)
        {
            Player.CurrentHP = 0;
            ShowError("You Died!"); // Add death-screen later
        }

        GameViewSetup.LabelHP.Text = $"{Player.CurrentHP} / {Player.MaxHP}♥";
        UpdateMPLabel();
    }

    // params: None
    // returns: Nothing
    // Triggers when the player wins a fight and handles XP/Level-Up logic
    public static void WinBattle()
    {
        EnemyIsAlive = false;
        Write("You Won!");
        
        var Gained = Player.HandleXP(CurrentEnemy ?? throw new Exception("This isn't supposed to happen"));
        Write($"You got {Gained} ⬙exp");
        
        CreateEnemy();
    }

    public static void LevelUp()
    {
        Clear();
        Write("----LEVEL UP!----");
        Write($"You are now level {Player.PlayerLVL}");
        Write($"You gained some stats:");
        Write($"HP: {Player.CurrentHP} → {Player.MaxHP}");
        Write($"MP: {Player.CurrentMP} → {Player.MaxMP}");
        Write("");
        Write("The thought of levelling up fills you with joy (+ 1 ☺)");

        Player.CurrentHP = Player.MaxHP;
        Player.CurrentMP = Player.MaxMP;
    }

    // params: None
    // returns: Nothing
    // Casts the spell that's been selected in the spellbook
    public static void CastSpellFromMenu()
    {
        Thread.Sleep(100);
        var S = CreateSpellInstance(CurrentlySelectedSpell);
        if ((Player.CurrentHP - S.Cost) >= 0)
        {
            S.Action();
        }
    }

    public static void CastSpellInDeck(ListViewItemEventArgs e)
    {
        bool Run = true;
        ShowData(Deck[e.Item].Name);
        var S = (Spell?)Activator.CreateInstance(Deck[e.Item]);
        if (S is null)
        {
            return;
        }
        if ((Player.CurrentMP - S.Cost) >= 0 && Run)
        {
            S.Action();
            Run = false;
        }
    }

    // params: None
    // returns: Nothing
    // Updates the MP-Label in the Fight-Scene
    public static void UpdateMPLabel()
    {
        GameViewSetup.LabelMP.Text = $"{Player.CurrentMP} / {Player.CurrentMP}⁂";
    }

    // params: None
    // returns: Nothing
    // Adds a given spell to the deck, if the spell fits
    public static void AddSpellToDeck()
    {
        var S = CreateSpellInstance(CurrentlySelectedSpell);
        if ((Deck.Count + S.SlotCost) <= DeckSize)
        {
            Deck.Add(Spells[CurrentlySelectedSpell]);
        }
        else
        {
            ShowError("Deck is full!");
        }
    }

    public static void HandleItemOptions(EventArgs e) 
    {
        // MessageBox.Query(54, 7, "Handle", $"Handled {Options.SelectedItem}", "Ok");
        ViewSetupFunctions[GameViewSetup.Options.SelectedItem]();
    }

    #endregion

    #region Util
    public static List<T> GetKeysAsList<T, S>(this Dictionary<T, S> Dict) where T : notnull where S : notnull
    {
        return Dict.Keys.ToList();
    }

    public static KeyValuePair<T, S> GetRandomKVP<T, S>(this Dictionary<T, S> Dict) where T : notnull where S : notnull
    {
        Random r = new Random();
        var n = r.Next(Dict.Keys.Count);
        var k = Dict.Keys.ToArray()[n];
        var v = Dict.Values.ToArray()[n];
        return new KeyValuePair<T, S>(k, v);
    }

    public static void Write(View v, string Text)
    {
        v.Text += Text + Environment.NewLine;
    }

    public static void Write(string Text)
    {
        GameViewSetup.FightTextContainer.Text += Text + Environment.NewLine;
        GameViewSetup.FightTextContainer.MoveEnd();
    }

    public static void Clear()
    {
        GameViewSetup.FightTextContainer.Text = string.Empty;
        GameViewSetup.FightTextContainer.MoveEnd();
    }

    public static void ListSpells()
    {
        foreach (Type t in Spells)
        {
            var i = (Spell?)Activator.CreateInstance(t);
            if (i is null)
                return;
            SpellOverview.Add(i.Name, i.Description);
        }
    }

    public static List<string> GetDeckSpells()
    {
        List<string> Catcher = new List<string>();
        foreach (Type t in Deck)
        {
            var i = (Spell?)Activator.CreateInstance(t);
            if (i is null)
                return new List<string>();
            Catcher.Add(i.Name);
        }
        return Catcher;
    }

    public static Spell CreateSpellInstance(int index)
    {
        if (index > Spells.Count)
            return new Spell();
        var i = (Spell?)Activator.CreateInstance(Spells[index]);
        if (i is null)
            throw new Exception("Given Spell does not exist");
        return i;
    }

    #endregion
}