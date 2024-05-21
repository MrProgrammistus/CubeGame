using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using PhysSim;
using ProjectCube.Scripts.RenderDir;

class Program()
{
	static GameWindowSettings gameWindowSettings = new();
	static NativeWindowSettings nativeWindowSettings = new();
	static GameWindow gameWindow = new(gameWindowSettings, nativeWindowSettings);

	static int vbo, vao;
	static Shader shader = new();

	static int w = 800, h = 600;

	static float[] data;

	static Player player = new(gameWindow);

	static Line x = new((-100, 0, 0), (200, 0, 0));
	static Line y = new((0, -100, 0), (0, 200, 0));
	static Line z = new((0, 0, -100), (0, 0, 200));

	static Prism cube = new((1, 1, 1), (4, 4, 4));
	static Cube player_cube = new((1, 1, 1), (-0.5f, -0.5f, -0.5f));
	static Quad plane = new((10, -2, 10), (-5, 0, -5));

	static void Main()
	{
		gameWindow.ClientSize = new(w, h);
		gameWindow.UpdateFrequency = 60;

		gameWindow.UpdateFrame += Update;
		gameWindow.Load += Start;
		
		gameWindow.Run();
	}

	static void Start()
	{
		(vbo, vao) = Graphics.CreateBufferOld();
		shader.Create("shader.vert", "shader.frag");
		shader.Use();

		GL.ClearColor(0.7f, 0.7f, 0.7f, 1.0f);
		GL.Viewport(0, 0, w, h);
		player.Resize();
	}

	static void Update(FrameEventArgs e)
	{
		player.Update((float)e.Time);

		GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		//if(gameWindow.IsKeyDown(Keys.E)) player_cube.Set(player_cube.size, player.position + (-0.5f, -0.5f, -0.5f));

		int color = 0;
		//Geometric cube = Program.cube;
		if (Collision.GJK(cube, player_cube, out Collision.Simplex points)) color = 2;
		Vector3 epaV = CollisonEPA.CustomEPA(cube, player_cube);
		if (color == 2)
		{
			Vector3 p = Projection(player.velocity, epaV);
			Vector3 p2 = Projection(player.deltaPos, epaV);
			player.velocity -= p * 2f;
			player.position -= p2;
			color = 0;
		}
		player.EndUpdate((float)e.Time);
		player_cube.Set(player_cube.size, player.position + (-0.5f, -0.5f, -0.5f));
		shader.Uniform("view", player.view);


		/*Line normal1 = new(cube.point2, cube.normal1);
		Line normal2 = new(cube.point6, cube.normal2);
		//Line normal3 = new(cube.point5, cube.normal3);
		Line normal4 = new(cube.point6, cube.normal4);
		Line normal5 = new(cube.point7, cube.normal5);
		Line normal6 = new(cube.point8, cube.normal6);*/

		Line epa = new(cube.pos , epaV);
		Line delta = new(cube.pos , Vector3.Normalize(player_cube.pos - cube.pos));

		Vector3 a = (2, 0, 0);
		Vector3 b = (1, 1, 1);
		Vector3 c = Projection(a, b);
		Line vectorA = new((-10, 10, -10), a);
		Line vectorB = new((-10, 10, -10), b);
		Line vectorC = new((-10, 10, -10), c);

		data = [.. cube.getVerts(color), .. player_cube.getVerts(color),
				/*.. normal1.getVerts(3), .. normal2.getVerts(3), //.. normal3.getVerts(3),
				.. normal4.getVerts(3), .. normal5.getVerts(3), .. normal6.getVerts(3),
				.. vectorA.getVerts(1), .. vectorB.getVerts(1), .. vectorC.getVerts(0),*/
				.. x.getVerts(1), .. y.getVerts(1), .. z.getVerts(1),
				.. plane.getVerts(0),
				.. epa.getVerts(1), .. delta.getVerts(1),
		];

		GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StreamDraw);

		GL.DrawArrays(PrimitiveType.Lines, 0, data.Length / 4);

		gameWindow.SwapBuffers();
	}

	public static Vector3 Projection(Vector3 a, Vector3 b)
	{
		float cl = Vector3.Dot(a, b) / b.Length;
		Vector3 c = b * cl / b.Length;
		return c;
	}
}

public class Geometric
{
	public Vector3[] points = [];

	public Vector3[] normals = [];

	public Vector3 pos;

	public Vector3 FindFurthestPoint(Vector3 dir)
	{
		Vector3 maxPoint = (0, 0, 0);
		float maxDist = float.MinValue;

		foreach (var i in points)
		{
			float distance = Vector3.Dot(i, dir);
			if (distance > maxDist)
			{
				maxDist = distance;
				maxPoint = i;
			}
		}

		return maxPoint;
	}
}

class Quad : Geometric
{
	public Vector3 size;

	public Vector3 point1;
	public Vector3 point2;
	public Vector3 point3;
	public Vector3 point4;

