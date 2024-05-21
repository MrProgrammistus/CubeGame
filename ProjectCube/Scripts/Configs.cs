using System.Globalization;
using System.Text;

namespace ProjectCube.Scripts
{
	internal class Configs(Window window)
	{
		public Window window = window;

		public static int width;
		public static int height;
		public static int maxFps;
		public static int fpsTimerUpdate;
		public static int PhysTimerUpdate;
		public static int GenTimerUpdate;

		public static int loadDistance;

		public static int arraySize;
		public static int arraysSize;

		public static int fov;
		public static float depthNear;
		public static float depthFar;

		public static float sensitivity;
		public static float speed;
		public static float ShiftSpeedMultiply;
		public static float firstYaw;
		public static float firstPitch;
		public static float maxPitch;
		public static float minPitch;
		public static float velocityK;

		public static string saveDir = "";
		public static bool save;

		public static void Load()
		{
			StreamReader file = new("Configs\\Window.conf", Encoding.GetEncoding(1251));

			width				= Convert.ToInt32(file.ReadLine().Split(":")[1]);
			height				= Convert.ToInt32(file.ReadLine().Split(":")[1]);
			maxFps				= Convert.ToInt32(file.ReadLine().Split(":")[1]);
			fpsTimerUpdate		= Convert.ToInt32(file.ReadLine().Split(":")[1]);
			PhysTimerUpdate		= Convert.ToInt32(file.ReadLine().Split(":")[1]);
			GenTimerUpdate		= Convert.ToInt32(file.ReadLine().Split(":")[1]);

			file.Close();
			file = new("Configs\\World.conf", Encoding.GetEncoding(1251));

			arraySize			= Convert.ToInt32(file.ReadLine().Split(":")[1]);
			arraysSize			= Convert.ToInt32(file.ReadLine().Split(":")[1]);

			file.Close();
			file = new("Configs\\Player.conf", Encoding.GetEncoding(1251));

			fov					= Convert.ToInt32(file.ReadLine().Split(":")[1]);
			depthNear			= float.Parse(file.ReadLine().Split(":")[1], CultureInfo.InvariantCulture.NumberFormat);
			depthFar			= float.Parse(file.ReadLine().Split(":")[1], CultureInfo.InvariantCulture.NumberFormat);
			file.ReadLine();
			sensitivity			= float.Parse(file.ReadLine().Split(":")[1], CultureInfo.InvariantCulture.NumberFormat);
			speed				= float.Parse(file.ReadLine().Split(":")[1], CultureInfo.InvariantCulture.NumberFormat);
			ShiftSpeedMultiply	= float.Parse(file.ReadLine().Split(":")[1], CultureInfo.InvariantCulture.NumberFormat);
			firstYaw			= float.Parse(file.ReadLine().Split(":")[1], CultureInfo.InvariantCulture.NumberFormat);
			firstPitch			= float.Parse(file.ReadLine().Split(":")[1], CultureInfo.InvariantCulture.NumberFormat);
			maxPitch			= float.Parse(file.ReadLine().Split(":")[1], CultureInfo.InvariantCulture.NumberFormat);
			minPitch			= float.Parse(file.ReadLine().Split(":")[1], CultureInfo.InvariantCulture.NumberFormat);
			velocityK			= float.Parse(file.ReadLine().Split(":")[1], CultureInfo.InvariantCulture.NumberFormat);

			file.Close();
			file = new("Configs\\Render.conf", Encoding.GetEncoding(1251));

			loadDistance		= Convert.ToInt32(file.ReadLine().Split(":")[1]);

			file.Close();
			file = new("Configs\\SaveLoad.conf", Encoding.GetEncoding(1251));

			saveDir				= file.ReadLine().Split(":")[1];
			save				= Convert.ToBoolean(file.ReadLine().Split(":")[1]);

			file.Close();
		}
	}
}
