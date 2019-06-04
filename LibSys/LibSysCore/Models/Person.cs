using Newtonsoft.Json;

namespace LibSysCore.Models
{
//	[DataContract]
	public class Person
	{
		[JsonProperty("userId")]
		public string UserId { get; set; }
		[JsonProperty("password")]
		public string Password { get; set; }

		public string Token { get; set; }
		[JsonProperty("isAdmin")]
		public string IsAdmin { get; set; }

	}

}