#### Project Structure

###### App Data

###### App Start

- WebApiConfig.cs

###### Controllers

###### Models

- ```csharp
  public class Product
      {
          public int Id { get; set; }
          public string Name { get; set; }
          public string Category { get; set; }
          public decimal Price { get; set; }
      }
  ```

###### Action Results

**Global.asaxs.cs**

- ```csharp
  public class WebApiApplication : System.Web.HttpApplication
  {
       protected void Application_Start()
       {
           GlobalConfiguration.Configure(WebApiConfig.Register);
       }
  }
  ```

**package.config**

- Define all packages that is imported

**Web.config**

--------------------------------------------------------

#### Controllers

- a *controller* is an object that handles HTTP requests

- inherit the **ApiController** class instead of the **Controller** class

- Add -> Controller -> Web API 2 Controller - Empty

- Don't need to put your controllers into a folder named Controllers. The folder name is just a convenient way to organize your source files.

- ```csharp
  public class ProductsController : ApiController
  {
      Product[] products = new Product[] 
      { 
          new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 }, 
          new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M }, 
          new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M } 
      };
  
      public IEnumerable<Product> GetAllProducts()
      {
          return products;
      }
  
      public IHttpActionResult GetProduct(int id)
      {
          var product = products.FirstOrDefault((p) => p.Id == id);
          if (product == null)
          {
              return NotFound();
          }
          return Ok(product);
      }
  }
  ```

-------------------------

#### Action Results

- ASP.NET Web API converts the return value from a controller action into an HTTP response message

- A Web API controller action can return any of the following:
  
  | Return type             | Response message                                                                                      |
  | ----------------------- | ----------------------------------------------------------------------------------------------------- |
  | void                    | Return empty 204 (No Content)                                                                         |
  | **HttpResponseMessage** | Convert directly to an HTTP response message.                                                         |
  | **IHttpActionResult**   | Call **ExecuteAsync** to create an **HttpResponseMessage**, then convert to an HTTP response message. |
  | Other type              | Write the serialized return value into the response body; return 200 (OK).                            |

- **void**
  
  - ```csharp
    public class ValuesController : ApiController
    {
        public void Post()
        {
        }
    }
    ```
  
  - HTTP Response
    
    ```
    HTTP/1.1 204 No Content
    Server: Microsoft-IIS/8.0
    Date: Mon, 27 Jan 2014 02:13:26 G
    ```

- **HttpResponseMessage**
  
  - ```csharp
    public HttpResponseMessage Get()
    {
        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "value");
        response.Content = new StringContent("hello", Encoding.Unicode);
        response.Headers.CacheControl = new CacheControlHeaderValue()
        {
            MaxAge = TimeSpan.FromMinutes(20)
        };
    
        // Get a list of products from a database.
        //IEnumerable<Product> products = GetProductsFromDB();
    
        // Write the list to the response body.
        //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, products);
        return response;
    }
    ```
  
  - HTTP Response
    
    ```
    HTTP/1.1 200 OK
    Cache-Control: max-age=1200
    Content-Length: 10
    Content-Type: text/plain; charset=utf-16
    Server: Microsoft-IIS/8.0
    Date: Mon, 27 Jan 2014 08:53:35 GMT
    
    hello
    ```

