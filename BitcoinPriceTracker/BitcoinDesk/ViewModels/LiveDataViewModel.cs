using BitcoinDesk.Helpers;
using BitcoinDesk.Models;
using BitcoinDesk.Services;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;

namespace BitcoinDesk.ViewModels
{
	public class LiveDataViewModel : ViewModelBase
	{
		private readonly BitcoinPriceRepository _repository;
		private readonly SavedDataViewModel _savedDataViewModel;
		private readonly DispatcherTimer _timer;
		private decimal _currentPriceCZK;
		private string _statusMessage;
		private bool _isLoading;

		public ObservableCollection<BitcoinPrice> LiveData { get; set; }

		public decimal CurrentPriceCZK
		{
			get => _currentPriceCZK;
			set
			{
				_currentPriceCZK = value;
				OnPropertyChanged(nameof(CurrentPriceCZK));
			}
		}

		public string StatusMessage
		{
			get => _statusMessage;
			set
			{
				_statusMessage = value;
				OnPropertyChanged(nameof(StatusMessage));
			}
		}

		public bool IsLoading
		{
			get => _isLoading;
			set
			{
				_isLoading = value;
				OnPropertyChanged(nameof(IsLoading));
			}
		}

		public ICommand RefreshDataCommand { get; }
		public ICommand SaveDataCommand { get; }

		public LiveDataViewModel(BitcoinPriceRepository repository, SavedDataViewModel savedDataViewModel)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_savedDataViewModel = savedDataViewModel ?? throw new ArgumentNullException(nameof(savedDataViewModel));
			LiveData = new ObservableCollection<BitcoinPrice>();

			RefreshDataCommand = new RelayCommand(async () => await RefreshData());
			SaveDataCommand = new RelayCommand(async () => await SaveData());

			var configFilePath = "config.json";
			var configData = File.ReadAllText(configFilePath);
			dynamic config = JsonConvert.DeserializeObject(configData);
			int refreshIntervalInSeconds = config?.Settings?.UpdateIntervalSeconds ?? 60;

			_timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(refreshIntervalInSeconds)
			};
			_timer.Tick += async (s, e) => await RefreshData();
			_timer.Start();

			IsLoading = false;
		}

		private async Task RefreshData()
		{
			try
			{
				IsLoading = true;
				StatusMessage = "Fetching data...";

				// Get price and exchange rate from ApiService
				var apiService = new ApiService();
				decimal priceEur = await apiService.GetBitcoinPriceEURAsync();
				decimal exchangeRate = await apiService.GetExchangeRateEURToCZKAsync();
				CurrentPriceCZK = priceEur * exchangeRate;

				var newRecord = new BitcoinPrice
				{
					Timestamp = DateTime.Now,
					PriceCZK = CurrentPriceCZK
				};
				LiveData.Insert(0, newRecord);

				StatusMessage = "Data fetched successfully.";
			}
			catch (Exception ex)
			{
				StatusMessage = $"Error fetching data: {ex.Message}";
			}
			finally
			{
				IsLoading = false;
			}
		}

		private async Task SaveData()
		{
			try
			{
				StatusMessage = "Saving data...";

				foreach (var record in LiveData)
				{
					await _repository.InsertBitcoinPriceAsync(record.PriceCZK, record.Note ?? "Auto-saved");
				}

				LiveData.Clear();

				StatusMessage = "All data saved successfully.";

				await _savedDataViewModel.LoadSavedDataAsync();
			}
			catch (Exception ex)
			{
				StatusMessage = $"Error saving data: {ex.Message}";
			}
		}
	}
}
