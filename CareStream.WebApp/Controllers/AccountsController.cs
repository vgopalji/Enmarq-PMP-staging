using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareStream.Models;
using CareStream.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;

namespace CareStream.WebApp.Controllers
{
    public class AccountsController : Controller
    {
        public async Task<IActionResult> Index(string Id)
        {
            var user = new User();

            var client = GraphClientUtility.GetGraphServiceClient();

            if (client == null)
            {
                return Ok(user);
            }

            //user = await client.Users[Id].Request().GetAsync();
            try
            {

                user = await client
                   .Users[Id]
                   .Request()
                   .GetAsync();

                var extensions = await client.Users[Id].Extensions.Request().GetAsync();
                user.Extensions = extensions;
            }
            catch (Exception ex)
            {

            }
            return View(user);

        }
    }
}
