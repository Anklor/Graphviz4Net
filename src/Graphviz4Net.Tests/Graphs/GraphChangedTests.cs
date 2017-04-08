
namespace Graphviz4Net.Tests.Graphs
{
    using NUnit.Framework;
    using Graphviz4Net.Graphs;

    [TestFixture]
    public class GraphChangedTests
    {
        private Graph<Model> graph;

        private int graphChangedCalled;

        [SetUp]
        public void SetUp()
        {
            graph = new Graph<Model>();
            graph.Changed += GraphChanged;
            graphChangedCalled = 0;
        }

        [Test]
        public void ChangedFiredAfterAddVertex()
        {
            graph.AddVertex(new Model());
            Assert.AreEqual(1, graphChangedCalled);
        }

        [Test]
        public void ChangedFiredAfterAddSubGraph()
        {
            graph.AddSubGraph(new SubGraph<Model>());
            Assert.AreEqual(1, graphChangedCalled);
        }

        [Test]
        public void ChangedFiredAfterAddVertexToSubGraph()
        {
            var subgraph = new SubGraph<Model>();
            graph.AddSubGraph(subgraph);
            graphChangedCalled = 0;
            subgraph.AddVertex(new Model());
            Assert.AreEqual(1, graphChangedCalled);
        }

        [Test]
        public void ChangedFiredAfterRemovalOfVertexWithEdges()
        {
            var model = new Model();
            graph.AddVertex(model);
            graphChangedCalled = 0;
            graph.RemoveVertexWithEdges(model);
            Assert.AreEqual(1, graphChangedCalled);            
        }

        private void GraphChanged(object sender, GraphChangedArgs e)
        {
            graphChangedCalled++;
        }        

        public class Model
        {
        }
    }
}
