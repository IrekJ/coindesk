
namespace BitcoinDesk.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		public LiveDataViewModel LiveDataViewModel { get; }
		public SavedDataViewModel SavedDataViewModel { get; }

		public MainViewModel(BitcoinPriceRepository repository)
		{
			SavedDataViewModel = new SavedDataViewModel(repository);
			LiveDataViewModel = new LiveDataViewModel(repository, SavedDataViewModel);
		}
	}
}
