// список проблем:
// 1. баги с коллизиями (цепляет блоки или падает в них (зависит от того что раньше поставить X или Y))
//

using StbImageSharp;
using System.Text;

namespace CubeGame.Scripts
{
    internal class Program
    {
        static Window window = new(800, 600, 120);
        static void Main()
        {
            //настройки
            StbImage.stbi_set_flip_vertically_on_load(1);
			if (!Directory.Exists("Worlds")) Directory.CreateDirectory("Worlds");
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			//загрузка настроек
			Configs.Read();

			//отправная точка страданий
			window.Run();
        }
    }
}
