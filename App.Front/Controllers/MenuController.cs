using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Aplication;
using App.Aplication.Extensions;
using App.Domain.Common;
using App.Domain.Entities.Menu;
using App.Front.Extensions;
using App.Front.Models;
using App.Service.Language;
using App.Service.Menu;
using App.Service.Static;
using Newtonsoft.Json;

namespace App.Front.Controllers
{
    public class MenuController : FrontBaseController
    {
        private readonly IMenuLinkService _menuLinkService;

        private readonly IStaticContentService _staticContentService;

        public MenuController(
            IMenuLinkService menuLinkService
            , IStaticContentService staticContentService
            )
        {
            _menuLinkService = menuLinkService;
            _staticContentService = staticContentService;
        }

        [PartialCache("Short")]
        public ActionResult GetContent(string menu, int page)
        {
            var viewBag = ViewBag;

            var menuLink = _menuLinkService.GetBySeoUrl(menu);

            if (menuLink == null)
            {
                return View();
            }

            var menuLinkLocalized = menuLink.ToModel();

            ViewBag.Title = menuLinkLocalized.MetaTitle ?? menuLinkLocalized.MenuName;
            ViewBag.MetaKeyWords = menuLinkLocalized.MetaKeywords;
            ViewBag.SiteUrl = Url.Action("GetContent", "Menu", new { menu, page, area = "" });
            ViewBag.Description = menuLinkLocalized.MetaDescription;
            ViewBag.Image = Url.Content(string.Concat("~/", menuLinkLocalized.ImageBigSize));

            //((dynamic)base.ViewBag).Title = menuLinkLocalized.MetaTitle;
            //((dynamic)base.ViewBag).KeyWords = menuLinkLocalized.MetaKeywords;
            //((dynamic)base.ViewBag).SiteUrl = base.Url.Action("GetContent", "Menu", new { menu = menu, page = page, area = "" });
            //((dynamic)base.ViewBag).Description = menuLinkLocalized.MetaDescription;
            //((dynamic)base.ViewBag).ImageBigSize = base.Url.Content(string.Concat("~/", menuLinkLocalized.ImageBigSize));

            if (menuLinkLocalized.TemplateType == (int)TemplateContent.News)
            {
                viewBag.MenuList = _menuLinkService.GetByOption(template: new List<int> { (int)TemplateContent.News });
                //IMenuLinkService menuLinkService = this._menuLinkService;
                //viewBag.MenuList = _menuLinkService.FindBy((MenuLink x) => x.TemplateType == 1, false);
            }


            ViewBag.ParentId = menuLink.ParentId;
            ViewBag.Attrs = Request["attribute"];
            ViewBag.Prices = Request["price"];
            ViewBag.KeyWords = Request["keywords"];
            ViewBag.ProAttrs = Request["proattribute"];

            ViewBag.ProductHot = Request["producthot"];
            ViewBag.ProductOld = Request["productold"];
            ViewBag.ProductNew = Request["productnew"];

            ViewBag.TemplateType = menuLink.TemplateType;
            ViewBag.MenuId = menuLink.Id;
            ViewBag.ImgePath = menuLink.ImageBigSize;
            ViewBag.VirtualId = menuLink.VirtualId;
            ViewBag.PageNumber = page;

            return View();
        }

        [PartialCache("Short")]
        public ActionResult GetFixItemContent(int id)
        {
            var menuLink = _menuLinkService.GetById(id);

            ViewBag.ImgUrl = menuLink.ImageBigSize;
            ViewBag.TitleFix = menuLink.MenuName;

            var menuLinks = _menuLinkService.GetByOption(parentId: new List<int> { id }, isDisplayHomePage: true);

            if (!menuLinks.IsAny())
            {
                return Json(new { data = "", success = true }
                    , JsonRequestBehavior.AllowGet);
            }

            return Json(new { data = this.RenderRazorViewToString("_PartialFixItemContent", menuLinks), success = true },
                JsonRequestBehavior.AllowGet);
        }

