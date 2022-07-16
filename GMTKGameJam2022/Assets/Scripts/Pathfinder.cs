using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using Priority_Queue;
using Debug = UnityEngine.Debug;

public class Pathfinder
{
    public class PathfinderResult<T>
    {
        public bool foundPath = false;
        public List<T> path = null;
        public float cost = Mathf.Infinity;
        public List<T> considered = null;

        PathfinderResult() { }

        public PathfinderResult(List<T> considered)
        {
            this.considered = considered;
        }

        public static PathfinderResult<T> CreatePath(Node<T> node)
        {
            var path = new List<T>();

            var current = node;
            while (current.parent != null)
            {
                path.Add(current.value);
                current = current.parent;
            }


            path.Add(current.value);

            path.Reverse();

            var res = new PathfinderResult<T>()
            {
                foundPath = true,
                path = path,
                cost = node.g
            };

            return res;
        }
    }

    public class Node<T>
    {
        public float g = Mathf.Infinity;
        public float h = Mathf.Infinity;
        public float f
        {
            get { return g + h; }
            private set { }
        }
        public T value;
        public Node<T> parent = null;

        public Node(T value, Node<T> parent)
        {
            this.value = value;
            this.parent = parent;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">the type that is being searched over</typeparam>
    /// <param name="start">initial T to explore</param>
    /// <param name="evaluate">predicate for if the goal is reached</param>
    /// <param name="getNeighbors">function that returns available neighbors to T t</param>
    /// <param name="cost">function to evaluate cost between T1 and T2</param>
    /// /// <param name="cost">function to estimate cost between T1 and the end</param>
    /// <param name="abort">number of tiles to explore before aborting search</param>
    /// <returns></returns>
    public PathfinderResult<T> GetPath<T>(T start, Func<T, bool> evaluate, Func<T, List<T>> getNeighbors, Func<T, T, float> cost, Func<T, float> hEstimator, int abort = 10000)
    {
        var sw = new Stopwatch();

        var map = new Dictionary<T, Node<T>>();

        var open = new SimplePriorityQueue<Node<T>, float>();
        var closed = new HashSet<T>();

        var startNode = new Node<T>(start, null)
        {
            g = 0
        };

        open.Enqueue(startNode, 0f);

        bool success = false;
        Node<T> end = null;

        while (open.Count > 0 && closed.Count < abort)
        {
            Node<T> current = open.Dequeue();
            closed.Add(current.value);

            // Debug.Log($"considering {current.value}");

            if (evaluate(current.value))
            {
                success = true;
                end = current;
                break;
            }

            foreach (var neighbor in getNeighbors(current.value))
            {


                if (closed.Contains(neighbor))
                {
                    //Debug.Log($"I have a neighbor {neighbor} already considered");
                    continue;
                }
                //else Debug.Log($"I have a neighbor {neighbor} not already considered");
                    

                var tentativeG = current.g + cost(current.value, neighbor);
                //Debug.Log($"cost {tentativeG}");

                // find the referenced node or create new
                // var neighborNode = map.GetValueOrDefault(neighbor, new Node<T>(neighbor, current));
                Node<T> neighborNode;
                if(map.ContainsKey(neighbor))
                {
                    neighborNode = map[neighbor];
                }
                else
                {
                    neighborNode = new Node<T>(neighbor, current);
                    map[neighbor] = neighborNode;
                }

                if (tentativeG < neighborNode.g || !open.Contains(neighborNode))
                {
                    // update neighbor
                    neighborNode.parent = current;
                    neighborNode.g = tentativeG;
                    neighborNode.h = hEstimator(neighbor);
                    //Debug.Log($"new cost");

                    if (!open.Contains(neighborNode))
                    {
                        //Debug.Log($"adding to queue");
                        open.Enqueue(neighborNode, neighborNode.f);
                    }
                }
            }
        }

        sw.Stop();
        // Debug.Log("completed task");

        if (success)
        {
            return PathfinderResult<T>.CreatePath(end);
        }
        else
        {
            var debugNodes = new List<T>();
            foreach (var item in closed)
            {
                debugNodes.Add(item);
            }
            return new PathfinderResult<T>(debugNodes);
        }
    }

}
