using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldReader
{
	internal class Read
	{
		public void r()
		{
			Console.ForegroundColor = ConsoleColor.Green;
			string? path = Console.ReadLine();
			if (path == "") path = "file.txt";

			FileStream file = File.OpenRead("C:\\Users\\ikoro\\source\\repos\\CubeGame\\CubeGame\\bin\\Debug\\net8.0\\Worlds\\" + path);

			byte Read()
			{
				int tmp = file.ReadByte();
				string tmp2 = Convert.ToString(tmp, 16).ToUpper();
				Console.Write((tmp2.Length < 2 ? "0" + tmp2 : tmp2) + " ");
				return (byte)tmp;
			}

			//размер файла в байтах и Кб
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write(file.Length + " " + file.Length / 1024 + "." + file.Length % 1024);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine();
			Console.WriteLine();

			//версия файла
			Read();
			Console.WriteLine();

			//размер
			byte[] tmp = new byte[4];
			for (int i = 0; i < 4; i++) tmp[i] = Read();
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(BitConverter.ToSingle(tmp));
			Console.ForegroundColor = ConsoleColor.Green;

			//позиция
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < 4; k++) tmp[k] = Read();
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.Write(BitConverter.ToSingle(tmp) + " ");
				Console.ForegroundColor = ConsoleColor.Green;
			}
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();

			file.Position = 0 * 20 + 17;
			for (int i = 0; i < 10; i++)
			{
				//размер
				for (int k = 0; k < 4; k++) tmp[k] = Read();
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(BitConverter.ToSingle(tmp));
				Console.ForegroundColor = ConsoleColor.Green;

				//позиция
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 4; k++) tmp[k] = Read();
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.Write(BitConverter.ToSingle(tmp) + " ");
					Console.ForegroundColor = ConsoleColor.Green;
				}
				Console.WriteLine();

				//тип
				for (int k = 0; k < 4; k++) tmp[k] = Read();
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(BitConverter.ToInt32(tmp));
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine();
			}

			file.Close();
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}
