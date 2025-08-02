using DCFApixels.DragonECS;
using Karpik.Engine.Shared.DragonECS;
using UnityEngine;

namespace TowerDefense.Core
{
    public class GridInitializationSystem : IEcsInject<EcsDefaultWorld>, IEcsRunOnEvent<SceneLoadedEvent>
    {
        private EcsDefaultWorld _world;

        public void RunOnEvent(ref SceneLoadedEvent evt)
        {
            var config = _world.Get<CurrentLevel>().Congig;
            if (config == null) return;
            var grid = config.Grid;
            var cellConfig = config.Grid.CellConfig;
            for (int x = 0; x < grid.GridSize.x; x++)
            {
                for (int y = 0; y < grid.GridSize.y; y++)
                {
                    var (go, cell) = Spawner.Spawn(cellConfig.Prefab, cellConfig);
                    var transform = cell.Get<TransformReference>().Transform;
                    transform.position = new Vector3(
                        grid.OriginPosition.x + x * grid.CellSize.x,
                        0.001f, // Slightly above ground to avoid z-fighting
                        grid.OriginPosition.y + y * grid.CellSize.y);
                    transform.localScale = new Vector3(
                        grid.CellSize.x / 10,
                        1f,
                        grid.CellSize.y / 10);
                    var rendererPool = _world.GetPool<RendererReference>();
                    rendererPool.Get(cell.ID).Renderer.material.color = cellConfig.Get<CellColor>().Color;
                    cell.Get<GridCoordinates>().Position.x = x;
                    cell.Get<GridCoordinates>().Position.y = y;
                }
            }
        }

        public void Inject(EcsDefaultWorld obj)
        {
            _world = obj;
        }
    }
}