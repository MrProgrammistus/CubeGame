
using OpenTK.Mathematics;

namespace ProjectCube.Scripts.TerrainDir
{
	internal class Generation : WorldConfig
	{
		Noise noise = new(0);

		public LineArray GenerateArray()
		{
			LineArray lineArray = new();

			for (int x = 0; x < arraySize; x++)
			{
				for (int y = 0; y < arraySize; y++)
				{
					lineArray.lines[x][y] = Generate((x, y));
				}
			}

			return lineArray;
		}

		public Line Generate(Vector2 pos)
		{
			return new(new Material(Type.stone), noise.HeightMap(pos));
		}
	}
}
