
namespace ProjectCube.Scripts.WorldDir.TerrainDir
{
    internal class Line
    {
        public Dictionary<int, Material> materials = [];

        public Line() { }
        public Line(int id, Material material)
        {
            materials.Add(id, material);
        }

        public float GetMaterialY(MaterialType materialType)
        {
            float y = 0;

            foreach (KeyValuePair<int, Material> i in materials)
            {
                if(i.Value.materialType == materialType) y += i.Value.y;
            }

            return y;
		}
    }
}
