using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OwinDemo.Controllers
{
    public class UserAboutController : Controller
    {
        // GET: UserAbout
        [Authorize]
        public ActionResult GetCurrent()
        {
            string uid = User.Identity.Name;
            //这里可以调用后台用户服务，获取用户相关数所，或者验证用户权限进行相应的操作
            if (uid == "goldenkey")     //  验证用户权限
            {
                return Json(new { IsRight = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { IsRight = false }, JsonRequestBehavior.AllowGet);
        }
    }
}