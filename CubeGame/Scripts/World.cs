using OpenTK.Mathematics;

namespace CubeGame.Scripts
{
	internal class World
	{
		// размер массива
		public const byte arraySize = 16;
		public const byte ySize = 4;

		// список загруженных массивов
		public Dictionary<Vector3, BlocksArray> arraysPos = [];

		// игрок
		public Player? player;

		public void Start(int width, int height)
		{
			// игрок
			player = new(width, height);
		}

		public bool Update(Vector3 playerPos, int renderDistance, Render render)
		{
			// генерация массивов
			// замок на переменной render.pLocker
			int ret = 0;
			if (render.pLocker || render.pWait)
			{
				Thread.Sleep(100);
				//Console.Write(".");
			}
			else
			{
				// начало замка
				render.pLocker = true;

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
						//Console.WriteLine($"удалён массив на координатах {i.Key.X} {i.Key.Y} {i.Key.Z} (дистанция {d})");
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

				render.pLocker = false;
				// конец
			}

			if (ret > 0) return true;
			else return false;
		}

		public void Stop(Render render)
		{
		_pLocker:
			Thread.Sleep(100);
			if(render.pLocker) goto _pLocker;
			// сохранение всех массивов перед закрытием игры
			foreach (KeyValuePair<Vector3, BlocksArray> i in arraysPos)
			{
				SaveLoad.Save($"blocksArray_{i.Key.X}_{i.Key.Y}_{i.Key.Z}.save", i.Value, (Vector3i)i.Key);
			}
		}

		public bool GetArray(Vector3i pos)
		{
			bool generate = !arraysPos.TryGetValue(pos, out _);
			if (generate)
			{
				{
					// загрузка
					if(SaveLoad.Load($"blocksArray_{pos.X}_{pos.Y}_{pos.Z}.save", pos, ref arraysPos, this));
					else
					{
						// генерация
						Generation.GenerateArray(ref arraysPos, pos, this);
					}
				}

				return true;
			}
			return false;
		}
	}
}
