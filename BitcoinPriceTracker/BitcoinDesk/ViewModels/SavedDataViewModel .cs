using BitcoinDesk;
using BitcoinDesk.Helpers;
using BitcoinDesk.Models;
using BitcoinDesk.ViewModels;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.Windows.Input;

public class SavedDataViewModel : ViewModelBase
{
	private readonly BitcoinPriceRepository _repository;
	private string _statusMessage;
	private bool _isLoading;
	private PlotModel _savedDataPlotModel;

	public ObservableCollection<BitcoinPrice> SavedData { get; set; }

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

	public PlotModel SavedDataPlotModel
	{
		get => _savedDataPlotModel;
		set
		{
			_savedDataPlotModel = value;
			OnPropertyChanged(nameof(SavedDataPlotModel));
		}
	}

	public ICommand LoadSavedDataCommand { get; }
	public ICommand DeleteDataCommand { get; }
	public ICommand UpdateDataCommand { get; }

	public SavedDataViewModel(BitcoinPriceRepository repository)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
		SavedData = new ObservableCollection<BitcoinPrice>();

		LoadSavedDataCommand = new RelayCommand(async () => await LoadSavedDataAsync());
		DeleteDataCommand = new RelayCommand(async () => await DeleteDataAsync());
		UpdateDataCommand = new RelayCommand(async () => await UpdateDataAsync());

		SavedDataPlotModel = new PlotModel { Title = "Bitcoin Price History" };
		InitializePlot();
	}

	private void InitializePlot()
	{
		SavedDataPlotModel.Axes.Clear();
		SavedDataPlotModel.Axes.Add(new DateTimeAxis
		{
			Position = AxisPosition.Bottom,
			StringFormat = "MM/dd HH:mm",
			Title = "Timestamp",
			IntervalType = DateTimeIntervalType.Hours,
			MajorGridlineStyle = LineStyle.Solid,
			MinorGridlineStyle = LineStyle.Dot
		});

		SavedDataPlotModel.Axes.Add(new LinearAxis
		{
			Position = AxisPosition.Left,
			Title = "Price (CZK)",
			MajorGridlineStyle = LineStyle.Solid,
			MinorGridlineStyle = LineStyle.Dot
		});

		UpdatePlotData();
	}

	private void UpdatePlotData()
	{
		var series = new LineSeries
		{
			Title = "BTC to CZK",
			StrokeThickness = 2,
			MarkerSize = 3,
			MarkerType = MarkerType.Circle,
			Color = OxyColors.SkyBlue
		};

		foreach (var item in SavedData)
		{
			series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(item.Timestamp), (double)item.PriceCZK));
		}

		SavedDataPlotModel.Series.Clear();
		SavedDataPlotModel.Series.Add(series);
		SavedDataPlotModel.InvalidatePlot(true);
	}

	public async Task LoadSavedDataAsync()
	{
		try
		{
			IsLoading = true;
			StatusMessage = "Loading saved data...";

			var savedRecords = await _repository.GetAllBitcoinPricesAsync();
			SavedData.Clear();

			foreach (var record in savedRecords)
			{
				SavedData.Add(record);
			}

			UpdatePlotData(); 
			StatusMessage = "Saved data loaded successfully.";
		}
		catch (Exception ex)
		{
			StatusMessage = $"Error loading saved data: {ex.Message}";
		}
		finally
		{
			IsLoading = false;
		}
	}

	public async Task DeleteDataAsync()
	{
		var selectedRecords = SavedData.Where(record => record.IsSelected).ToList();

		if (selectedRecords.Any())
		{
			try
			{
				foreach (var record in selectedRecords)
				{
					await _repository.DeleteBitcoinPriceAsync(record.ID);
					SavedData.Remove(record);
				}

				UpdatePlotData(); 
				StatusMessage = "Selected data deleted successfully.";
				ShowStatusMessageDialog(StatusMessage);
			}
			catch (Exception ex)
			{
				StatusMessage = $"Error deleting data: {ex.Message}";
				ShowStatusMessageDialog(StatusMessage);
			}
		}
		else
		{
			StatusMessage = "Please select records to delete.";
			ShowStatusMessageDialog(StatusMessage);
		}
	}

	public async Task UpdateDataAsync()
	{
		try
		{
			StatusMessage = "Saving changes...";

			var updatedRecords = SavedData.Where(record => record.IsSelected).ToList();

			if (updatedRecords.Any())
			{
				foreach (var record in updatedRecords)
				{
					await _repository.UpdateNoteAsync(record);
				}

				UpdatePlotData(); 
				StatusMessage = "Changes saved successfully.";
			}
			else
			{
				StatusMessage = "No records selected for update.";
			}

			ShowStatusMessageDialog(StatusMessage);
		}
		catch (Exception ex)
		{
			StatusMessage = $"Error saving changes: {ex.Message}";
			ShowStatusMessageDialog(StatusMessage);
		}
	}

	private void ShowStatusMessageDialog(string message)
	{
		var statusDialog = new StatusMessageDialog(message);
		statusDialog.ShowDialog();
	}
}
