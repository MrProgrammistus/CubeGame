using OpenTK.Mathematics;
using ProjectCube.Scripts.WorldDir.PlayerDir;
using ProjectCube.Scripts.WorldDir.TerrainDir;

namespace ProjectCube.Scripts
{
	internal class SaveLoad : SaveLoadConfigs
	{
		public static void CreateSave()
		{
			string path = $"Worlds\\{saveDir}\\";
			if (!Directory.Exists(path + "Terrain\\")) Directory.CreateDirectory(path + "Terrain\\");

			FileStream file = File.OpenWrite(path + "World.save");
			file.WriteByte((byte)WorldConfig.arraySize);
			file.Close();
		}
		public static void SaveLineArray(LineArray lineArray)
		{
			string path = $"Worlds\\{saveDir}\\";
			if (!Directory.Exists(path + "Terrain\\")) Directory.CreateDirectory(path + "Terrain\\");
			path += $"Terrain\\LineArray {lineArray.arrPos.X} {lineArray.arrPos.Y}.save";
			FileStream file = File.OpenWrite(path);
			for (int x = 0; x < WorldConfig.arraySize; x++)
			{
				for (int z = 0; z < WorldConfig.arraySize; z++)
				{
					file.Write(BitConverter.GetBytes((int)lineArray.lines[x][z].material.type));
					file.Write(BitConverter.GetBytes(lineArray.lines[x][z].y));
				}
			}
			file.Close();
		}
		public static void SavePlayer(Player player)
		{
			string path = $"Worlds\\{saveDir}\\Player.save";
			FileStream file = File.OpenWrite(path);
			file.Write(BitConverter.GetBytes(player.position.X));
			file.Write(BitConverter.GetBytes(player.position.Y));
			file.Write(BitConverter.GetBytes(player.position.Z));
			file.Close();
		}

		public static bool TryLoadSave(Player player)
		{
			string path = $"Worlds\\{saveDir}\\World.save";
			if (!File.Exists(path)) return false;

			FileStream file = File.OpenRead(path);
			WorldConfig.arraySize = file.ReadByte();
			file.Close();

			path = $"Worlds\\{saveDir}\\Player.save";
			file = File.OpenRead(path);
			byte[] bytes = new byte[4];
			file.Read(bytes);
			player.position.X = BitConverter.ToInt32(bytes);
			file.Read(bytes);
			player.position.Y = BitConverter.ToInt32(bytes);
			file.Read(bytes);
			player.position.Z = BitConverter.ToInt32(bytes);
			file.Close();

			return true;
		}
		public static LineArray TryLoadArray(Vector2 arrPos, Window window)
		{
			LineArray lineArray = new(arrPos, window);
			byte[] bytes = new byte[4];

			string path = $"Worlds\\{saveDir}\\Terrain\\LineArray {arrPos.X} {arrPos.Y}.save";
			if (!File.Exists(path)) return null;

			FileStream file = File.OpenRead(path);
			for (int x = 0; x < WorldConfig.arraySize; x++)
			{
				for (int z = 0; z < WorldConfig.arraySize; z++)
				{
					file.Read(bytes);
					WorldDir.TerrainDir.Type type = (WorldDir.TerrainDir.Type)BitConverter.ToInt32(bytes);
					file.Read(bytes);
					float y = BitConverter.ToSingle(bytes);

					lineArray.lines[x][z] = new(new(type), y);
				}
			}
			file.Close();

			return lineArray;
		}
	}
}
