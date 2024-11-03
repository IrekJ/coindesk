using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using BitcoinDesk.Models;

namespace BitcoinDesk.Services
{
	public class ApiService
	{
		private readonly HttpClient _httpClient;
		private readonly string _bitcoinPriceUrl;
		private readonly string _exchangeRateUrl;

		public ApiService()
		{
			_httpClient = new HttpClient();
			var configFilePath = "config.json";

			if (File.Exists(configFilePath))
			{
				var configData = File.ReadAllText(configFilePath);
				dynamic config = JsonConvert.DeserializeObject(configData);
				_bitcoinPriceUrl = config?.ApiEndpoints?.BitcoinPriceUrl
								   ?? throw new Exception("Bitcoin Price URL not found in config.");
				_exchangeRateUrl = config?.ApiEndpoints?.ExchangeRateUrl
								   ?? throw new Exception("Exchange Rate URL not found in config.");
			}
			else
			{
				throw new FileNotFoundException("Configuration file config.json is missing.");
			}
		}

		public async Task<decimal> GetBitcoinPriceEURAsync()
		{

			HttpResponseMessage response = await _httpClient.GetAsync(_bitcoinPriceUrl);
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dynamic data = JsonConvert.DeserializeObject(jsonResponse);
				return (decimal)(data?.bpi?.EUR?.rate_float ?? 0);
			}
			throw new Exception("Failed to fetch data from Coindesk API.");
		}

		public async Task<decimal> GetExchangeRateEURToCZKAsync()
		{
			string date = DateTime.Now.ToString("yyyy-MM-dd");
			string apiUrl = _exchangeRateUrl.Replace("{date}", date);

			HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
			if (response.IsSuccessStatusCode)
			{
				var jsonResponse = await response.Content.ReadAsStringAsync();
				var rateArray = JsonConvert.DeserializeObject< ExchangeRatesResponse>(jsonResponse);
				return rateArray?.rates?.FirstOrDefault(rate => rate.currencyCode == "EUR")?.rate ?? 0;

			}
			throw new Exception("Failed to fetch data from Exchange Rate API.");
		}

		public async Task<decimal> CalculateBitcoinPriceCZKAsync()
		{
			decimal priceEUR = await GetBitcoinPriceEURAsync();
			decimal exchangeRateCZK = await GetExchangeRateEURToCZKAsync();
			return priceEUR * exchangeRateCZK;
		}
	}
}
