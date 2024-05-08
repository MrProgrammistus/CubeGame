using OpenTK.Graphics.OpenGL4;

namespace ProjectCube.Scripts.RenderDir
{
    internal class Render : RenderConfig
    {
        int vbo, vao;
        Shader shader = new();

        public void Start()
        {
			vbo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

			vao = GL.GenVertexArray();
			GL.BindVertexArray(vao);
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, 4 * sizeof(float), 3 * sizeof(float));

			shader.Create("Shaders/Shader.vert", "Shaders/Shader.frag");

			GL.ClearColor(0.7f, 0.7f, 0.7f, 1.0f);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GL.Enable(EnableCap.CullFace);
		}

		public void Update(Window window)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.BlendEquation(BlendEquationMode.FuncAdd);

			shader.Use();
			shader.Uniform("view", window.world.player.view);
			//shader.Uniform("view", Matrix4.Identity);
			GL.BindVertexArray(vao);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

			float[] data = { 0.505f,  0.505f,  0.505f, 0,
						 0.505f,  0.505f, -0.505f, 0,
						 0.505f,  0.505f, -0.505f, 0,
						 0.505f, -0.505f, -0.505f, 0,
						 0.505f, -0.505f, -0.505f, 0,
						 0.505f, -0.505f,  0.505f, 0,
						 0.505f, -0.505f,  0.505f, 0,
						 0.505f,  0.505f,  0.505f, 0,

						-0.505f,  0.505f,  0.505f, 0,
						-0.505f,  0.505f, -0.505f, 0,
						-0.505f,  0.505f, -0.505f, 0,
						-0.505f, -0.505f, -0.505f, 0,
						-0.505f, -0.505f, -0.505f, 0,
						-0.505f, -0.505f,  0.505f, 0,
						-0.505f, -0.505f,  0.505f, 0,
						-0.505f,  0.505f,  0.505f, 0,

						 0.505f,  0.505f,  0.505f, 0,
						-0.505f,  0.505f,  0.505f, 0,
						 0.505f,  0.505f, -0.505f, 0,
						-0.505f,  0.505f, -0.505f, 0,
						 0.505f, -0.505f,  0.505f, 0,
						-0.505f, -0.505f,  0.505f, 0,
						 0.505f, -0.505f, -0.505f, 0,
						-0.505f, -0.505f, -0.505f, 0,};

			GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StreamDraw);

			GL.DrawArrays(PrimitiveType.Lines, 0, data.Length / 4);
		}

		public void Stop()
		{
			shader.Dispose();
		}
    }
}
