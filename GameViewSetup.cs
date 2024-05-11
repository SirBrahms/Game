namespace Game;
using Terminal.Gui;
using NStack;
using System.Runtime.CompilerServices;
using Game.Spells;
using System.Reflection.Metadata.Ecma335;
using Game.Enemies;
using Game.Types;
using System.Threading.Tasks.Sources;

static class GameViewSetup 
{
    # region Views
    public static Toplevel Top = Application.Top;

    private static Window MainWindow = new Window("Main")
    {
        X = 0,
        Y = 1
    };

    public static ListView Options = new ListView();

    // Main Frame Views
    private static FrameView ViewOptions = new FrameView("Options (Ctrl+O)");
    private static FrameView ViewCurrent = new FrameView("Current");

    // Spell Views
    private static FrameView ViewSpells = new FrameView("Spells");
    public static FrameView ViewDetailSpells = new FrameView("Details");
    public static TextView ViewSpellText = new TextView();
    public static Label LabelDeckSpace = new Label();
    public static ListView ListSpells = new ListView();
    public static List<View> InvisibleViews = new List<View>(); // Views that need to be visualised when a certain condition is met.

    // Fight Views
    public static FrameView ViewEnemy = new FrameView();
    private static FrameView ViewFightText = new FrameView();
    public static TextView FightTextContainer = new TextView();
    public static FrameView ViewInventory = new FrameView("Inventory");
    public static Label LabelHP = new Label("");
    public static Label LabelMP = new Label("");
    public static FrameView ViewDeck = new FrameView("Deck");
    public static ListView ListDeck = new ListView();
    //public static GraphView DisplayGraph = new GraphView();
    public static GraphView FightDisplayGraph = new GraphView()
    {
        Height = Dim.Fill(),
        Width = Dim.Fill(),
    };

    #endregion

    #region Data
    // Main Overview
    private static List<string> ViewList = new List<string>()
    {
        "Dashboard",
        "Spellbook",
        "Fight",
        "Apprentice"
    };

    #endregion

    public static void SetupGame() 
    {
        PrepareMainWindow();
        Top.Add(MainWindow);
    }

    private static void PrepareMainWindow() 
    {
        // Main Menu Overlay
        var MainMenu = new MenuBar(new MenuBarItem[] {
                            new MenuBarItem("_File", new MenuItem[] {
                                new MenuItem("_Quit", "Ctrl+Q", () => {if (GameActions.Logout()) Top.Running = false;}),
                                new MenuItem("_New", "", () => GameActions.DoNothing(), shortcut: Key.CtrlMask | Key.N),
                                new MenuItem("Remove Main _Window", "", () => Top.Remove(MainWindow), shortcut: Key.CtrlMask | Key.W)
                            }),
                            new MenuBarItem("_Help", new MenuItem[] {
                                new MenuItem("_About", "Displays an About-Page", () => GameActions.ShowAbout())
                            })
        });

        Top.Add(MainMenu);

        SetupLstOptions();
        SetupDashboard();

        // FrameViews
        ViewOptions.Width = Dim.Percent(25);
        ViewOptions.Height = Dim.Fill();
        ViewOptions.X = 0;

        ViewCurrent.X = Pos.Right(ViewOptions);
        ViewCurrent.Width = Dim.Fill();
        ViewCurrent.Height = Dim.Fill();
        ViewCurrent.Border.BorderStyle = BorderStyle.None;

        // Adding
        ViewOptions.Add(Options);

        MainWindow.Add(ViewOptions, ViewCurrent);
    }

    private static void SetupLstOptions()
    {
        Options.SetSource(ViewList);
        Options.Width = Dim.Fill();
        Options.Height = Dim.Fill();
        Options.Shortcut = Key.CtrlMask | Key.O;
        Options.ShortcutAction = () => Options.SetFocus();

        Options.OpenSelectedItem += GameActions.HandleItemOptions;
    }

    #region View Setups
    public static void SetupDashboard()
    {
        ViewCurrent.RemoveAll();

        ViewCurrent.Title = "Dashboard";

    }

