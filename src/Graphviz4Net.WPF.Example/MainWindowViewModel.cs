using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Graphviz4Net.WPF.Example
{
    using Graphs;
    using System.ComponentModel;

    public class Person : INotifyPropertyChanged
    {
        private readonly Graph<Person> graph;

        public Person(Graph<Person> graph)
        {
            this.graph = graph;
            Avatar = "./Avatars/avatarAnon.gif";
        }

    	private string name;
    	public string Name
    	{
    		get { return name; }
    		set
    		{
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
    	}

    	public string Avatar { get; set; }

        public string Email => Name.ToLower().Replace(' ', '.') + "@gmail.com";


        public ICommand RemoveCommand => new RemoveCommandImpl(this); 


        private class RemoveCommandImpl : ICommand
        {
            private Person person;

            public RemoveCommandImpl(Person person)
            {
                this.person = person;
            }

            public void Execute(object parameter)
            {
                person.graph.RemoveVertexWithEdges(person);
            }

            public bool CanExecute(object parameter) => true;
            

            public event EventHandler CanExecuteChanged;
        }

    	public event PropertyChangedEventHandler PropertyChanged;
    }

    public class DiamondArrow
    {
    }

    public class Arrow
    {        
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            var graph = new Graph<Person>();
            var a = new Person(graph) { Name = "Jonh", Avatar = "./Avatars/avatar1.jpg" };
            var b = new Person(graph) { Name = "Michael", Avatar = "./Avatars/avatar2.gif" };
            var c = new Person(graph) { Name = "Kenny" };
            var d = new Person(graph) { Name = "Lisa" };
            var e = new Person(graph) { Name = "Lucy", Avatar = "./Avatars/avatar3.jpg" };
            var f = new Person(graph) { Name = "Ted Mosby" };
            var g = new Person(graph) { Name = "Glen" };
            var h = new Person(graph) { Name = "Alice", Avatar = "./Avatars/avatar1.jpg" };

            graph.AddVertex(a);
            graph.AddVertex(b);
            graph.AddVertex(c);
            graph.AddVertex(d);
            graph.AddVertex(e);
            graph.AddVertex(f);

            var subGraph = new SubGraph<Person> { Label = "Work" };
            graph.AddSubGraph(subGraph);
            subGraph.AddVertex(g);
            subGraph.AddVertex(h);
            graph.AddEdge(new Edge<Person>(g, h));
            graph.AddEdge(new Edge<Person>(a, g));

            var subGraph2 = new SubGraph<Person> {Label = "School"};
            graph.AddSubGraph(subGraph2);
            var loner = new Person(graph) { Name = "Loner", Avatar = "./Avatars/avatar1.jpg" };
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

        public Graph<Person> Graph { get; private set; }

        private LayoutEngine _layoutEngine = LayoutEngine.Dot;
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
            if (PersonNames.Any(x => x == NewPersonName))
            {
                // such a person already exists: there should be some validation message, but 
                // it is not so important in a demo
                return;
            }

            var p = new Person(Graph) { Name = NewPersonName };
            Graph.AddVertex(p);
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
