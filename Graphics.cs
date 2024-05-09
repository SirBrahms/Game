namespace Game;
using System.Drawing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Terminal.Gui;
using Terminal.Gui.Graphs;
using NStack;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Tga;

class Graphics
{
	public GraphView DisplayGraph { get; }

	// System.Drawing.Color to Terminal.Gui.Attribute Conversion
	private Dictionary<System.Drawing.Color, Terminal.Gui.Attribute> Colors = new Dictionary<System.Drawing.Color, Terminal.Gui.Attribute>()
	{
		{System.Drawing.Color.Black, Application.Driver.MakeAttribute(Terminal.Gui.Color.Black, Terminal.Gui.Color.Black)}, // Black
		{System.Drawing.Color.White, Application.Driver.MakeAttribute(Terminal.Gui.Color.White, Terminal.Gui.Color.Black)}, // White
		{System.Drawing.Color.Blue, Application.Driver.MakeAttribute(Terminal.Gui.Color.Blue, Terminal.Gui.Color.Black)}, // Blue
		{System.Drawing.Color.Aqua, Application.Driver.MakeAttribute(Terminal.Gui.Color.BrightBlue, Terminal.Gui.Color.Black)}, // BrightBlue
		{System.Drawing.Color.LightCyan, Application.Driver.MakeAttribute(Terminal.Gui.Color.BrightCyan, Terminal.Gui.Color.Black)}, // BrightCyan
		{System.Drawing.Color.LimeGreen, Application.Driver.MakeAttribute(Terminal.Gui.Color.BrightGreen, Terminal.Gui.Color.Black)}, // BrightGreen
		{System.Drawing.Color.Magenta, Application.Driver.MakeAttribute(Terminal.Gui.Color.Magenta, Terminal.Gui.Color.Black)}, // Magenta
		{System.Drawing.Color.Red, Application.Driver.MakeAttribute(Terminal.Gui.Color.Red, Terminal.Gui.Color.Black)}, // Red
		{System.Drawing.Color.OrangeRed, Application.Driver.MakeAttribute(Terminal.Gui.Color.BrightRed, Terminal.Gui.Color.Black)}, // BrightRed (???)
		{System.Drawing.Color.Cyan, Application.Driver.MakeAttribute(Terminal.Gui.Color.Cyan, Terminal.Gui.Color.Black)}, // Cyan
		{System.Drawing.Color.LightYellow, Application.Driver.MakeAttribute(Terminal.Gui.Color.BrightYellow, Terminal.Gui.Color.Black)}, // BrightYellow
		{System.Drawing.Color.Yellow, Application.Driver.MakeAttribute(Terminal.Gui.Color.BrightYellow, Terminal.Gui.Color.Black)}, // Yellow -> Bright Yellow
		{System.Drawing.Color.Brown, Application.Driver.MakeAttribute(Terminal.Gui.Color.Brown, Terminal.Gui.Color.Black)}, // Brown
		{System.Drawing.Color.DarkGray, Application.Driver.MakeAttribute(Terminal.Gui.Color.DarkGray, Terminal.Gui.Color.Black)}, // DarkGray
		{System.Drawing.Color.Gray, Application.Driver.MakeAttribute(Terminal.Gui.Color.Gray, Terminal.Gui.Color.Black)}, // Gray
		{System.Drawing.Color.Green, Application.Driver.MakeAttribute(Terminal.Gui.Color.Green, Terminal.Gui.Color.Black)}, // Green
	};
	// Image Storing
	private List<ColorPoint> ImageToDraw = new List<ColorPoint>();
	private SixLabors.ImageSharp.Image? Img;


	public Graphics(GraphView gv)
	{
		DisplayGraph = gv;
	}

