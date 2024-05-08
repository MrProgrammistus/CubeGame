using ProjectCube.Scripts;
using StbImageSharp;
using System.Text;

class Program
{
	static Window window = new();

	static void Main()
	{
		StbImage.stbi_set_flip_vertically_on_load(1);
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

		Configs.Load();

		window.Run();
	}
}