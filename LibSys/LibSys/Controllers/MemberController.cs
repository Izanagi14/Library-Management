using LibSys.Helper;
using LibSysCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibSys.Controllers
{
	public class MemberController : BaseController
	{
		[HttpGet]
		public JsonResult Index()
		{
			string token = Request.Headers.Get("Authorization") ?? "";
			TokenValidation tokenValidation = DBContext.IsTokenValid(token);
			if (tokenValidation.Validate)
			{
				return Json(DBOperations.GetAllIssuedBook(tokenValidation.UserId), JsonRequestBehavior.AllowGet);
			}
			else
			{
				return Json(MessageHandler.GetMessageHandler("Error", "Invalid/Expired Token"));
			}
		}

		[HttpPost]
		public JsonResult Borrow(string bookId)
		{
			string token = Request.Headers.Get("Authorization") ?? "";
			TokenValidation tokenValidation = DBContext.IsTokenValid(token);
			if (tokenValidation.Validate)
			{
				string message = DBOperations.BorrowBook(tokenValidation.UserId, bookId);
				if (message.StartsWith("Success"))
					return Json(MessageHandler.GetMessageHandler("Success", message));
				else
					return Json(MessageHandler.GetMessageHandler("Error", message));
			}
			else
			{
				return Json(MessageHandler.GetMessageHandler("Error", "Invalid/Expired Token"));
			}
		}

		[HttpPost]
		public JsonResult Return(string bookId)
		{
			string token = Request.Headers.Get("Authorization") ?? "";
			TokenValidation tokenValidation = DBContext.IsTokenValid(token);
			if (tokenValidation.Validate)
			{
				string message = DBOperations.ReturnBook(tokenValidation.UserId, bookId);
				if (message.StartsWith("Success"))
					return Json(MessageHandler.GetMessageHandler("Success", message));
				else
					return Json(MessageHandler.GetMessageHandler("Error", message));
			}
			else
			{
				return Json(MessageHandler.GetMessageHandler("Error", "Invalid/Expired Token"));
			}
		}
    }
}