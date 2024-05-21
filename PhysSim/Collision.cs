using OpenTK.Mathematics; 

namespace PhysSim
{
	public class Collision
	{
		static Vector3 Support(Geometric g1, Geometric g2, Vector3 dir)
		{
			return g1.FindFurthestPoint(dir) - g2.FindFurthestPoint(-dir);
		}

		public static bool GJK(Geometric g1, Geometric g2, out Simplex points)
		{
			Vector3 support = Support(g1, g2, (1, 0, 0));

			points = new();
			points.Push_front(support, (1, 0, 0));
			Vector3 direction = -support;

			for(int i = 0; i < 100; i++)
			{
				support = Support(g1, g2, direction);

				if (Vector3.Dot(support, direction) <= 0) return false;

				points.Push_front(support, direction);

				if (NextSimplex(ref points, ref direction))
				{
					return true;
				}
			}
			Console.WriteLine("Ошибка коллизии");
			return false;
		}
		static bool NextSimplex(ref Simplex points, ref Vector3 direction)
		{
			switch (points.size)
			{
				case 2: return Line(ref points, ref direction);
				case 3: return Triangle(ref points, ref direction);
				case 4: return Tetrahedron(ref points, ref direction);
			}

			return false;
		}

		static bool SameDirection(Vector3 direction, Vector3 ao)
		{
			return Vector3.Dot(direction, ao) > 0;
		}

		static bool Line(ref Simplex points, ref Vector3 direction)
		{
			Vector3 a = points.points[0];
			Vector3 b = points.points[1];

			Vector3 ab = b - a;
			Vector3 ao = -a;

			if (SameDirection(ab, ao)) direction = Vector3.Cross(Vector3.Cross(ab, ao), ab);
			else
			{
				points.points = [a];
				direction = ao;
			}

			return false;
		}
		static bool Triangle(ref Simplex points, ref Vector3 direction)
		{
			Vector3 a = points.points[0];
			Vector3 b = points.points[1];
			Vector3 c = points.points[2];

			Vector3 ab = b - a;
			Vector3 ac = c - a;
			Vector3 ao = -a;

			Vector3 abc = Vector3.Cross(ab, ac);

			if(SameDirection(Vector3.Cross(abc, ac), ao))
			{
				if (SameDirection(ac, ao))
				{
					points.points = [a, c];
					direction = Vector3.Cross(Vector3.Cross(ac, ao), ac);
				}
				else
				{
					points.points = [a, b];
					return Line(ref points, ref direction);
				}
			}
			else
			{
				if(SameDirection(Vector3.Cross(ab, abc), ao))
				{
					points.points = [a, b];
					return Line(ref points, ref direction);
				}
				else
				{
					if(SameDirection(abc, ao)) direction = abc;
					else
					{
						points.points = [a, c, b];
						direction = -abc;
					}
				}
			}

			return false;
		}
		static bool Tetrahedron(ref Simplex points, ref Vector3 direction)
		{
			Vector3 a = points.points[0];
			Vector3 b = points.points[1];
			Vector3 c = points.points[2];
			Vector3 d = points.points[3];

			Vector3 ab = b - a;
			Vector3 ac = c - a;
			Vector3 ad = d - a;
			Vector3 ao = -a;

			Vector3 abc = Vector3.Cross(ab, ac);
			Vector3 acd = Vector3.Cross(ac, ad);
			Vector3 adb = Vector3.Cross(ad, ab);

			if (SameDirection(abc, ao))
			{
				points.points = [a, b, c];
				return Triangle(ref points, ref direction);
			}

			if (SameDirection(acd, ao))
			{
				points.points = [a, c, d];
				return Triangle(ref points, ref direction);
			}

			if (SameDirection(adb, ao))
			{
				points.points = [a, d, b];
				return Triangle(ref points, ref direction);
			}

			return true;
		}

		public class Simplex
		{
			public Vector3[] points = [];
			public Vector3[] points0 = [];
			public int size { get { return points.Length; } }
			public void Push_front(Vector3 point, Vector3 point0)
			{
				if (size == 0)		points = [point];
				else if (size == 1)	points = [point, points[0]];
				else if (size == 2) points = [point, points[0], points[1]];
				else				points = [point, points[0], points[1], points[2]];
				
				if (points0.Length == 0)		points0 = [point0];
				else if (points0.Length == 1)	points0 = [point0, points0[0]];
				else if (points0.Length == 2)	points0 = [point0, points0[0], points0[1]];
				else							points0 = [point0, points0[0], points0[1], points0[2]];
			}
		}
	}
}
