using BitcoinDesk.Models;
using System.Data.SqlClient;

namespace BitcoinDesk
{
	public class BitcoinPriceRepository
	{
		private readonly string _connectionString;

		public BitcoinPriceRepository(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task InsertBitcoinPriceAsync(decimal priceCZK, string note = null)
		{
			await using var conn = new SqlConnection(_connectionString);
			string query = "INSERT INTO BitcoinPrices (PriceCZK, Note) VALUES (@PriceCZK, @Note)";

			await using var cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@PriceCZK", priceCZK);
			cmd.Parameters.AddWithValue("@Note", (object)note ?? DBNull.Value);

			await conn.OpenAsync();
			await cmd.ExecuteNonQueryAsync();
		}

		public async Task<List<BitcoinPrice>> GetAllBitcoinPricesAsync()
		{
			var prices = new List<BitcoinPrice>();

			await using var conn = new SqlConnection(_connectionString);
			string query = "SELECT * FROM BitcoinPrices";

			await using var cmd = new SqlCommand(query, conn);
			await conn.OpenAsync();

			await using var reader = await cmd.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				prices.Add(new BitcoinPrice
				{
					ID = reader.GetInt32(0),
					Timestamp = reader.GetDateTime(1),
					PriceCZK = reader.GetDecimal(2),
					Note = !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty,
					IsSelected = false
				});
			}

			return prices;
		}

		public async Task UpdateNoteAsync(BitcoinPrice record)
		{
			await using var conn = new SqlConnection(_connectionString);
			string query = "UPDATE BitcoinPrices SET Note = @Note WHERE ID = @ID";

			await using var cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@Note", record.Note);
			cmd.Parameters.AddWithValue("@ID", record.ID);

			await conn.OpenAsync();
			await cmd.ExecuteNonQueryAsync();
		}

		public async Task DeleteBitcoinPriceAsync(int id)
		{
			await using var conn = new SqlConnection(_connectionString);
			string query = "DELETE FROM BitcoinPrices WHERE ID = @ID";

			await using var cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddWithValue("@ID", id);

			await conn.OpenAsync();
			await cmd.ExecuteNonQueryAsync();
		}
	}
}
