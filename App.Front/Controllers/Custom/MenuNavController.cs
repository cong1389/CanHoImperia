using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Aplication;
using App.Domain.Common;
using App.FakeEntity.Menu;
using App.Front.Extensions;
using App.Front.Models;
using App.Service.Menu;

namespace App.Front.Controllers.Custom
{
    public class MenuNavController : FrontBaseController
    {
        private readonly IMenuLinkService _menuLinkService;

        public MenuNavController(IMenuLinkService menuLinkService)
        {
            _menuLinkService = menuLinkService;
        }

        [PartialCache("Long")]
        private List<MenuNavViewModel> CreateMenuNav(int? parentId, IEnumerable<MenuNavViewModel> source)
        {
            var ieMenuNav = (from x in source
                                                orderby x.OrderDisplay descending
                                                select x).Where(x =>
                                                {
                                                    var nullable1 = x.ParentId;
                                                    var nullable = parentId;
                                                    if (nullable1.GetValueOrDefault() != nullable.GetValueOrDefault())
                                                    {
                                                        return false;
                                                    }
                                                    return nullable1.HasValue == nullable.HasValue;
                                                }).Select(x => new MenuNavViewModel
                                                {
                                                    MenuId = x.MenuId,
                                                    ParentId = x.ParentId,
                                                    MenuName = x.MenuName,
                                                    SeoUrl = x.SeoUrl,
                                                    OrderDisplay = x.OrderDisplay,
                                                    ImageUrl = x.ImageUrl,
                                                    CurrentVirtualId = x.CurrentVirtualId,
                                                    VirtualId = x.VirtualId,
                                                    TemplateType = x.TemplateType,
                                                    OtherLink = x.OtherLink,
                                                    IconNav = x.IconNav,
                                                    IconBar = x.IconBar,
                                                    ChildNavMenu = CreateMenuNav(x.MenuId, source)
                                                }).ToList();

            return ieMenuNav;
        }

        [ChildActionOnly]
        public ActionResult GetFooterLink()
        {
            var menuNavs = new List<MenuNavViewModel>();

            var menuLinks = _menuLinkService.GetByOption(new List<int> { (int)Position.Footer });

            if (!menuLinks.IsAny())
            {
                return PartialView(menuNavs);
            }

            var menuNav = menuLinks.Select(x => new MenuNavViewModel
            {
                MenuId = x.Id,
                ParentId = x.ParentId,
                MenuName = x.MenuName,
                SeoUrl = x.SeoUrl,
                OrderDisplay = x.OrderDisplay,
                ImageUrl = x.ImageBigSize,
                CurrentVirtualId = x.CurrentVirtualId,
                VirtualId = x.VirtualId,
                TemplateType = x.TemplateType,
                IconNav = x.ImageMediumSize,
                IconBar = x.ImageSmallSize
            });
            menuNavs = CreateMenuNav(null, menuNav);

            return PartialView(menuNavs);
        }

        [NonAction]
        [PartialCache("Long")]
        public ActionResult TopMenu()
        {
            var menuLinks = _menuLinkService.GetByOption(new List<int> { (int)Position.Top });

            //Convert to localized
            menuLinks = menuLinks.Select(x => x.ToModel());

            var menuNavs = new List<MenuNavViewModel>();
            if (!menuLinks.Any())
            {
                return PartialView(menuNavs);
            }

            var menuNav = menuLinks.Select(x => new MenuNavViewModel
            {
                MenuId = x.Id,
                ParentId = x.ParentId,
                MenuName = x.MenuName,
                SeoUrl = x.SeoUrl,
                OtherLink = x.SourceLink,
                OrderDisplay = x.OrderDisplay,
                ImageUrl = x.ImageBigSize,
                CurrentVirtualId = x.CurrentVirtualId,
                VirtualId = x.VirtualId,
                TemplateType = x.TemplateType,
                IconNav = x.ImageMediumSize,
                IconBar = x.ImageSmallSize
            });

            menuNavs = CreateMenuNav(null, menuNav);

            return PartialView(menuNavs);
        }

