using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

//User Namespaces
using LibSysCore.DBConnection;
using LibSysCore.Models;
using LibSysCore.Helper;
using System.Globalization;

namespace LibSysCore.DataAccess
{
	public class DBOperations : ACryptoHandler
	{
		#region Public Methods
		public static bool Login(string userId, string password, ref Person person)
		{
			password = ComputeSha256Hash(password);
			bool isAdmin = false;
			string query = string.Format("select userId, isAdmin from person where userId='{0}' AND password='{1}'", userId, password); ;
			using (var myDBReader = new MySqlDBReader(query))
			{
				var reader = myDBReader.Reader;
				while (reader.Read())
				{
					person.UserId = reader.GetString("userId");
					isAdmin = bool.Parse(reader.GetString("isAdmin"));
				}
			}
			if (isAdmin)
			{
				person.Token = DBContext.GenerateToken("Admin", person.UserId, DateTime.UtcNow.Ticks);
			}
			else
			{
				person.Token = DBContext.GenerateToken("Normal", person.UserId, DateTime.UtcNow.Ticks);
			}
			return true;
		}
		public static string InsertNewMembers(Person person)
		{
			try
			{

				InitiateConnectionProcess(string.Format(@"insert into Person values('{0}','{1}',{2})", person.UserId, ComputeSha256Hash(person.Password)
					, int.Parse(person.IsAdmin)));
				return "Success " + person.UserId + "Inserted ";

			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
		public static string DeleteMember(string userId)
		{
			try
			{
				var query = string.Format(@"delete from Person where userId LIKE '%{0}%'", userId);
				InitiateConnectionProcess(query);
				return userId + " Successfully Deleted";
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public static List<Book> GetBooks(string user)
		{
			List<Book> books = new List<Book>();
			string query;
			int check = -1;
			if (user == "Admin")
			{
				check = 1;
				query = "select * from Books";
			}
			else
			{
				if (user != null)
				{
					check = 2;
					query = string.Format("select returnStatus, bookId, available,userId, name, issuedOn, validTo from Books where userId='{0}'", user);
				}
				else
				{
					check = 3;
					query = "select returnStatus, bookId, available, name from Books";
				}
			}
			using (var myDBReader = new MySqlDBReader(query))
			{
				var reader = myDBReader.Reader;
				if (reader.FieldCount == 0)
				{
					return books;
				}
				while (reader.Read())
				{
					FillBooks(ref books, check, ref reader);
				}
			}
			return books;
		}
		public static List<Book> GetBooksByName(string user, string bookName)
		{
			List<Book> books = new List<Book>();
			string query;
			int check = -1;
			if (user == "Admin")
			{
				check = 1;
				query = string.Format("select * from Books where name='{0}'", bookName);
			}
			else
			{
				if (user != null)
				{
					check = 2;
					query = string.Format("select returnStatus, bookId, available,userId, name, issuedOn, validTo from Bookswhere userId='{0}' and name='{1}'", user, bookName);
				}
				else
				{
					check = 3;
					query = string.Format("select returnStatus, bookId, available, name from Books where name='{0}'", bookName);
				}
			}
			using (var myDBReader = new MySqlDBReader(query))
			{
				var reader = myDBReader.Reader;
				if (reader.FieldCount == 0)
				{
					return books;
				}
				while (reader.Read())
				{
					FillBooks(ref books, check, ref reader);
				}
			}
			return books;
		}
		public static string InsertBooks(Book book)
		{
			try
			{
				string query = string.Format("Insert into books(`bookId`, `count`, `name`, `available`) values('{0}', {1}, '{2}', {3})", book.BookId, book.Count, book.Name, book.Count);
				InitiateConnectionProcess(query);
				return "Success Book with Book-Id " + book.BookId + " Successfully Inserted";
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public static string BorrowBook(string userId, string bookId)
		{
			if (CheckUserCurrentRecord(userId, bookId))
			{
				if (CheckAvailability(bookId))
				{
					return InitiateBorrowBook(userId, bookId);
				}
				else
				{
					return "Book is not Available";
				}
			}
			else
			{
				return "You already have maximum the number of books allowed to issue";
			}
		}

		public static List<UserBook> GetAllIssuedBook(string userId)
		{
			List<UserBook> userBooks = new List<UserBook>();
			string query = string.Format("Select * from bookuser where userId='{0}'", userId);
			using (var myDBReader = new MySqlDBReader(query))
			{
				var reader = myDBReader.Reader;
				while (reader.Read())
				{
					userBooks.Add(new UserBook()
					{
						BookId = reader.GetString("bookId"),
						UserId = reader.GetString("userId"),
						IssuedOn = reader.GetString("issuedOn"),
						ReturnedOn =  reader.GetString("returnedOn")
					});
				}
			}
			return userBooks;
		}
		public static string ReturnBook(string userId, string bookId)
		{
			return InitiateReturnBook(userId, bookId);
		}
		#endregion

		#region Private Methods
		private static string InitiateConnectionProcess(string query)
		{
			try
			{
				MySqlDBConnection connection = new MySqlDBConnection();
				MySqlConnection connect = connection.SQLConnection;
				connection.RunInTrx(query, ref connect);
				return "Success";
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
		private static bool CheckAvailability(string bookId)
		{
			string query = string.Format("Select available from Books where bookId='{0}'", bookId);
			using (var myDBReader = new MySqlDBReader(query))
			{
				var reader = myDBReader.Reader;
				while (reader.Read())
				{
					int status = int.Parse(SafeGetString(reader, "available"));
					if (status > 0)
					{
						return true;
					}
				}
			}
			return false;

		}
		private static bool CheckUserCurrentRecord(string userId, string bookId)
		{
			int maxAllowed = 2;
			int issued = 0;
			string query = string.Format("Select returnStatus, bookId from Books where userId='{0}'", userId);
			using (var myDBReader = new MySqlDBReader(query))
			{
				var reader = myDBReader.Reader;
				while (reader.Read())
				{
					int status = int.Parse(SafeGetString(reader, "returnStatus"));
					if (SafeGetString(reader, "bookId") == null || SafeGetString(reader, "bookId") == "")
					{
						return false;
					}
					if (status < 1)
					{
						issued++;
					}
				}
			}
			return issued <= maxAllowed;
		}
		private static string InitiateBorrowBook(string userId, string bookId)
		{
			string issueDay = DateTime.UtcNow.ToString("MM/dd/yyyy");
			string lastDay = DateTime.UtcNow.AddDays(7).ToString("MM/dd/yyyy");
			string query = string.Format("Update Books Set " +
							"userId='{0}'," +
							"available=available-1," +
							"issuedOn='{1}'," +
							"issuedBy='Admin'," +
							"validTo='{2}'," +
							"returnStatus=0 where bookId='{3}'"
			, userId, DateTime.UtcNow.ToString(), DateTime.UtcNow.AddDays(7).ToString(), bookId);
			string insertBookUserQuery = String.Format("Insert into bookuser(`bookId`, `userId`, `issuedOn`, `returnedOn`) values('{0}', '{1}', '{2}', '{3}')", bookId, userId, issueDay, string.Empty);
			try
			{
				string firstCheck = InitiateConnectionProcess(query);
				if (firstCheck == "Success")
				{
					if (InitiateConnectionProcess(insertBookUserQuery) == "Success")
					{
						return "Success you have borrowed the book";
					}
					else
					{
						string returnStateQuery = string.Format("Update Books Set " +
							"userId='{0}'," +
							"available=available+1," +
							"issuedOn='{1}'," +
							"issuedBy='{2}'," +
							"validTo='{3}'," +
							"returnStatus=-1 where bookId='{3}'"
							, string.Empty, "2018-01-01 00:00:00", string.Empty, "2018-01-01 00:00:00", bookId);
						return InitiateConnectionProcess(query);
					}
				}
				return firstCheck;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
		private static void FillBooks(ref List<Book> books, int choice, ref MySqlDataReader reader)
		{
			switch (choice)
			{
				case 1:
					books.Add(new Book()
					{
						BookId = SafeGetString(reader, "bookId"),
						UserId = SafeGetString(reader, "userId"),
						Count = int.Parse(SafeGetString(reader, "count")),
						IssuedBy = SafeGetString(reader, "issuedBy"),

						IssuedOn = SafeGetString(reader, "issuedOn"),
						DueOn = SafeGetString(reader, "validTo"),

						Available = int.Parse(SafeGetString(reader, "available")),
						Name = SafeGetString(reader, "name"),
						ReturnStatus = int.Parse(SafeGetString(reader, "returnStatus"))
					});
					break;
				case 2:
					books.Add(new Book()
					{
						BookId = SafeGetString(reader, "bookId"),
						Count = int.MinValue,
						IssuedBy = null,
						UserId = SafeGetString(reader, "userId"),

						IssuedOn = SafeGetString(reader, "issuedOn"),
						DueOn = SafeGetString(reader, "validTo"),

						Available = int.Parse(SafeGetString(reader, "available")),
						ReturnStatus = int.Parse(SafeGetString(reader, "returnStatus")),
						Name = SafeGetString(reader, "name")
					});
					break;
				case 3:
					books.Add(new Book()
					{
						BookId = SafeGetString(reader, "bookId"),
						UserId = null,
						Count = int.MinValue,
						IssuedBy = null,
						IssuedOn = null,
						DueOn = null,
						Available = int.Parse(SafeGetString(reader, "available")),
						ReturnStatus = int.Parse(SafeGetString(reader, "returnStatus")),
						Name = SafeGetString(reader, "name")
					});
					break;
				default:
					books.Add(new Book());
					break;
			}
		}
		private static string SafeGetString(MySqlDataReader reader, string colName)
		{
			int colIndex = reader.GetOrdinal(colName);

			if (!reader.IsDBNull(colIndex))
			{
				return reader.GetString(colIndex);
			}
			else
			{
				return null;
			}
		}

		private static string InitiateReturnBook(string userId, string bookId)
		{
			string issuedOn = "";
			string validTo = "";
			string query = string.Format("select * from Books where bookId='{0}' and userId='{1}'", bookId, userId);
			using (var myDBReader = new MySqlDBReader(query))
			{
				var reader = myDBReader.Reader;
				while (reader.Read())
				{
					issuedOn = SafeGetString(reader, "issuedOn");
					validTo = SafeGetString(reader, "validTo");
				}
			}
			query = string.Format("Update Books Set " +
							"userId='{0}'," +
							"available=available+1," +
							"issuedOn='{1}'," +
							"issuedBy='{2}'," +
							"validTo='{3}'," +
							"returnStatus=1 where bookId='{4}' and userId='{5}'"
			, string.Empty, "2018-01-01 00:00:00", string.Empty, "2018-01-01 00:00:00", bookId, userId);
			try
			{
				string firstCheck = InitiateConnectionProcess(query);
				if (firstCheck == "Success")
				{
					query = string.Format("Update bookuser Set returnedOn='{0}' where bookId='{1}' and userId='{2}'", DateTime.UtcNow.ToString("MM/dd/yyyy"), bookId, userId);
					if (InitiateConnectionProcess(query) == "Success")
					{
						return "Success You have retured the book";
					}
					else
					{
						query = string.Format("Update Books Set " +
							"userId='{0}'," +
							"available=available-1," +
							"issuedOn='{1}'," +
							"issuedBy='{2}'," +
							"validTo='{3}'," +
							"returnStatus=0 where bookId='{4}' and userId='{5}'"
							, userId, issuedOn, "Admin", validTo, bookId, userId);
						return InitiateConnectionProcess(query);
					}
				}
				else
				{
					return firstCheck;
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
		#endregion
	}
}