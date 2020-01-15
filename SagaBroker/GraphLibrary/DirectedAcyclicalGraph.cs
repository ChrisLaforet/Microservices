using System;
using System.Collections.Generic;
using System.Text;
using SagaBroker.Exception;

namespace SagaBroker.GraphLibrary
{
	public class DirectedAcyclicalGraph<T>
	{
		private DAGNode<T> root;
		private int totalNodes = 0;

		public DirectedAcyclicalGraph() { }

		public void Add(T node)
		{
			if (root != null)
				throw new GraphRootAlreadyExistsException("Node " + node + " cannot be rooted");

			root = new DAGNode<T>(node);
			++totalNodes;
		}

		public void AddChild(T child, T parent)
		{
			if (parent == null)
			{
				Add(child);
				return;
			}

			DAGNode<T> parentNode = FindNode(parent);
			if (parentNode == null)
				throw new GraphNodeMissingException("Cannot locate parent node " + parent);

			CheckChildIsNotItsOwnParent(parentNode, child);

			parentNode.AddChild(new DAGNode<T>(child));
			++totalNodes;
		}

		private DAGNode<T> FindNode(T node)
		{
			return root.FindNode(node);
		}

		private void CheckChildIsNotItsOwnParent(DAGNode<T> parentNode, T child)
		{
			var currentNode = parentNode;
			while (currentNode != null)
			{
				if (currentNode.NodeValue.Equals(child))
					throw new CyclicDependencyException("The node " + child + " is being inserted in line with itself");
				currentNode = currentNode.ParentNode;
			}
		}

		public void Validate()
		{
			if (root == null || Count == 1)
				return;
			root.Validate();
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

	class DAGNode<T>
	{
		public DAGNode(T nodeValue, List<DAGNode<T>> children = null)
		{
			NodeValue = nodeValue;
			Children = children ?? new List<DAGNode<T>>();
			foreach (var child in Children)
				child.ParentNode = this;
		}

		public T NodeValue { get; private set; }
		public DAGNode<T> ParentNode { get; private set; }

		public DAGNode<T> FindNode(T searchValue)
		{
			if (this.NodeValue.Equals(searchValue))
				return this;
			foreach (var child in this.Children)
			{
				DAGNode<T> node = child.FindNode(searchValue);
				if (node != null)
					return node;
			}
			return null;
		}

		private void Validate(Stack<T> pathStack)
		{
			if (pathStack.Contains(this.NodeValue))
				throw new CyclicDependencyException("Found a cyclic reference with " + pathStack.ToString());

			pathStack.Push(this.NodeValue);
			foreach (var child in this.Children)
			{
				child.Validate(pathStack);
			}
			pathStack.Pop();
		}

		public void Validate()
		{
			this.Validate(new Stack<T>());
		}


		public List<DAGNode<T>> Children { get; private set; }

		public void AddChild(DAGNode<T> node)
		{
			node.ParentNode = this;
			Children.Add(node);
		}
	}
}
