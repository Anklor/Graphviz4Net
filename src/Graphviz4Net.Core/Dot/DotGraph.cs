
namespace Graphviz4Net.Dot
{
    using Graphs;

    public class DotGraph<TVertexId> : 
        Graph<DotVertex<TVertexId>, DotSubGraph<TVertexId>, DotEdge<TVertexId>, Edge<DotSubGraph<TVertexId>>> 
    {
        private BoundingBox bondingBox = null;

        public double? Width
        {
            get { return BoundingBox.RightX - BoundingBox.LeftX; }
        }

        public double? Height
        {
            get { return BoundingBox.UpperY - bondingBox.LowerY; }
        }

        public BoundingBox BoundingBox
        {
            get
            {
                string newBb;
                Attributes.TryGetValue("bb", out newBb);
                if (bondingBox == null || bondingBox.Equals(newBb) == false)
                {
                    bondingBox = new BoundingBox(newBb);
                }

                return bondingBox;
            }
        }       
    }
}
