using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ProjectCube.Scripts.WorldDir.PlayerDir;
using ProjectCube.Scripts.WorldDir.SceneDir;

namespace ProjectCube.Scripts.WorldDir.PlanetsDir
{
	internal class Planets(Window window) : InterBase
	{
		//public float[] verts = [];
		//public float[] verts_line = [];

		GenSphereTerrain sphereTerrain = new GenSphereTerrain();

		Dictionary<Vector2, Lines> lines = new Dictionary<Vector2, Lines>();

		(Vector3, Vector3, Vector3)[] triangles = [];
		float r = 300;
		Vector3 pos = (0, 0, 0);

		float renderDistance = 200;

		public override void Start()
		{
			List<float> verts = [];
			List<float> verts_line = [];

			triangles = PlanetBase.CreatePlanetBase(5, r);
			sphereTerrain.Gen(ref triangles);
			//sphereTerrain.GenLines(out Lines lines, triangles[0], r);

			foreach (var i in triangles)
			{
				Vector3 mid = (i.Item1 + i.Item2 + i.Item3) / 3;
				if ((mid - window.scene.Find("Player").GetElement<Player>().position).Length < 100)
				{
					sphereTerrain.GenLines(out Lines tmp, i, 3, r);
					this.lines.Add(tmp.posOF, tmp);

					foreach (var j in tmp.lines)
					{
						verts.AddRange([j.posXYZ1.X, j.posXYZ1.Y, j.posXYZ1.Z, 0]);
						verts.AddRange([j.posXYZ2.X, j.posXYZ2.Y, j.posXYZ2.Z, 0]);
						verts.AddRange([j.posXYZ3.X, j.posXYZ3.Y, j.posXYZ3.Z, 0]);
					}
				}
				else
				{
					verts.AddRange([i.Item1.X, i.Item1.Y, i.Item1.Z, 0]);
					verts.AddRange([i.Item2.X, i.Item2.Y, i.Item2.Z, 0]);
					verts.AddRange([i.Item3.X, i.Item3.Y, i.Item3.Z, 0]);
				}
			}
			/*foreach (var i in lines.lines)
			{
				verts.AddRange([i.posXYZ1.X, i.posXYZ1.Y, i.posXYZ1.Z, 0]);
				verts.AddRange([i.posXYZ2.X, i.posXYZ2.Y, i.posXYZ2.Z, 0]);
				verts.AddRange([i.posXYZ3.X, i.posXYZ3.Y, i.posXYZ3.Z, 0]);
			}*/

			verts_line.AddRange([0, 0, 0, 1]);
			verts_line.AddRange([r * 2, 0, 0, 1]);
			
			verts_line.AddRange([0, 0, 0, 2]);
			verts_line.AddRange([0, r * 2, 0, 2]);
			
			verts_line.AddRange([0, 0, 0, 3]);
			verts_line.AddRange([0, 0, r * 2, 3]);
			
			verts_line.AddRange([0, 0, 0, 4]);
			verts_line.AddRange([-r * 2, 0, 0, 4]);
			
			verts_line.AddRange([0, 0, 0, 5]);
			verts_line.AddRange([0, -r * 2, 0, 5]);
			
			verts_line.AddRange([0, 0, 0, 6]);
			verts_line.AddRange([0, 0, -r * 2, 6]);

			window.scene.Find("Planet0").GetElement<RenderObject>().verts0 = [.. verts.ToArray()];
			window.scene.Find("Planet0").GetElement<RenderObject>().verts1 = [.. verts_line.ToArray()];
		}

		public override void Update(float time)
		{
			if (window.gameWindow.KeyboardState.IsKeyPressed(Keys.E))
			{
				List<float> verts = [];

				Matrix4 matrix = Matrix4.Identity;

				SphereVector vector = SphereVector.ToSphere(window.scene.Find("Player").GetElement<Player>().position);
				Console.WriteLine($"R {vector.R}  O {vector.O}  F {vector.F}");
				matrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(-vector.O));
				matrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-vector.F));

				Vector3 local0 = window.scene.Find("Player").GetElement<Player>().position;
				Vector3 localX = local0 + (new Vector4(10, 0, 0, 1) * matrix).Xyz;
				Vector3 localY = local0 + (new Vector4(0, 10, 0, 1) * matrix).Xyz;
				Vector3 localZ = local0 + (new Vector4(0, 0, 10, 1) * matrix).Xyz;

