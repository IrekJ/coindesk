using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinDesk.Models
{
	public class ExchangeRate
    {
		public string validFor { get; set; } = string.Empty;
		public string country { get; set; } = string.Empty;
		public string currency { get; set; } = string.Empty;
		public decimal rate { get; set; } = 0;
		public string currencyCode { get; set; } = string.Empty;
	}

	public class ExchangeRatesResponse
	{
		public ExchangeRate[] rates { get; set; } = [];
	}

}
