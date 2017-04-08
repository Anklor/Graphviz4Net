
namespace Graphviz4Net.WPF.ViewModels
{
    using Graphs;

    public class EdgeArrowViewModel
    {
        public EdgeArrowViewModel(IEdge edge, object arrow)
        {
            Edge = edge;
            Arrow = arrow;
        }

        public IEdge Edge { get; private set; }

        public object Arrow { get; private set; }
    }
}
