using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CareStream.WebApp.Controllers
{
    public class BaseController : Controller
    {
        public void ShowSuccessMessage(string message)
        {
            TempData["UserMessage"] = message;
            TempData["UserMsgColor"] = "green";
        }

        public void ShowErrorMessage(string message)
        {
            TempData["UserMessage"] = message;
            TempData["UserMsgColor"] = "red";
        }

        public object GetSuccessMessage(string message)
        {
            return new { Message = message, MsgColor = "green" };
        }

        public object GetErrorMessage(string message)
        {
            return new { Message = message, MsgColor = "red" };
        }
    }
}
