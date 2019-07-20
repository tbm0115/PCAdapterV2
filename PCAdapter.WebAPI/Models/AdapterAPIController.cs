using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PCAdapter.WebAPI.Controllers
{
  public class AdapterAPIController:ApiController
  {
    public AdapterContext.AdapterModel _ctx;

    public AdapterAPIController()
    {
      this._ctx = new AdapterContext.AdapterModel("DefaultConnection");
    }

    protected override void Dispose(bool disposing)
    {
      this._ctx.Dispose();
      base.Dispose(disposing);
    }
  }
}