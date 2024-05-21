using OpenTK.Mathematics;
using ProjectCube.Scripts.WorldDir.PlayerDir;

namespace ProjectCube.Scripts.WorldDir.TerrainDir
{
	internal class Terrain(Window window) : Configs(window)
	{
		public Generation generation = new(window);

		public Dictionary<Vector2, LineArray> lineArrays = new();

		List<int> freeBufferId = [];

		public int arraysCount, pointsCount;

		public List<Tuple<int, float[]>> verts = [];
		public List<Tuple<int, float[]>> verts_l = [];

		public void Start()
		{
			if (!SaveLoad.TryLoadSave(window.scene.Find("Player").GetElement<Player>()))
			{
				SaveLoad.CreateSave();
			}

			arraysCount = loadDistance * loadDistance * 4;
			pointsCount = arraySize * arraySize;

			int i = 0;
			for (int x = -loadDistance; x < loadDistance; x++)
			{
				for (int z = -loadDistance; z < loadDistance; z++)
				{
					if (MathF.Sqrt(x * x + z * z) < loadDistance)
					{
						freeBufferId.Add(i);
						i++;
					}
				}
			}
		}

		public void GenUpdate()
		{
			if (!window.render.pLocker && !window.render.pWait && !window.render.reRender)
			{
				window.render.pLocker = true;

				Vector2 playerPos = window.scene.Find("Player").GetElement<Player>().position.Xz;
				Vector2 offset = (playerPos.X > 0 ? 0 : -1, playerPos.Y > 0 ? 0 : -1);
				playerPos = (Vector2i)(window.scene.Find("Player").GetElement<Player>().position.Xz / arraySize + offset);

				bool reRender = false;
				// очистка флагов
				foreach (KeyValuePair<Vector2, LineArray> i in lineArrays)
				{
					i.Value.flags = 0;
				}
				// заполнение флагов
				for (int x = -loadDistance; x < loadDistance; x++)
				{
					for (int z = -loadDistance; z < loadDistance; z++)
					{
						if (MathF.Sqrt(x * x + z * z) < loadDistance)
						{
							if (lineArrays.TryGetValue((x, z) + playerPos, out LineArray lineArray)) lineArray.flags += 1;
						}
					}
				}
				// удаление далёких массивов
				foreach (KeyValuePair<Vector2, LineArray> i in lineArrays)
				{
					if ((i.Value.flags & 1) != 1)
					{
						SaveLoad.SaveLineArray(i.Value);
						freeBufferId.Add(i.Value.bufferId);
						lineArrays.Remove(i.Key);
						reRender = true;
					}
				}
				// рендер территории
				int n = 0;
				for (int i = -loadDistance; i < loadDistance + 1; i++)
				{
					for (int x = -i; x < i; x++)
					{
						for (int z = -i; z < i; z++)
						{
							if (MathF.Sqrt(x * x + z * z) < i)
							{
								LineArray lineArray;
								if (!lineArrays.ContainsKey((x, z) + playerPos))
								{
									if ((lineArray = SaveLoad.TryLoadLineArray((x, z) + playerPos, freeBufferId[0], window)) == null)
									{
										lineArray = generation.GenerateArray((x, z) + playerPos, freeBufferId[0], window);
									}
									freeBufferId.RemoveAt(0);
									lineArrays.Add((x, z) + playerPos, lineArray);
									reRender = true;
									n++;
									if (n > arraysSize) goto a;
								}
							}
						}
					}
					if (n > 1) goto a;
				}
			a:
				// перезаливка в буфер, если надо
				if (reRender)
				{
					verts.Clear();
					verts_l.Clear();
					List<LineArray> list = [];
					foreach (KeyValuePair<Vector2, LineArray> i in lineArrays)
					{
						if ((i.Value.flags & 1) != 1 || (i.Value.flags & 2) == 2)
						{
							list.Add(i.Value);
							if (lineArrays.TryGetValue(i.Key - (1, 0), out LineArray lineArray) && !list.Contains(lineArray)) list.Add(lineArray);
							if (lineArrays.TryGetValue(i.Key - (0, 1), out lineArray) && !list.Contains(lineArray)) list.Add(lineArray);
							if (lineArrays.TryGetValue(i.Key - (1, 1), out lineArray) && !list.Contains(lineArray)) list.Add(lineArray);
						}
					}
					foreach (LineArray i in list)
					{
						i.GenVerts();
						verts.Add(new(i.bufferId, i.verts));
						verts_l.Add(new(i.bufferId, i.verts_l));
					}

					window.render.reRender = true;
				}

				window.render.pLocker = false;
			}
		}

        public void Stop()
        {
			Console.Write("Подготовка к сохранению");
		a:
			if (!window.render.pLocker && !window.render.pWait)
			{
				window.render.pLocker = true;

				Console.WriteLine();
				float j = 0;
				foreach (KeyValuePair<Vector2, LineArray> i in lineArrays)
				{
					Console.SetCursorPosition(0, Console.CursorTop);
					Console.Write($"Сохранение мира [{string.Concat(Enumerable.Repeat("#", (int)MathF.Ceiling(j / lineArrays.Count * 64)))}" +
												   $"{string.Concat(Enumerable.Repeat("-", (int)MathF.Floor((lineArrays.Count - j) / lineArrays.Count * 64)))}]");
					j++;
					SaveLoad.SaveLineArray(i.Value);
				}
				SaveLoad.SavePlayer(window.scene.Find("Player").GetElement<Player>());
			}
			else
			{
				Thread.Sleep(1000);
				Console.Write(".");
				goto a;
			}
		}
    }
}
