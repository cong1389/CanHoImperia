using System.Web.Mvc;
using App.Service.Manufacturers;

namespace App.Front.Controllers
{
    public class FlowStepController : Controller
    {
        private readonly IManufacturerService _manufacturerService;
        public FlowStepController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }

        public ActionResult FlowStepHome()
        {
            var flowSteps = _manufacturerService.FindBy(x => x.Status == 1);

            return PartialView(flowSteps);
        }

    }
}