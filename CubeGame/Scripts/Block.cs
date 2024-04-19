using CubeGame.Scripts.Blocks;
using OpenTK.Mathematics;

namespace CubeGame.Scripts
{
	internal class Block
	{
		public Block(Type type)
		{
			Type = type;
		}

		public void SetConf()
		{
			if (Type == Type.air || Type == Type.water) alphaType = true;
			else alphaType = false;

			if (Type == Type.plant) Script = new Plant();
			else Script = null;
		}

		// 0bXX_XX_XX_XX
		//   || || || ||
		//   || || || |1
		//   || || || 2
		//   || || |4
		//   || || 8
		//   || |16
		//   || 32
		//   |64
		//   128

		public static float[] cubeVerticesXp = {
			 0.5f,  0.5f,  0.5f, 0b01_01_00_00,//16
			 0.5f, -0.5f,  0.5f, 0b00_01_00_00,//17
			 0.5f,  0.5f, -0.5f, 0b11_01_00_00,//18
			 0.5f, -0.5f,  0.5f, 0b00_01_00_00,//17
			 0.5f, -0.5f, -0.5f, 0b10_01_00_00,//19
			 0.5f,  0.5f, -0.5f, 0b11_01_00_00,//18
		};
		public static float[] cubeVerticesYp = {
			 0.5f,  0.5f,  0.5f, 0b10_00_01_00,//4
			 0.5f,  0.5f, -0.5f, 0b11_00_01_00,//6
			-0.5f,  0.5f,  0.5f, 0b00_00_01_00,//5
			-0.5f,  0.5f,  0.5f, 0b00_00_01_00,//5
			 0.5f,  0.5f, -0.5f, 0b11_00_01_00,//6
			-0.5f,  0.5f, -0.5f, 0b01_00_01_00,//7
			};
		public static float[] cubeVerticesZp = {
			 0.5f,  0.5f,  0.5f, 0b11_00_00_01,//0
			-0.5f,  0.5f,  0.5f, 0b01_00_00_01,//1
			 0.5f, -0.5f,  0.5f, 0b10_00_00_01,//2
			-0.5f,  0.5f,  0.5f, 0b01_00_00_01,//1
			-0.5f, -0.5f,  0.5f, 0b00_00_00_01,//3
			 0.5f, -0.5f,  0.5f, 0b10_00_00_01,//2
			};
		public static float[] cubeVerticesXm = {
			-0.5f,  0.5f,  0.5f, 0b11_10_00_00,//20
			-0.5f,  0.5f, -0.5f, 0b01_10_00_00,//22
			-0.5f, -0.5f,  0.5f, 0b10_10_00_00,//21
			-0.5f, -0.5f,  0.5f, 0b10_10_00_00,//21
			-0.5f,  0.5f, -0.5f, 0b01_10_00_00,//22
			-0.5f, -0.5f, -0.5f, 0b00_10_00_00,//23
		};
		public static float[] cubeVerticesYm = {
			 0.5f, -0.5f, -0.5f, 0b10_00_10_00,//12
			 0.5f, -0.5f,  0.5f, 0b11_00_10_00,//14
			-0.5f, -0.5f, -0.5f, 0b00_00_10_00,//13
			-0.5f, -0.5f, -0.5f, 0b00_00_10_00,//13
			 0.5f, -0.5f,  0.5f, 0b11_00_10_00,//14
			-0.5f, -0.5f,  0.5f, 0b01_00_10_00,//15
		};
		public static float[] cubeVerticesZm = {
			 0.5f,  0.5f, -0.5f, 0b01_00_00_10,//8
			 0.5f, -0.5f, -0.5f, 0b00_00_00_10,//10
			-0.5f,  0.5f, -0.5f, 0b11_00_00_10,//9
			-0.5f,  0.5f, -0.5f, 0b11_00_00_10,//9
			 0.5f, -0.5f, -0.5f, 0b00_00_00_10,//10
			-0.5f, -0.5f, -0.5f, 0b10_00_00_10,//11
		};

		public Type Type { get { return _type; } set { _type = value; SetConf(); } }
		public Type _type;
	    public List<float[]> cubeVertices = [];

		public bool alphaType;

		public BaseBS? Script;

		public static List<Tuple<Vector3i, Vector3i, BlocksArray, Type, BaseBS>> queue = [];

