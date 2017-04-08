using System.Collections.Generic;
using System.Linq;

namespace Graphviz4Net.WPF.Example
{
    using Entities;
    using Graphs;
    using System.ComponentModel;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            Graph<Person> graph = new Graph<Person>();
            Person a = new Person(graph) { Name = "John", Avatar = "./Avatars/avatar1.jpg" };
            Person b = new Person(graph) { Name = "Michael", Avatar = "./Avatars/avatar2.gif" };
            Person c = new Person(graph) { Name = "Kenny" };
            Person d = new Person(graph) { Name = "Lisa" };
            Person e = new Person(graph) { Name = "Lucy", Avatar = "./Avatars/avatar3.jpg" };
            Person f = new Person(graph) { Name = "Ted Mossy" };
            Person g = new Person(graph) { Name = "Glen" };
            Person h = new Person(graph) { Name = "Alice", Avatar = "./Avatars/avatar1.jpg" };

            graph.AddVertex(a);
            graph.AddVertex(b);
            graph.AddVertex(c);
            graph.AddVertex(d);
            graph.AddVertex(e);
            graph.AddVertex(f);

            SubGraph<Person> subGraph = new SubGraph<Person> { Label = "Work" };
            graph.AddSubGraph(subGraph);
            subGraph.AddVertex(g);
            subGraph.AddVertex(h);
            graph.AddEdge(new Edge<Person>(g, h));
            graph.AddEdge(new Edge<Person>(a, g));
            SubGraph<Person> subGraph2 = new SubGraph<Person> {Label = "School"};
            graph.AddSubGraph(subGraph2);
            Person loner = new Person(graph) { Name = "Loner", Avatar = "./Avatars/avatar1.jpg" };
            subGraph2.AddVertex(loner);
            graph.AddEdge(new Edge<SubGraph<Person>>(subGraph, subGraph2) { Label = "Link between groups" } );
            graph.AddEdge(new Edge<Person>(c, d) { Label = "In love", DestinationArrowLabel = "boyfriend", SourceArrowLabel = "girlfriend" });
            graph.AddEdge(new Edge<Person>(c, g, new Arrow(), new Arrow()));
            graph.AddEdge(new Edge<Person>(c, a, new Arrow()) { Label = "Boss" });
            graph.AddEdge(new Edge<Person>(d, h, new DiamondArrow(), new DiamondArrow()));
            graph.AddEdge(new Edge<Person>(f, h, new DiamondArrow(), new DiamondArrow()));
            graph.AddEdge(new Edge<Person>(f, loner, new DiamondArrow(), new DiamondArrow()));
            graph.AddEdge(new Edge<Person>(f, b, new DiamondArrow(), new DiamondArrow()));
            graph.AddEdge(new Edge<Person>(e, g, new Arrow(), new Arrow()) { Label = "Siblings" });
            Graph = graph;
            Graph.Changed += GraphChanged;
            NewPersonName = "Enter new name";
            UpdatePersonNewName = "Enter new name";
        }

        /// <summary>
        /// 
        /// </summary>
        public Graph<Person> Graph { get; set; } = new Graph<Person>();

        
        private LayoutEngine _layoutEngine = LayoutEngine.Dot;
        /// <summary>
        /// 
        /// </summary>
        public LayoutEngine LayoutEngine
        {
            get { return _layoutEngine; }
            set
            {
                _layoutEngine = value;
                RaisePropertyChanged("LayoutEngine");
            }
        }

        public string NewPersonName { get; set; }

		public string UpdatePersonName { get; set; }

		public string UpdatePersonNewName { get; set; }

        public IEnumerable<string> PersonNames => Graph.AllVertices.Select(x => x.Name); 
        
        public string NewEdgeStart { get; set; }

        public string NewEdgeEnd { get; set; }

        public string NewEdgeLabel { get; set; }

        public void CreateEdge()
        {
            if (string.IsNullOrWhiteSpace(NewEdgeStart) || string.IsNullOrWhiteSpace(NewEdgeEnd))
                return;

            Graph.AddEdge(new Edge<Person>(GetPerson(NewEdgeStart), GetPerson(NewEdgeEnd))
            {
                Label = NewEdgeLabel
            });
        }

        public void CreatePerson()
        {
            // such a person already exists: there should be some validation message, but  it is not so important in a demo
            if (PersonNames.Any(x => x == NewPersonName))
                return;

            Graph.AddVertex(new Person(Graph) { Name = NewPersonName });
        }

		public void UpdatePerson()
		{
			if (string.IsNullOrWhiteSpace(UpdatePersonName)) 
				return;

            GetPerson(UpdatePersonName).Name = UpdatePersonNewName;
            RaisePropertyChanged("PersonNames");
            RaisePropertyChanged("Graph");
		}

        public event PropertyChangedEventHandler PropertyChanged;

        private void GraphChanged(object sender, GraphChangedArgs e)
        {
            RaisePropertyChanged("PersonNames");
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private Person GetPerson(string name) => Graph.AllVertices.First(x => string.CompareOrdinal(x.Name, name) == 0);        
    }
}
