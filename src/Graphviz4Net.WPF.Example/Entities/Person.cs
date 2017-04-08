using Graphviz4Net.Graphs;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Graphviz4Net.WPF.Example.Entities
{
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
}
