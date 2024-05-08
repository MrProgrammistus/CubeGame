using OpenTK.Mathematics;

namespace ProjectCube.Scripts.TerrainDir
{
	internal class Noise
	{
		FixedRandom rand;

		public Noise(int seed)
		{
			rand = new(seed);
		}

		public float HeightMap(Vector2 pos)
		{
			return PerlinNoise(pos, 1);
		}

		public float PerlinNoise(Vector2 pos, int n)
		{
			Vector2 pos1 = pos / n * n;
			Vector2 pos2 = pos1 + (pos.X < 0 ? -n : n, pos.Y < 0 ? -n : n);

			float r0 = rand.Next(pos1);
			float r1 = rand.Next((pos1.X, pos2.Y));
			float r2 = rand.Next(pos2);
			float r3 = rand.Next((pos2.X, pos1.Y));

			float d0 = Distance(pos.X - pos1.X, n);
			float d1 = Distance(pos.X - pos2.X, n);
			float d2 = Distance(pos.Y - pos1.Y, n);
			float d3 = Distance(pos.Y - pos1.Y, n);

			float o0 = (r1 * d1 + r2 * d0) * d2;
			float o1 = (r2 * d2 + r3 * d3) * d0;
			float o2 = (r3 * d0 + r0 * d1) * d3;
			float o3 = (r0 * d3 + r1 * d2) * d1;

			return (o0 + o1 + o2 + o3) / 2;
		}

		public float Distance(float a, int n)
		{
			return (MathF.Sin(((MathF.Abs(a) / n) - 0.5f) * MathF.PI) + 1) / 2;
		}
	}
}
