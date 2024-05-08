
namespace ProjectCube.Scripts.TerrainDir
{
	internal class LineArray : WorldConfig
	{
		public Line[][] lines = [];

		LineArray()
		{
			lines = new Line[arraySize][];
			for(int i = 0; i < 16; i++)
			{
				lines[i] = new Line[arraySize];
			}
		}
	}
}
