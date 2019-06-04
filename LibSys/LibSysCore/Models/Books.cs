using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
namespace LibSysCore.Models
{
	public class Book
	{
		[JsonProperty("bookId", NullValueHandling = NullValueHandling.Ignore)]
		public string BookId { get; set; }

		[JsonProperty("userId", NullValueHandling = NullValueHandling.Ignore)]
		public string UserId { get; set; }

		[JsonProperty("issuedBy", NullValueHandling = NullValueHandling.Ignore)]
		public string IssuedBy { get; set; }

		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		[JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
		public int Count { get; set; }

		[JsonProperty("issuedOn", NullValueHandling = NullValueHandling.Ignore)]
		public string IssuedOn { get; set; }

		[JsonProperty("validTo", NullValueHandling = NullValueHandling.Ignore)]
		public string DueOn { get; set; }

		[JsonProperty("available", NullValueHandling = NullValueHandling.Ignore)]
		public int Available { get; set; }
		[JsonProperty("returnStatus", NullValueHandling = NullValueHandling.Ignore)]
		public int ReturnStatus { get; set; }
	}
}