		public static void SetBlockQueue(Vector3i bp, Vector3i ap, BlocksArray tmp, Type type, BaseBS bs)
		{
			queue.Add(new(bp, ap, tmp, type, bs));
		}

		public static void SetBlocks(Render render)
		{
			foreach (Tuple<Vector3i, Vector3i, BlocksArray, Type, BaseBS> q in queue)
			{
				Vector3i bp = q.Item1;
				Vector3i ap = q.Item2;
				BlocksArray tmp = q.Item3;
				Type type = q.Item4;
				BaseBS bs = q.Item5;

				if (bp.X < 0)
				{
					bp.X += World.arraySize;
					ap.X -= 1;
				}
				if (bp.X >= World.arraySize)
				{
					bp.X -= World.arraySize;
					ap.X += 1;
				}

				if (bp.Y < 0)
				{
					bp.Y += World.arraySize;
					ap.Y -= 1;
				}
				if (bp.Y >= World.arraySize)
				{
					bp.Y -= World.arraySize;
					ap.Y += 1;
				}

				if (bp.Z < 0)
				{
					bp.Z += World.arraySize;
					ap.Z -= 1;
				}
				if (bp.Z >= World.arraySize)
				{
					bp.Z -= World.arraySize;
					ap.Z += 1;
				}

				if (!tmp.world.arraysPos.TryGetValue(ap, out tmp)) continue;

				tmp.GetBlock(bp, ap).Type = type;
				if(bs != null) tmp.GetBlock(bp, ap).Script = bs;
				render.GenVert(ref tmp, ap);
				if (bp.X == 0					&& render.world.arraysPos.TryGetValue(ap - (1, 0, 0), out tmp)) render.GenVert(ref tmp, ap - (1, 0, 0));
				if (bp.X == World.arraySize - 1 && render.world.arraysPos.TryGetValue(ap + (1, 0, 0), out tmp)) render.GenVert(ref tmp, ap + (1, 0, 0));
				if (bp.Y == 0					&& render.world.arraysPos.TryGetValue(ap - (0, 1, 0), out tmp)) render.GenVert(ref tmp, ap - (0, 1, 0));
				if (bp.Y == World.arraySize - 1 && render.world.arraysPos.TryGetValue(ap + (0, 1, 0), out tmp)) render.GenVert(ref tmp, ap + (0, 1, 0));
				if (bp.Z == 0					&& render.world.arraysPos.TryGetValue(ap - (0, 0, 1), out tmp)) render.GenVert(ref tmp, ap - (0, 0, 1));
				if (bp.Z == World.arraySize - 1 && render.world.arraysPos.TryGetValue(ap + (0, 0, 1), out tmp)) render.GenVert(ref tmp, ap + (0, 0, 1));
				render.reRender = true;
			}
			queue.Clear();
		}

		public void Refresh(BlocksArray blocksArray, Vector3i pos, Vector3i arrPos)
		{
			cubeVertices = [];
			Block tmp = blocksArray.GetBlock(pos + (1, 0, 0), arrPos);
			if (tmp.alphaType && tmp.Type != Type) cubeVertices.Add(cubeVerticesXp);

			tmp = blocksArray.GetBlock(pos - (1, 0, 0), arrPos);
			if (tmp.alphaType && tmp.Type != Type) cubeVertices.Add(cubeVerticesXm);

			tmp = blocksArray.GetBlock(pos + (0, 1, 0), arrPos);
			if (tmp.alphaType && tmp.Type != Type) cubeVertices.Add(cubeVerticesYp);

			tmp = blocksArray.GetBlock(pos - (0, 1, 0), arrPos);
			if (tmp.alphaType && tmp.Type != Type) cubeVertices.Add(cubeVerticesYm);

			tmp = blocksArray.GetBlock(pos + (0, 0, 1), arrPos);
			if (tmp.alphaType && tmp.Type != Type) cubeVertices.Add(cubeVerticesZp);

			tmp = blocksArray.GetBlock(pos - (0, 0, 1), arrPos);
			if (tmp.alphaType && tmp.Type != Type) cubeVertices.Add(cubeVerticesZm);
		}
	}

	enum Type
	{
		zero,   // ничего
		air,    // воздух
		stone,  // камень
		soil,   // земля
		sand,   // песок
		water,  // вода
		plant,  // растение
		log,    // дерево
		leaves, // листья
	}
}
