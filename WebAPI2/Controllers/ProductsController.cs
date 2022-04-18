using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using WebAPI2.Models;

namespace WebAPI2.Controllers
{
    /// <summary>
    /// The Authorize filter checks to see if the user is authenticated. 
    /// If the user is not authenticated, it returns HTTP status code 401 (Unauthorized) without invoking the action. 
    /// You can apply the filter globally, at the controller level, or at the level of individual actions.
    /// </summary>
    //[Authorize]
    public class ProductsController : ApiController
    {
        Product[] products = new Product[]
        {
            new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
            new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
            new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
        };

        //public IEnumerable<Product> GetAllProducts()
        //{
        //    return products;
        //}

        public IHttpActionResult GetProduct(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            //var product = products.SingleOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        public HttpResponseMessage Get()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, products, "application/json");
            return response;
        }
    }
}
