namespace Game;
using Terminal.Gui;
using NStack;

class Program
{
	static void Main(string[] args)
	{
		Application.Init();

		GameActions.Init();
		GameViewSetup.SetupGame();

		Application.Run();
		Application.Shutdown();
	}
}
