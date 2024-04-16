using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CubeGame.Scripts
{
	internal class Player
	{
		// позиция игрока
		public Vector3 position = (0, 16, 0);
		public Vector3 velocity;
		public Vector3 rotation;
		public Vector3i selectBlock;
		public Vector3i selectBlockm;
		public bool isSelectBlock;
		float speed = 0.2f;
		float distance = 100;

		Vector3 lastPosition;

		// камера
		public Camera camera;

		public Player(int width, int height)
		{
			lastPosition = position;
			camera = new(width, height);
		}

		public Matrix4 Update(GameWindow gameWindow, float deltaTime, Render render)
		{
			// ========== ДВИЖЕНИЕ ==========
			// гравитация
			//velocity.Y -= 2 * deltaTime;

			// замедление
			velocity *= 0.9f;
			if (velocity.Length < 0.0001f) velocity = (0, 0, 0);

			// ускорение (управление с клавиатуры и мыши)
			camera.Move(gameWindow, deltaTime, ref velocity, ref rotation, speed);

			// движение
			position += velocity;

			// коллизии
			//position = Collision.CheckCollision(lastPosition, position, ref velocity, render.world);
			lastPosition = position;

			// ========== ВЫДЕЛЕНИЕ ==========
			// Куда смотрит
			for (int i = 0; i < distance; i+= 1)
			{
				(Vector3i gp, Vector3i ap, Vector3i bp) = BlocksArray.GetPoses(position + rotation * i);
				if (render.world.arraysPos.TryGetValue(ap, out BlocksArray? tmp) && tmp.GetBlock(bp, ap) != null && tmp.GetBlock(bp, ap).Type != Type.air)
				{
					for (float j = 0; j < 1; j += 0.0001f)
					{
						(gp, ap, bp) = BlocksArray.GetPoses(position + rotation * (i + j - 1));
						if (render.world.arraysPos.TryGetValue(ap, out tmp) && tmp.GetBlock(bp, ap) != null && tmp.GetBlock(bp, ap).Type != Type.air)
						{
							isSelectBlock = true;
							selectBlock = gp;
							goto a;
						}
						else
						{
							selectBlockm = gp;
						}
					}
				}
			}
			isSelectBlock = false;
		a:;

			// ========== УПРАВЛЕНИЕ БЛОКАМИ ==========
			// если нажат ПКМ
			if (gameWindow.IsMouseButtonPressed(MouseButton.Right))
			{
				(Vector3i ap, Vector3i bp) = BlocksArray.GetPoses(selectBlockm);

				if (render.world.arraysPos.TryGetValue(ap, out BlocksArray? tmp))
				{
					tmp.GetBlock(bp, ap).Type = Type.stone;
					render.GenVert(ref tmp, ap);
					if(bp.X == 0				   && render.world.arraysPos.TryGetValue(ap - (1, 0, 0), out tmp)) render.GenVert(ref tmp, ap - (1, 0, 0));
					if(bp.X == World.arraySize - 1 && render.world.arraysPos.TryGetValue(ap + (1, 0, 0), out tmp)) render.GenVert(ref tmp, ap + (1, 0, 0));
					if(bp.Y == 0				   && render.world.arraysPos.TryGetValue(ap - (0, 1, 0), out tmp)) render.GenVert(ref tmp, ap - (0, 1, 0));
					if(bp.Y == World.arraySize - 1 && render.world.arraysPos.TryGetValue(ap + (0, 1, 0), out tmp)) render.GenVert(ref tmp, ap + (0, 1, 0));
					if(bp.Z == 0				   && render.world.arraysPos.TryGetValue(ap - (0, 0, 1), out tmp)) render.GenVert(ref tmp, ap - (0, 0, 1));
					if(bp.Z == World.arraySize - 1 && render.world.arraysPos.TryGetValue(ap + (0, 0, 1), out tmp)) render.GenVert(ref tmp, ap + (0, 0, 1));
					render.reRender = true;
				}
			}
			// если нажат ЛКМ
			if (gameWindow.IsMouseButtonPressed(MouseButton.Left))
			{
				(Vector3i ap, Vector3i bp) = BlocksArray.GetPoses(selectBlock);

				if (render.world.arraysPos.TryGetValue(ap, out BlocksArray? tmp))
				{
					tmp.GetBlock(bp, ap).Type = Type.air;
					render.GenVert(ref tmp, ap);
					if (bp.X == 0					&& render.world.arraysPos.TryGetValue(ap - (1, 0, 0), out tmp)) render.GenVert(ref tmp, ap - (1, 0, 0));
					if (bp.X == World.arraySize - 1 && render.world.arraysPos.TryGetValue(ap + (1, 0, 0), out tmp)) render.GenVert(ref tmp, ap + (1, 0, 0));
					if (bp.Y == 0					&& render.world.arraysPos.TryGetValue(ap - (0, 1, 0), out tmp)) render.GenVert(ref tmp, ap - (0, 1, 0));
					if (bp.Y == World.arraySize - 1 && render.world.arraysPos.TryGetValue(ap + (0, 1, 0), out tmp)) render.GenVert(ref tmp, ap + (0, 1, 0));
					if (bp.Z == 0					&& render.world.arraysPos.TryGetValue(ap - (0, 0, 1), out tmp)) render.GenVert(ref tmp, ap - (0, 0, 1));
					if (bp.Z == World.arraySize - 1 && render.world.arraysPos.TryGetValue(ap + (0, 0, 1), out tmp)) render.GenVert(ref tmp, ap + (0, 0, 1));
					render.reRender = true;
				}
			}

			// ========== КОНЕЦ ==========
			return Matrix4.LookAt(position, position + rotation, (0, 1, 0)) * camera.projection;
		}
	}
}