- **IHttpActionResult**
  
  - defines an **HttpResponseMessage** factory
  
  - **IHttpActionResult** contains a single method, **ExecuteAsync**, which asynchronously creates an **HttpResponseMessage** instance.
  
  - ```csharp
    public interface IHttpActionResult
    {
        Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken);
    }
    ```
  
  - If a controller action returns an **IHttpActionResult**, Web API calls the **ExecuteAsync** method to create an **HttpResponseMessage**. Then it converts the **HttpResponseMessage** into an HTTP response message.
  
  - ```csharp
    //Action Result
    public class TextResult : IHttpActionResult
    {
        string _value;
        HttpRequestMessage _request;
    
        public TextResult(string value, HttpRequestMessage request)
        {
            _value = value;
            _request = request;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage()
            {
                Content = new StringContent(_value),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }
    }
    ```
  
  - ```csharp
    //Controller
    public class ValuesController : ApiController
    {
        public IHttpActionResult Get()
        {
            return new TextResult("hello", Request);
        }
    }
    ```
  
  - HTTP Response
    
    ```
    HTTP/1.1 200 OK
    Content-Length: 5
    Content-Type: text/plain; charset=utf-8
    Server: Microsoft-IIS/8.0
    Date: Mon, 27 Jan 2014 08:53:35 GMT
    
    hello
    ```
  
  - Other Results: **BadRequestResult**, **NotFoundResult**, **JsonResult**, etc. View more at **[System.Web.Http.Results](https://msdn.microsoft.com/library/system.web.http.results.aspx)**

- **Other Return Types**
  
  - uses a [media formatter](https://docs.microsoft.com/en-us/aspnet/web-api/overview/formats-and-model-binding/media-formatters) to serialize the return value. Then Web API writes the serialized value into the response body. The response status code is 200 (OK).
  
  - Disadvantage: cannot directly return an error code, such as 404.
  
  - However, you can throw an **HttpResponseException** for error codes. See more [Exception Handling in ASP.NET Web API](https://docs.microsoft.com/en-us/aspnet/web-api/overview/error-handling/exception-handling).
  
  - ```csharp
    public class ProductsController : ApiController
    {
        public IEnumerable<Product> Get()
        {
            return GetAllProductsFromDB();
        }
    }
    ```

-----------------------

#### Other Informations

- **Entity Framework (EF)** is an object-relational mapper (ORM) that enables you to create data access applications by programming. View more [Get Start with Entity Framework]([Get started with Entity Framework 6 - EF6 | Microsoft Docs](https://docs.microsoft.com/vi-vn/ef/ef6/get-started?redirectedfrom=MSDN))

- Context: represents the Entity Framework's database context. This class derives from **DContext** and exposes **DbSet** properties that represent collections of the entities described above.

- `[Authorize]` filter checks to see if the user is authenticated. 
  
  - If not, it returns HTTP status code 401 (Unauthorized) without invoking the action
  
  - Apply the filter globally, at the controller level, or at the level of individual actions

---------------

#### Routing

- When the Web API framework receives a request, it routes the request to an action

- To determine which action to invoke, the framework uses a **routing table**

- ```csharp
  routes.MapHttpRoute(
      name: "API Default",
      routeTemplate: "api/{controller}/{id}",
      defaults: new { id = RouteParameter.Optional }
  );
  ```

- Route is defined in the **WebApiConfig.cs** file, in App_Start directory. See [Configuring ASP.NET Web API](https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/configuring-aspnet-web-api)

- In **route template**:
  
  - default route template is `"api/{controller}/{id}"`
  
  - Find controller: `ProductsController` -> add `products` to `{controller}`
  
  - Find action: name begins with that HTTP verb name, example
    
    - looks for an action prefixed with "Get", such as "GetContact" or "GetAllContacts"
    
    - applies only to GET, POST, PUT, DELETE, HEAD, OPTIONS, and PATCH verbs

- **Routing Variations**
  
  - **HTTP verbs**
    
    - `[HttpGet]`
    
    - `[HttpPut]`
    
    - `[HttpPost]`
    
    - `[HttpDelete]`
    
    - `[HttpHead]`
    
    - `[HttpOptions]`
    
    - `[HttpPatch]`
    
    - `[AcceptVerbs]` for a list of HTTP verbs
    
    - ```csharp
      public class ProductsController : ApiController
      {
          [HttpGet]
          public Product FindProduct(id) {} 
      
          [AcceptVerbs("GET", "HEAD")]
          public Product FindProduct(id) {}
      }
      ```
  
  - **Routing by Action Name**
    
    - ```csharp
      routes.MapHttpRoute(
          name: "ActionApi",
          routeTemplate: "api/{controller}/{action}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );
      ```
    
    - Ex:
      
      ```csharp
      public class ProductsController : ApiController
      {
          [HttpGet]
          public string Details(int id);
      }
      ```
      
      GET request for "api/products/details/1" would map to the `Details` method
    
    - Override the action name by using the `[ActionName]` attribute. 
      Example 2actions that map to "api/products/thumbnail/*id*"
      
      ```csharp
      public class ProductsController : ApiController
      {
          [HttpGet]
          [ActionName("Thumbnail")]
          public HttpResponseMessage GetThumbnailImage(int id);
      
          [HttpPost]
          [ActionName("Thumbnail")]
          public void AddThumbnailImage(int id);
      }
      ```
  
  - **Non-Actions**
    
    - To prevent a method from getting invoked as an action, use the `[NonAction]` attribute
      
      ```csharp
      // Not an action method.
      [NonAction]  
      public string GetPrivateData() { ... }
      ```


