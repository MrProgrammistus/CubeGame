using OpenTK.Mathematics;

namespace ProjectCube.Scripts.TerrainDir
{
	internal class FixedRandom
	{
		readonly float[] randNum = new float[3];
		readonly Random rand;
		readonly Dictionary<Vector2 ,float> cache = [];

		public FixedRandom(int seed)
		{
			rand = new(seed);

			for (int i = 0; i < randNum.Length; i++)
			{
				randNum[i] = rand.NextSingle();
			}
		}

		public float Next(Vector2 pos)
		{
			if (cache.TryGetValue(pos, out float r)) return r;

			r = MathF.Sin(pos.X * randNum[0] + pos.Y * randNum[1] + randNum[2]);
			r = (r + 1) / 2;
			r = ((int)(r * 65535) & 65534) / 65535f;

			cache.Add(pos, r);
			return r;
		}
	}
}
