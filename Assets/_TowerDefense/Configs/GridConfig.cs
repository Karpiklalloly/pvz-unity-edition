using UnityEngine;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "TowerDefence/Grid Config")]
    public class GridConfig : ScriptableObject
    {
        [Tooltip("Размер сетки (Колонки x Строки)")]
        public Vector2 GridSize = new Vector2(9, 5);

        [Tooltip("Размер одной ячейки в мировых координатах Unity")]
        public Vector2 CellSize = new Vector2(1.0f, 1.0f);

        [Tooltip("Позиция центра левой нижней ячейки (0,0)")]
        public Vector2 OriginPosition = new Vector2(-4f, -2f);
        public GridCell CellConfig = null;
    }
}