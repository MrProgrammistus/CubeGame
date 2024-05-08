using ProjectCube.Scripts.WorldDir.PlayerDir;

namespace ProjectCube.Scripts.WorldDir
{
	internal class World : WorldConfig
	{
		public Player player = new();

		public void Update(Window window, double time)
		{
			player.Update(window, time);
		}
	}
}
