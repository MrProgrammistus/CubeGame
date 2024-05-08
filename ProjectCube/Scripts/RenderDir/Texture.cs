using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace ProjectCube.Scripts.RenderDir
{
	internal class Texture
	{
		public int texture;

		public void Create(string path, TextureUnit tu)
		{
			//загрузка текстуры
			texture = GL.GenTexture();
			Use(tu);
			ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

			//настройка текстуры
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
		}

		public void Use(TextureUnit tu)
		{
			//бинд текстуры в нужный текстурный блок
			GL.ActiveTexture(tu);
			GL.BindTexture(TextureTarget.Texture2D, texture);
		}
	}
}
