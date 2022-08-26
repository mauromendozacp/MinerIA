﻿using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    #region ENUMS
    [System.Serializable]
    public enum MODE
    {
        BREADTHFIRST,
        DEPHFIRST,
        DIJKSTRA,
        ASTAR
    }
    #endregion

    #region PRIVATE_FIELDS
    private MODE mode = default;

    private List<int> openedNodeIds = null;
    private List<int> closedNodeIds = null;

    private Vector2Int destinationPos = Vector2Int.zero;
    #endregion

    #region CONSTRUCTS
    public Pathfinding(MODE mode)
    {
        this.mode = mode;

        openedNodeIds = new List<int>();
        closedNodeIds = new List<int>();
    }
    #endregion

    #region PUBLIC_METHODS
    public List<Vector2Int> GetPath(Node[] map, Node origin, Node destination)
    {
        openedNodeIds.Add(origin.ID);
        destinationPos = destination.position;

        Node currentNode = origin;
        while (currentNode.position != destination.position)
        {
            currentNode = GetNextNode(map);

            if (currentNode == null)
                return new List<Vector2Int>();

            for (int i = 0; i < currentNode.adjacentNodeIDs.Count; i++)
            {
                if (currentNode.adjacentNodeIDs[i] != -1)
                {
                    if (map[currentNode.adjacentNodeIDs[i]].state == Node.NodeState.Ready)
                    {
                        map[currentNode.adjacentNodeIDs[i]].Open(currentNode.ID, currentNode.totalWight);
                        openedNodeIds.Add(map[currentNode.adjacentNodeIDs[i]].ID);
                    }
                }
            }

            currentNode.Close();
            openedNodeIds.Remove(currentNode.ID);
            closedNodeIds.Add(currentNode.ID);
        }

        List<Vector2Int> path = GeneratePath(map, currentNode);
        return path;
    }
    #endregion

    #region PRIVATE_METHODS
    private List<Vector2Int> GeneratePath(Node[] map, Node current)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        while (current.openerID != -1)
        {
            path.Add(current.position);
            current = map[current.openerID];
        }

        path.Add(current.position);
        path.Reverse();

        return path;
    }

    private Node GetNextNode(Node[] map)
    {
        switch (mode)
        {
            case MODE.BREADTHFIRST:
                return map[openedNodeIds[0]];

            case MODE.DEPHFIRST:
                return map[openedNodeIds[openedNodeIds.Count - 1]];

            case MODE.DIJKSTRA:
                {
                    Node node = null;
                    int currentMaxWeight = int.MaxValue;

                    for (int i = 0; i < openedNodeIds.Count; i++)
                    {
                        if (map[openedNodeIds[i]].totalWight < currentMaxWeight)
                        {
                            node = map[openedNodeIds[i]];
                            currentMaxWeight = map[openedNodeIds[i]].totalWight;
                        }
                    }

                    return node;
                }

            case MODE.ASTAR:
                {
                    Node node = null;
                    int currentWeightAndDistanceMax = int.MaxValue;

                    for (int i = 0; i < openedNodeIds.Count; i++)
                    {
                        if (map[openedNodeIds[i]].totalWight + GetManhattanDistance(map[openedNodeIds[i]].position, destinationPos) < currentWeightAndDistanceMax)
                        {
                            node = map[openedNodeIds[i]];
                            currentWeightAndDistanceMax = map[openedNodeIds[i]].totalWight + GetManhattanDistance(map[openedNodeIds[i]].position, destinationPos);
                        }
                    }

                    return node;
                }
        }

        return null;
    }

    private int GetManhattanDistance(Vector2Int origin, Vector2Int destination)
    {
        int distX = Mathf.Abs(origin.x - destination.x);
        int distY = Mathf.Abs(origin.y - destination.y);

        return distX + distY;
    }
    #endregion
}
