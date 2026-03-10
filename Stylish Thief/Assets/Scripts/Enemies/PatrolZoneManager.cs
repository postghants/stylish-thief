using PZP;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Diagnostics;

public class PatrolZoneManager : MonoBehaviour
{
    public static PatrolZoneManager instance;

    [Header("Path Generation")]
    public float verticalWeight = 1;
    public float maxPathDistance = 40;

    [Header("Internal")]
    public List<PatrolZone> zones = new();
    public List<Node> nodes = new();
    public List<NodePair> pairs = new();

    private void Start()
    {
        if (zones.Count <= 1) { return; }

        foreach(PatrolZone zone in zones)
        {
            zone.OnStart();
            nodes.Add(new(zone));
        }

        foreach (Node zone1 in nodes)
        {
            foreach (Node zone2 in nodes)
            {
                if (zone1 == zone2) { continue; }
                foreach (NodeConnection connection in zone1.connections) { if (connection.other.zone == zone2.zone) { continue; } }

                Vector3 closest1 = zone1.zone.ClosestPoint(zone2.zone.ClosestPoint(zone1.zone.transform.position));
                Vector3 closest2 = zone2.zone.ClosestPoint(closest1);
                Vector3 weighted = closest1 - closest2;
                weighted.y *= verticalWeight;

                float distance = (closest1 - closest2).magnitude;
                if (distance > maxPathDistance) { continue; }
                float distanceWeighted = weighted.magnitude;

                pairs.Add(new(zone1, zone2, Vector3.Distance(closest1, closest2), distanceWeighted));
                zone1.connections.Add(new(zone2, distance, distanceWeighted));
                zone2.connections.Add(new(zone1, distance, distanceWeighted));
                Debug.Log("Added patrol zone connection " + zone1.zone.gameObject + " " + zone2.zone.gameObject);
            }
        }
    }

    public List<PatrolZone> FindShortestPath(PatrolZone start, PatrolZone end, float maxStepSize)
    {
        Node startNode = null;
        foreach(Node zone in nodes) { if(zone.zone == start) { startNode = zone; break; } }
        if(startNode == null) { return null; }
        Node endNode = null;
        foreach (Node zone in nodes) { if (zone.zone == end) { endNode = zone; break; } }
        if (endNode == null) { return null; }

        Utils.PriorityQueue<Node, float> frontier = new();
        Dictionary<Node, Node> came_from = new();
        Dictionary<Node, float> cost = new();


        frontier.Enqueue(startNode, 0);
        came_from.Add(startNode, null);
        cost.Add(startNode, 0);

        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();
            if (current == endNode)
            {
                Debug.Log($"Cells evaluated: {came_from.Count}");
                break;
            }
            foreach (NodeConnection connection in current.connections)
            {
                float neighborCost = cost[current] + connection.distanceWeighted;
                if ((!cost.ContainsKey(connection.other) || neighborCost < cost[connection.other]))
                {
                    if (!cost.TryAdd(connection.other, neighborCost))
                    {
                        cost[connection.other] = neighborCost;
                    }
                    frontier.Enqueue(connection.other, neighborCost + Heuristic(endNode.zone.transform.position, connection.other.zone.transform.position));
                    if (!came_from.TryAdd(connection.other, current))
                    {
                        came_from[connection.other] = current;
                    }
                }
            }
        }
        if (!came_from.ContainsKey(endNode))
        {
            return null;
        }

        Node backtrackCell = endNode;
        Stack<PatrolZone> pathStack = new();
        List<PatrolZone> path = new();
        while (backtrackCell != startNode)
        {
            pathStack.Push(backtrackCell.zone);
            backtrackCell = came_from[backtrackCell];
        }
        while (pathStack.Count > 0)
        {
            path.Add(pathStack.Pop());
        }
        return path;
    }

    public float Heuristic(Vector3 start, Vector3 end)
    {
        return Vector3.Distance(start, end);
    }



    private void Reset()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        zones.AddRange(FindObjectsByType<PatrolZone>(FindObjectsSortMode.None));
    }
}

namespace PZP
{

    public class Node
    {
        public PatrolZone zone;
        public List<NodeConnection> connections = new();

        public Node(PatrolZone zone)
        {
            this.zone = zone;
        }
    }

    public class NodePair
    {
        public Node[] zones;
        public float distanceReal;
        public float distanceWeighted;

        public NodePair(Node zone1, Node zone2, float distanceReal, float distanceWeighted)
        {
            zones = new Node[2] { zone1, zone2 };
            this.distanceReal = distanceReal;
            this.distanceWeighted = distanceWeighted;
        }
    }
    public class NodeConnection
    {
        public Node other;
        public float distanceReal;
        public float distanceWeighted;

        public NodeConnection(Node other, float distanceReal, float distanceWeighted)
        {
            this.other = other;
            this.distanceReal = distanceReal;
            this.distanceWeighted = distanceWeighted;
        }
    }
}
