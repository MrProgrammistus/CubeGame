using OpenTK.Mathematics;

namespace ProjectCube.Scripts.WorldDir.TerrainDir
{
    internal class Noise
    {
        FixedRandom rand0;
        FixedRandom rand1;
        FixedRandom rand2;
        FixedRandom rand3;
        FixedRandom rand4;
        FixedRandom rand5;
        FixedRandom rand6;
        FixedRandom rand7;
        FixedRandom rand8;

        public Noise(int seed)
        {
            rand0 = new(seed);
            rand1 = new(seed + 1);
            rand2 = new(seed + 2);
            rand3 = new(seed + 3);
            rand4 = new(seed + 4);
            rand5 = new(seed + 5);
            rand6 = new(seed + 6);
            rand7 = new(seed + 7);
            rand8 = new(seed + 8);
        }

        public float HeightMap(Vector2 pos)
        {
            float r;

            r =  PerlinNoise(pos, 2048, rand0) * 128;
            r += PerlinNoise(pos, 1024, rand1) * 96;
            r += PerlinNoise(pos, 512,  rand2) * 64;
            r += PerlinNoise(pos, 256,  rand3) * 48;
            r += PerlinNoise(pos, 128,  rand4) * 12;
            r += PerlinNoise(pos, 64,   rand5) * 8;
            r += PerlinNoise(pos, 32,   rand6) * 4;
            r += PerlinNoise(pos, 16,   rand7) * 2;
            r += PerlinNoise(pos, 8,    rand8) * 1;

			r /= 128 + 96 + 64 + 48 + 12 + 8 + 4 + 2 + 1;

			return r * 128/* - 64*/;
        }

        public float SphereHeightMap(Vector2 pos)
        {
			float r;

			r  = PerlinNoise(pos, 6,  rand0, true) * 1;
			r += PerlinNoise(pos, 12, rand1, true) * 2;
			r += PerlinNoise(pos, 18, rand2, true) * 3;
            r *= Sin(pos.X);
			return r / 6;
		}

        public float PerlinNoise(Vector2 pos, int n, FixedRandom rand, bool sphere = false)
        {
			Vector2 pos1 = (Vector2i)(pos / n) * n;
            Vector2 pos2 = pos1 + (pos.X < 0 ? -n : n, pos.Y < 0 ? -n : n);

            if (sphere)
            {
				if (pos1.Y <= -180) pos1.Y += 360;
				if (pos2.Y <= -180) pos2.Y += 360;
			}

			float r0 = rand.Next(pos1);
            float r1 = rand.Next((pos1.X, pos2.Y));
            float r2 = rand.Next(pos2);
            float r3 = rand.Next((pos2.X, pos1.Y));

            float d0 = Distance(pos.X - pos1.X, n);
            float d1 = Distance(pos.X - pos2.X, n);
            float d2 = Distance(pos.Y - pos1.Y, n);
            float d3 = Distance(pos.Y - pos2.Y, n);

            float o0 = (r1 * d1 + r2 * d0) * d2;
            float o1 = (r2 * d2 + r3 * d3) * d0;
            float o2 = (r3 * d0 + r0 * d1) * d3;
            float o3 = (r0 * d3 + r1 * d2) * d1;

            return (o0 + o1 + o2 + o3) / 2;
        }

        public float Distance(float a, int n)
        {
            return (MathF.Sin((MathF.Abs(a) / n - 0.5f) * MathF.PI) + 1) / 2;
        }

        public float Sin(float a)
        {
            return MathF.Sin(MathHelper.DegreesToRadians(a));
        }
    }
}
