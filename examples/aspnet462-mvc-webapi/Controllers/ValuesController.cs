using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace aspnet462_mvc_webapi.Controllers
{
  public class ValuesController : ApiController
  {
    // GET: api/Values
    public IEnumerable<string> Get()
    {
      throw new NotImplementedException("Unhandled Web API exception");

      return new string[] { "value1", "value2" };
    }

    // GET: api/Values/5
    public string Get(int id)
    {
      return "value";
    }

    // POST: api/Values
    public void Post([FromBody] string value)
    {
    }

    // PUT: api/Values/5
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE: api/Values/5
    public void Delete(int id)
    {
    }
  }
}
