using System.Windows;

namespace BitcoinDesk
{
	public partial class StatusMessageDialog : Window
	{
		public StatusMessageDialog(string message)
		{
			InitializeComponent();
			MessageTextBlock.Text = message;
		}

		private void OnOkButtonClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
