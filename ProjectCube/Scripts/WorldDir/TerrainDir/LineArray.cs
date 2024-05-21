using OpenTK.Mathematics;
using ProjectCube.Scripts.RenderDir;

namespace ProjectCube.Scripts.WorldDir.TerrainDir
{
    internal class LineArray : Configs
    {
        public Line[][] lines = [];

        public Vector2 arrPos;

        public float[] verts = [];
        public float[] verts_l = [];

		public int flags;
		public int bufferId;

        public LineArray(Vector2 arrPos, int bufferId, Window window) : base(window)
        {
            this.arrPos = arrPos;
			this.bufferId = bufferId;

            lines = new Line[arraySize][];
            for (int i = 0; i < arraySize; i++) lines[i] = new Line[arraySize];
        }

        public Line GetLine(Vector2i pos)
        {
            if (pos.X >= 0 && pos.X < arraySize && pos.Y >= 0 && pos.Y < arraySize) return lines[pos.X][pos.Y];
            else
            {
                if      (pos.X < 0          && window.world.terrain.lineArrays.TryGetValue(arrPos - (1, 0), out LineArray lineArray)) return lineArray.GetLine(pos + (arraySize, 0));
                else if (pos.X >= arraySize && window.world.terrain.lineArrays.TryGetValue(arrPos + (1, 0), out           lineArray)) return lineArray.GetLine(pos - (arraySize, 0));
                else if (pos.Y < 0          && window.world.terrain.lineArrays.TryGetValue(arrPos - (0, 1), out           lineArray)) return lineArray.GetLine(pos + (0, arraySize));
                else if (pos.Y >= arraySize && window.world.terrain.lineArrays.TryGetValue(arrPos + (0, 1), out           lineArray)) return lineArray.GetLine(pos - (0, arraySize));
                else return null;
            }
        }

		public void GenVerts()
		{
			List<float> pointsData = [];
			List<float> pointsData_l = [];

			for (int x = 0; x < arraySize; x++)
			{
				for (int z = 0; z < arraySize; z++)
				{
					float y = 0, yx= 0, yz = 0, yxz = 0;
					for (int i = 1; i < 3; i++)
					{
						if (x < arraySize - 1 && z < arraySize - 1)
						{
							Vector3 pos = (x + arrPos.X * arraySize, lines[x][z].GetMaterialY((MaterialType)i) + y, z + arrPos.Y * arraySize);
							Vector3 posX = (x + arrPos.X * arraySize + 1, lines[x + 1][z].GetMaterialY((MaterialType)i) + yx, z + arrPos.Y * arraySize);
							Vector3 posZ = (x + arrPos.X * arraySize, lines[x][z + 1].GetMaterialY((MaterialType)i) + yz, z + arrPos.Y * arraySize + 1);
							Vector3 posXZ = (x + arrPos.X * arraySize + 1, lines[x + 1][z + 1].GetMaterialY((MaterialType)i) + yxz, z + arrPos.Y * arraySize + 1);

							if (pos.Y != y && posX.Y != yx && posZ.Y != yz && posXZ.Y != yxz)
							{
								if (i == 1) pointsData.AddRange(Graphics.Plane(pos, posX, posZ, posXZ, i * 256));
								else if (i == 2) pointsData_l.AddRange(Graphics.Plane(pos, posX, posZ, posXZ, i * 256));

								y += lines[x][z].GetMaterialY((MaterialType)i);
								yx += lines[x + 1][z].GetMaterialY((MaterialType)i);
								yz += lines[x][z + 1].GetMaterialY((MaterialType)i);
								yxz += lines[x + 1][z + 1].GetMaterialY((MaterialType)i);
							}
							else
							{
								if (i == 1) pointsData.AddRange(Graphics.Plane((0, 0, 0), (0, 0, 0), (0, 0, 0), (0, 0, 0), 0));
								else if (i == 2) pointsData_l.AddRange(Graphics.Plane((0, 0, 0), (0, 0, 0), (0, 0, 0), (0, 0, 0), 0));
							}
						}
						else if (GetLine((x + 1, z)) != null && GetLine((x, z + 1)) != null && GetLine((x + 1, z + 1)) != null)
						{
							Vector3 pos = (x + arrPos.X * arraySize, GetLine((x, z)).GetMaterialY((MaterialType)i) + y, z + arrPos.Y * arraySize);
							Vector3 posX = (x + arrPos.X * arraySize + 1, GetLine((x + 1, z)).GetMaterialY((MaterialType)i) + yx, z + arrPos.Y * arraySize);
							Vector3 posZ = (x + arrPos.X * arraySize, GetLine((x, z + 1)).GetMaterialY((MaterialType)i) + yz, z + arrPos.Y * arraySize + 1);
							Vector3 posXZ = (x + arrPos.X * arraySize + 1, GetLine((x + 1, z + 1)).GetMaterialY((MaterialType)i) + yxz, z + arrPos.Y * arraySize + 1);
							
							if(pos.Y != y && posX.Y != yx && posZ.Y != yz && posXZ.Y != yxz)
							{
								if (i == 1) pointsData.AddRange(Graphics.Plane(pos, posX, posZ, posXZ, i * 256));
								else if (i == 2) pointsData_l.AddRange(Graphics.Plane(pos, posX, posZ, posXZ, i * 256));

								y += GetLine((x, z)).GetMaterialY((MaterialType)i);
								yx += GetLine((x + 1, z)).GetMaterialY((MaterialType)i);
								yz += GetLine((x, z + 1)).GetMaterialY((MaterialType)i);
								yxz += GetLine((x + 1, z + 1)).GetMaterialY((MaterialType)i);
							}
							else
							{
								if (i == 1) pointsData.AddRange(Graphics.Plane((0, 0, 0), (0, 0, 0), (0, 0, 0), (0, 0, 0), 0));
								else if (i == 2) pointsData_l.AddRange(Graphics.Plane((0, 0, 0), (0, 0, 0), (0, 0, 0), (0, 0, 0), 0));
							}
						}
						else
						{
							if (i == 1) pointsData.AddRange(Graphics.Plane((0, 0, 0), (0, 0, 0), (0, 0, 0), (0, 0, 0), 0));
							else if (i == 2) pointsData_l.AddRange(Graphics.Plane((0, 0, 0), (0, 0, 0), (0, 0, 0), (0, 0, 0), 0));
						}
					}
				}
			}

			verts = [.. pointsData];
			verts_l = [.. pointsData_l];
		}
	}
}
