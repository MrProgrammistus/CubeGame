using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using ProjectCube.Scripts.RenderDir;
using ProjectCube.Scripts.WorldDir;
using System.Timers;

namespace ProjectCube.Scripts
{
    internal class Window : WindowConfigs
	{
		GameWindowSettings gameWindowSettings = new();
		NativeWindowSettings nativeWindowSettings = new();
		public GameWindow gameWindow;


        System.Timers.Timer fpsTimer = new(fpsTimerUpdate);
		int fps;

		System.Timers.Timer PhysTimer = new(PhysTimerUpdate);


		public Render render = new();
		public World world = new();

		public Window()
		{
			gameWindow = new(gameWindowSettings, nativeWindowSettings);

			gameWindow.ClientSize = new(width, height);
			gameWindow.UpdateFrequency = maxFps;

			gameWindow.Load += Start;
			gameWindow.UpdateFrame += Update;
			gameWindow.Unload += Stop;
			gameWindow.Resize += Resize;

			void Time(object source, ElapsedEventArgs e) => gameWindow.Title = $"ProjectCube (FPS: {fps})";
			fpsTimer.Elapsed += Time;
			fpsTimer.AutoReset = true;
			fpsTimer.Start();

			PhysTimer.Elapsed += PhysUpdate;
			PhysTimer.AutoReset = true;
			PhysTimer.Start();
		}

		public void Run() => gameWindow.Run();

		public void Start()
		{
			render.Start();
		}

		public void Update(FrameEventArgs e)
		{
			fps = (int)(1 / e.Time);

			render.Update(this);
			gameWindow.SwapBuffers();

			world.Update(this, e.Time );
		}

		public void PhysUpdate(object source, ElapsedEventArgs e)
		{

		}

		public void Stop()
		{
			render.Stop();
		}



		public void Resize(ResizeEventArgs e)
		{
			width = e.Width;
			height = e.Height;
			GL.Viewport(0, 0, width, height);

			world.player.Resize();
		}
	}
}
