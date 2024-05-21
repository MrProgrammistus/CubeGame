using ProjectCube.Scripts.WorldDir.PlanetsDir;
using ProjectCube.Scripts.WorldDir.PlayerDir;
using ProjectCube.Scripts.WorldDir.TerrainDir;

namespace ProjectCube.Scripts.WorldDir
{
	internal class World(Window window) : Configs(window)
	{
		//public Player player = new(window);
		public Terrain terrain = new(window);
		//public Planets planets = new(window);

		public void PhysUpdate()
		{
			
		}
	}
}
