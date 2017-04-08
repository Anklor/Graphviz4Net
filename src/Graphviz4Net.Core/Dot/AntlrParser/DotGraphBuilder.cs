
namespace Graphviz4Net.Dot.AntlrParser
{
    using System.Collections.Generic;

    public interface IDotGraphBuilder
    {
        void EnterSubGraph(string name);

        void LeaveSubGraph();

        void AddEdge(string sourceStr, string targetStr, IDictionary<string, string> attributes);

        void AddVertex(string idStr, IDictionary<string, string> attributes);

        void AddGraphAttributes(IDictionary<string, string> attributes);
    }

    /// <summary>
    /// The implementor must initialize <see cref="DotGraph"/> in its constructor!
    /// </summary>
    public abstract class DotGraphBuilder<TVertexId> : IDotGraphBuilder
    {
        private DotSubGraph<TVertexId> subGraph;

        public DotGraph<TVertexId> DotGraph { get; protected set; }

        public void AddGraphAttributes(IDictionary<string, string> attributes)
        {
            if (attributes == null)
            {
                return;
            }

            foreach (var attribute in attributes)
            {
                if (subGraph == null)
                {
                    DotGraph.Attributes.Add(attribute);
                }
                else
                {
                    subGraph.Attributes.Add(attribute);
                }
            }
        }

        public void EnterSubGraph(string name)
        {
            subGraph = new DotSubGraph<TVertexId> {Name = name};
            DotGraph.AddSubGraph(subGraph);
        }

        public void LeaveSubGraph()
        {
            subGraph = null;
        }

        public void AddEdge(string sourceStr, string targetStr, IDictionary<string, string> attributes)
        {
            var source = GetVertex(sourceStr);
            var target = GetVertex(targetStr);
            DotGraph.AddEdge(new DotEdge<TVertexId>(source, target, attributes));
        }

        public void AddVertex(string idStr, IDictionary<string, string> attributes)
        {
            var vertex = CreateVertex(idStr, attributes);

            if (subGraph == null)
            {
                DotGraph.AddVertex(vertex);
            }
            else
            {
                subGraph.AddVertex(vertex);
            }
        }

        protected abstract DotVertex<TVertexId> CreateVertex(string idStr, IDictionary<string, string> attributes);

        protected abstract DotVertex<TVertexId> GetVertex(string idStr);
    }
}
