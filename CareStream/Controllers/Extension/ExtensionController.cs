using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Newtonsoft.Json;

namespace CareStream.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtensionController : ControllerBase
    {
        private readonly ILoggerManager _logger;

        public ExtensionController(ILoggerManager logger)
        {
            _logger = logger;
        }

        // GET: api/<ExtensionController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ExtensionController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ExtensionController>
        [HttpPost]
        public async Task<string> Post([FromBody] ExtensionModel extension)
        {
            try
            {
                //Check Null condition and add logging
                _logger.LogInfo("ExtensionController-Post: [Started] creation of user attribute in Azure AD B2C");

                if (extension == null)
                {
                    _logger.LogError("ExtensionController-Post: Input cannot be null");
                    return string.Empty;
                }

                var json = JsonConvert.SerializeObject(extension);

                var authenticationResult = GraphClientUtility.GetAuthentication();

                if(authenticationResult == null)
                {
                    _logger.LogError("ExtensionController-Post: Unable to get the Access token and Authentication Result");
                    return string.Empty;
                }
                var accessToken = authenticationResult.Result.AccessToken;

                var tenantId = GraphClientUtility.TenantId;
                var api = $"{CareStreamConst.ForwardSlash}{CareStreamConst.Applications}{CareStreamConst.ForwardSlash}{GraphClientUtility.b2cExtensionAppClientId}{CareStreamConst.ForwardSlash}{CareStreamConst.ExtensionProperties}";

                HttpClient httpClient = new HttpClient();
                string url = GraphClientUtility.AADGraphResourceId + tenantId + api + "?" + GraphClientUtility.AADGraphVersion;

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization = new AuthenticationHeaderValue(CareStreamConst.Bearer, accessToken);
                request.Content = new StringContent(json, Encoding.UTF8, $"{CareStreamConst.Application}{CareStreamConst.ForwardSlash}{CareStreamConst.Json}");
                HttpResponseMessage response = await httpClient.SendAsync(request);


                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    object formatted = JsonConvert.DeserializeObject(error);

                    var errorMessage = "Error Calling the Graph API: \n" + JsonConvert.SerializeObject(formatted, Formatting.Indented);

                    _logger.LogError($"ExtensionController-Post: {errorMessage}");
                    return errorMessage;
                }


                _logger.LogInfo("ExtensionController-Post: [Completed] creation of user attribute in Azure AD B2C");


                return await response.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError("ExtensionController-Post: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }

        // PUT api/<ExtensionController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ExtensionController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
