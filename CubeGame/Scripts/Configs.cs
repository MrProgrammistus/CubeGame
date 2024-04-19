using System.Text;

namespace CubeGame.Scripts
{
	internal class Configs
	{
		public static int renderDist = 8;
		public static int genY = 4;
		public static string saveFile = "test";
		public static Type block = Type.stone;

		public static void Read()
		{
			StreamReader file = new("Configs\\General.conf", Encoding.GetEncoding(1251));

			renderDist = Convert.ToInt32(file.ReadLine().Split(":")[1]);
			genY = Convert.ToInt32(file.ReadLine().Split(":")[1]);
			saveFile = file.ReadLine().Split(":")[1];
			block = (Type)Convert.ToInt32(file.ReadLine().Split(":")[1]);

			file.Close();
		}
	}
}
