using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LibSys.Helper;

// User Namespaces
using LibSysCore.DataAccess;
using LibSysCore.Models;
namespace LibSys.Controllers
{
    public class BaseController : Controller
    {
		//POST : Get login token
		[HttpPost]
		public JsonResult Login(string userId, string password)
		{
			Person person = new Person();
			Dictionary<string, string> token = new Dictionary<string, string>();
			if (DBOperations.Login(userId, password, ref person))
			{
				token.Add("Token", person.Token);
				return Json(token);
			}
			token.Add("Error", "Not A Valid Admin");
			return Json(new Dictionary<string, string>());
		}

		[HttpGet]
		public JsonResult Books(string bookName, string userName)
		{
			string token = Request.Headers.Get("Authorization") ?? "";
			TokenValidation tokenValidation = DBContext.IsTokenValid(token);
			if (bookName == null)
			{
				if (tokenValidation.Validate)
				{
					return Json(DBOperations.GetBooks(tokenValidation.Type == UserType.Admin ? "Admin" : (userName != null?tokenValidation.UserId:null)), JsonRequestBehavior.AllowGet);
				}
			}
			else
			{
				if (tokenValidation.Validate)
				{
					return Json(DBOperations.GetBooksByName(tokenValidation.Type == UserType.Admin ? "Admin" : (userName == null ? tokenValidation.UserId : null), bookName), JsonRequestBehavior.AllowGet);
				}
			}
			return Json(MessageHandler.GetMessageHandler("Error", "Invalid/Expired Token"), JsonRequestBehavior.AllowGet);
		}
	}
}