
namespace Graphviz4Net.Tests.Graphs
{
    using System.Linq;
    using NUnit.Framework;
    using Graphviz4Net.Graphs;

    [TestFixture]
    public class GraphRemoveVertexWithEdgesTests
    {
        private Graph<string> graph;

        [SetUp]
        public void SetUp()
        {
            graph = new Graph<string>();
            graph.AddVertex("a");
            graph.AddVertex("b");
            var subGraph = new SubGraph<string>();
            graph.AddSubGraph(subGraph);
            subGraph.AddVertex("c");

            graph.AddEdge(new Edge<string>("a", "b"));
            graph.AddEdge(new Edge<string>("b", "c"));
        }

        [Test]
        public void WhenRemoveNodeAEdgeFromAToBIsRemoved()
        {
            graph.RemoveVertexWithEdges("a");
            Assert.AreEqual(1, graph.Vertices.Count());
            Assert.AreEqual("b", graph.Vertices.First());
            Assert.AreEqual(1, graph.Edges.Count());
            Assert.AreEqual("b", graph.Edges.First().Source);
        }

        [Test]
        public void WhenRemoveNodeCEdgeFromBToCIsRemoved()
        {
            graph.RemoveVertexWithEdges("c");
            Assert.AreEqual(2, graph.Vertices.Count());
            Assert.AreEqual(0, graph.SubGraphs.First().Vertices.Count());
            Assert.AreEqual(1, graph.Edges.Count());
            Assert.AreEqual("a", graph.Edges.First().Source);
        }
    }
}