    public static void SetupSpell()
    {
        ViewCurrent.RemoveAll();

        ViewCurrent.Title = "Spellbook";
        
        // Spell Overview
        ViewSpells.Width = Dim.Percent(50);
        ViewSpells.Height = Dim.Fill();
        ViewSpells.X = 0;

        ListSpells.SetSource(GameActions.SpellOverview.GetKeysAsList());
        ListSpells.Width = Dim.Fill();
        ListSpells.Height = Dim.Fill();
        ListSpells.SelectedItem = 0;
        ListSpells.OpenSelectedItem += (e) => GameActions.HandleSpellSelection(e);

        ViewSpells.Add(ListSpells);

        // Detail Spells
        ViewDetailSpells.Width = Dim.Fill();
        ViewDetailSpells.Height = Dim.Fill();
        ViewDetailSpells.X = Pos.Right(ViewSpells);

        ViewSpellText.Height = Dim.Percent(70);
        ViewSpellText.Width = Dim.Fill();
        ViewSpellText.WordWrap = true;
        ViewSpellText.ReadOnly = true;
        ViewSpellText.CanFocus = false;
        ViewSpellText.ColorScheme = new ColorScheme() {
                                                        Normal = Application.Driver.MakeAttribute(Terminal.Gui.Color.White, Terminal.Gui.Color.Blue), 
                                                        Disabled = Application.Driver.MakeAttribute(Terminal.Gui.Color.White, Terminal.Gui.Color.Blue)
                                                    };

        ViewDetailSpells.Add(ViewSpellText);

        var ButtonCast = new Button()
        {
            Text = "Cast Spell!",
            Y = Pos.Percent(80),
            X = Pos.Center(),
            TextAlignment = TextAlignment.Justified,
            Height = 1,
            Visible = false
        };
        ButtonCast.Clicked += () => GameActions.CastSpellFromMenu();

        var ButtonAddToDeck = new Button()
        {
            Text = "Add Spell To Deck!",
            X = Pos.Center(),
            Y = Pos.Bottom(ButtonCast) + 1,
            TextAlignment = TextAlignment.Centered,
            Height = 1,
            Visible = false
        };
        ButtonAddToDeck.Clicked += () => GameActions.AddSpellToDeck();

        LabelDeckSpace.X = Pos.Center();
        LabelDeckSpace.Y = Pos.Bottom(ButtonCast) + 2;
        LabelDeckSpace.TextAlignment = TextAlignment.Centered;
        LabelDeckSpace.Visible = false;

        InvisibleViews.AddRange(new View[] { ButtonCast, ButtonAddToDeck, LabelDeckSpace });
        ViewDetailSpells.Add(ButtonCast, ButtonAddToDeck, LabelDeckSpace);

        //--
        ViewCurrent.Add(ViewSpells, ViewDetailSpells);
    }

    public static void SetupFight()
    {
        ViewCurrent.RemoveAll();

        ViewCurrent.Title = "Fight";

        // View Fight
        ViewFightText.Height = Dim.Percent(75);
        ViewFightText.Width = Dim.Percent(50);
        ViewFightText.X = 0;

        FightTextContainer.Height = Dim.Fill();
        FightTextContainer.Width = Dim.Fill();
        FightTextContainer.WordWrap = true;
        FightTextContainer.ReadOnly = true;
        FightTextContainer.ColorScheme = new ColorScheme() 
                                        {
                                            Normal = Application.Driver.MakeAttribute(Terminal.Gui.Color.White, Terminal.Gui.Color.Black), 
                                            Disabled = Application.Driver.MakeAttribute(Terminal.Gui.Color.White, Terminal.Gui.Color.Black)
                                        };
        FightTextContainer.CanFocus = false;
        ViewFightText.Add(FightTextContainer);

        // View Enemy
        ViewEnemy.Height = Dim.Percent(75);
        ViewEnemy.Width = Dim.Percent(50);
        ViewEnemy.X = Pos.Right(ViewFightText);
        ViewEnemy.Border.BorderStyle = BorderStyle.Double;
        ViewEnemy.CanFocus = false;

        FightDisplayGraph.Reset();
        FightDisplayGraph.AxisX.Visible = false;
        FightDisplayGraph.AxisX.Increment = 0;
        FightDisplayGraph.AxisX.ShowLabelsEvery = 0;

        FightDisplayGraph.AxisY.Visible = false;
        FightDisplayGraph.AxisY.Increment = 0;
        FightDisplayGraph.AxisY.ShowLabelsEvery = 0;

        FightDisplayGraph.CanFocus = false;
        Application.Resized += GameActions.Graphics.FitImageIntoDisplay;

        ViewEnemy.Add(FightDisplayGraph);

        // View Inventory
        SetupViewInventory();
        EnemyManager.CreateEnemy();


        //--
        ViewCurrent.Add(ViewFightText, ViewEnemy, ViewInventory);
        //EnemyManager.CreateEnemy();
    }

