using System.Data.SqlClient;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace BitcoinDesk.Services
{
	public static class DataService
	{
		private static readonly string ConnectionString;

		static DataService()
		{
			var configFilePath = "config.json";
			if (File.Exists(configFilePath))
			{
				var configData = File.ReadAllText(configFilePath);
				dynamic config = JsonConvert.DeserializeObject(configData);
				ConnectionString = config?.Database?.ConnectionString ?? throw new Exception("Connection string not found in config.");
			}
			else
			{
				throw new FileNotFoundException("Configuration file config.json is missing.");
			}
		}

		public static async Task SaveBitcoinPriceAsync(decimal price, string note)
		{
			using (var connection = new SqlConnection(ConnectionString))
			{
				await connection.OpenAsync();

				string query = "INSERT INTO BitcoinPrices (PriceCZK, Note) VALUES (@PriceCZK, @Note)";
				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@PriceCZK", price);
					command.Parameters.AddWithValue("@Note", note);
					await command.ExecuteNonQueryAsync();
				}
			}
		}
	}
}
