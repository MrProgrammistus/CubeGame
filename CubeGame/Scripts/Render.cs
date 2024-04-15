using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System.Timers;

namespace CubeGame.Scripts
{
	internal class Render
    {
		//точки куба
		float[] data = { 0.505f,  0.505f,  0.505f, 0,
						 0.505f,  0.505f, -0.505f, 0,
						 0.505f,  0.505f, -0.505f, 0,
						 0.505f, -0.505f, -0.505f, 0,
						 0.505f, -0.505f, -0.505f, 0,
						 0.505f, -0.505f,  0.505f, 0,
						 0.505f, -0.505f,  0.505f, 0,
						 0.505f,  0.505f,  0.505f, 0,

						-0.505f,  0.505f,  0.505f, 0,
						-0.505f,  0.505f, -0.505f, 0,
						-0.505f,  0.505f, -0.505f, 0,
						-0.505f, -0.505f, -0.505f, 0,
						-0.505f, -0.505f, -0.505f, 0,
						-0.505f, -0.505f,  0.505f, 0,
						-0.505f, -0.505f,  0.505f, 0,
						-0.505f,  0.505f,  0.505f, 0,

						 0.505f,  0.505f,  0.505f, 0,
						-0.505f,  0.505f,  0.505f, 0,
						 0.505f,  0.505f, -0.505f, 0,
						-0.505f,  0.505f, -0.505f, 0,
						 0.505f, -0.505f,  0.505f, 0,
						-0.505f, -0.505f,  0.505f, 0,
						 0.505f, -0.505f, -0.505f, 0,
						-0.505f, -0.505f, -0.505f, 0,};

		// шейдеры
		public Shader? shader;
        public Shader? shader_line;

        // мир
        public World world = new();

        // дистанция рендера
        const int renderDistance = 5;

		// всякое
		int count;
		public bool reRender;

		// замки
		private readonly object locker = new();
        public bool pLocker;
        public bool pWait;

		// буферы
		int vbo;
        int vao;
		int vbo_line;
		int vao_line;

		// текстуры
		Texture texture;

        System.Timers.Timer? timer;

        public void Load(int width, int height)
        {
			//текстуры
			texture = new("Textures\\texture.png", TextureUnit.Texture0);

			// загрузка драйверов
            (vbo, vao, shader) = DriverGL.Load();
            (vbo_line, vao_line, shader_line) = DriverGL.Load(frag: "Shaders/shader_line.frag");

			// стартовые манипуляции с миром
			world.Start(width, height);

			// таймер
			timer = new(50);
			timer.Elapsed += Update10t;
			timer.AutoReset = true;
            timer.Start();
		}

		void Update10t(object source, ElapsedEventArgs e)
		{
            lock (locker)
            {
				// обновление мира
                if (world.Update(world.player.position, renderDistance, this))
                {
                    reRender = true;
                }
			}
		}

		public void RenderFrame(GameWindow gameWindow, float deltaTime)
        {
			// позиция камеры (игрока)
			Matrix4 view = world.player.Update(gameWindow, deltaTime, this);

			// подготовка
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            shader?.Use();
			shader?.Uniform("view", view);

			GL.BindVertexArray(vao);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

			// обновление текстур (сцена)
			if (reRender)
            {
                // замок на переменной pLocker и pWait 
                if (pLocker)
                {
                    pWait = true;
                }
                else
                {
					pLocker = true;

					// подсчёт вершин
					count = 0;
					foreach (KeyValuePair<Vector3, BlocksArray> i in world.arraysPos) count += i.Value.vert.Count;
					// создание буфера
					int offset = 0;

					GL.BufferData(BufferTarget.ArrayBuffer, (count + offset) * sizeof(float), new float[count + offset], BufferUsageHint.StreamDraw);

					foreach (KeyValuePair<Vector3, BlocksArray> i in world.arraysPos)
					{
						// заливка в буфер
						GL.BufferSubData(BufferTarget.ArrayBuffer, offset * sizeof(float), i.Value.vert.Count * sizeof(float), i.Value.vert.ToArray()); // иногда зависает
						offset += i.Value.vert.Count;
					}

					pLocker = false;
					pWait = false;
					reRender = false;
                }
			}

            // рисование элементов
			GL.DrawArrays(PrimitiveType.Triangles, 0, count / 4);

			// рисование выделенного блока
			if (world.player.isSelectBlock)
			{
				// подготовка
				shader_line?.Use();
				shader_line?.Uniform("view", view);
				GL.BindVertexArray(vao_line);
				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_line);

				// загрузка в буфер
				List<float> tmp = [];
				AddPosArray(ref tmp, data, world.player.selectBlock, 0);
				GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), tmp.ToArray(), BufferUsageHint.StreamDraw);

				// рисование
				GL.DrawArrays(PrimitiveType.Lines, 0, data.Length / 4);
			}
		}

		//======================================== доп методы ========================================

		public void GenVert(ref BlocksArray blocksArray, Vector3i arrPos)
		{
			blocksArray.vert = [];
			Block block;

			// перебор блоков в массиве
			for (int i = 0; i < blocksArray.size; i++)
			{
				for (int j = 0; j < blocksArray.size; j++)
				{
					for (int k = 0; k < blocksArray.size; k++)
					{
						// определение вершин
						block = blocksArray.GetBlock((i, j, k), arrPos);

						// запись в массив вершин если блок не воздух
						if (block.Type != Type.air)
						{
							block.Refresh(blocksArray, (i, j, k), arrPos);

							Vector3i pos = (i, j, k) + arrPos * blocksArray.size;
							for (int a = 0; a < block.cubeVertices.Count; a++)
							{
								// добавляем вершины в общий массив
								AddPosArray(ref blocksArray.vert, block.cubeVertices[a], pos, block.Type);
							}
						}
					}
				}
			}
		}

		void AddPosArray(ref List<float> vert, float[] arr, Vector3 pos, Type type)
        {
            for (int i = 0; i < arr.Length / 4; i++)
            {
                vert.Add(arr[i * 4 + 0] + pos.X);
                vert.Add(arr[i * 4 + 1] + pos.Y);
                vert.Add(arr[i * 4 + 2] + pos.Z);
                vert.Add(arr[i * 4 + 3] + (int)type * 256);
            }
        }
    }
}
