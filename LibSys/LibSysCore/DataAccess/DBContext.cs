using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using LibSysCore.Models;

namespace LibSysCore.DataAccess
{
	public class TokenValidation
	{
		public bool Validate { get; set; }	
		public UserType Type { get; set; }
		public string UserId { get; set; }
	}
	public enum UserType
	{
		Admin, 
		Normal, 
		NotRegistered
	}
	public class DBContext
	{
		public static string GenerateToken(string type, string userId, long ticks)
		{
			string hash = string.Join(":", new string[] {type, "rz8LuOtFBXphj9WQfvFh"});
			string hashLeft = "";
			string hashRight = "";
			using (HMAC hmac = HMACSHA256.Create("HmacSHA256"))
			{
				hmac.Key = Encoding.UTF8.GetBytes(GetHashedPassword("Test"));
				hmac.ComputeHash(Encoding.UTF8.GetBytes(hash));
				hashLeft = string.Join(":", new String[] { hmac.Hash.ToString(), hash});
				hashRight = string.Join(":", new string[] { userId, ticks.ToString() });
			}
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join(":", hashLeft, hashRight)));
		}
		public static string GetHashedPassword(string password)
		{
			string key = string.Join(":", new string[] { password, "rz8LuOtFBXphj9WQfvFh"});
			using (HMAC hmac = HMACSHA256.Create("HmacSHA256"))
			{
				// Hash the key.
				hmac.Key = Encoding.UTF8.GetBytes("rz8LuOtFBXphj9WQfvFh");
				hmac.ComputeHash(Encoding.UTF8.GetBytes(key));
				return Convert.ToBase64String(hmac.Hash);
			}
		}
		public static TokenValidation IsTokenValid(string token)
		{
			TokenValidation tokenValidation = new TokenValidation();
			try
			{
				// Base64 decode the string, obtaining the token:username:timeStamp.
				string key = Encoding.UTF8.GetString(Convert.FromBase64String(token));
				// Split the parts.
				string[] parts = key.Split(new char[] { ':' });
				if (parts.Length == 5)
				{
					// Get the hash message, username, and timestamp.
					string hash = parts[0];
					string username = parts[3];
					long ticks = long.Parse(parts[4]);
					DateTime timeStamp = new DateTime(ticks);
					// Ensure the timestamp is valid.
					bool expired = Math.Abs((DateTime.UtcNow - timeStamp).TotalMinutes) > 200;
					if (!expired)
					{
						if (parts[2] == "rz8LuOtFBXphj9WQfvFh")
						{
							tokenValidation.Validate = true;
							tokenValidation.UserId = username;
							tokenValidation.Type = parts[1] == "Admin" ? UserType.Admin : UserType.Normal;
						}
						else
						{
							tokenValidation.Validate = false;
							tokenValidation.Type = UserType.NotRegistered;
						}
					}
				}
			}
			catch
			{
			}
			return tokenValidation;
		}
	}
}