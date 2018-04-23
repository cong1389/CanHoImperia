using System.Web.Mvc;
using App.Front.Extensions;
using App.Service.Menu;
using App.Service.Static;

namespace App.Front.Controllers
{
    public class StaticContentController : FrontBaseController
    {
        private readonly IStaticContentService _staticContentService;

        public StaticContentController(IStaticContentService staticContentService            
            , IMenuLinkService menuLinkService
            )
        {
            _staticContentService = staticContentService;
        }

        // GET: StaticContent
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult GetSlogan(int menuId)
        {
            var staticContent = _staticContentService.Get(x => x.Id == menuId, true);

            if (staticContent == null)
            {
                return HttpNotFound();
            }

            var staticContentLocalized = staticContent.ToModel();

            return PartialView(staticContentLocalized);
        }

        [ChildActionOnly]
        public ActionResult PostDetailPolices(int menuId)
        {
            var staticContent = _staticContentService.Get(x => x.Id == menuId, true);

            if (staticContent == null)
            {
                return HttpNotFound();
            }

            var staticContentLocalized = staticContent.ToModel();

            return PartialView(staticContentLocalized);
        }

        [ChildActionOnly]
        public ActionResult GetHomeIntro(int menuId)
        {
            var staticContent = _staticContentService.Get(x => x.Id == menuId, true);

            if (staticContent == null)
            {
                return HttpNotFound();
            }

            var staticContentLocalized = staticContent.ToModel();

            return PartialView(staticContentLocalized);
        }
    }
}