using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;
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
		float[] data_curs;

		// шейдеры
		public Shader? shader;
        public Shader? shader_line;

        // мир
        public World world = new();

		// дистанция рендера
		//const int renderDistance = 8;
		public static int renderDistance;

		// всякое
		int count;
		int count_alpha;
		public bool reRender;

		// замки
		private readonly object locker = new();
		private readonly object locker2 = new();
        public bool pLocker;
        public bool pWait;

		// буферы
		int vbo;
        int vao;
		int vbo_alpha;
		int vao_alpha;
		int vbo_line;
		int vao_line;

		// текстуры
		Texture texture;
		Texture texture2;
		Texture texture_leaves;
		Texture texture_log;

        public System.Timers.Timer? timer;
        public System.Timers.Timer? timer2;
		public Stopwatch stopwatch = new();

		float time;

		public void Load(int width, int height)
		{
			// подгрузка настроек
			renderDistance = Configs.renderDist;

			// текстуры
			texture = new("Textures\\texture.png", TextureUnit.Texture0);
			texture2 = new("Textures\\texture2.png", TextureUnit.Texture1);
			texture_leaves = new("Textures\\texture_leaves.png", TextureUnit.Texture2);
			texture_log = new("Textures\\texture_log.png", TextureUnit.Texture3);

			// загрузка драйверов
			DriverGL.GLLoad();
			(vbo, vao, shader) = DriverGL.Load();
			(vbo_alpha, vao_alpha, _) = DriverGL.Load("");
			(vbo_line, vao_line, shader_line) = DriverGL.Load(frag: "Shaders/shader_line.frag");

			// стартовые манипуляции с миром
			world.Start(width, height);

			// таймер
			timer = new(50);
			timer.Elapsed += Update50t;
			timer.AutoReset = true;
			timer.Start();
			
			timer = new(500);
			timer.Elapsed += Update500t;
			timer.AutoReset = true;
			timer.Start();
		}

		void Update50t(object source, ElapsedEventArgs e) //переместить в World
		{
			try
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		void Update500t(object source, ElapsedEventArgs e)
		{
			try
			{
				lock (locker2)
				{
					stopwatch.Start();
					world.UpdateBlock(this);
					stopwatch.Stop();
					//Console.WriteLine(stopwatch.ElapsedMilliseconds);
					stopwatch.Reset();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			time++;
			if (time > 5) time = 0; 
		}

		public void RenderFrame(GameWindow gameWindow, float deltaTime, int width, int height)
        {
			//чистка буфера
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			// позиция камеры (игрока)
			Matrix4 view = world.player.Update(gameWindow, deltaTime, this);

			//=============================================================================================

			// подготовка
			GL.BlendEquation(BlendEquationMode.FuncAdd);
			shader?.Use();
			shader?.Uniform("view", view);
			shader?.Uniform("time", time);

			GL.BindVertexArray(vao);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

			// рисование элементов
			GL.BindVertexArray(vao);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.DrawArrays(PrimitiveType.Triangles, 0, count / 4);

			GL.BindVertexArray(vao_alpha);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_alpha);
			GL.DrawArrays(PrimitiveType.Triangles, 0, count_alpha / 4);

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
					count_alpha = 0;
					foreach (KeyValuePair<Vector3, BlocksArray> i in world.arraysPos)
					{
						count += i.Value.vert.Count;
						count_alpha += i.Value.vert_alpha.Count;
					}

					// создание буфера
					int offset = 0;
					int offset_alpha = 0;
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
					//Console.Write(1);
					GL.BufferData(BufferTarget.ArrayBuffer, (count + offset) * sizeof(float), (nint)null, BufferUsageHint.StreamDraw);
					//Console.Write(2);
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_alpha);
					//Console.Write(3);
					GL.BufferData(BufferTarget.ArrayBuffer, (count_alpha + offset_alpha) * sizeof(float), (nint)null, BufferUsageHint.StreamDraw);
					//Console.WriteLine(4);

					foreach (KeyValuePair<Vector3, BlocksArray> i in world.arraysPos)
					{
						// заливка в буфер
						GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
						GL.BufferSubData(BufferTarget.ArrayBuffer, offset * sizeof(float), i.Value.vert.Count * sizeof(float), i.Value.vert.ToArray()); // иногда зависает

						GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_alpha);
						GL.BufferSubData(BufferTarget.ArrayBuffer, offset_alpha * sizeof(float), i.Value.vert_alpha.Count * sizeof(float), i.Value.vert_alpha.ToArray()); // иногда зависает

						offset += i.Value.vert.Count;
						offset_alpha += i.Value.vert_alpha.Count;
					}

					pLocker = false;
					pWait = false;
					reRender = false;
                }
			}

			//=============================================================================================

			GL.BlendEquation(BlendEquationMode.FuncSubtract);

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
				AddPosArray(ref tmp, data, world.player.selectBlock, 0, 0);
				GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), tmp.ToArray(), BufferUsageHint.StreamDraw);

				// рисование элементов
				GL.DrawArrays(PrimitiveType.Lines, 0, data.Length / 4);
			}

			//=============================================================================================

			// рисование курсора
			// подготовка
			shader_line?.Use();
			shader_line?.Uniform("view", Matrix4.Identity);
			GL.BindVertexArray(vao_line);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_line);

			// загрузка в буфер
			float px = 2f / width;
			float py = 2f / height;
			data_curs = [-px, -py, 0, 0,
						  px,  py, 0, 0,
						 -px,  py, 0, 0,

						  px,  py, 0, 0,
						 -px, -py, 0, 0,
						  px, -py, 0, 0,];
			GL.BufferData(BufferTarget.ArrayBuffer, data_curs.Length * sizeof(float), data_curs, BufferUsageHint.StreamDraw);

			// рисование элементов
			GL.DrawArrays(PrimitiveType.Triangles, 0, data_curs.Length / 4);

			//=============================================================================================
		}

		//======================================== доп методы ========================================

		public void GenVert(ref BlocksArray blocksArray, Vector3i arrPos)
		{
			blocksArray.vert = [];
			blocksArray.vert_alpha = [];
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
								if (!block.alphaType)
								{
									AddPosArray(ref blocksArray.vert, block.cubeVertices[a], pos, block.Type, 0);
								}
								else
								{
									AddPosArray(ref blocksArray.vert_alpha, block.cubeVertices[a], pos, block.Type, 0);
								}
							}
						}
					}
				}
			}
		}

		void AddPosArray(ref List<float> vert, float[] arr, Vector3 pos, Type type, int inf)
        {
            for (int i = 0; i < arr.Length / 4; i++)
            {
                vert.Add(arr[i * 4 + 0] + pos.X);
                vert.Add(arr[i * 4 + 1] + pos.Y);
                vert.Add(arr[i * 4 + 2] + pos.Z);
                vert.Add(arr[i * 4 + 3] + (int)type * 256 + inf * 65536);
            }
        }
    }
}
