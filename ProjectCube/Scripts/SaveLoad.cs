using OpenTK.Mathematics;
using ProjectCube.Scripts.WorldDir.PlayerDir;
using ProjectCube.Scripts.WorldDir.TerrainDir;

namespace ProjectCube.Scripts
{
	internal class SaveLoad(Window window) : Configs(window)
	{
		// создать папку сохранения
		public static void CreateSave()
		{
			if (!save) return;

			string path = $"Worlds\\{saveDir}\\";
			if (!Directory.Exists(path + "Terrain\\")) Directory.CreateDirectory(path + "Terrain\\");

			FileStream file = File.OpenWrite(path + "World.save");
			file.WriteByte((byte)arraySize);
			file.Close();
		}

		// сохранить LineArray
		public static void SaveLineArray(LineArray lineArray)
		{
			if (!save) return;

			string path = $"Worlds\\{saveDir}\\";
			if (!Directory.Exists(path + "Terrain\\")) Directory.CreateDirectory(path + "Terrain\\");
			path += $"Terrain\\LineArray {lineArray.arrPos.X} {lineArray.arrPos.Y}.save";
			FileStream file = File.OpenWrite(path);
			for (int x = 0; x < arraySize; x++)
			{
				for (int z = 0; z < arraySize; z++)
				{
					//кол-во слоёв в линии
					file.WriteByte((byte)lineArray.lines[x][z].materials.Count);
					for(int i = 0; i < lineArray.lines[x][z].materials.Count; i++)
                    {
						if(lineArray.lines[x][z].materials.TryGetValue(i, out Material material))
						{
							//инфа о слое
							file.Write(BitConverter.GetBytes((int)material.type));
							file.Write(BitConverter.GetBytes(material.y));
						}
					}
				}
			}
			file.Close();
		}
		
		// сохранить данные о игроке
		public static void SavePlayer(Player player)
		{
			if (!save) return;

			string path = $"Worlds\\{saveDir}\\Player.save";
			FileStream file = File.OpenWrite(path);
			file.Write(BitConverter.GetBytes(player.position.X));
			file.Write(BitConverter.GetBytes(player.position.Y));
			file.Write(BitConverter.GetBytes(player.position.Z));
			file.Write(BitConverter.GetBytes(player.angleRotation.X));
			file.Write(BitConverter.GetBytes(player.angleRotation.Y));
			file.Close();
		}



		// попытаться загрузить данные о игроке
		public static bool TryLoadSave(Player player)
		{
			if (!save) return true;

			string path = $"Worlds\\{saveDir}\\World.save";
			if (!File.Exists(path)) return false;
			path = $"Worlds\\{saveDir}\\Player.save";
			if (!File.Exists(path)) return false;

			path = $"Worlds\\{saveDir}\\World.save";
			FileStream file = File.OpenRead(path);
			arraySize = file.ReadByte();
			file.Close();

			path = $"Worlds\\{saveDir}\\Player.save";
			file = File.OpenRead(path);
			byte[] bytes = new byte[4];
			file.Read(bytes);
			player.position.X = BitConverter.ToSingle(bytes);
			file.Read(bytes);
			player.position.Y = BitConverter.ToSingle(bytes);
			file.Read(bytes);
			player.position.Z = BitConverter.ToSingle(bytes);
			file.Read(bytes);
			player.angleRotation.X = BitConverter.ToSingle(bytes);
			file.Read(bytes);
			player.angleRotation.Y = BitConverter.ToSingle(bytes);
			file.Close();

			return true;
		}
		// попытаться загрузить LineArray
		public static LineArray TryLoadLineArray(Vector2 arrPos, int bufferId, Window window)
		{
			if (!save) return null;

			LineArray lineArray = new(arrPos, bufferId, window);
			byte[] bytes = new byte[4];

			string path = $"Worlds\\{saveDir}\\Terrain\\LineArray {arrPos.X} {arrPos.Y}.save";
			if (!File.Exists(path)) return null;

			FileStream file = File.OpenRead(path);
			for (int x = 0; x < arraySize; x++)
			{
				for (int z = 0; z < arraySize; z++)
				{
					//кол-во слоёв в линии
					int n = file.ReadByte();
					lineArray.lines[x][z] = new();

					for (int i = 0; i < n; i++)
					{
						//инфа о слое
						file.Read(bytes);
						WorldDir.TerrainDir.Type type = (WorldDir.TerrainDir.Type)BitConverter.ToInt32(bytes);
						file.Read(bytes);
						float y = BitConverter.ToSingle(bytes);

						lineArray.lines[x][z].materials.Add(i, new(type, y));
					}
				}
			}
			file.Close();

			return lineArray;
		}
	}
}
