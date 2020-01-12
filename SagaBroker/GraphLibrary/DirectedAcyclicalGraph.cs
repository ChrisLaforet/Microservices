using System;
using System.Collections.Generic;
using System.Text;
using SagaBroker.Exception;

namespace SagaBroker.GraphLibrary
{
	public class DirectedAcyclicalGraph
	{
		private DAGNode root;
		private int totalNodes = 0;

		public DirectedAcyclicalGraph() { }

		public void Add(string node)
		{
			if (root != null)
				throw new GraphRootAlreadyExistsException("Node " + node + " cannot be rooted");

			root = new DAGNode(node);
			++totalNodes;
		}

		public void AddChild(string node, string parent)
		{
			if (parent == null)
			{
				Add(node);
				return;
			}

			DAGNode parentNode = FindNode(parent);
			if (parentNode == null)
				throw new GraphNodeMissingException("Cannot locate parent node " + parent);
			parentNode.AddChild(new DAGNode(node));
			++totalNodes;
		}

		private DAGNode FindNode(string node)
		{
			return root.FindNode(node);
		}

		public bool Validate()
		{
			if (root == null || Count == 1)
				return true;
			return root.Validate();
		}

		public bool IsEmpty
		{
			get
			{
				return root == null;
			}
		}

		public int Count
		{
			get
			{
				return totalNodes;
			}
		}
	}

	class DAGNode
	{
		public DAGNode(string name, List<DAGNode> children = null)
		{
			Name = name;
			Children = children == null ? new List<DAGNode>() : children;
		}

		public string Name { get; private set; }

		public DAGNode FindNode(string name)
		{
			if (this.Name.CompareTo(name) == 0)
				return this;
			foreach (var child in this.Children)
			{
				DAGNode node = child.FindNode(name);
				if (node != null)
					return node;
			}
			return null;
		}

		public bool Validate()
		{
			// build lists - check for duplicate as building occurs
			return true;
		}


		public List<DAGNode> Children { get; private set; }

		public void AddChild(DAGNode node)
		{
			Children.Add(node);
		}
	}
}
