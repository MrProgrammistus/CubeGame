using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ProjectCube.Scripts.RenderDir
{
	public class Graphics
	{
		public static float[] Plane(Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4, float data)
		{
			return [pos1.X, pos1.Y, pos1.Z, pos2.Y, pos3.Y, pos4.Y, data];
		}

		public static (int, int) CreateBuffer()
		{
			int vbo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			
			int vao = GL.GenVertexArray();
			GL.BindVertexArray(vao);

			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);

			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));

			GL.EnableVertexAttribArray(2);
			GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 7 * sizeof(float), 4 * sizeof(float));

			GL.EnableVertexAttribArray(3);
			GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, 7 * sizeof(float), 5 * sizeof(float));

			GL.EnableVertexAttribArray(4);
			GL.VertexAttribPointer(4, 1, VertexAttribPointerType.Float, false, 7 * sizeof(float), 6 * sizeof(float));

			return (vbo, vao);
		}

		public static (int, int) CreateBufferOld()
		{
			int vbo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

			int vao = GL.GenVertexArray();
			GL.BindVertexArray(vao);
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, 4 * sizeof(float), 3 * sizeof(float));

			return (vbo, vao);
		}
	}
}
