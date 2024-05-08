
namespace ProjectCube.Scripts.TerrainDir
{
	internal class LineArray : WorldConfig
	{
		public Line[][] lines = [];

		public LineArray()
		{
			lines = new Line[arraySize][];
			for(int i = 0; i < arraySize; i++) lines[i] = new Line[arraySize];
		}
	}
}
