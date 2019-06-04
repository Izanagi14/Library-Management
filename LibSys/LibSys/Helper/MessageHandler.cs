using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
namespace LibSys.Helper
{
	public class MessageHandler
	{
		[JsonProperty("Type")]
		public string ResultType { set; get; }

		[JsonProperty("Message")]
		public string Result { set; get; }

		public static MessageHandler GetMessageHandler(string type, string message)
		{
			return new MessageHandler()
			{
				Result = message,
				ResultType = type
			};
		}

	}
}