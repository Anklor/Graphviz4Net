using System.Windows;



namespace Graphviz4Net.WPF.Example
{   
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            viewModel = new MainWindowViewModel();
            DataContext = viewModel;
            InitializeComponent();

            UpdatePerson.Click += UpdatePersonClick;            
            AddNewPerson.Click += AddNewPersonClick;
            AddNewEdge.Click += AddNewEdgeClick;
        }

		void UpdatePersonClick(object sender, RoutedEventArgs e)
		{
            viewModel.UpdatePersonName = (string)UpdatePersonName.SelectedItem;
            viewModel.UpdatePerson();
		}

        private void AddNewPersonClick(object sender, RoutedEventArgs e)
        {
            viewModel.CreatePerson();
        }

        private void AddNewEdgeClick(object sender, RoutedEventArgs e)
        {
            viewModel.NewEdgeStart = (string)NewEdgeStart.SelectedItem;
            viewModel.NewEdgeEnd = (string)NewEdgeEnd.SelectedItem;
            viewModel.CreateEdge();
        }        
    }
}
