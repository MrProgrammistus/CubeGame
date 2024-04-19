using OpenTK.Mathematics;


namespace CubeGame.Scripts
{
	internal class Generation
	{
		public static Noise n = new();

		public static void GenerateArray(ref Dictionary<Vector3, BlocksArray> arraysPos, Vector3i pos, World world)
		{
			BlocksArray tmpArr = new(World.arraySize, 1, world);

			arraysPos.Add(pos, tmpArr);

			for (int i = 0; i < World.arraySize; i++)
			{
				for (int j = 0; j < World.arraySize; j++)
				{
					for (int k = 0; k < World.arraySize; k++)
					{
						tmpArr.blocks[i][j][k] = Generate(i, j, k, pos);
					}
				}
			}
		}

		public static Block Generate(int i, int j, int k, Vector3i pos)
		{
			float r = n.Generate(i + pos.X * World.arraySize, k + pos.Z * World.arraySize) * 128 - 64;
			float y = j + pos.Y * World.arraySize;

			if		(r - y < 5 && r - y >= 0 && y < 5) return new(Type.sand);
			else if (r - y < 5 && r - y >= 0) return new(Type.soil);
			else if (y < r) return new(Type.stone);
			else if (y < 0) return new(Type.water);
			else  return new(Type.air);
		}
	}
}
