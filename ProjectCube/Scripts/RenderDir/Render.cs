using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using ProjectCube.Scripts.WorldDir.PlanetsDir;
using ProjectCube.Scripts.WorldDir.PlayerDir;
using ProjectCube.Scripts.WorldDir.SceneDir;

namespace ProjectCube.Scripts.RenderDir
{
    internal class Render(Window window) : Configs(window)
    {
        int vbo, vao;
        int vbo_l, vao_l;
        int vbo_base, vao_base;
        int vbo_base_l, vao_base_l;

        Shader shader = new();
        Shader shader_base = new();
        Shader planetShader = new();

		public bool reRender;
		public bool reRenderSphere;

		public bool pLocker;
		public bool pWait;

		public int c0, c1, c2, c3;

		public void Start()
        {
			(vbo, vao) = Graphics.CreateBuffer();
			(vbo_l, vao_l) = Graphics.CreateBuffer();
			(vbo_base, vao_base) = Graphics.CreateBufferOld();
			(vbo_base_l, vao_base_l) = Graphics.CreateBufferOld();

			shader.Create("Shaders/Shader_terrain.vert", "Shaders/Shader_terrain.geom", "Shaders/Shader_terrain.frag");
			shader_base.Create("Shaders/Shader_base.vert", "Shaders/Shader_base.frag");
			planetShader.Create("Shaders/PlanetsShaders/Shader.vert", "Shaders/PlanetsShaders/Shader.frag");

			GL.ClearColor(0.3f, 0.5f, 0.7f, 1.0f);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GL.Enable(EnableCap.CullFace);

			GL.BindVertexArray(vao);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BufferData(BufferTarget.ArrayBuffer, window.world.terrain.arraysCount * window.world.terrain.pointsCount* 7 * sizeof(float), 0, BufferUsageHint.StreamDraw);

			GL.BindVertexArray(vao_l);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_l);
			GL.BufferData(BufferTarget.ArrayBuffer, window.world.terrain.arraysCount * window.world.terrain.pointsCount* 7 * sizeof(float), 0, BufferUsageHint.StreamDraw);

			// планеты
			reRenderSphere = true;
		}

		public void Update()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.BlendEquation(BlendEquationMode.FuncAdd);

			//------------------
			planetShader.Use();
			planetShader.Uniform("view", window.scene.Find("Player").GetElement<Player>().view);
			planetShader.Uniform("transform[0]", Matrix4.Identity);
			planetShader.Uniform("transform[1]", window.scene.Find("Planet0").GetMatrix());
			planetShader.Uniform("transform[2]", window.scene.Find("Planet1").GetMatrix());
			planetShader.Uniform("transform[3]", window.scene.Find("Planet2").GetMatrix());

			if (reRenderSphere)
			{
				//поиск всех вершин на сцене
				List<float> verts0 = [];
				List<float> verts1 = [];
                foreach (var i in window.scene.gameObjects)
                {
					if (i.GetElement<RenderObject>() != null)
					{
						verts0.AddRange(i.GetElement<RenderObject>().verts0);
						verts1.AddRange(i.GetElement<RenderObject>().verts1);
					}
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_base);
				GL.BufferData(BufferTarget.ArrayBuffer, verts0.Count * sizeof(float), verts0.ToArray(), BufferUsageHint.StreamDraw);

				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_base_l);
				GL.BufferData(BufferTarget.ArrayBuffer, verts1.Count * sizeof(float), verts1.ToArray(), BufferUsageHint.StreamDraw);

				c0 = verts0.Count;
				c1 = verts1.Count;

				reRenderSphere = false;
			}

			GL.BindVertexArray(vao_base);
			GL.DrawArrays(PrimitiveType.Triangles, 0, c0 / 4);

			GL.BindVertexArray(vao_base_l);
			GL.DrawArrays(PrimitiveType.Lines, 0, c1 / 4);
			//------------------

			shader.Use();
			shader.Uniform("view", window.scene.Find("Player").GetElement<Player>().view);
			shader.Uniform("playerPos", window.scene.Find("Player").position);

			if (reRender && !pLocker)
			{
				pLocker = true;

                foreach (Tuple<int, float[]> i in window.world.terrain.verts)
                {
					GL.BindVertexArray(vao);
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
					GL.BufferSubData(BufferTarget.ArrayBuffer, i.Item1 * window.world.terrain.pointsCount* 7 * sizeof(float), i.Item2.Length * sizeof(float), i.Item2);
				}
				foreach (Tuple<int, float[]> i in window.world.terrain.verts_l)
                {
					GL.BindVertexArray(vao_l);
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_l);
					GL.BufferSubData(BufferTarget.ArrayBuffer, i.Item1 * window.world.terrain.pointsCount* 7 * sizeof(float), i.Item2.Length * sizeof(float), i.Item2);
				}

				reRender = false;
				pLocker = false;
				pWait = false;
			}
			else if(reRender && pLocker)
			{
				pWait = true;
			}

			GL.BindVertexArray(vao);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.DrawArrays(PrimitiveType.Points, 0, window.world.terrain.arraysCount * window.world.terrain.pointsCount);
			
			GL.BindVertexArray(vao_l);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_l);
			GL.DrawArrays(PrimitiveType.Points, 0, window.world.terrain.arraysCount * window.world.terrain.pointsCount);
		}

		public void Stop()
		{
			shader.Dispose();
		}
    }
}
