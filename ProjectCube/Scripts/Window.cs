using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using ProjectCube.Scripts.RenderDir;
using ProjectCube.Scripts.WorldDir;
using ProjectCube.Scripts.WorldDir.SceneDir;
using System.Diagnostics;
using System.Timers;

namespace ProjectCube.Scripts
{
	class Base(Window window)
	{
		public Window window = window;
	}

    internal class Window : Configs
	{
		readonly GameWindowSettings gameWindowSettings = new();
		readonly NativeWindowSettings nativeWindowSettings = new();
		public GameWindow gameWindow;


        System.Timers.Timer fpsTimer = new(fpsTimerUpdate);
		System.Timers.Timer PhysTimer = new(PhysTimerUpdate);
		System.Timers.Timer GenTimer = new(GenTimerUpdate);

		int FPS, PWT, GWT;

		public Render render;
		public World world;
		public Scene scene;

		private readonly object locker = new();

		//public bool reRender;

		public Window() : base(null)
		{
			render = new(this);
			world = new(this);
			scene = new(this);

			gameWindow = new(gameWindowSettings, nativeWindowSettings);

			gameWindow.ClientSize = new(width, height);
			gameWindow.UpdateFrequency = maxFps;

			gameWindow.Load += Start;
			gameWindow.UpdateFrame += Update;
			gameWindow.Unload += Stop;
			gameWindow.Resize += Resize;

			void Time(object source, ElapsedEventArgs e) => gameWindow.Title = $"ProjectCube (FPS: {FPS}; PUT: {PWT}; GUT: {GWT})";
			fpsTimer.Elapsed += Time;
			fpsTimer.AutoReset = true;
			fpsTimer.Start();

			PhysTimer.Elapsed += PhysUpdate;
			PhysTimer.AutoReset = true;

			GenTimer.Elapsed += GenUpdate;
			GenTimer.AutoReset = true;
		}

		public void Run() => gameWindow.Run();

		public void Start()
		{
			scene.Awake();



			world.terrain.Start();
			//world.planets.Start();

			foreach (var i in scene.gameObjects) foreach (var j in i.elements) j.Start();

			render.Start();
			PhysTimer.Start();
			GenTimer.Start();
		}

		public void Update(FrameEventArgs e)
		{
			FPS = (int)Math.Round(1 / e.Time);

			render.Update();
			//reRender = render.reRenderSphere;
			gameWindow.SwapBuffers();

            foreach (var i in scene.gameObjects) foreach (var j in i.elements) j.Update((float)e.Time);

			//world.player.Update((float)e.Time);
			//world.planets.Update();
		}

		public void PhysUpdate(object source, ElapsedEventArgs e)
		{
			Stopwatch sw = new();
			sw.Start();

			world.PhysUpdate();

			foreach (var i in scene.gameObjects) foreach (var j in i.elements) j.PhysUpdate();

			sw.Stop();
			PWT = (int)Math.Round((double)sw.ElapsedMilliseconds > 0 ? (double)sw.ElapsedMilliseconds : 0);
		}

		public void GenUpdate(object source, ElapsedEventArgs e)
		{
			Stopwatch sw = new();
			sw.Start();

			lock (locker)
			{
				//world.terrain.GenUpdate(); // генерация ландшафта

				foreach (var i in scene.gameObjects) foreach (var j in i.elements) j.GenUpdate();
				//render.reRenderSphere = reRender;
			}

			sw.Stop();
			GWT = (int)Math.Round((double)sw.ElapsedMilliseconds > 0 ? (double)sw.ElapsedMilliseconds : 0);
		}

		public void Stop()
		{
			render.Stop();
			world.terrain.Stop();

			foreach (var i in scene.gameObjects) foreach (var j in i.elements) j.Stop();

		}



		public void Resize(ResizeEventArgs e)
		{
			width = e.Width;
			height = e.Height;
			GL.Viewport(0, 0, width, height);

			foreach (var i in scene.gameObjects) foreach (var j in i.elements) j.Resize();

			//world.player.Resize();
		}
	}
}