	// Params: None
	// Returns: Nothing
	// Displays a graph with randomly changing Data
	public void DisplayRandomGraph()
	{
		//GameViewSetup.DisplayGraph.Reset();
		DisplayGraph.Clear();
		DisplayGraph.Reset();

		var black = Application.Driver.MakeAttribute(Terminal.Gui.Color.Black, Terminal.Gui.Color.Black);
		var cyan = Application.Driver.MakeAttribute(Terminal.Gui.Color.BrightCyan, Terminal.Gui.Color.Black);
		var magenta = Application.Driver.MakeAttribute(Terminal.Gui.Color.BrightMagenta, Terminal.Gui.Color.Black);
		var red = Application.Driver.MakeAttribute(Terminal.Gui.Color.BrightRed, Terminal.Gui.Color.Black);

		DisplayGraph.GraphColor = black;

		List<Terminal.Gui.PointF> RandomPoints = new List<Terminal.Gui.PointF>()
		{

		};

		Random r = new Random ();

		var line = new PathAnnotation() {
			LineColor = cyan,
			Points = RandomPoints.OrderBy (p => p.X).ToList (),
			BeforeSeries = true,
			LineRune = new Rune('█'),
		};

		Func<MainLoop, bool> genSample = (l) => {
			// generate a random sample
			RandomPoints.Clear();
			DisplayGraph.Annotations.Remove(line);
			for (int j = 0; j < 10; j++) 
				RandomPoints.Add(new Terminal.Gui.PointF(r.Next(DisplayGraph.Frame.Size.Width), r.Next(DisplayGraph.Frame.Size.Height)));
			
			line.Points = RandomPoints.OrderBy(p => p.X).ToList();
			
			DisplayGraph.Annotations.Add(line);
			DisplayGraph.SetNeedsDisplay();
			return DisplayGraph.Annotations.Contains(line);
		};

		Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(1000), genSample);

