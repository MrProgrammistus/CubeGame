using OpenTK.Mathematics; 

namespace ProjectCube.Scripts.WorldDir.SceneDir
{
    internal class GameObject
	{
        public GameObject(string name, List<InterBase> elements = null, GameObject parent = null, Vector3 pos = default, Vector3 rot = default, Vector3 scl = default)
        {
            if(elements != null) this.elements = elements;
            else this.elements = [];

			foreach (var i in elements) i.gameObject = this;

			this.name = name;
            this.parent = parent;
			position = pos;
			rotation = rot;
			scale = scl == default ? (1, 1, 1) : scl;
		}

        public T GetElement<T>()
        {
            foreach (var i in elements) if (i is T t) return t;

			return default;
        }

        public Matrix4 GetMatrix()
        {
            Matrix4 matrix = Matrix4.Identity;
            matrix *= Matrix4.CreateScale(scale);
            matrix *= Matrix4.CreateRotationX(rotation.X);
            matrix *= Matrix4.CreateRotationY(rotation.Y);
            matrix *= Matrix4.CreateRotationZ(rotation.Z);
            matrix *= Matrix4.CreateTranslation(position);
            if (parent != null) matrix *= parent.GetMatrix();
            return matrix;
        }

        public string name;

		public Vector3 position;
		public Vector3 rotation;
		public Vector3 scale;

        public GameObject parent;

        public List<InterBase> elements;
    }
}
