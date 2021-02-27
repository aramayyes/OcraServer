using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using OcraServer.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using OcraServer.Models.ViewModels;

namespace OcraServer.Controllers
{
    [Route("api/{lang}/[controller]")]
    public class ValuesController : Controller
    {
        IHostingEnvironment _appEnvironment;

        public ValuesController(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }


        // GET api/values
        [Authorize(Roles = Extra.Constants.ApplicationRoles.CLIENT_ROLE)]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value123", "value2" };
        }

        ///// <summary>
        ///// GET: api/values/GetMostViewed/{id}
        ///// ROUTING TYPE: attribute-based
        ///// </summary>
        ///// <returns>An array of {n} Json-serialized objects representing the items with most user views.</returns>
        //[HttpGet("{id}")]
        //public IActionResult Get(int id)
        //{
        //	return Forbid();
        //	// return Ok(new { ValueR = "SomeString" });
        //}

        //[HttpGet("r/{id}")]
        //public IActionResult GetR(int id)
        //{
        //	return new JsonResult((new { ValueR = "SomeString" }));
        //}


        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}

        [HttpPost("SendTestNotification")]
        public async Task<IActionResult> SendTestNotification([FromBody] PrivateData privateData)
        {
            if (privateData == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (HttpClient client = new HttpClient())
            {
                var body = JObject.FromObject(new
                {
                    to = privateData.IIDToken,
                    data = new
                    {
                        ReservationID = 0,
                        RestaurantNameArm = "HB",
                        RestaurantNameEng = "HB",
                        RestaurantNameRus = "HB",
                        RestaurantImg = "http://www.irest.am/them/irest/img/organization/gall/1450283498,4947.jpeg",
                        ReservationDateTime = new DateTime(2017, 9, 2, 16, 0, 0),
                        Status = ReservationStatus.Accepted,
                        UserFirstName = "Hovhannes",
                    }
                }, new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                try
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + "AAAAbGXXRVU:APA91bG2lBcODRTlGtOXsgpC5HHgz4qIH29uv2ut9be4vEMmzjHe2pljaIAtR5edzcAQek2rvqiidzaI7PY6UWl1kPcJ_IIztXSjgoZyz1zVUXOJKPza2lOS4xoYUbgDlSiEfssUTvUQ");
                    var content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");

                    var result = await client.PostAsync($"https://fcm.googleapis.com/fcm/send", content);
                    if (result.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                    {
                        var json = JObject.Parse(await result.Content.ReadAsStringAsync());
                        var success = json["success"].ToString();
                        var error = json["results"][0]["error"]?.ToString();

                        return Ok($"success : {success}\nerror : {error ?? ""}");
                    }
                    else
                    {
                        return Unauthorized();
                    }


                }
                catch
                {
                    throw;
                }
            }
        }

        public class PrivateData
        {
            [Required]
            public string IIDToken { get; set; }

        }
    }
}