		DisplayGraph.SetNeedsDisplay();
	}

	// Params: (string)Path of the image to draw
	// Returns: Nothing
	// Displays an image withing the graph view
	public void DrawImage(string ImgPath)
	{
		DisplayGraph.Clear();
		DisplayGraph.Reset();
		ImageToDraw.Clear();
		//DisplayGraph.SetNeedsDisplay();
		using (var Image = SixLabors.ImageSharp.Image.Load<Argb32>(ImgPath))
		{
			Img = Image;

			Image.Mutate(i => RotateExtensions.Rotate(i, RotateMode.Rotate180)); // Flip the image upside down, since the coordinates of the Graph view are the inverse of the cordinates of the bitmap

			// Read image into Storage
			for (int x = 0; x < Image.Width; x++)
			{
				for (int y = 0; y < Image.Height; y++)
				{
					var ColorPixel = ConvertSystemColorToTerminalAttribute(System.Drawing.Color.FromArgb((int)Image[x, y].Argb));

					ImageToDraw.Add(new ColorPoint
									{
										X = x,
										Y = y,
										PixColorAttribute = ColorPixel
									});
				
				}
			}

			// Drawing
			DisplayGraph.GraphColor = Colors[System.Drawing.Color.Black]; // Set Background

			// Seperate image into different colors and draw the resulting map to the screen
			foreach (var Attr in Colors.Values)
			{
				// Seperation
				List<ColorPoint> ColorMap = ImageToDraw.Where(c => c.PixColorAttribute == Attr).ToList(); // Get all pixels with same color
			
				List<Terminal.Gui.PointF> DrawPointList = new List<Terminal.Gui.PointF>();
				foreach (var ColP in ColorMap)
				{
					DrawPointList.Add(ColP.ConvertToPoint());
				}

				var FinalPoints = new ScatterSeries()
				{
					Points = DrawPointList,
					Fill = new GraphCellToRender(new Rune('█'), Attr)
				};

				DisplayGraph.Series.Add(FinalPoints);
			}
		
			DisplayGraph.SetNeedsDisplay();

			try 
			{
				var ViewWidth = DisplayGraph.Frame.Size.Width; // Width (x) of the graph view
				var ViewHeight = DisplayGraph.Frame.Size.Height; // Height (y) of the graph view
				// If the image is larger than the display, scale it down, so that it fits the screen
				float ZoomX = 0;
				float ZoomY = 0;

				if (Img.Height > ViewHeight)
				{
					ZoomY =  (float)Img.Height / (float)ViewHeight;
				}
				if (Img.Width > ViewWidth)
				{
					ZoomX = (float)Img.Width / (float)ViewWidth;
				}
				Zoom(ZoomX, ZoomY);

				//GameActions.ShowData(ZoomX.ToString(), ViewWidth.ToString(), ZoomY.ToString(), ViewHeight.ToString());

				// Move Image to the middle of the display
				MarginX(true, (ViewWidth - Img.Width) / 2);
				MarginY(true, (ViewHeight - Img.Height) / 2);
			}
			catch (Exception ex)
			{
				GameActions.ShowError(ex.Message);
			}
		}
		

	}

	// Params: Event Args passed by event handler
	// Returns: Nothing
	// Re-Centers the image and adjusts the zoom when the window is resized (kinda janky)
	public void FitImageIntoDisplay(Application.ResizedEventArgs ra)
	{
		if (Img is null)
			return;

		var ViewWidth = DisplayGraph.Frame.Size.Width; // Width (x) of the graph view
		var ViewHeight = DisplayGraph.Frame.Size.Height; // Height (y) of the graph view
		// If the image is larger than the display, scale it down, so that it fits the screen
		float ZoomX = 0;
		float ZoomY = 0;

		if (Img.Height > ViewHeight)
		{
			ZoomY =  (float)Img.Height / (float)ViewHeight;
		}
		if (Img.Width > ViewWidth)
		{
			ZoomX = (float)Img.Width / (float)ViewWidth;
		}
		Zoom(ZoomX, ZoomY);

		//GameActions.ShowData(ZoomX.ToString(), ViewWidth.ToString(), ZoomY.ToString(), ViewHeight.ToString());

		// Move Image to the middle of the display
		MarginX(true, (ViewWidth - Img.Width) / 2);
		MarginY(true, (ViewHeight - Img.Height) / 2);

		GameViewSetup.Top.SetNeedsDisplay();
	}

	#region Util
	private Terminal.Gui.Attribute ConvertSystemColorToTerminalAttribute(System.Drawing.Color Col)
	{
		return Colors.Values.ToList()[FindClosestColorRGB(Col)];
	}

	private int FindClosestColorRGB(System.Drawing.Color Target)
	{
		var colorDiffs = Colors.GetKeysAsList().Select(n => ColorDiff(n, Target)).Min(n => n);
		return Colors.GetKeysAsList().FindIndex(n => ColorDiff(n, Target) == colorDiffs);
	}

	private int ColorDiff(System.Drawing.Color c1, System.Drawing.Color c2) 
	{ return  (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R) 
							   + (c1.G - c2.G) * (c1.G - c2.G)
							   + (c1.B - c2.B) * (c1.B - c2.B)); 
	}

	private void Zoom(float Factor)
	{
		DisplayGraph.CellSize = new Terminal.Gui.PointF 
		(
			DisplayGraph.CellSize.X * Factor,
			DisplayGraph.CellSize.Y * Factor
		);
		DisplayGraph.AxisX.Increment *= Factor;
		DisplayGraph.AxisY.Increment *= Factor;
		DisplayGraph.SetNeedsDisplay();
	}

	private void Zoom(float FactorX, float FactorY)
	{
		DisplayGraph.CellSize = new Terminal.Gui.PointF(1, 1);

		DisplayGraph.CellSize = new Terminal.Gui.PointF 
		(
			DisplayGraph.CellSize.X * FactorX > 0 ? FactorX : 1,
			DisplayGraph.CellSize.Y * FactorY > 0 ? FactorY : 1
		);
		//DisplayGraph.AxisX.Increment *= FactorX;
		//DisplayGraph.AxisY.Increment *= FactorY;
		DisplayGraph.AxisX.Increment = 0;
		DisplayGraph.AxisY.Increment = 0;
		DisplayGraph.SetNeedsDisplay();
	}

	private void MarginX(bool Increase, int Width)
	{
		DisplayGraph.MarginLeft = 0;

		DisplayGraph.MarginLeft = (uint)Math.Max(0, DisplayGraph.MarginLeft + (Increase ? Width : -Width)); 
		
		DisplayGraph.SetNeedsDisplay ();
	}

	private void MarginY(bool Increase, int Height)
	{
		DisplayGraph.MarginBottom = 0;

		DisplayGraph.MarginBottom = (uint)Math.Max(0, DisplayGraph.MarginBottom + (Increase ? Height : -Height));
		
		DisplayGraph.SetNeedsDisplay ();
	}

	#endregion
}

struct ColorPoint
{
	public int X { get; set; }
	public int Y { get; set; }
	public Terminal.Gui.Attribute PixColorAttribute { get; set; }

	public Terminal.Gui.PointF ConvertToPoint()
	{
		return new Terminal.Gui.PointF(X, Y);
	}
}