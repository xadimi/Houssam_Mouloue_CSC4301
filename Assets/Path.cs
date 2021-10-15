using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Transform seeker, target;
    int c1 = 0;
    int c2 = 0;
    int c3 = 0;
    int c4 = 0;
    Grid grid;
    void Awake()
    {
        grid = GetComponent<Grid>();
    }
    void Update() {
        c1 = 0;
        c2 = 0;
        c3 = 0;
        

        var w1 = new System.Diagnostics.Stopwatch();
        var w2 = new System.Diagnostics.Stopwatch();
        var w3 = new System.Diagnostics.Stopwatch();
       

        w1.Start();
        A(seeker.position, target.position);
        w1.Stop();
        Debug.Log($"Execution Time Euclidian: {w1.ElapsedMilliseconds * 100} ms, retracement : {c1}");

        w2.Start();
        BFSfind(seeker.position, target.position);
        w2.Stop();
        Debug.Log($"Execution Time Euclidian: {w2.ElapsedMilliseconds * 100} ms, retracement : {c2}");

        w3.Start();
        DFSfind(seeker.position, target.position);
        w3.Stop();
        Debug.Log($"Execution Time Euclidian: {w3.ElapsedMilliseconds * 100} ms, retracement : {c3}");

        A (seeker.position, target.position);
        DFSfind(seeker.position, target.position);
        BFSfind(seeker.position, target.position);
    }



    void DFSfind(Vector3 startpos, Vector3 targetpos){ 
        var time = new System.Diagnostics.Stopwatch();
        time.Start();
        Node startx = grid.NodeFromWorldPoint(startpos);
        Node targetz = grid.NodeFromWorldPoint(targetpos);


        Stack<Node> work = new Stack<Node> ();
        List<Node> visited = new List<Node> ();

        work.Push (startx);
        visited.Add (startx);
        

        while(work.Count > 0){

            Node current = work.Pop ();
            if (current == targetz) {

                RetracePath2(startx,current);
                return;

            } else {
                
                foreach (Node neighbour in grid.GetNeighbours(current)) {
                if (!neighbour.walkable || visited.Contains(neighbour)) {
                    continue;
                }

                    Node currentChild = neighbour;
                    neighbour.history = current;
                    if(!visited.Contains(currentChild)){
                        work.Push (currentChild);
                        visited.Add (currentChild);

                    }
                }   
            }
        }
        time.Stop();
        Debug.Log($"Execution Time Manhattan: {time.ElapsedMilliseconds} ms");
    }
    void BFSfind(Vector3 startpos, Vector3 targetpos) {
        var time = new System.Diagnostics.Stopwatch();
        time.Start();
        Node starta = grid.NodeFromWorldPoint(startpos);
        Node targetb = grid.NodeFromWorldPoint(targetpos);

        Queue<Node> work = new Queue<Node>();
        List<Node> visited = new List<Node>();

        work.Enqueue (starta);
        visited.Add (starta);
        

        while(work.Count > 0){

            Node current = work.Dequeue ();

            if(current == targetb){
                // we found a solution!!
                RetracePath3(starta,current);
                return;

            } else {
                foreach (Node neighbour in grid.GetNeighbours(current)) {
                if (!neighbour.walkable || visited.Contains(neighbour)) {
                    continue;
                }
                    
                    Node currentChild = neighbour;
                    neighbour.history = current;
                    if(!visited.Contains(currentChild)){

                        work.Enqueue (currentChild);
                        visited.Add (currentChild);
                    }

                }
            }
        }
        time.Stop();
        Debug.Log($"Execution Time Manhattan: {time.ElapsedMilliseconds} ms");
        
    }
    void A(Vector3 startPos, Vector3 targetPos) {
        var time = new System.Diagnostics.Stopwatch();
        time.Start();
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i ++) {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost){
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == targetNode) {
                RetracePath(startNode,targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(node)) {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        time.Stop();
        Debug.Log($"Execution Time Manhattan: {time.ElapsedMilliseconds} ms");
    }
    void RetracePath2(Node startx, Node targetz) {
        List<Node> path2 = new List<Node>();
        Node currentNode = targetz;
        while (currentNode != startx) {
            path2.Add(currentNode);
            currentNode = currentNode.history;
        }
        path2.Reverse();
        grid.path2 = path2;

    }
    void RetracePath3(Node starta, Node targetb) {
        List<Node> path3 = new List<Node>();
        Node currentNode = targetb;
        while (currentNode != starta) {
            path3.Add(currentNode);
            currentNode = currentNode.history;
        }
        path3.Reverse();
        grid.path3 = path3;

    }
    void RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        grid.path = path;

    }
    int GetDistance(Node nodeA, Node nodeB) {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14*dstY + 10* (dstX-dstY);
        return 14*dstX + 10 * (dstY-dstX);
    }
}
