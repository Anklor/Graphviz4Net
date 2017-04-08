
namespace Graphviz4Net.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;

    public class Graph<TVertex, TSubGraph, TVeticesEdge, TSubGraphsEdge> : IGraph, IAttributed, IVerticesCollection<TVertex> 
        where TVeticesEdge : IEdge<TVertex> 
        where TSubGraphsEdge: IEdge<TSubGraph>
        where TSubGraph : ISubGraph<TVertex>
    {
        private readonly IList<IEdge> edges = new List<IEdge>();

        private readonly IList<TVertex> vertices = new List<TVertex>();

        private readonly IList<TSubGraph> subGraphs = new List<TSubGraph>();

        private readonly IDictionary<string, string> attributes = new Dictionary<string, string>();

        private int startChangesCalled = 0;

        #region Properties

        /// <summary>
        /// This collection is not specialized, because edges might connect either nodes or sub-graphs, 
        /// therefore we cannot choose between <c>TVerticesEdge</c> or <c>TSubgraphsEdge</c>. 
        /// If you want 'type safe' access use <see cref="VerticesEdges"/> or <see cref="SubGraphsEdges"/>.
        /// </summary>
        public IEnumerable<IEdge> Edges
        {
            get { return edges; }
        }

        public IEnumerable<TVertex> Vertices
        {
            get { return vertices; }
        }

        public IEnumerable<TSubGraph> SubGraphs
        {
            get { return subGraphs; }
        }

        public IEnumerable<TVeticesEdge> VerticesEdges
        {
            get { return Edges.OfType<TVeticesEdge>(); }
        }

        public IEnumerable<TSubGraphsEdge> SubGraphsEdges
        {
            get { return Edges.OfType<TSubGraphsEdge>(); }
        }

        public event EventHandler<GraphChangedArgs> Changed;

        /// <summary>
        /// Gets the height/width ratio that should be fulfilled when generating the layout.
        /// </summary>
        public double? Ratio
        {
            get { return Utils.ParseInvariantNullableDouble(Attributes.GetValue("ratio")); }
            set { Attributes["ratio"] = value.ToInvariantString(); }
        }

        /// <summary>
        /// Gets the vertices on the top level and all the vertices from subgraphs.
        /// </summary>
        public IEnumerable<TVertex> AllVertices
        {
            get
            {
                return vertices.Concat(subGraphs.SelectMany(x => x.Vertices));
            }
        }

        public IDictionary<string, string> Attributes
        {
            get { return attributes; }
        }

        public RankDirection Rankdir
        {
            get { return RankDirection.FromString(attributes.GetValue("rankdir", "LR")); }
            set { attributes["rankdir"] = value.ToString(); }
        }

        #endregion

        #region Add operations

        public void AddEdge(TVeticesEdge edge)
        {
            Contract.Requires(edge != null);
            Contract.Requires(AllVertices.Contains(edge.Source), "Edge's source does not belong to the graph.");
            Contract.Requires(AllVertices.Contains(edge.Destination), "Edge's source does not belong to the graph.");
            edges.Add(edge);
            RaiseChanged();
        }

        public void AddEdge(TSubGraphsEdge edge)
        {
            Contract.Requires(edge != null);
            Contract.Requires(SubGraphs.Contains(edge.Source), "Edge's source does not belong to the graph.");
            Contract.Requires(SubGraphs.Contains(edge.Destination), "Edge's source does not belong to the graph.");
            edges.Add(edge);
            RaiseChanged();
        }

        public void AddVertex(TVertex vertex)
        {
            Contract.Requires(vertex != null);
            Contract.Requires(AllVertices.Contains(vertex) == false, "Vertex is already in the graph.");
            vertices.Add(vertex);
            RaiseChanged();
        }

        public void AddSubGraph(TSubGraph subGraph)
        {
            Contract.Requires(subGraph != null);
            subGraphs.Add(subGraph);
            RaiseChanged();
            subGraph.Changed += SubGraphChanged;
        }

        #endregion

        /// <summary>
        /// Removes a vertex from this graph. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// It cannot remove vertex from sub-graphs, for this 
        /// one has to invoke specific method on sub-graph, if the sub-graph supports it.
        /// </para>
        /// <para>
        /// To remove a vertex from sub-graph one has invoke RemoveVertex on that particular sub-graph. 
        /// Class <see cref="Graph{TVertex,TEdge}"/> provides method <see cref="Graph{TVertex,TEdge}.RemoveVertexWithEdges"/>, 
        /// which is capable of removing a vertex from a sub-graph and which also removes all edges that contain the vertex.
        /// </para>
        /// </remarks>
        /// <param name="vertex">Vertex to be removed.</param>
        public void RemoveVertex(TVertex vertex)
        {
            Contract.Requires(vertex != null);
            Contract.Requires(
                Vertices.Contains(vertex),
                "RemoveVertex: given vertex is not part of the graph. See the API documentation for more details.");
            vertices.Remove(vertex);
            RaiseChanged();
        }

        public void RemoveEdge(IEdge edge)
        {
            Contract.Requires(edge != null);
            Contract.Requires(edges.Contains(edge), "RemoveEdge: given edge is not part of the graph");
            edges.Remove(edge);
            RaiseChanged();
        }

        public void RemoveSubGraph(TSubGraph subGraph)
        {
            Contract.Requires(subGraph != null);
            Contract.Requires(subGraphs.Contains(subGraph), "RemoveSubGraph: given subgraph is not part of the graph.");
            subGraphs.Remove(subGraph);
            RaiseChanged();
        }

        #region Explicit IGraph implementation

        IEnumerable<object> IGraph.Vertices
        {
            get
            {
                // Note: we use Cast<object> instead of casting to IEnumerable<object>, because 
                // silverlight seems not to support the former
                return vertices.Cast<object>();
            }
        }

        IEnumerable<ISubGraph> IGraph.SubGraphs
        {
            get { return subGraphs.Cast<ISubGraph>(); }
        }

        #endregion

        #region Explicit IVerticesCollection implementation

        IEnumerable<object> IVerticesCollection.Vertices
        {
            get { return vertices.Cast<object>(); }
        }

        void IVerticesCollection.AddVertex(object vertex)
        {
            if (vertex is TVertex == false)
            {
                throw new NotSupportedException(
                    string.Format(
                        "AddVertex(object) called with {0}, but this graph is generic and supports only vertices of type {1}",
                        vertex != null ? vertex.GetType().Name : "null",
                        typeof (TVertex)));

            }

            AddVertex((TVertex) vertex);
        }

        #endregion

        public override string ToString()
        {
            var result = new StringBuilder("graph: ");
            foreach (var vertex in Vertices)
            {
                result.AppendLine(vertex.ToString());
            }

            foreach (var subGraph in SubGraphs)
            {
                result.Append(subGraph);
                result.AppendLine();
            }

            foreach (var edge in Edges)
            {
                result.AppendLine(edge.ToString());
            }

            return result.ToString();
        }

        protected void StartChanges()
        {
            startChangesCalled++;
        }

        protected void EndChanges()
        {
            if (startChangesCalled == 0)
            {
                throw new InvalidOperationException("Cannot call EndChanges before StartChanges.");
            }

            startChangesCalled--;
            if (startChangesCalled == 0)
            {
                RaiseChanged();
            }
        }

        protected void RaiseChanged()
        {
            if (Changed != null && startChangesCalled == 0)
            {
                Changed(this, new GraphChangedArgs());
            }
        }

        private void SubGraphChanged(object sender, GraphChangedArgs e)
        {
            RaiseChanged();
        }
    }

    public class Graph<TVertex, TEdge> : Graph<TVertex, SubGraph<TVertex>, TEdge, Edge<SubGraph<TVertex>>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Removes given vertex and all related edges.
        /// </summary>
        /// <remarks>Edges with <paramref name="vertex"/> as <see cref="IEdge.Source"/> or <see cref="IEdge.Destination"/>
        /// will be removed as well. 
        /// This operation performs several searches in list of vertices, therefore, it may be quiet slow.</remarks>
        /// <param name="vertex">The vertex to be removed.</param>
        public void RemoveVertexWithEdges(TVertex vertex)
        {
            Contract.Requires(vertex != null);
            StartChanges();
            bool found = false;

            if (Vertices.Contains(vertex))
            {
                RemoveVertex(vertex);
                found = true;
            }
            else
            {
                foreach (var subGraph in SubGraphs)
                {
                    if (subGraph.Vertices.Contains(vertex))
                    {
                        subGraph.RemoveVertex(vertex);
                        found = true;
                    }
                }
            }

            if (found)
            {
                RemoveEdgesWith(vertex);
                EndChanges();
                return;
            }

            throw new ArgumentException(
                "RemoveVertexWithEdges: given vertex is not part of the graph nor part of any of it's subgraphs.");
        }

        private void RemoveEdgesWith(TVertex vertex)
        {
            var edgesToRemove = Edges.Where(e => e.Source.Equals(vertex) || e.Destination.Equals(vertex)).ToArray();
            foreach (var e in edgesToRemove)
            {
                RemoveEdge(e);
            }
        }
    }

    public class Graph<TVertex> : Graph<TVertex, Edge<TVertex>>
    {
    }
}
