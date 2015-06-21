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

using System;
using System.Collections.Generic;
using System.Text;

namespace tainicom.DataStructures.Graphs
{  
    public class Graph<TNode> : IEnumerable<TNode>
    {
        private NodeList<TNode> nodeSet;

        public Graph() : this(null) { }
        public Graph(NodeList<TNode> nodeSet)
        {
            if (nodeSet == null)
                this.nodeSet = new NodeList<TNode>();
            else
                this.nodeSet = nodeSet;
        }

        public void AddNode(TNode value)
        {
            // adds a node to the graph
            nodeSet.Add(new Node<TNode>(value));
        }

        public Node<TNode> FindNode(TNode node)
        {
            return nodeSet.FindByValue(node);
        }

        public void AddEdge(TNode from, TNode to) {AddDirectedEdge(from, to, 1f);}
        public void AddEdge(TNode nodeA, TNode nodeB, float weight)
        {
            Node<TNode> xnodeA = FindNode(nodeA);
            Node<TNode> xnodeB = FindNode(nodeB);
            Edge<TNode> edge = new Edge<TNode>(xnodeA, xnodeB, false, weight);

            xnodeA.EdgesUndirected.Add(edge);
            xnodeB.EdgesUndirected.Add(edge);
        }

        public void AddDirectedEdge(TNode from, TNode to) {AddDirectedEdge(from, to, 1f);}
        public void AddDirectedEdge(TNode from, TNode to, float weight)
        {
            Node<TNode> xfrom = FindNode(from);
            Node<TNode> xto = FindNode(to);

            Edge<TNode> edge = new Edge<TNode>(xfrom, xto, true, weight);

            xfrom.EdgesOutgoing.Add(edge);
            xto.EdgesIncoming.Add(edge);
        }

        public bool Contains(TNode value)
        {
            return nodeSet.FindByValue(value) != null;
        }

        public bool Remove(TNode value)
        {
            // first remove the node from the nodeset
            Node<TNode> nodeToRemove = (Node<TNode>)nodeSet.FindByValue(value);
            if (nodeToRemove == null)
                // node wasn't found
                return false;

            // otherwise, the node was found
            nodeSet.Remove(nodeToRemove);

            foreach(var xedge in nodeToRemove.EdgesUndirected)
            {
                xedge.From.EdgesUndirected.Remove(xedge);
                xedge.To.EdgesUndirected.Remove(xedge);
            }
            foreach (var xedge in nodeToRemove.EdgesOutgoing)
            {
                xedge.From.EdgesOutgoing.Remove(xedge);
                xedge.To.EdgesIncoming.Remove(xedge);
            }
            foreach (var xedge in nodeToRemove.EdgesIncoming)
            {
                xedge.From.EdgesIncoming.Remove(xedge);
                xedge.To.EdgesOutgoing.Remove(xedge);
            }
            

            return true;
        }

        public NodeList<TNode> Nodes
        {
            get { return nodeSet;}
        }

        public int Count
        {
            get { return nodeSet.Count; }
        }


        #region IEnumerable<TNode> Members

        public IEnumerator<TNode> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
