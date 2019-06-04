using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibSysCore.Models
{
	public class UserBook
	{
		[JsonProperty("bookId", NullValueHandling = NullValueHandling.Ignore)]
		public string BookId { get; set; }

		[JsonProperty("userId", NullValueHandling = NullValueHandling.Ignore)]
		public string UserId { get; set; }

		[JsonProperty("issuedOn", NullValueHandling = NullValueHandling.Ignore)]
		public string IssuedOn { get; set; }

		[JsonProperty("returnedOn", NullValueHandling = NullValueHandling.Ignore)]
		public string ReturnedOn { get; set; }
	}

}