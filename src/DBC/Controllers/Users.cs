using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBC.Models.DB;
using Microsoft.AspNet.Mvc;
using Mvc.JQuery.Datatables;
using Microsoft.Data.Entity;

namespace DBC.Controllers
{
    public class Users : Controller
    {
        public Users(ApplicationContext context)
        {
            DbContext = context;
        }

        public ApplicationContext DbContext { get; private set; }
        [HttpGet]
        [AllowAnonymous]
        public ViewResult Index()
        {
            //var x = DbContext.Users.Include(u => u.Logins).Select(l => l.Logins).ToArray();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult Index([FromBody]DataTablesRequest dTRequest)
        {
            return new Mvc.JQuery.Datatables.DataTables().GetJSonResult(
                DbContext.Users
                .Include(u => u.Logins)//.Select(l=>l.LoginProvider))
                .Include(u => u.Claims)
                .Include(u => u.Roles)
                , dTRequest);
        }
        [HttpGet]
        [AllowAnonymous]
        public ViewResult Create()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public PartialViewResult Edit(string Id)
        {
            
            var User=DbContext.Users.Include(u=>u.Logins).Include(u => u.Claims).Include(u => u.Roles).Where(u => u.Id == Id).FirstOrDefault();
            return PartialView(User);
        }
        [HttpPost]
        [AllowAnonymous]
        public PartialViewResult Edit(ApplicationUser model)
        {
            return PartialView();
        }
    }
}
