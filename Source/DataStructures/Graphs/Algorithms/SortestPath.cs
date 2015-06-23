#region License 
//   Copyright 2015 Kastellanos Nikolaos
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
#endregion

using System.Collections.Generic;
using tainicom.DataStructures.Collections;

namespace tainicom.DataStructures.Graphs.Algorithms
{
    public class SortestPath<TNode>
    {
        Graph<TNode> graph;
        List<TNode> path = new List<TNode>();

        public List<TNode> Path { get { return path; } }


        public SortestPath(Graph<TNode> graph)
        {
            this.graph = graph;
        }

        public bool Execute(TNode from, TNode to)
        {
            Node<TNode> xfrom = graph.FindNode(from);
            Node<TNode> xto = graph.FindNode(to);

            List<Node<TNode>> closed = new List<Node<TNode>>();
            Dictionary<Node<TNode>,int> lookupOpen = new Dictionary<Node<TNode>,int>();
            Graph<TNode> map = new Graph<TNode>();
            map.AddNode(xfrom.Value);

            PriorityQueue<int, Node<TNode>> priorityOpen = new PriorityQueue<int, Node<TNode>>();
            priorityOpen.Enqueue(0, xfrom);
            lookupOpen.Add(xfrom,0);

            while (priorityOpen.Count > 0)
            {
                int distance       = priorityOpen.Peek().Key;
                Node<TNode> current = priorityOpen.Peek().Value;
                
                if (current == xto)
                    return ReconstructPath(map, current.Value);
                
                lookupOpen.Remove(current);
                priorityOpen.Dequeue();
                closed.Add(current);
                foreach (var neighbor in current.EdgesOutgoing)
                {
                    if (closed.Contains(neighbor.To)) continue;

                    int tentative_distance = distance + (int)neighbor.Weight;
                    
                    if(!lookupOpen.ContainsKey(neighbor.To) || tentative_distance < lookupOpen[neighbor.To]) 
                    { 
                        //add neighbor to openset

                        int neighborPriority = (lookupOpen.ContainsKey(neighbor.To))?lookupOpen[neighbor.To]:0;
                        priorityOpen.Enqueue(neighborPriority, neighbor.To);
                        lookupOpen.Add(neighbor.To, neighborPriority);

                        map.AddNode(neighbor.To.Value);
                        map.AddDirectedEdge(current.Value,neighbor.To.Value);
                    }
                }
            }
            return false;
        }

        private bool ReconstructPath(Graph<TNode> map, TNode target)
        {
            Node<TNode> xtarget = map.FindNode(target);
            path.Clear();
            path.Add(xtarget.Value);
            while (xtarget.EdgesIncoming.Count > 0)
            {
                xtarget = xtarget.EdgesIncoming[0].From;
                path.Insert(0, xtarget.Value);
            }

            return true;
        }



    }
}
