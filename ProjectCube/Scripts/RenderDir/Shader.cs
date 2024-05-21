using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace ProjectCube.Scripts.RenderDir
{
	public class Shader
	{
		public int shaderProgram;

		public void Create(string vertexPath, string fragmentPath)
		{
			int vertexShader = CreateShader(vertexPath, ShaderType.VertexShader);
			int fragmentShader = CreateShader(fragmentPath, ShaderType.FragmentShader);

			shaderProgram = GL.CreateProgram();
			GL.AttachShader(shaderProgram, vertexShader);
			GL.AttachShader(shaderProgram, fragmentShader);
			GL.LinkProgram(shaderProgram);
			GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out int log);
			if (log == 0) Console.WriteLine(GL.GetProgramInfoLog(shaderProgram));

			GL.DetachShader(shaderProgram, vertexShader);
			GL.DetachShader(shaderProgram, fragmentShader);
			GL.DeleteShader(vertexShader);
			GL.DeleteShader(fragmentShader);
		}

		public void Create(string vertexPath, string geometryPath, string fragmentPath)
		{
			int vertexShader = CreateShader(vertexPath, ShaderType.VertexShader);
			int geometryShader = CreateShader(geometryPath, ShaderType.GeometryShader);
			int fragmentShader = CreateShader(fragmentPath, ShaderType.FragmentShader);

			shaderProgram = GL.CreateProgram();
			GL.AttachShader(shaderProgram, vertexShader);
			GL.AttachShader(shaderProgram, geometryShader);
			GL.AttachShader(shaderProgram, fragmentShader);
			GL.LinkProgram(shaderProgram);
			GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out int log);
			if (log == 0) Console.WriteLine(GL.GetProgramInfoLog(shaderProgram));

			GL.DetachShader(shaderProgram, vertexShader);
			GL.DetachShader(shaderProgram, geometryShader);
			GL.DetachShader(shaderProgram, fragmentShader);
			GL.DeleteShader(vertexShader);
			GL.DeleteShader(geometryShader);
			GL.DeleteShader(fragmentShader);
		}

		public int CreateShader(string shaderPath, ShaderType shaderType)
		{
			string shaderSource = File.ReadAllText(shaderPath);
			int shader = GL.CreateShader(shaderType);
			GL.ShaderSource(shader, shaderSource);

			GL.CompileShader(shader);
			GL.GetShader(shader, ShaderParameter.CompileStatus, out int log);
			if (log == 0) Console.WriteLine(GL.GetShaderInfoLog(shader));

			return shader;
		}

		public void Use() => GL.UseProgram(shaderProgram);

		public void Dispose()
		{
			GL.DeleteProgram(shaderProgram);
			GC.SuppressFinalize(this);
		}


		public void Uniform(string name, float data) => GL.Uniform1(GL.GetUniformLocation(shaderProgram, name), data);
		public void Uniform(string name, Vector2 data) => GL.Uniform2(GL.GetUniformLocation(shaderProgram, name), data);
		public void Uniform(string name, Vector3 data) => GL.Uniform3(GL.GetUniformLocation(shaderProgram, name), data);
		public void Uniform(string name, Vector4 data) => GL.Uniform4(GL.GetUniformLocation(shaderProgram, name), data);
		public void Uniform(string name, Matrix4 data) => GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, name), true, ref data);
	}
}