    public static void SetupViewInventory()
    {
        ViewInventory.RemoveAll();

        ViewInventory.Height = Dim.Fill();
        ViewInventory.Width = Dim.Fill();
        ViewInventory.Y = Pos.Bottom(ViewFightText);

        LabelHP.X = Pos.Center() + 5;
        LabelHP.TextAlignment = TextAlignment.Centered;
        ViewInventory.Add(LabelHP);

        LabelMP.X = Pos.Left(LabelHP) - 15;
        LabelMP.TextAlignment = TextAlignment.Centered;
        ViewInventory.Add(LabelMP);

        ViewDeck.Y = Pos.Bottom(LabelHP);
        ViewDeck.Height = Dim.Fill() - 2;
        ViewDeck.Width = Dim.Fill();
        ViewDeck.Border.BorderStyle = BorderStyle.None;

        ListDeck.SetSource(GameActions.GetDeckSpells());
        ListDeck.Width = Dim.Fill();
        ListDeck.Height = Dim.Fill();
        ListDeck.OpenSelectedItem += GameActions.CastSpellInDeck;

        ViewDeck.Add(ListDeck);

        ViewInventory.Add(ViewDeck);

        var ButtonFight = new Button("Punch")
        {
            X = Pos.Center(),
            Y = Pos.Bottom(ViewDeck) + 1,
        };
        ButtonFight.Clicked += () => EnemyManager.FightCurrentEnemy();
        ViewInventory.Add(ButtonFight);
        ViewInventory.FocusFirst();
        ViewInventory.SetFocus();
    }

    public static void SetupAfterFightChoices(params string[] Directions)
    {
        LabelHP.X = Pos.Center() + 5;
        LabelHP.TextAlignment = TextAlignment.Centered;
        ViewInventory.Add(LabelHP);

        LabelMP.X = Pos.Left(LabelHP) - 15;
        LabelMP.TextAlignment = TextAlignment.Centered;
        ViewInventory.Add(LabelMP);

        ViewInventory.RemoveAll();
        List<EButton> Buttons = new List<EButton>();

        if (Directions.Length == 0)
        {
            throw new Exception("No room choices");
        }
        
        //GameActions.ShowData(Directions.Length.ToString());

        for (int i = 0; i < Directions.Length; i++)
        {
            var btn = new EButton
            {
                Text = $"Go {Directions[i]}"
            };

            if (i == 0)
            {
                btn.X = 1;
            }
            else
            {
                btn.X = Pos.Left(Buttons[i - 1]) + 10;
            }
            btn.Y = Pos.Center();
            btn.Data = i;

            btn.Clicked += (e) => RoomManager.LoadRoomFromSelection(e); 
            Buttons.Add(btn);
            ViewInventory.Add(btn);
        }
    }

    public static void SetupApprentices()
    {
        ViewCurrent.RemoveAll();

        ViewCurrent.Title = "Dashboard";
    }

    #endregion


}