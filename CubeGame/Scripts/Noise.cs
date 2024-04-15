using OpenTK.Mathematics;

namespace CubeGame.Scripts
{
    internal class Noise
    {
        public Dictionary<Vector2i, float> rand0 = [];
        public Dictionary<Vector2i, float> rand05 = [];
        public Dictionary<Vector2i, float> rand1 = [];
        public Dictionary<Vector2i, float> rand2 = [];
        public Dictionary<Vector2i, float> rand3 = [];
        public Dictionary<Vector2i, float> rand4 = [];
        public Dictionary<Vector2i, float> gen = [];

        public float Generate(int x, int y)
        {
			if (gen.TryGetValue((x, y), out float r)) return r;

			r =  SmoothRandom(x, y, 512, 99.01f, 98.83f, 99.41f, rand0) * 64;
			r += SmoothRandom(x, y, 256, 98.51f, 99.31f, 98.59f, rand05) * 48;

			r += SmoothRandom(x, y, 128, 99.49f, 99.67f, 99.73f, rand1) * 12;
			r += SmoothRandom(x, y, 64,  99.29f, 99.31f, 99.41f, rand2) * 8;
			r += SmoothRandom(x, y, 32,  99.01f, 99.07f, 99.23f, rand3) * 4;
			r += SmoothRandom(x, y, 16,  98.71f, 98.83f, 98.87f, rand4) * 2;
			r += SmoothRandom(x, y, 8,   98.51f, 98.57f, 98.59f, rand4) * 1;
			r /= 64 + 48 + 12 + 8 + 4 + 2 + 1;

			gen.Add((x, y), r);
			return r;
        }

        public float Random(int x, int y, float a, float b, float c, Dictionary<Vector2i, float> rand)
        {
			if (rand.TryGetValue((x, y), out float r)) return r;

			r = MathF.Sin(x * a + y * b + c);
			r = (r + 1) / 2;
			r *= 1000000;
			r = ((int)r & 1023) / 1023f;

			rand.Add((x, y), r);
			return r;
        }

        public float SmoothRandom(int x, int y, int n, float a, float b, float c, Dictionary<Vector2i, float> rand)
        {
            //числа
			int x1 = x / n * n;
            int y1 = y / n * n;
			int x2 = x1 + (x < 0 ? -n : n);
			int y2 = y1 + (y < 0 ? -n : n);

			//получение случайного значения
			float r0 = Random(x1, y1, a, b, c, rand);
			float r1 = Random(x1, y2, a, b, c, rand);
			float r2 = Random(x2, y2, a, b, c, rand);
			float r3 = Random(x2, y1, a, b, c, rand);

			//дистанция
			float d0 = Distance(x - x1, n);
			float d1 = Distance(x - x2, n);
			float d2 = Distance(y - y1, n);
			float d3 = Distance(y - y2, n);

			//сглаживание
			float o0 = (r1 * d1 + r2 * d0) * d2;
			float o1 = (r2 * d2 + r3 * d3) * d0;
			float o2 = (r3 * d0 + r0 * d1) * d3;
			float o3 = (r0 * d3 + r1 * d2) * d1;

			return (o0 + o1 + o2 + o3) / 2;
        }

        public float Distance(int a, int n)
        {
			return (MathF.Sin(((MathF.Abs(a) / n) - 0.5f) * MathF.PI) + 1) / 2;
		}
    }
}
