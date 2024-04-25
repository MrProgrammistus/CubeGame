using OpenTK.Mathematics;

namespace CubeGame.Scripts
{
    internal class BlocksArray
    {
        //шум
        public static Noise n = new();

        //данные
        public byte size;
        public float scale;
        public Block[][][]? blocks;

		public List<float> vert = [];
		public List<float> vert_alpha = [];

        //ссылки world
        public World world;

        public BlocksArray(byte size, float scale, World world)
        {
            this.size = size;
            this.scale = scale;
            this.world = world;

			blocks = new Block[size][][];
			for (int i = 0; i < size; i++)
			{
				blocks[i] = new Block[size][];
				for (int j = 0; j < size; j++)
				{
					blocks[i][j] = new Block[size];
				}
			}
		}

		public Block GetBlock(Vector3i pos, Vector3i arrPos)
        {
            if (blocks != null)
            {
                if (pos.X >= 0 && pos.X < size && pos.Y >= 0 && pos.Y < size && pos.Z >= 0 && pos.Z < size)
                    return blocks[pos.X][pos.Y][pos.Z];
                else
                {
                    BlocksArray tmp;
                    if (pos.X < 0     && world.arraysPos.TryGetValue(arrPos - (1, 0, 0), out tmp)) return tmp.GetBlock(pos + (World.arraySize, 0, 0), arrPos - (1, 0, 0));
                    if (pos.X >= size && world.arraysPos.TryGetValue(arrPos + (1, 0, 0), out tmp)) return tmp.GetBlock(pos - (World.arraySize, 0, 0), arrPos + (1, 0, 0));
                    if (pos.Y < 0     && world.arraysPos.TryGetValue(arrPos - (0, 1, 0), out tmp)) return tmp.GetBlock(pos + (0, World.arraySize, 0), arrPos - (0, 1, 0));
                    if (pos.Y >= size && world.arraysPos.TryGetValue(arrPos + (0, 1, 0), out tmp)) return tmp.GetBlock(pos - (0, World.arraySize, 0), arrPos + (0, 1, 0));
                    if (pos.Z < 0     && world.arraysPos.TryGetValue(arrPos - (0, 0, 1), out tmp)) return tmp.GetBlock(pos + (0, 0, World.arraySize), arrPos - (0, 0, 1));
                    if (pos.Z >= size && world.arraysPos.TryGetValue(arrPos + (0, 0, 1), out tmp)) return tmp.GetBlock(pos - (0, 0, World.arraySize), arrPos + (0, 0, 1));
				}
            }
            return Generation.Generate(pos.X, pos.Y, pos.Z, arrPos);
		}
        public Block GetBlockFast(Vector3i pos)
        {
			if (blocks != null) return blocks[pos.X][pos.Y][pos.Z];
            return new(Type.zero);
		}

		public static (Vector3i, Vector3i, Vector3i) GetPoses(Vector3 position, float offset = 0.5f)
        {
			Vector3i globalPos = ((int)MathF.Floor(position.X + offset),
								  (int)MathF.Floor(position.Y + offset),
								  (int)MathF.Floor(position.Z + offset));
			Vector3i arrPos = ((int)MathF.Floor((float)globalPos.X / World.arraySize),
							   (int)MathF.Floor((float)globalPos.Y / World.arraySize),
							   (int)MathF.Floor((float)globalPos.Z / World.arraySize));
            Vector3i blockPos = globalPos - arrPos * World.arraySize;
            return (globalPos, arrPos, blockPos);
		}
        public static (Vector3i, Vector3i) GetPoses(Vector3i position)
        {
			Vector3i arrPos = ((int)MathF.Floor((float)position.X / World.arraySize),
							   (int)MathF.Floor((float)position.Y / World.arraySize),
							   (int)MathF.Floor((float)position.Z / World.arraySize));
            Vector3i blockPos = position - arrPos * World.arraySize;
            return (arrPos, blockPos);
		}
    }
}