        [PartialCache("Short")]
        public ActionResult GetLeftFixItem(int id)
        {
            var menuLinks = _menuLinkService.GetByOption(parentId: new List<int> { id }, isDisplayHomePage: true);

            if (!menuLinks.IsAny())
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { data = this.RenderRazorViewToString("_PartialLeftFixItemHome", menuLinks), success = true },
                JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        [PartialCache("Short")]
        public ActionResult MenuHomeTab()
        {
            var menuLinks = _menuLinkService.GetByOption(template: new List<int> { (int)TemplateContent.Product }, isDisplayHomePage: true);

            return PartialView(menuLinks);
        }

        [ChildActionOnly]
        public ActionResult GetStaticContent(int menuId, string virtualId, string title)
        {
            var breadCrumbs = new List<BreadCrumb>();
            var virtualIds = virtualId.Split('/');

            for (var i = 0; i < virtualIds.Length; i++)
            {
                var str = virtualIds[i];
                var menuLink = _menuLinkService.GetByParentId(menuId, str);

                if (menuLink != null)
                {
                    breadCrumbs.Add(new BreadCrumb
                    {
                        Title = menuLink.GetLocalized(x => x.MenuName, menuLink.Id),
                        Current = false,
                        Url = Url.Action("GetContent", "Menu", new { area = "", menu = menuLink.SeoUrl })
                    });
                }
            }

            breadCrumbs.Add(new BreadCrumb
            {
                Current = true,
                Title = title
            });
            ViewBag.BreadCrumb = breadCrumbs;

            var staticContent = _staticContentService.Get(x => x.MenuId == menuId, true);
            if (staticContent != null)
            {
                staticContent = staticContent.ToModel();

                var viewCount = staticContent;
                viewCount.ViewCount = viewCount.ViewCount + 1;
                _staticContentService.Update(staticContent);

                //Lấy bannerId từ post để hiển thị banner trên post
                ViewBag.BannerId = staticContent.MenuId;

                ViewBag.Title = staticContent.Title;
            }

            ViewBag.MenuId = menuId;

            return PartialView(staticContent);
        }

        public ActionResult GetStaticContentParent(int menuId, string title, string virtualId)
        {
            var breadCrumbs = new List<BreadCrumb>();
            var virtualIds = virtualId.Split('/');

            var staticContent = _staticContentService.Get(x => x.MenuId == menuId && x.Status == 1);

            //Convert to localized
            var staticContentLocalized = staticContent.ToModel();

            var viewBag = ViewBag;

            var menuLinks = _menuLinkService.FindBy(x => x.Id == menuId && x.Status == 1);

            //Convert to localized
            menuLinks = menuLinks.Select(x => x.ToModel());

            if (menuLinks.IsAny())
            {
                viewBag.ListItems = menuLinks;
            }

            //var strArrays1 = virtualIds;
            for (var i = 0; i < virtualIds.Length; i++)
            {
                var item = virtualIds[i];
                var menuLink = _menuLinkService.Get(x => x.CurrentVirtualId.Equals(item) && !x.MenuName.Equals(title));
                if (menuLink != null)
                {
                    breadCrumbs.Add(new BreadCrumb
                    {
                        Title = menuLink.GetLocalized(x => x.MenuName, menuLink.Id),
                        Current = false,
                        Url = Url.Action("GetContent", "Menu", new { area = "", menu = menuLink.SeoUrl })
                    });
                }
            }

            breadCrumbs.Add(new BreadCrumb
            {
                Current = true,
                Title = staticContentLocalized.Title
            });

            ViewBag.TitleNews = staticContentLocalized.Title;
            ViewBag.BreadCrumb = breadCrumbs;
            ViewBag.Title = staticContentLocalized.Title;

            return PartialView(staticContentLocalized);
        }

        [ChildActionOnly]
        public ActionResult GetStaticHot(string virtualId)
        {
            var str = virtualId;
            var strArrays = str.Split('/');

            if (strArrays.Length >= 3)
            {
                str = $"{strArrays[0]}/{strArrays[1]}";
            }

            var menuLinks = _menuLinkService.FindBy(x => x.VirtualId.Contains(str) && x.TemplateType == (int)TemplateContent.FixItem);

            return PartialView(menuLinks);
        }

        public ActionResult Search(SeachConditions conditions)
        {
            var objCondition = JsonConvert.SerializeObject(conditions);
            var cookie = new HttpCookie("system_search", objCondition) { Expires = DateTime.Now.AddDays(1.0) };
            Response.Cookies.Add(cookie);

            var byId = new MenuLink();
            var menuLinks = _menuLinkService.GetByOption(isDisplayHomePage: true, template: new List<int> { (int)TemplateContent.Product });

            if (menuLinks.IsAny())
            {
                byId = menuLinks.FirstOrDefault();
            }

            return RedirectToAction("SearchResult", "Post",
                new { catUrl = byId?.SeoUrl, parameters = conditions.Keywords.NonAccent(), area = "" });
        }
    }
}