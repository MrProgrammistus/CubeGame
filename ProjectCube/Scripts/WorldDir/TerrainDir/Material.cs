
namespace ProjectCube.Scripts.WorldDir.TerrainDir
{
    internal class Material
    {
        public Type type;
        public MaterialType materialType;
        public float y;
        public Material(Type type, float y)
        {
            this.type = type;
            this.y = y;

            if (type == Type.stone) materialType = MaterialType.solid;
            if (type == Type.water) materialType = MaterialType.liquid;
			if (type == Type.air) materialType = MaterialType.gas;
		}
    }

    enum Type
    {
        zero,
        stone,
        water,
		air,
	}

    enum MaterialType
    {
        zero,
		solid,
        liquid,
        gas,
	}
}
