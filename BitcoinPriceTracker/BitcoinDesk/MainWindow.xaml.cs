using System.Windows;
using BitcoinDesk.ViewModels;

namespace BitcoinDesk
{
	public partial class MainWindow : Window
	{
		private readonly string connectionString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=coinDesk;Integrated Security=True;";


		public MainWindow()
		{
			InitializeComponent();

			var repository = new BitcoinPriceRepository(connectionString);
			var viewModel = new MainViewModel(repository);
			this.DataContext = viewModel;
		}
	}
}
