using OpenTK.Graphics.OpenGL4;

namespace CubeGame.Scripts
{
	internal class DriverGL
	{
		public static (int, int, Shader) Load(string vert = "Shaders/shader.vert", string frag = "Shaders/shader.frag")
		{
			// настройки шейдеров
			Shader shader = new(vert, frag);

			// настройки буферов
			int vbo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

			int vao = GL.GenVertexArray();
			GL.BindVertexArray(vao);
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, 4 * sizeof(float), 3 * sizeof(float));

			// настройки GL
			GL.ClearColor(0.7f, 0.7f, 0.7f, 1);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GL.Enable(EnableCap.CullFace);

			return (vbo, vao, shader);
		}
	}
}
