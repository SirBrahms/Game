namespace Game;
using System.Text;
using System.Reflection;
using Terminal.Gui;
using Game.Enemies;
using Game.Items;
using Game.Spells;
using NStack;
using Game.Types;

static class GameActions 
{
    #region Data

    public static Dictionary<string, string> SpellOverview = new Dictionary<string, string>()
    {
        /*{"Conjure Grass", $"This Magical Spell Creates an equally magical patch of grass in front of its user."},
        {"Create Lamp", $"A lamp will appear out of thin air in front of you. You won't know what kind of Lamp, but it'll  be a lamp."},
        {"Summon Pen", $"This spell summons a pen straight from the ninth circle of hell."}*/
    };

    public static Graphics Graphics = new Graphics(GameViewSetup.FightDisplayGraph);

    // Fight
    //public static IEnemy? CurrentEnemy;
    //private static bool EnemyIsAlive = true;
    //private static List<Type> Enemies = new List<Type>();
    private static List<Type> Items = new List<Type>();
    public static List<Type> Spells = new List<Type>();
    // Deck
    public static List<Type> Deck = new List<Type>();
    public static int DeckSize = 2;
    public static int CurrentlySelectedSpell = 0;

    // Display
    public static List<Action> ViewSetupFunctions = new List<Action>()
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
            //Enemies = EnemyManager.GetAllEnemies();
            RoomManager.Init();
            EnemyManager.Init();
            GetAllItems();
            GetAllSpells();
        }
        catch (Exception ex)
        {
            ShowError($"{ex.Message}: {ex.StackTrace}");
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
        var SpellInstance = (Spell?)Activator.CreateInstance(Deck[e.Item]) ?? throw new Exception("Somehow got Null: CreateSpell");
        if (Player.CurrentMP - SpellInstance.Cost >= 0)
        {
            SpellInstance.Action();
            //Player.CurrentHP -= SpellInstance.Cost;
        }
        else
        {
            Player.CurrentMP = 0;
            UpdateMPLabel();
        }
    }

    // params: None
    // returns: Nothing
    // Updates the MP-Label in the Fight-Scene
    public static void UpdateMPLabel()
    {
        GameViewSetup.LabelMP.Text = $"{Player.CurrentMP} / {Player.MaxMP}⁂";
    }

    // params: None
    // returns: Nothing
    // Adds a given spell to the deck, if the spell fits
    public static void AddSpellToDeck()
    {
        var S = CreateSpellInstance(CurrentlySelectedSpell);
        if ((Deck.Count + S.SlotCost) <= DeckSize)
        {
            if (Deck.Contains(Spells[CurrentlySelectedSpell]))
            {
                return;
            }
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

    public static void Write(string Text = "")
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