	public Quad(Vector3 size, Vector3 pos) => Set(size, pos);
	public void Set(Vector3 size, Vector3 pos)
	{
		this.size = size;
		this.pos = pos;

		point1 = pos;
		point2 = (pos.X, pos.Y, size.Z + pos.Z);
		point3 = size + pos;
		point4 = (size.X + pos.X, pos.Y, pos.Z);

		normals = [(0, 1, 0)];
		points = [point1, point2, point3, point4];
	}

	public float[] getVerts(int intData)
	{
		float[] data = {point1.X, point1.Y, point1.Z, intData,
						point2.X, point2.Y, point2.Z, intData,
						point2.X, point2.Y, point2.Z, intData,
						point3.X, point3.Y, point3.Z, intData,
						point3.X, point3.Y, point3.Z, intData,
						point4.X, point4.Y, point4.Z, intData,
						point4.X, point4.Y, point4.Z, intData,
						point1.X, point1.Y, point1.Z, intData,};
		return data;
	}
}

class Triangle : Geometric
{
	public Vector3 size;

	public Vector3 point1;
	public Vector3 point2;
	public Vector3 point3;

	public Triangle(Vector3 size, Vector3 pos) => Set(size, pos);
	public void Set(Vector3 size, Vector3 pos)
	{
		this.size = size;
		this.pos = pos;

		point1 = pos;
		point2 = (pos.X, size.Y + pos.Y, pos.Z);
		point3 = size + pos;

		points = [point1, point2, point3];
	}

	public float[] getVerts(int intData)
	{
		float[] data = {point1.X, point1.Y, 0, intData,
						point2.X, point2.Y, 0, intData,
						point2.X, point2.Y, 0, intData,
						point3.X, point3.Y, 0, intData,
						point3.X, point3.Y, 0, intData,
						point1.X, point1.Y, 0, intData,};
		return data;
	}
}

class Line(Vector3 pos1, Vector3 pos2)
{
	public Vector3 point1 = pos1;
	public Vector3 point2 = pos2 + pos1;

	public float[] getVerts(int intData)
	{
		float[] data = {point1.X, point1.Y, point1.Z, intData,
						point2.X, point2.Y, point2.Z, intData,};
		return data;
	}
}

class Point(Vector2 pos)
{
	public Vector2 pos = pos;

	public Vector2 point1 = (pos.X + 5, pos.Y + 5);
	public Vector2 point2 = (pos.X - 5, pos.Y + 5);
	public Vector2 point3 = (pos.X - 5, pos.Y - 5);
	public Vector2 point4 = (pos.X + 5, pos.Y - 5);

	public float[] getVerts(int intData)
	{
		float[] data = {point1.X, point1.Y, 0, intData,
						point2.X, point2.Y, 0, intData,
						point2.X, point2.Y, 0, intData,
						point3.X, point3.Y, 0, intData,
						point3.X, point3.Y, 0, intData,
						point4.X, point4.Y, 0, intData,
						point4.X, point4.Y, 0, intData,
						point1.X, point1.Y, 0, intData,};
		return data;
	}
}

class Polygon : Geometric
{
	public Polygon(Vector3[] pos) => Set(pos);
	public void Set(Vector3[] pos)
	{
		points = pos;
	}

	public float[] getVerts(int intData)
	{
		List<float> data = [];

        for(int i = 0; i < points.Length; i++)
        {
			data.AddRange([points[i].X, points[i].Y, points[i].Z, intData]);
			if(i < points.Length - 1) data.AddRange([points[i + 1].X, points[i + 1].Y, points[i + 1].Z, intData]);
			else data.AddRange([points[0].X, points[0].Y, points[0].Z, intData]);
		}

        return data.ToArray();
	}
}

class Cube : Geometric
{
	public Vector3 size;

	public Vector3 point1;
	public Vector3 point2;
	public Vector3 point3;
	public Vector3 point4;
	public Vector3 point5;
	public Vector3 point6;
	public Vector3 point7;
	public Vector3 point8;

	public Vector3 normal1;
	public Vector3 normal2;
	public Vector3 normal3;
	public Vector3 normal4;
	public Vector3 normal5;
	public Vector3 normal6;

	public Cube(Vector3 size, Vector3 pos) => Set(size, pos);
	public void Set(Vector3 size, Vector3 pos)
	{
		this.size = size;
		this.pos = pos;

		point1 = pos;
		point2 = (pos.X, size.Y + pos.Y, pos.Z);
		point3 = pos + (size.X, size.Y, 0);
		point4 = (size.X + pos.X, pos.Y, pos.Z);

		point5 = pos + (0, 0, size.Z);
		point6 = (pos.X, size.Y + pos.Y, pos.Z + size.Z);
		point7 = size + pos;
		point8 = (size.X + pos.X, pos.Y, pos.Z + size.Z);

		normal1 = Vector3.Cross(point1 - point2, point2 - point3);
		normal2 = Vector3.Cross(point7 - point6, point6 - point5);
		normal3 = Vector3.Cross(point1 - point5, point5 - point6);
		normal4 = Vector3.Cross(point2 - point6, point6 - point7);
		normal5 = Vector3.Cross(point3 - point7, point7 - point8);
		normal6 = Vector3.Cross(point4 - point8, point8 - point1);

		normals = [normal1, normal2, normal3, normal4, normal5, normal6];
		points = [point1, point2, point3, point4, point5, point6, point7, point8];
	}

