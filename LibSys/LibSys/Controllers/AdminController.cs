using System;
using System.Collections.Generic;
using System.Web.Mvc;
using LibSys.Helper;

// User Namespaces
using LibSysCore.DataAccess;
using LibSysCore.Models;
namespace LibSys.Controllers
{
	public class AdminController : BaseController
	{

		public JsonResult Index()
		{
			return Json(MessageHandler.GetMessageHandler("Error", "Please Login"), JsonRequestBehavior.AllowGet);
		}


		//POST: Admin/AddMembers
		[HttpPost]
		public JsonResult Members(List<Person> persons)
		{
			string token = Request.Headers.Get("Authorization") ?? "";
			List<MessageHandler> messageHandlers = new List<MessageHandler>();
			try
			{
				TokenValidation tokenValidation = DBContext.IsTokenValid(token);
				if (tokenValidation.Validate)
				{
					if (tokenValidation.Type == UserType.Admin)
					{
						foreach (Person person in persons)
						{
							string message = DBOperations.InsertNewMembers(person);
							messageHandlers.Add(MessageHandler.GetMessageHandler(message.StartsWith("Success") ? "Success" : "Error", message));
						}
					}
					else
					{
						messageHandlers.Add(MessageHandler.GetMessageHandler("Error", "Hack attack"));
					}
				}
				else
				{
					messageHandlers.Add(MessageHandler.GetMessageHandler("Error", "Invalid/Expired Token"));
				}
			}
			catch (Exception ex)
			{
				messageHandlers.Add(MessageHandler.GetMessageHandler("Error", ex.Message));
			}
			return Json(messageHandlers);
		}

		// POST: Admin/DeleteMember
		[HttpPost]
		public JsonResult Member(string userId)
		{
			string token = Request.Headers.Get("Authorization") ?? "";
			TokenValidation tokenValidation = DBContext.IsTokenValid(token);
			if (tokenValidation.Validate)
			{
				if (tokenValidation.Type == UserType.Admin)
				{
					try
					{
						string result = DBOperations.DeleteMember(userId);
						return Json(MessageHandler.GetMessageHandler("Success", result));
					}
					catch (Exception ex)
					{
						return Json(MessageHandler.GetMessageHandler("Error", ex.Message));
					}
				}
				else
				{
					return Json(MessageHandler.GetMessageHandler("Error", "Hack attack"));
				}
			}
			else
			{
				return Json(MessageHandler.GetMessageHandler("Error", "Invalid/Expired Token"));
			}
		}
		
		//POST:  Add Books to the library
		[HttpPost]
		public JsonResult Books(List<Book> books)
		{
			List<MessageHandler> messageHandlers = new List<MessageHandler>();
			string token = Request.Headers.Get("Authorization") ?? "";
			TokenValidation tokenValidation = DBContext.IsTokenValid(token);
			if (tokenValidation.Validate)
			{
				foreach (Book book in books)
				{
					string result = DBOperations.InsertBooks(book);
					messageHandlers.Add(new MessageHandler()
					{
						ResultType = result.StartsWith("Success") ? "Success" : "Error",
						Result = result 
					});
				}
				return Json(messageHandlers);
			}
			else
			{
				return Json(MessageHandler.GetMessageHandler("Error", "Invalid/Expired Token"), JsonRequestBehavior.AllowGet);
			}
		}
	}
}