        [ChildActionOnly]
        public ActionResult GetTopMenu()
        {
            return TopMenu();
        }

        [ChildActionOnly]
        public ActionResult GetTopMenuMobile()
        {
            return TopMenu();
        }

        [ChildActionOnly]
        //[PartialCache("Long")]
        public ActionResult GetTopHead()
        {
            var menuNavs = new List<MenuNavViewModel>();
            var menuLinks = _menuLinkService.GetByOption(new List<int> { (int)Position.TopHead });

            //Convert to localized
            menuLinks = menuLinks.Select(x => x.ToModel());

            if (!menuLinks.Any())
            {
                return PartialView(menuNavs);
            }

            var menuNav = menuLinks.Select(x => new MenuNavViewModel
            {
                MenuId = x.Id,
                ParentId = x.ParentId,
                MenuName = x.MenuName,
                SeoUrl = x.SeoUrl,
                OrderDisplay = x.OrderDisplay,
                ImageUrl = x.ImageBigSize,
                CurrentVirtualId = x.CurrentVirtualId,
                VirtualId = x.VirtualId,
                TemplateType = x.TemplateType,
                IconNav = x.ImageMediumSize,
                IconBar = x.ImageSmallSize
            });

            return PartialView(menuNav);
        }
        
        [ChildActionOnly]
        public PartialViewResult MenuNavLeft(string virtualId)
        {
            var ieMenuNav = PrepareMenuNavBase(virtualId);

            return PartialView(ieMenuNav);
        }

        [ChildActionOnly]
        public PartialViewResult MenuNavNews(string virtualId)
        {
            var ieMenuNav = PrepareMenuNavBase(virtualId);

            return PartialView(ieMenuNav);
        }

        [ChildActionOnly]
        public PartialViewResult MenuHomeSlide(string virtualId)
        {
            var ieMenuNav = PrepareMenuNavBase(virtualId);

            return PartialView(ieMenuNav);
        }

        [NonAction]
        private IEnumerable<MenuNavViewModel> PrepareMenuNavBase(string virtualId)
        {
            virtualId = virtualId != null && virtualId.Count(i => i.Equals('/')) > 0
                ? virtualId.Split('/')[0]
                : virtualId;

            var menuLinks = _menuLinkService.GetByOption(virtualId: virtualId);

            if (!menuLinks.Any())
            {
                return null;
            }

            //Convert to localized
            menuLinks = menuLinks.Where(x => x.ParentId != null).Select(x => x.ToModel());

            var ieMenuNav = menuLinks.Select(x => new MenuNavViewModel
            {
                MenuId = x.Id,
                ParentId = x.ParentId,
                MenuName = x.MenuName,
                SeoUrl = x.SeoUrl,
                OrderDisplay = x.OrderDisplay,
                ImageUrl = x.ImageBigSize,
                CurrentVirtualId = x.CurrentVirtualId,
                VirtualId = x.VirtualId,
                TemplateType = x.TemplateType,
                IconNav = x.ImageMediumSize,
                IconBar = x.ImageSmallSize
            });

            return ieMenuNav;

        }

        [ChildActionOnly]
        [PartialCache("Long")]
        public ActionResult GetMenuCategory()
        {
            var menuNavs = new List<MenuNavViewModel>();
            var menuLinks = _menuLinkService.GetByOption(new List<int> { (int)Position.Top, (int)Position.SiderBar });

            if (menuLinks.Any())
            {
                var menuNav =
                    from x in menuLinks
                    select new MenuNavViewModel
                    {
                        MenuId = x.Id,
                        ParentId = x.ParentId,
                        MenuName = x.MenuName,
                        SeoUrl = x.SeoUrl,
                        OrderDisplay = x.OrderDisplay,
                        ImageUrl = x.ImageBigSize,
                        CurrentVirtualId = x.CurrentVirtualId,
                        VirtualId = x.VirtualId,
                        TemplateType = x.TemplateType,
                        IconNav = x.ImageMediumSize,
                        IconBar = x.ImageSmallSize
                    };
                menuNavs = CreateMenuNav(null, menuNav);
            }
            return PartialView(menuNavs);
        }

