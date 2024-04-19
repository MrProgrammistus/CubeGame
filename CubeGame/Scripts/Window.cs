using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Timers;

namespace CubeGame.Scripts
{
    internal class Window
    {
        GameWindowSettings gameWindowSettings = new();
        NativeWindowSettings nativeWindowSettings = new();
        GameWindow gameWindow;

        Render render = new();

        int width, height, fps;

        //фпс
		System.Timers.Timer timer;
        int realFps;

		public Window(int width, int height, int fps)
        {
			//настройки и создание окна
			gameWindow = new(gameWindowSettings, nativeWindowSettings);

            this.width = width;
            this.height = height;
            this.fps = fps;
            gameWindow.ClientSize = new(width, height);
            gameWindow.UpdateFrequency = fps;

            //прерывания
            gameWindow.Load += Load;
            gameWindow.RenderFrame += RenderFrame;
            gameWindow.Unload += Unload;

            gameWindow.Resize += Resize;

            //отображение фпс
			timer = new System.Timers.Timer(500);
			void Time(object source, ElapsedEventArgs e) => gameWindow.Title = $"CubeGame (FPS: {realFps})";
			timer.Elapsed += Time;
			timer.AutoReset = true;
		}

        public void Run() => gameWindow.Run();

        void Load()
        {
			timer.Enabled = true;
			//gameWindow.VSync = VSyncMode.On;
			render.Load(width, height);
        }
        void RenderFrame(FrameEventArgs e)
        {
            try
            {
				realFps = (int)(1 / e.Time);
				render.RenderFrame(gameWindow, (float)e.Time, width, height);
				gameWindow.SwapBuffers();
			}
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        void Unload()
        {
            render.shader?.Dispose();
            render.shader_line?.Dispose();
            render.world.Stop(render);
        }

        void Resize(ResizeEventArgs e)
        {
            width = e.Width;
            height = e.Height;
            GL.Viewport(0, 0, width, height);
            render.world.player.camera.Resize((float)width / height);
        }
    }
}
