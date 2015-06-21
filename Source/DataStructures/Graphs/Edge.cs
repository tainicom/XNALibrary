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

namespace tainicom.DataStructures.Graphs
{
    public class Edge<TNode>
    {
        public readonly Node<TNode> From;
        public readonly Node<TNode> To;
        public readonly bool IsDirected;
        public readonly float Weight;

        public Edge(Node<TNode> nodeA, Node<TNode> nodeB): this(nodeA, nodeB,false,1f) { }
        public Edge(Node<TNode> nodeA, Node<TNode> nodeB, bool isDirected): this(nodeA,nodeB,isDirected,1f) {}
        public Edge(Node<TNode> nodeA, Node<TNode> nodeB, float weight) : this(nodeA, nodeB, false, weight) { }

        public Edge(Node<TNode> nodeA, Node<TNode> nodeB, bool isDirected, float weight)
        {
            this.From = nodeA;
            this.To = nodeB;
            this.IsDirected = isDirected;
            this.Weight = weight; 
        }

        public override string ToString()
        {   
            if(this.IsDirected)
                return String.Format("From:{0}, To:{1}, Weight:{2}",From,To,Weight);
            else
                return String.Format("NodeA:{0}, NodeB:{1}, Weight:{2}",From,To,Weight);
        
        }
    }


}
