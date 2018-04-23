using System;
using System.Data.Entity;
using System.Web.Mvc;
using App.Service.Ads;

namespace App.Front.Controllers
{
    public class BannerController : FrontBaseController
    {
        private readonly IBannerService _bannerService;

        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [ChildActionOnly]
        public ActionResult BannerHomeProduct()
        {
            var banners = _bannerService.FindBy(
                x => x.Status == 1 && x.PageBanner.Position == 10 &&
                     (!x.FromDate.HasValue || DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) >= 0) &&
                     (!x.ToDate.HasValue || DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) <= 0));

            return PartialView(banners);
        }

        [ChildActionOnly]
        public ActionResult BannerTop(int? menuId, string title)
        {
            var banners = _bannerService.FindBy(x =>
                x.MenuId == menuId && x.Status == 1 && x.PageBanner.Position == 1 &&
                (!x.FromDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) >= 0) &&
                (!x.ToDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) <= 0));

            ViewBag.Title = title;

            return PartialView(banners);
        }

        [ChildActionOnly]
        public ActionResult BannerTopOfNewsPage(int? menuId, string title)
        {
            var banners = _bannerService.Get(x =>
                x.MenuId == menuId && x.Status == 1 && x.PageBanner.Position == 1 &&
                (!x.FromDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) >= 0) &&
                (!x.ToDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) <= 0));

            ViewBag.Title = title;

            return PartialView(banners);
        }

        [ChildActionOnly]
        public ActionResult GetBannerBootom(int? menuId)
        {
            var banners = _bannerService.FindBy(x =>
                x.PageBanner.Position == 9 && x.Status == 1 &&
                (!x.FromDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) >= 0) &&
                (!x.ToDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) <= 0));

            return PartialView(banners);
        }

        [ChildActionOnly]
        public ActionResult GetBannerFooter()
        {
            var banners = _bannerService.FindBy(x =>
                x.PageBanner.Position == 2 && x.Status == 1 &&
                (!x.FromDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) >= 0) &&
                (!x.ToDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) <= 0));

            return PartialView(banners);
        }

        [ChildActionOnly]
        public ActionResult GetBannerLeft()
        {
            var banners = _bannerService.FindBy(x =>
                x.PageBanner.Position == 3 && x.Status == 1 &&
                (!x.FromDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) >= 0) &&
                (!x.ToDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) <= 0));

            return PartialView(banners);
        }

        [ChildActionOnly]
        public ActionResult GetBannerMiddle(int? menuId)
        {
            var banners = _bannerService.FindBy(x =>
                x.MenuId == menuId && x.PageBanner.Position == 6 && x.Status == 1 &&
                (!x.FromDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) >= 0) &&
                (!x.ToDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) <= 0));

            return PartialView(banners);
        }

        [ChildActionOnly]
        public ActionResult GetBannerOnMenu()
        {
            var banners = _bannerService.FindBy(x =>
                x.PageBanner.Position == 8 && x.Status == 1 &&
                (!x.FromDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) >= 0) &&
                (!x.ToDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) <= 0));

            return PartialView(banners);
        }

        [ChildActionOnly]
        public ActionResult GetBannerRight()
        {
            var banners = _bannerService.FindBy(x =>
                x.PageBanner.Position == 4 && x.Status == 1 &&
                (!x.FromDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) >= 0) &&
                (!x.ToDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) <= 0));

            return PartialView(banners);
        }

        [ChildActionOnly]
        public ActionResult GetBannerSideBar(int? menuId)
        {
            var banners = _bannerService.FindBy(x =>
                x.PageId == 7 && x.PageBanner.Position == 5 && x.Status == 1 &&
                (!x.FromDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) >= 0) &&
                (!x.ToDate.HasValue ||
                 DbFunctions.DiffHours(x.ToDate.Value, DateTimeOffset.UtcNow.Offset) <= 0));

            return PartialView(banners);
        }
    }
}