				verts.AddRange([local0.X, local0.Y, local0.Z, 1]);
				verts.AddRange([localX.X, localX.Y, localX.Z, 1]);

				verts.AddRange([local0.X, local0.Y, local0.Z, 2]);
				verts.AddRange([localY.X, localY.Y, localY.Z, 2]);

				verts.AddRange([local0.X, local0.Y, local0.Z, 3]);
				verts.AddRange([localZ.X, localZ.Y, localZ.Z, 3]);

				window.scene.Find("Planet0").GetElement<RenderObject>().verts1 = [.. verts.ToArray()];
				window.render.reRenderSphere = true;
			}
		}

		public override void GenUpdate()
		{
			List<float> verts = [];

			//генерация территории
			verts = [];
			bool reRender = false;
			foreach (var i in triangles)
			{
				Vector3 mid = (i.Item1 + i.Item2 + i.Item3) / 3;
				if ((mid - window.scene.Find("Player").GetElement<Player>().position).Length < renderDistance)
				{
					Vector2 pos = SphereVector.ToSphere(mid).GetVector2();
					if (!lines.TryGetValue(pos, out Lines tmp))
					{
						sphereTerrain.GenLines(out tmp, i, 3, r);
						lines.Add(pos, tmp);
						reRender = true;
						Bit.SetBit(ref tmp.flags, 0);
					}
					else
					{
						Bit.SetBit(ref tmp.flags, 0);
					}
				}
			}
			//удаление далеких массивов
			foreach (var i in lines)
			{
				if (!Bit.GetBit(i.Value.flags, 0))
				{
					lines.Remove(i.Key);
					reRender = true;
				}
				else
				{
					Bit.ResetBit(ref i.Value.flags, 0);
				}
			}
			//рендер территории если надо
			if (reRender)
			{
				foreach (var i in triangles)
				{
					Vector3 mid = (i.Item1 + i.Item2 + i.Item3) / 3;
					Vector2 pos = SphereVector.ToSphere(mid).GetVector2();
					if (lines.TryGetValue(pos, out Lines tmp))
					{
						foreach (var j in tmp.lines)
						{
							verts.AddRange([j.posXYZ1.X, j.posXYZ1.Y, j.posXYZ1.Z, 0]);
							verts.AddRange([j.posXYZ2.X, j.posXYZ2.Y, j.posXYZ2.Z, 0]);
							verts.AddRange([j.posXYZ3.X, j.posXYZ3.Y, j.posXYZ3.Z, 0]);
						}
					}
					else
					{
						verts.AddRange([i.Item1.X, i.Item1.Y, i.Item1.Z, 0]);
						verts.AddRange([i.Item2.X, i.Item2.Y, i.Item2.Z, 0]);
						verts.AddRange([i.Item3.X, i.Item3.Y, i.Item3.Z, 0]);
					}
				}

				window.scene.Find("Planet0").GetElement<RenderObject>().verts0 = [.. verts.ToArray()];

				reRender = false;
				window.render.reRenderSphere = true;
			}
		}

		public static Vector3 Projection(Vector3 a, Vector3 b)
		{
			float cl = Vector3.Dot(a, b) / b.Length;
			Vector3 c = b * cl / b.Length;
			return c;
		}
	}

	public struct SphereVector()
	{
		public float R;
		public float O;
		public float F;

		public Vector3 ToDecant()
		{
			Vector3 o;

			float O = MathHelper.DegreesToRadians(this.O);
			float F = MathHelper.DegreesToRadians(this.F);

			o.X = R * MathF.Sin(O) * MathF.Cos(F);
			o.Z = R * MathF.Sin(O) * MathF.Sin(F);
			o.Y = R * MathF.Cos(O);

			return o;
		}
		public static SphereVector ToSphere(Vector3 pos)
		{
			SphereVector o;

			o.R = pos.Length;
			o.O = MathF.Atan2(MathF.Sqrt(pos.X * pos.X + pos.Z * pos.Z), pos.Y);
			o.F = MathF.Atan2(pos.Z, pos.X);

			o.O = MathHelper.RadiansToDegrees(o.O);
			o.F = MathHelper.RadiansToDegrees(o.F);

			return o;
		}

		public Vector2 GetVector2()
		{
			return (O, F);
		}
	}
}
