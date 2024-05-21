using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ProjectCube.Scripts.WorldDir.PlayerDir;
using ProjectCube.Scripts.WorldDir.SceneDir;

namespace ProjectCube.Scripts.WorldDir.PlanetsDir
{
	internal class Planet(Window window, float radius, int iterPlanet, int iterArray, int planetID, float mass, float rotate = 0) : InterBase
	{
		GenSphereTerrain sphereTerrain = new();

		Dictionary<Vector2, Lines> lines = [];

		(Vector3, Vector3, Vector3)[] triangles = [];

		float renderDistance = 200;

		bool reRender = true;

		public float mass = mass;

		public override void Start()
		{
			List<float> verts = [];
			List<float> verts_line = [];

			triangles = PlanetBase.CreatePlanetBase(iterPlanet, radius);
			sphereTerrain.Gen(ref triangles);

			verts_line.AddRange([0, 0, 0, 1 + planetID * 64]);
			verts_line.AddRange([radius * 2, 0, 0, 1 + planetID * 64]);

			verts_line.AddRange([0, 0, 0, 2 + planetID * 64]);
			verts_line.AddRange([0, radius * 2, 0, 2 + planetID * 64]);

			verts_line.AddRange([0, 0, 0, 3 + planetID * 64]);
			verts_line.AddRange([0, 0, radius * 2, 3 + planetID * 64]);

			verts_line.AddRange([0, 0, 0, 4 + planetID * 64]);
			verts_line.AddRange([-radius * 2, 0, 0, 4 + planetID * 64]);

			verts_line.AddRange([0, 0, 0, 5 + planetID * 64]);
			verts_line.AddRange([0, -radius * 2, 0, 5 + planetID * 64]);

			verts_line.AddRange([0, 0, 0, 6 + planetID * 64]);
			verts_line.AddRange([0, 0, -radius * 2, 6 + planetID * 64]);

			gameObject.GetElement<RenderObject>().verts0 = [.. verts.ToArray()];
			gameObject.GetElement<RenderObject>().verts1 = [.. verts_line.ToArray()];
		}

		public override void Update(float time)
		{
			if (window.gameWindow.KeyboardState.IsKeyPressed(Keys.E))
			{
				List<float> verts = [];

				Matrix4 matrix = Matrix4.Identity;

				SphereVector vector = SphereVector.ToSphere(window.scene.Find("Player").GetElement<Player>().position);
				Console.WriteLine($"radius {vector.R}  O {vector.O}  F {vector.F}");
				matrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(-vector.O));
				matrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-vector.F));

				Vector3 local0 = window.scene.Find("Player").GetElement<Player>().position;
				Vector3 localX = local0 + (new Vector4(10, 0, 0, 1) * matrix).Xyz;
				Vector3 localY = local0 + (new Vector4(0, 10, 0, 1) * matrix).Xyz;
				Vector3 localZ = local0 + (new Vector4(0, 0, 10, 1) * matrix).Xyz;

				verts.AddRange([local0.X, local0.Y, local0.Z, 1 + planetID * 64]);
				verts.AddRange([localX.X, localX.Y, localX.Z, 1 + planetID * 64]);

				verts.AddRange([local0.X, local0.Y, local0.Z, 2 + planetID * 64]);
				verts.AddRange([localY.X, localY.Y, localY.Z, 2 + planetID * 64]);

				verts.AddRange([local0.X, local0.Y, local0.Z, 3 + planetID * 64]);
				verts.AddRange([localZ.X, localZ.Y, localZ.Z, 3 + planetID * 64]);

				gameObject.GetElement<RenderObject>().verts1 = [.. gameObject.GetElement<RenderObject>().verts1, .. verts.ToArray()];
				window.render.reRenderSphere = true;
			}
		}

		public override void PhysUpdate()
		{
			gameObject.rotation.X += rotate;
		}

		public override void GenUpdate()
		{
			List<float> verts = [];

			//генерация территории
			verts = [];
			foreach (var i in triangles)
			{
				Vector4 t4 = new((i.Item1 + i.Item2 + i.Item3).X / 3, (i.Item1 + i.Item2 + i.Item3).Y / 3, (i.Item1 + i.Item2 + i.Item3).Z / 3, 1);
				Vector3 mid = ((i.Item1 + i.Item2 + i.Item3) / 3);
				Vector3 t3 = (t4 * gameObject.GetMatrix()).Xyz;
				Vector3 playerPos = window.scene.Find("Player").GetElement<Player>().position;
				if ((t3 - playerPos).Length < renderDistance)
				{
					Vector2 pos = SphereVector.ToSphere(mid).GetVector2();
					if (!lines.TryGetValue(pos, out Lines tmp))
					{
						sphereTerrain.GenLines(out tmp, i, iterArray, radius);
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
							verts.AddRange([j.posXYZ1.X, j.posXYZ1.Y, j.posXYZ1.Z, planetID * 64]);
							verts.AddRange([j.posXYZ2.X, j.posXYZ2.Y, j.posXYZ2.Z, planetID * 64]);
							verts.AddRange([j.posXYZ3.X, j.posXYZ3.Y, j.posXYZ3.Z, planetID * 64]);
						}
					}
					else
					{
						verts.AddRange([i.Item1.X, i.Item1.Y, i.Item1.Z, planetID * 64]);
						verts.AddRange([i.Item2.X, i.Item2.Y, i.Item2.Z, planetID * 64]);
						verts.AddRange([i.Item3.X, i.Item3.Y, i.Item3.Z, planetID * 64]);
					}
				}

				gameObject.GetElement<RenderObject>().verts0 = [.. verts.ToArray()];

				reRender = false;
				window.render.reRenderSphere = true;
			}
		}
	}
}
