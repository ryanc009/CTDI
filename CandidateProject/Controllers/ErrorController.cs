using System.Web.Mvc;

namespace CandidateProject.Controllers
{
    public class ErrorController : Controller
    {
        private const string _viewName = "Error";

        // GET: Error
        public ActionResult Index()
        {
            ViewBag.Message = "Internal Server Error.";
            return View(_viewName);
        }

        public ActionResult NotFound()
        {
            ViewBag.Message = "The resource being requested could not be located.";
            return View(_viewName);
        }

        public ActionResult BadRequest()
        {
            ViewBag.Message = "The request was not formatted properly.";
            return View(_viewName);
        }
    }
}