using System;
using System.Web.Http;

namespace Web.MVCApp.Controllers
{
    public class DataController : ApiController
    {
        [HttpGet]
        public int AddOne(int id)
        {
            return id + 1;
        }

        [HttpGet]
        public int AddOneExp(int id)
        {
            throw new ArithmeticException("Cant add one to " + id);
        }

    }
}