	public float[] getVerts(int intData)
	{
		float[] data = {point1.X, point1.Y, point1.Z, intData,
						point2.X, point2.Y, point2.Z, intData,
						point2.X, point2.Y, point2.Z, intData,
						point3.X, point3.Y, point3.Z, intData,
						point3.X, point3.Y, point3.Z, intData,
						point4.X, point4.Y, point4.Z, intData,
						point4.X, point4.Y, point4.Z, intData,
						point1.X, point1.Y, point1.Z, intData,

						point5.X, point5.Y, point5.Z, intData,
						point6.X, point6.Y, point6.Z, intData,
						point6.X, point6.Y, point6.Z, intData,
						point7.X, point7.Y, point7.Z, intData,
						point7.X, point7.Y, point7.Z, intData,
						point8.X, point8.Y, point8.Z, intData,
						point8.X, point8.Y, point8.Z, intData,
						point5.X, point5.Y, point5.Z, intData,

						point1.X, point1.Y, point1.Z, intData,
						point5.X, point5.Y, point5.Z, intData,

						point2.X, point2.Y, point2.Z, intData,
						point6.X, point6.Y, point6.Z, intData,

						point3.X, point3.Y, point3.Z, intData,
						point7.X, point7.Y, point7.Z, intData,

						point4.X, point4.Y, point4.Z, intData,
						point8.X, point8.Y, point8.Z, intData,
						};
		return data;
	}
}

class Prism : Geometric
{
	public Vector3 size;

	//public Vector3 point1;
	public Vector3 point2;
	public Vector3 point3;
	public Vector3 point4;
	//public Vector3 point5;
	public Vector3 point6;
	public Vector3 point7;
	public Vector3 point8;

	public Vector3 normal1;
	public Vector3 normal2;
	//public Vector3 normal3;
	public Vector3 normal4;
	public Vector3 normal5;
	public Vector3 normal6;

	public Prism(Vector3 size, Vector3 pos) => Set(size, pos);
	public void Set(Vector3 size, Vector3 pos)
	{
		this.size = size;
		this.pos = pos;

		//point1 = pos;
		point2 = (pos.X, size.Y + pos.Y, pos.Z);
		point3 = pos + (size.X, size.Y, 0);
		point4 = (size.X + pos.X, pos.Y, pos.Z);

		//point5 = pos + (0, 0, size.Z);
		point6 = (pos.X, size.Y + pos.Y, pos.Z + size.Z);
		point7 = size + pos;
		point8 = (size.X + pos.X, pos.Y, pos.Z + size.Z);

		normal1 = Vector3.Cross(point2 - point3, point3 - point4);
		normal2 = Vector3.Cross(point8 - point7, point7 - point6);
		//normal3 = Vector3.Cross(point1 - point5, point5 - point6);
		normal4 = Vector3.Cross(point2 - point6, point6 - point7);
		normal5 = Vector3.Cross(point3 - point7, point7 - point8);
		normal6 = Vector3.Cross(point4 - point8, point8 - point2);

		normals = [normal1, normal2, normal4, normal5, normal6];
		points = [point2, point3, point4, point6, point7, point8];
	}

	public float[] getVerts(int intData)
	{
		float[] data = {//point1.X, point1.Y, point1.Z, intData,
						//point2.X, point2.Y, point2.Z, intData,
						point2.X, point2.Y, point2.Z, intData,
						point3.X, point3.Y, point3.Z, intData,
						point3.X, point3.Y, point3.Z, intData,
						point4.X, point4.Y, point4.Z, intData,
						point4.X, point4.Y, point4.Z, intData,
						point2.X, point2.Y, point2.Z, intData,
						//point1.X, point1.Y, point1.Z, intData,

						//point5.X, point5.Y, point5.Z, intData,
						//point6.X, point6.Y, point6.Z, intData,
						point6.X, point6.Y, point6.Z, intData,
						point7.X, point7.Y, point7.Z, intData,
						point7.X, point7.Y, point7.Z, intData,
						point8.X, point8.Y, point8.Z, intData,
						point8.X, point8.Y, point8.Z, intData,
						point6.X, point6.Y, point6.Z, intData,
						//point5.X, point5.Y, point5.Z, intData,

						//point1.X, point1.Y, point1.Z, intData,
						//point5.X, point5.Y, point5.Z, intData,

						point2.X, point2.Y, point2.Z, intData,
						point6.X, point6.Y, point6.Z, intData,

						point3.X, point3.Y, point3.Z, intData,
						point7.X, point7.Y, point7.Z, intData,

						point4.X, point4.Y, point4.Z, intData,
						point8.X, point8.Y, point8.Z, intData,
						};
		return data;
	}
}