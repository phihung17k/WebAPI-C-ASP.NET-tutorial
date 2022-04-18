using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using WebAPI2.Results;

namespace WebAPI2.Controllers
{
    public class ValuesController : ApiController
    {
        public void Post()
        {

        }

        /*
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "valued");
            response.Content = new StringContent("hello world", Encoding.Unicode);
            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.MaxValue
            };
            return response;
        }
        */

        public IHttpActionResult Get()
        {
            return new TextResult("hello w", Request);
        }
    }
}