using ProjectCube.Scripts.WorldDir.PlanetsDir;
using ProjectCube.Scripts.WorldDir.PlayerDir;

namespace ProjectCube.Scripts.WorldDir.SceneDir
{
    internal class Scene(Window window)
    {
        public List<GameObject> gameObjects =
        [
            new("Player", [new Player(window)]),
            new("Planet0", [new RenderObject(), new Planet(window, 300, 5, 3, 1, 27000000, 0.001f)], pos: (0, 0, 0), rot: (0, 0, 45)),
            new("Planet1", [new RenderObject(), new Planet(window, 50, 3, 3, 2, 125000_000000)], pos: (500, 0, 0)),
            new("Planet2", [new RenderObject(), new Planet(window, 20, 3, 3, 3, 8000)], pos: (0, 0, 1000)),
        ];

        public void Awake()
        {
            gameObjects[3].parent = gameObjects[1];
        }
        
        public GameObject Find(string name)
        {
            foreach (var i in gameObjects) if (i.name == name) return i;

			return null;
        }
    }
}
