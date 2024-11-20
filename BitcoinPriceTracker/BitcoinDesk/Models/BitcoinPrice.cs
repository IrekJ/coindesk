namespace BitcoinDesk.Models
{
	public class BitcoinPrice 
	{
		private string _note = string.Empty;

		public int ID { get; set; }
		public DateTime Timestamp { get; set; }
		public decimal PriceCZK { get; set; }
		public string Note
		{
			get
			{
				return _note;
			}

			set
			{
				if (_note != value)
				{
					_note = value;
					IsSelected = true;
				}
			}
		}
		public bool IsSelected { get; set; } 

	}
}
