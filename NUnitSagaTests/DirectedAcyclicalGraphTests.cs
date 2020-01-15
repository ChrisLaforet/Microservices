using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SagaBroker.Exception;
using SagaBroker.GraphLibrary;

namespace NUnitSagaTests
{
	public class DirectedAcylicalGraphTests
	{
		[Test]
		public void WhenCreatingAGraph_WithoutNodes_ReturnsCountOf0AndIsEmpty()
		{
			DirectedAcyclicalGraph<string> graph = new DirectedAcyclicalGraph<string>();
			Assert.IsTrue(graph.IsEmpty);
			Assert.AreEqual(0, graph.Count);
		}

		[Test]
		public void WhenCreatingAGraph_With1Node_ReturnsCountOf1AndNotIsEmpty()
		{
			DirectedAcyclicalGraph<string> graph = new DirectedAcyclicalGraph<string>();
			graph.Add("A");
			Assert.IsFalse(graph.IsEmpty);
			Assert.AreEqual(1, graph.Count);
		}

		[Test]
		public void WhenCreatingAGraph_With2RootNodes_ThrowsException()
		{
			DirectedAcyclicalGraph<string> graph = new DirectedAcyclicalGraph<string>();
			graph.Add("A");
			Assert.Throws<GraphRootAlreadyExistsException>(delegate { graph.Add("A"); });
		}

		[Test]
		public void WhenCreatingAGraph_With2RelatedNodes_ReturnsCountOf2()
		{
			DirectedAcyclicalGraph<string> graph = new DirectedAcyclicalGraph<string>();
			graph.Add("A");
			graph.AddChild("B", "A");
			Assert.AreEqual(2, graph.Count);
		}

		[Test]
		public void WhenCreatingAGraph_With2UnrelatedNodes_ThrowsException()
		{
			DirectedAcyclicalGraph<string> graph = new DirectedAcyclicalGraph<string>();
			graph.Add("A");
			
			Assert.Throws<GraphNodeMissingException>(delegate { graph.AddChild("B", "C"); });
		}

		[Test]
		public void WhenCreatingAGraph_With2NodesOffRoot_ReturnsCountOf3()
		{
			DirectedAcyclicalGraph<string> graph = new DirectedAcyclicalGraph<string>();
			graph.Add("A");
			graph.AddChild("B", "A");
			graph.AddChild("C", "A");
			Assert.AreEqual(3, graph.Count);
		}

		[Test]
		public void WhenCreatingAGraph_WithNoCycle_ValidateReturnsSuccessfully()
		{
			DirectedAcyclicalGraph<string> graph = new DirectedAcyclicalGraph<string>();
			graph.Add("A");
			graph.AddChild("B", "A");
			graph.AddChild("C", "B");
			graph.AddChild("D", "B");
			graph.AddChild("E", "D");
			graph.AddChild("F", "E");
			graph.AddChild("X", "A");
			graph.AddChild("Y", "X");
			graph.AddChild("Z", "Y");
			graph.Validate();
			Assert.Pass();
		}

		[Test]
		public void WhenCreatingAGraph_WithCycle_ValidateThrowsException()
		{
			DirectedAcyclicalGraph<string> graph = new DirectedAcyclicalGraph<string>();
			graph.Add("A");
			graph.AddChild("B", "A");
			graph.AddChild("C", "B");
			graph.AddChild("D", "B");
			graph.AddChild("E", "D");
			Assert.Throws<CyclicDependencyException>(delegate { graph.AddChild("B", "E"); });
			//graph.AddChild("X", "A");
			//graph.AddChild("Y", "X");
			//graph.AddChild("Z", "Y");
			//Assert.Throws<CyclicDependencyException>(delegate { graph.Validate(); });
		}
	}
}
