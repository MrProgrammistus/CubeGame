using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;

namespace CubeGame.Scripts
{
	internal class Block
	{
		public Block(Type type)
		{
			Type = type;
			SetAlphaType();
		}

		public void SetAlphaType()
		{
			if (Type == Type.air || Type == Type.water) alphaType = true;
			else alphaType = false;
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

		public Type Type { get { return _type; } set { _type = value; SetAlphaType(); } }
		public Type _type;
	    public List<float[]> cubeVertices = [];

		public bool alphaType;

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
		zero,  // ничего
		air,   // воздух
		stone, // камень
		soil,  // земля
		sand,  // песок
		water, // вода
	}
}