        [ChildActionOnly]
        public ActionResult GetMenuVerticalMega()
        {
            var menuLinks = _menuLinkService.GetByOption(new List<int> { (int)Position.SiderBar });

            if (!menuLinks.Any())
            {
                return HttpNotFound();
            }

            var menuNav =
                from x in menuLinks
                select new MenuNavViewModel
                {
                    MenuId = x.Id,
                    ParentId = x.ParentId,
                    MenuName = x.MenuName,
                    SeoUrl = x.SeoUrl,
                    OrderDisplay = x.OrderDisplay,
                    ImageUrl = x.ImageBigSize,
                    CurrentVirtualId = x.CurrentVirtualId,
                    VirtualId = x.VirtualId,
                    TemplateType = x.TemplateType,
                    IconNav = x.ImageMediumSize,
                    IconBar = x.ImageSmallSize
                };

            var menuNavs = CreateMenuNav(null, menuNav);

            return PartialView(menuNavs);
        }

        [ChildActionOnly]
        public ActionResult GetMenuLinkSideBar(string virtualId, List<int> proAttrs = null)
        {
            virtualId = virtualId != null && virtualId.Count(i => i.Equals('/')) > 0
                ? virtualId.Split('/')[0]
                : virtualId;

            var menuLinks = _menuLinkService.GetByOption(virtualId: virtualId);

            if (!menuLinks.IsAny())
            {
                return HttpNotFound();
            }

            //Convert to localized
            menuLinks = menuLinks.Select(x => x.ToModel());

            var menuNav =
                from x in menuLinks
                select new MenuNavViewModel
                {
                    MenuId = x.Id,
                    ParentId = x.ParentId,
                    MenuName = x.MenuName,
                    SeoUrl = x.SeoUrl,
                    OrderDisplay = x.OrderDisplay,
                    ImageUrl = x.ImageBigSize,
                    CurrentVirtualId = x.CurrentVirtualId,
                    VirtualId = x.VirtualId,
                    TemplateType = x.TemplateType,
                    IconNav = x.ImageMediumSize,
                    IconBar = x.ImageSmallSize
                };


            IEnumerable<MenuNavViewModel> menuNavs = CreateMenuNav(null, menuNav);

            ViewBag.ProIds = proAttrs;

            return PartialView(menuNavs);
        }
        
        [ChildActionOnly]
        public ActionResult Header()
        {
            return PartialView();
        }

        [PartialCache("Long")]
        public ActionResult StickyBar()
        {
            var menuNavs = new List<MenuNavViewModel>();
            var menuLinks = _menuLinkService.GetByOption(new List<int> { (int)Position.Middle }, isDisplaySearch: true);

            if (menuLinks.Any())
            {
                var menuNav =
                    from x in menuLinks
                    select new MenuNavViewModel
                    {
                        MenuId = x.Id,
                        ParentId = x.ParentId,
                        MenuName = x.MenuName,
                        SeoUrl = x.SeoUrl,
                        OrderDisplay = x.OrderDisplay,
                        ImageUrl = x.ImageBigSize,
                        CurrentVirtualId = x.CurrentVirtualId,
                        VirtualId = x.VirtualId,
                        TemplateType = x.TemplateType,
                        IconNav = x.ImageMediumSize,
                        IconBar = x.ImageSmallSize
                    };
                menuNavs = CreateMenuNav(null, menuNav);
            }
            return PartialView(menuNavs);
        }
    }


}