using OpenTK.Mathematics;

/// перевод из декартовых в сферические:
/// 
/// x = r * sin(o) * cos(f)
/// z = r * sin(o) * sin(f)
/// y = r * cos(o)
/// 
/// перевод из сферических в декартовые:
/// 
/// r = sqrt(x^2 + y^2 + z^2)
/// o = arccos(z / r)
/// f = arctg(y / x)
/// 
/// обозначение декартовых в сферических:
/// 
/// x = r
/// y = o
/// z = f
/// 

namespace ProjectCube.Scripts.WorldDir.PlanetsDir
{
	internal class PlanetBase
	{
		const float angle = 63.434949f;

		public static (Vector3, Vector3, Vector3)[] CreatePlanetBase(int n, float radius)
		{
			if (n < 1) return [];

			(Vector3, Vector3, Vector3)[] triangles0 = [];
			List<(Vector3, Vector3, Vector3)> triangles = [];

			triangles.AddRange(CreatePlanetBase(radius));

			for (int m = 0; m < n - 1; m++)
			{
				triangles0 = [.. triangles];
				triangles.Clear();
				foreach (var i in triangles0)
				{
					Vector3 i1 = i.Item1;
					Vector3 i2 = i.Item2;
					Vector3 i3 = i.Item3;

					Vector3 i12 = (i1 + i2) / 2;
					Vector3 i23 = (i2 + i3) / 2;
					Vector3 i31 = (i3 + i1) / 2;
					
					i12 = i12.Normalized() * radius;
					i23 = i23.Normalized() * radius;
					i31 = i31.Normalized() * radius;

					triangles.Add((i1, i12, i31));
					triangles.Add((i12, i2, i23));
					triangles.Add((i23, i3, i31));
					triangles.Add((i12, i23, i31));
				}
			}

			return [.. triangles];
		}

		public static (Vector3, Vector3, Vector3)[] CreatePlanetBase(float radius)
		{
			List<(Vector3, Vector3, Vector3)> triangles = [];

			Vector3 pos;

			float a = 360 / 5;

			pos = (radius, 0, 0);
			for (int i = 0; i < 5; i++)
			{
				triangles.Add(((radius, angle, i * a), pos, (radius, angle, (i + 1) * a)));
				triangles.Add(((radius, angle, i * a), (radius, angle, (i + 1) * a), (radius, 180 - angle, i * a + a / 2)));
			}

			pos = (radius, 180, 0);
			for (int i = 0; i < 5; i++)
			{
				triangles.Add(((radius, 180 - angle, (i + 1) * a + a / 2), (radius, 180 - angle, i * a + a / 2), (radius, angle, (i + 1) * a)));
				triangles.Add((pos, (radius, 180 - angle, i * a + a / 2), (radius, 180 - angle, (i + 1) * a + a / 2)));
			}

			for (int i = 0; i < triangles.Count; i++)
			{
				triangles[i] = (ToDecant(triangles[i].Item1), ToDecant(triangles[i].Item2), ToDecant(triangles[i].Item3));
			}

			return [.. triangles];
		}

		public static Vector3 ToDecant(Vector3 pos)
		{
			Vector3 o;

			pos.Y = MathHelper.DegreesToRadians(pos.Y);
			pos.Z = MathHelper.DegreesToRadians(pos.Z);

			o.X = pos.X * MathF.Sin(pos.Y) * MathF.Cos(pos.Z);
			o.Z = pos.X * MathF.Sin(pos.Y) * MathF.Sin(pos.Z);
			o.Y = pos.X * MathF.Cos(pos.Y);

			return o;
		}
		public static Vector3 ToSphere(Vector3 pos)
		{
			Vector3 o;

			o.X = pos.Length;
			o.Y = MathF.Atan(MathF.Sqrt(pos.X * pos.X + pos.Z * pos.Z) / pos.Y);
			o.Z = MathF.Atan2(pos.Z, pos.X);

			o.Y = MathHelper.RadiansToDegrees(o.Y);
			o.Z = MathHelper.RadiansToDegrees(o.Z);

			return o;
		}
	}
}
