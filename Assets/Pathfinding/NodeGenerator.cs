using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class NodeGenerator : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [Header("General Settings")]
    [SerializeField] private Pathfinding.MODE pathfindingMode = default;
    [SerializeField] private Agent agent = null;

    [Header("Map Settings")]
    [SerializeField] private Vector2Int mapSize = Vector2Int.zero;
    [SerializeField] private Vector2Int origin = Vector2Int.zero;
    [SerializeField] private Vector2Int destination = Vector2Int.zero;
    [SerializeField] private List<Vector2Int> blockeds = null;
    [SerializeField] private List<Node.NodeWeight> weights = null;
    #endregion

    #region PRIVATE_FIELDS
    private Node[] map = null;
    private Pathfinding pathfinding = null;
    #endregion

    #region UNITY_CALLS
    private void Awake()
    {
        pathfinding = new Pathfinding(pathfindingMode);
        map = new Node[mapSize.x * mapSize.y];

        NodeUtils.MapSize = mapSize;

        int ID = 0;
        for (int i = 0; i < mapSize.y; i++)
        {
            for (int j = 0; j < mapSize.x; j++)
            {
                map[ID] = new Node(ID, new Vector2Int(j, i));
                ID++;
            }
        }

        for (int i = 0; i < blockeds.Count; i++)
        {
            map[NodeUtils.PositionToIndex(blockeds[i])].Block();
        }
        for (int i = 0; i < weights.Count; i++)
        {
            map[NodeUtils.PositionToIndex(weights[i].position)].SetWeight(weights[i].weight);
        }
    }

    private void Start()
    {
        List<Vector2Int> path = GetPath(origin, destination);
        if (path != null)
        {
            map[NodeUtils.PositionToIndex(origin)].color = Color.yellow;
            map[NodeUtils.PositionToIndex(destination)].color = Color.cyan;

            agent.StartPathfiding(path, SetWalkedColor);
        }
    }

    private void OnDrawGizmos()
    {
        if (map == null)
            return;

        GUIStyle style = new GUIStyle() { fontSize = 10 };

        foreach (Node node in map)
        {
            Gizmos.color = node.color;

            Vector3 worldPosition = new Vector3(node.position.x, node.position.y, 0.0f);
            Gizmos.DrawWireSphere(worldPosition, 0.2f);
            Handles.Label(worldPosition, node.position.ToString(), style);
        }
    }
    #endregion

    #region PUBLIC_METHODS
    public void SetWalkedColor(Vector2Int position)
    {
        map[NodeUtils.PositionToIndex(position)].color = Color.magenta;
    }
    #endregion

    #region PRIVATE_METHODS
    private List<Vector2Int> GetPath(Vector2Int origin, Vector2Int destination)
    {
        return pathfinding.GetPath(map,
            map[NodeUtils.PositionToIndex(origin)],
            map[NodeUtils.PositionToIndex(destination)]);
    }
    #endregion
}
