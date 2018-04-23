using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Admin.Helpers;
using App.Aplication;
using App.Core.Caching;
using App.Core.Utils;
using App.Domain.Entities.Ads;
using App.FakeEntity.Ads;
using App.Framework.Ultis;
using App.Service.Ads;
using App.Service.Menu;
using AutoMapper;
using Resources;

namespace App.Admin.Controllers
{
    public class BannerController : BaseAdminController
    {
        private const string CacheBannerKey = "db.Banner";
        private readonly ICacheManager _cacheManager;

        private readonly IMenuLinkService _menuLinkService;

        private readonly IBannerService _bannerService;

        private readonly IPageBannerService _pageBannerService;
        private readonly IImagePlugin _imagePlugin;

        public BannerController(IBannerService bannerService
            , IMenuLinkService menuLinkService
            , IPageBannerService pageBannerService
            , ICacheManager cacheManager, IImagePlugin imagePlugin)
        {
            _bannerService = bannerService;
            _menuLinkService = menuLinkService;
            _pageBannerService = pageBannerService;
            _cacheManager = cacheManager;
            _imagePlugin = imagePlugin;

            //Clear cache
            _cacheManager.RemoveByPattern(CacheBannerKey);
        }

        [RequiredPermisson(Roles = "CreateEditMenu")]
        public ActionResult Create()
        {
            var model = new BannerViewModel
            {
                OrderDisplay = 1,
                Status = 1
            };

            return View(model);
        }

        [HttpPost]
        [RequiredPermisson(Roles = "CreateEditMenu")]
        public ActionResult Create(BannerViewModel model, string returnUrl)
        {
            ActionResult action;
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", MessageUI.ErrorMessage);

                    return View(model);
                }

                if (model.Image != null && model.Image.ContentLength > 0)
                {
                    var folderName = Utils.FolderName(model.Title);
                    var fileExtension = Path.GetExtension(model.Image.FileName);
                    var fileNameOriginal = Path.GetFileNameWithoutExtension(model.Image.FileName);

                    var fileName = fileNameOriginal.FileNameFormat(fileExtension);

                    _imagePlugin.CropAndResizeImage(model.Image, $"{Contains.AdsFolder}{folderName}/", fileName, ImageSize.BannerWithBigSize, ImageSize.BannerHeightBigSize, true);

                    model.ImgPath = $"{Contains.AdsFolder}{folderName}/{fileName}";
                }

                var banner = Mapper.Map<BannerViewModel, Banner>(model);
                _bannerService.Create(banner);

                Response.Cookies.Add(new HttpCookie("system_message", string.Format(MessageUI.CreateSuccess, FormUI.Banner)));
                if (!Url.IsLocalUrl(returnUrl) || returnUrl.Length <= 1 || !returnUrl.StartsWith("/") || returnUrl.StartsWith("//") || returnUrl.StartsWith("/\\"))
                {
                    action = RedirectToAction("Index");
                }
                else
                {
                    action = Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                ExtentionUtils.Log(string.Concat("Banner.Create: ", ex.Message));
                ModelState.AddModelError("", ex.Message);

                return View(model);
            }

            return action;
        }

        [RequiredPermisson(Roles = "DeleteBanner")]
        public ActionResult Delete(string[] ids)
        {
            try
            {
                if (ids.Length != 0)
                {
                    var banners =
                        from id in ids
                        select _bannerService.GetById(int.Parse(id));
                    _bannerService.BatchDelete(banners);
                }
            }
            catch (Exception exception1)
            {
                var exception = exception1;
                ExtentionUtils.Log(string.Concat("Banner.Delete: ", exception.Message));
            }
            return RedirectToAction("Index");
        }

        [RequiredPermisson(Roles = "CreateEditBanner")]
        public ActionResult Edit(int id)
        {
            var bannerViewModel = Mapper.Map<Banner, BannerViewModel>(_bannerService.GetById(id));

            return View(bannerViewModel);
        }

        [HttpPost]
        [RequiredPermisson(Roles = "CreateEditBanner")]
        public ActionResult Edit(BannerViewModel model, string returnUrl)
        {
            ActionResult action;
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", MessageUI.ErrorMessage);
                    return View(model);
                }

                var byId = _bannerService.GetById(model.Id);
                if (model.Image != null && model.Image.ContentLength > 0)
                {
                    var folderName = Utils.FolderName(model.Title);
                    var fileNameOriginal = Path.GetFileNameWithoutExtension(model.Image.FileName);
                    var fileExtension = Path.GetExtension(model.Image.FileName);

                    var fileName = fileNameOriginal.FileNameFormat(fileExtension);

                    _imagePlugin.CropAndResizeImage(model.Image, $"{Contains.AdsFolder}{folderName}/", fileName, ImageSize.BannerWithBigSize, ImageSize.BannerHeightBigSize, true);

                    model.ImgPath = $"{Contains.AdsFolder}{folderName}/{fileName}";
                    //var fileName = Path.GetFileName(model.Image.FileName);
                    //var extension = Path.GetExtension(model.Image.FileName);
                    ////fileName = string.Concat(model.FullName.NonAccent(""), extension);
                    //var str = Path.Combine(Server.MapPath(string.Concat("~/", Contains.AdsFolder)), fileName);
                    //model.Image.SaveAs(str);
                    //model.ImgPath = string.Concat(Contains.AdsFolder, fileName);
                }

                var banner = Mapper.Map(model, byId);
                _bannerService.Update(banner);

                Response.Cookies.Add(new HttpCookie("system_message", string.Format(MessageUI.UpdateSuccess, FormUI.Banner)));
                if (!Url.IsLocalUrl(returnUrl) || returnUrl.Length <= 1 || !returnUrl.StartsWith("/") || returnUrl.StartsWith("//") || returnUrl.StartsWith("/\\"))
                {
                    action = RedirectToAction("Index");
                }
                else
                {
                    action = Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ExtentionUtils.Log(string.Concat("Banner.Edit: ", ex.Message));

                return View(model);
            }
            return action;
        }

        [RequiredPermisson(Roles = "ViewBanner")]
        public ActionResult Index(int page = 1, string keywords = "")
        {
            ViewBag.Keywords = keywords;
            var sortingPagingBuilder = new SortingPagingBuilder
            {
                Keywords = keywords,
                Sorts = new SortBuilder
                {
                    ColumnName = "CreatedDate",
                    ColumnOrder = SortBuilder.SortOrder.Descending
                }
            };
            var paging = new Paging
            {
                PageNumber = page,
                PageSize = PageSize,
                TotalRecord = 0
            };
            var banners = _bannerService.PagedList(sortingPagingBuilder, paging);
            if (banners.IsAny())
            {
                var pageInfo = new Helper.PageInfo(ExtentionUtils.PageSize, page, paging.TotalRecord, i => Url.Action("Index", new { page = i, keywords }));
                ViewBag.PageInfo = pageInfo;
            }

            return View(banners);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.RouteData.Values["action"].Equals("create") || filterContext.RouteData.Values["action"].Equals("edit"))
            {
                var menuLinks = _menuLinkService.FindBy(x => x.Status == 1 && x.TemplateType != 5, true);
                ViewBag.MenuList = menuLinks;

                var pageBanners = _pageBannerService.FindBy(x => x.Status == 1);
                ViewBag.PageBanners = pageBanners;
            }
        }
    }
}