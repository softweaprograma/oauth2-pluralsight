﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;

namespace Pluralsight.Client.M2.Simple.Controllers
{
    public class HomeController : Controller
    {
        private static string Message { get; set; } = "";
        private static string Code { get; set; }
        private static string Token { get; set; }

        public IActionResult Index()
        {
            return View("Index", Message);
        }

        private const string ClientId = "simple_client";
        private const string ClientSecret = "secret";
        private const string RedirectUri = "http://localhost:5001/callback";

        public IActionResult Authorize()
        {
            Message += "\n\nRedirecting to authorization endpoint...";
            return Redirect($"http://localhost:5000/connect/authorize?client_id={ClientId}&scope=wiredbrain_api.rewards&redirect_uri={RedirectUri}&response_type=code&response_mode=query");
        }

        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            Code = code;
            Message += "\nApplication Authorized!";

            Message += "\n\nCalling token endpoint...";
            var tokenClient = new TokenClient("http://localhost:5000/connect/token", ClientId, ClientSecret);
            var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(Code, RedirectUri);

            if (tokenResponse.IsError)
            {
                Message += "\nToken request failed";
                return RedirectToAction("Index");
            }

            Token = tokenResponse.AccessToken;
            Message += "\nToken Received!";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CallApi()
        {
            var httpClient = new HttpClient();
            if (Token != null) httpClient.SetBearerToken(Token);

            var response = await httpClient.GetAsync("http://localhost:5002/api/rewards");

            if (response.IsSuccessStatusCode) Message += "\n\nAPI access authorized!";
            else if (response.StatusCode == HttpStatusCode.Unauthorized) Message += "\nUnable to contact API: Unauthorized!";
            else Message += $"\n\nUnable to contact API. Status code {response.StatusCode}";

            return RedirectToAction("Index");
        }
    }
}
