using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Paper.Client.Mvc.Controllers
{
  [Route("[controller]/1/[action]")]
  public class ApiController : Controller
  {
    public IActionResult Query()
    {
      return View("Outer/Query", this.User);
    }
  }
}