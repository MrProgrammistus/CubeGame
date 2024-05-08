
namespace ProjectCube.Scripts.TerrainDir
{
	internal class Terrain : WorldConfig
	{
		public Generation generation = new();
		public LineArray lineArray = new();

		public void Start()
		{
			lineArray = generation.GenerateArray();
		}
	}
}
