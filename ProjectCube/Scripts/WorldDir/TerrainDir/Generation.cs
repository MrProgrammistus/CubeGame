using OpenTK.Mathematics;

namespace ProjectCube.Scripts.WorldDir.TerrainDir
{
    internal class Generation(Window window) : Configs(window)
	{
        Noise noise = new(1);

        public LineArray GenerateArray(Vector2 arrPos, int bufferId, Window window)
        {
            LineArray lineArray = new(arrPos, bufferId, window);

            for (int x = 0; x < arraySize; x++)
            {
                for (int z = 0; z < arraySize; z++)
                {
                    lineArray.lines[x][z] = Generate((x + arrPos.X * arraySize, z + arrPos.Y * arraySize));
                }
            }

            return lineArray;
        }

        public Line Generate(Vector2 pos)
        {
            float y = (int)(noise.HeightMap(pos) * 10) / 10f;

            Line line = new(0, new(Type.stone, y));

            if(y < 64) line.materials.Add(1, new(Type.water, 64 - y + 0.01f));

            return line;
        }
    }
}
