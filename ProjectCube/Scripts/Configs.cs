using System.Text;

namespace ProjectCube.Scripts
{
	internal class Configs
	{
		public static void Load()
		{
			StreamReader file = new("Configs\\Window.conf", Encoding.GetEncoding(1251));

			WindowConfigs.width			  = Convert.ToInt32(file.ReadLine().Split(":")[1]);
			WindowConfigs.height		  = Convert.ToInt32(file.ReadLine().Split(":")[1]);
			WindowConfigs.maxFps		  = Convert.ToInt32(file.ReadLine().Split(":")[1]);
			WindowConfigs.fpsTimerUpdate  = Convert.ToInt32(file.ReadLine().Split(":")[1]);
			WindowConfigs.PhysTimerUpdate = Convert.ToInt32(file.ReadLine().Split(":")[1]);

			file.Close();
			file = new("Configs\\World.conf", Encoding.GetEncoding(1251));

			WorldConfig.arraySize = Convert.ToInt32(file.ReadLine().Split(":")[1]);

			file.Close();
		}
	}

	public class WindowConfigs
	{
		public static int width;
		public static int height;
		public static int maxFps;
		public static int fpsTimerUpdate;
		public static int PhysTimerUpdate;
	}
	
	public class RenderConfig
	{

	}

	public class WorldConfig
	{
		public static int arraySize;
	}

	public class PlayerConfig
	{

	}
}
