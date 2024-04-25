using CubeGame.Scripts.Blocks;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;

namespace CubeGame.Scripts
{
	internal class World
	{
		// размер массива
		public const byte arraySize = 16;
		//public const byte ySize = 4;
		public static int ySize;

		// список загруженных массивов
		public Dictionary<Vector3, BlocksArray> arraysPos = [];

		// игрок
		public Player? player;

		public bool pW2;

		public void Start(int width, int height)
		{
			// подгрузка настроек
			ySize = Configs.genY;

			// игрок
			player = new(width, height);
		}

		public void UpdateBlock(Render render)
		{
		pLocker:
			if (render.pLocker || render.pWait)
			{
				Thread.Sleep(10);
				//Console.Write(".");
				pW2 = true;
				goto pLocker;
			}
			else
			{
				// начало замка
				render.pLocker = true;

				foreach (KeyValuePair<Vector3, BlocksArray> i in arraysPos)
				{
					Vector3 pos = i.Key;
					for (int x = 0; x < arraySize; x++)
					{
						for (int y = 0; y < arraySize; y++)
						{
							for (int z = 0; z < arraySize; z++)
							{
								i.Value.GetBlockFast((x, y, z)).Script?.Update((x, y, z), pos, i.Value, render);
							}
						}
					}
				}
				render.pLocker = false;
				pW2 = false;
				// конец
			}
		}

		public bool Update(Vector3 playerPos, int renderDistance, Render render)
		{
			// генерация массивов
			// замок на переменной render.pLocker
			int ret = 0;
			if (render.pLocker || render.pWait || pW2)
			{
				Thread.Sleep(100);
				//Console.Write(".");
			}
			else
			{
				// начало замка
				render.pLocker = true;

				// обновление блоков

				// перебор, для удаления дальних массивов
				foreach (KeyValuePair<Vector3, BlocksArray> i in arraysPos)
				{
					float d = Vector2.Distance(new Vector2(i.Key.X + 0.5f, i.Key.Z + 0.5f) * arraySize, render.world.player.position.Xz);
					if (d > (renderDistance + 1.5f) * arraySize)
					{
						// сохранение перед удалением
						SaveLoad.Save($"blocksArray_{i.Key.X}_{i.Key.Y}_{i.Key.Z}.save", i.Value, (Vector3i)i.Key);

						// удаление
						arraysPos.Remove(i.Key);
					}
				}

				// добавление массива
				List<Vector3i> poses = [];
				ret = 0;
				for (int d = 0; d < renderDistance + 1; d++)
				{
					for (int i = -d; i < d; i++)
					{
						for (int j = -ySize; j < ySize; j++)
						{
							for (int k = -d; k < d; k++)
							{
								if ((i + 0.5f) * (i + 0.5f) + (k + 0.5f) * (k + 0.5f) < renderDistance * renderDistance)
								{
									Vector3i pos = (Vector3i)playerPos / arraySize + (i, j, k);
									pos.Y = j;
									if (GetArray(pos))
									{
										ret++;
										if (!poses.Contains(pos)) poses.Add(pos);

										if (ret > 10) goto _out;
									}
								}
							}
						}
					}
					if (ret > 0) goto _out;
				}
			_out:
				// генерация вершин
				foreach (Vector3i pos in poses)
				{
					if (arraysPos.TryGetValue(pos, out BlocksArray? tmp))
						render.GenVert(ref tmp, pos);
				}

				Block.SetBlocks(render);

				render.pLocker = false;
				// конец
			}

			if (ret > 0) return true;
			else return false;
		}

		public void Stop(Render render)
		{
			render.timer.Stop();
		_pLocker:
			Thread.Sleep(100);
			if(render.pLocker) goto _pLocker;
			render.pLocker = true;

			// сохранение всех массивов перед закрытием игры
			float j = 0;
			foreach (KeyValuePair<Vector3, BlocksArray> i in arraysPos)
			{
				Console.SetCursorPosition(0, Console.CursorTop);
				Console.Write($"Сохранение мира [{string.Concat(Enumerable.Repeat("#", (int)MathF.Ceiling(j / arraysPos.Count * 64)))}" +
											   $"{string.Concat(Enumerable.Repeat("-", (int)MathF.Floor((arraysPos.Count - j) / arraysPos.Count * 64)))}]");

				j++;
				SaveLoad.Save($"blocksArray_{i.Key.X}_{i.Key.Y}_{i.Key.Z}.save", i.Value, (Vector3i)i.Key);
			}
		}

		public bool GetArray(Vector3i pos)
		{
			bool generate = !arraysPos.TryGetValue(pos, out _);
			if (generate)
			{
				// загрузка
				if (SaveLoad.Load($"blocksArray_{pos.X}_{pos.Y}_{pos.Z}.save", pos, ref arraysPos, this)) ;
				else
				{
					// генерация
					Generation.GenerateArray(ref arraysPos, pos, this);
				}
				return true;
			}
			return false;
		}
	}
}
