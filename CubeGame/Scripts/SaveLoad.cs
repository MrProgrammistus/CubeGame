using OpenTK.Mathematics;

/// структура файлов:
/// читать файлы миров в HEX формате
///
/// файл с данными мира:
/// начало;
/// 1  байт  - версия (byte)
/// 64 байта - название мира (string)
/// 1 байт   - размер массива (byte)
/// конец;
/// 
/// файл с массивами:
/// начало;
/// 1  байт  - версия (byte)
/// X  байт  - блоки:
///		4  байта - тип блока (Type)
/// конец;
namespace CubeGame.Scripts
{
    internal class SaveLoad
    {
        const byte version = 0; // <== ВАЖНО - версия загрузки, увеличивать при изменении загрузчика с момента последнего релиза

        public static void Save(string path, BlocksArray blocksArray, Vector3i pos)
        {
            //открыть файл
            FileStream file;
            if(!Directory.Exists($"Worlds\\{Configs.saveFile}")) Directory.CreateDirectory($"Worlds\\{Configs.saveFile}");

            file = File.OpenWrite($"Worlds\\{Configs.saveFile}\\{path}");

            //записать версию сохранения
            file.WriteByte(version);

            for (int i = 0; i < blocksArray.size; i++)
            {
                for (int j = 0; j < blocksArray.size; j++)
                {
                    for (int k = 0; k < blocksArray.size; k++)
                    {
                        Block block = blocksArray.GetBlock((i, j, k), pos);

                        //записать тип
                        file.Write(BitConverter.GetBytes((int)block.Type));
                    }
                }
            }

            //закрыть файл
            file.Close();
		}
    
        public static bool Load(string path, Vector3i pos, ref Dictionary<Vector3, BlocksArray> arraysPos, World world)
		{
            if (!File.Exists($"Worlds\\{Configs.saveFile}\\{path}")) return false;

			FileStream file = File.OpenRead($"Worlds\\{Configs.saveFile}\\{path}");

			int version = file.ReadByte();
			switch (version)
            {
                case 0:
                    Version0(file, pos, ref arraysPos, world);
                    break;
            }

			file.Close();

            return true;
        }

        public static void Version0(FileStream file, Vector3i pos, ref Dictionary<Vector3, BlocksArray> arraysPos, World world)
        {
            byte[] tmp = new byte[4];

            BlocksArray tmpArr = new(World.arraySize, 1, world);

			arraysPos.Add(pos, tmpArr);

            for (int i = 0; i < World.arraySize; i++)
            {
                for (int j = 0; j < World.arraySize; j++)
                {
                    for (int k = 0; k < World.arraySize; k++)
                    {
                        //тип
						file.Read(tmp);
						Type type = (Type)BitConverter.ToInt32(tmp);

                        tmpArr.blocks[i][j][k] = new(type);
					}
                }
            }
		}
    }
}
