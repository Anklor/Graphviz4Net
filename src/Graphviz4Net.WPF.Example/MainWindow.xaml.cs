
namespace Graphviz4Net.WPF.Example
{
    using System.Windows;

    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            viewModel = new MainWindowViewModel();
            DataContext = viewModel;
            InitializeComponent();
            this.AddNewEdge.Click += AddNewEdgeClick;
            this.AddNewPerson.Click += AddNewPersonClick;
			this.UpdatePerson.Click += UpdatePersonClick;
        }

		void UpdatePersonClick(object sender, RoutedEventArgs e)
		{
            viewModel.UpdatePersonName = (string) this.UpdatePersonName.SelectedItem;
            viewModel.UpdatePerson();
		}

        private void AddNewPersonClick(object sender, RoutedEventArgs e)
        {
            viewModel.CreatePerson();
        }

        private void AddNewEdgeClick(object sender, RoutedEventArgs e)
        {
            viewModel.NewEdgeStart = (string) this.NewEdgeStart.SelectedItem;
            viewModel.NewEdgeEnd = (string)this.NewEdgeEnd.SelectedItem;
            viewModel.CreateEdge();
        }        
    }
}
