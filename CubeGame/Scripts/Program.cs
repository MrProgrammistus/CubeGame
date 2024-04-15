// список проблем:
// 1. баги с рендерингом 1 (иногда провисает при выполнении функции reRender)
// 2. баги с рендерингом 2 (неправильная прогрузка текстур на стыках массивов)
// 3. баги с коллизиями (цепляет блоки или падает в них (зависит от того что раньше поставить X или Y))
//

using StbImageSharp;

namespace CubeGame.Scripts
{
    internal class Program
    {
        static Window window = new(800, 600, 60);
        static void Main()
        {
            //настройки
            StbImage.stbi_set_flip_vertically_on_load(1);
			if (!Directory.Exists("Worlds")) Directory.CreateDirectory("Worlds");

			//отправная точка страданий
			window.Run();
        }
    }
}
