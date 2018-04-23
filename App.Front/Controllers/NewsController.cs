using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using App.Aplication;
using App.Core.Utils;
using App.Domain.Common;
using App.Domain.Entities.Data;
using App.Domain.Entities.Menu;
using App.Framework.Ultis;
using App.Front.Extensions;
using App.Front.Models;
using App.Service.Common;
using App.Service.Language;
using App.Service.Menu;
using App.Service.News;
using App.Service.Static;

namespace App.Front.Controllers
{
    public class NewsController : FrontBaseController
    {
        private readonly INewsService _newsService;

        private readonly IMenuLinkService _menuLinkService;

        private readonly IStaticContentService _staticContentService;

        public NewsController(
            INewsService newsService
            , IMenuLinkService menuLinkService
            , IStaticContentService staticContentService
            , IWorkContext workContext)
        {
            _newsService = newsService;
            _menuLinkService = menuLinkService;
            _staticContentService = staticContentService;
        }

        [ChildActionOnly]
        [PartialCache("Short")]
        public ActionResult BreadCrumNews(string virtualId)
        {
            ViewBag.VirtualId = virtualId;
            var lstMenuLink = new List<MenuLink>();
            var menuLinks = _menuLinkService.FindBy(x => x.TemplateType == (int)TemplateContent.News && x.Status == (int)Status.Enable, true);
            if (menuLinks.IsAny())
            {
                lstMenuLink.AddRange(menuLinks);
                ViewBag.TitleNews = menuLinks.ElementAt(0).MenuName;
            }

            return PartialView(lstMenuLink);
        }

        public ActionResult GetCareerByCategory(string virtualCategoryId, int page, string title)
        {
            var sortBuilder = new SortBuilder
            {
                ColumnName = "CreatedDate",
                ColumnOrder = SortBuilder.SortOrder.Descending
            };
            var paging = new Paging
            {
                PageNumber = page,
                PageSize = PageSize,
                TotalRecord = 0
            };
            var news = _newsService.FindAndSort(x => !x.Video && x.Status == (int)Status.Enable && x.VirtualCategoryId.Contains(virtualCategoryId), sortBuilder, paging);
            if (news.IsAny())
            {
                var pageInfo = new Helper.PageInfo(ExtentionUtils.PageSize, page, paging.TotalRecord, i => Url.Action("GetContent", "Menu", new { page = i }));
                ViewBag.PageInfo = pageInfo;
                ViewBag.CountItem = pageInfo.TotalItems;
            }
            ViewBag.Title = title;
            ViewBag.virtualCategoryId = virtualCategoryId;

            return PartialView(news);
        }

        [ChildActionOnly]
        [PartialCache("Long")]
        public ActionResult GetHomeNews()
        {
            var iePost = _newsService.GetByOption(isDisplayHomePage:true);
            var lstPost = iePost.ToList();

            iePost = from x in lstPost orderby x.OrderDisplay descending select x;

            return PartialView(iePost);
        }

        public ActionResult GetNewsByCategory(string virtualCategoryId, int? menuId, string title, int page, int? month, int? year)
        {
            var viewBag = ViewBag;

            var sortBuilder = new SortBuilder
            {
                ColumnName = "CreatedDate",
                ColumnOrder = SortBuilder.SortOrder.Descending
            };
            var paging = new Paging
            {
                PageNumber = page,
                PageSize = PageSize,
                TotalRecord = 0
            };

            var news = _newsService.FindAndSort(x => !x.Video && x.Status == (int)Status.Enable && x.VirtualCategoryId.Contains(virtualCategoryId)
            , sortBuilder, paging);

            if (news == null)
            {
                return HttpNotFound();
            }

            Expression<Func<StaticContent, bool>> status = x => x.Status == (int)Status.Enable;
            viewBag.fixItems = _staticContentService.GetTop(3, status, x => x.ViewCount);

            if (month != null)
            {
                news = news.Where(n => n.CreatedDate.Month == month);
            }

            if (year != null)
            {
                news = news.Where(n => n.CreatedDate.Year == year);
            }

            var newsLocalized = news
                .Select(x => x.ToModel());

            if (news.IsAny())
            {
                var pageInfo = new Helper.PageInfo(ExtentionUtils.PageSize, page, paging.TotalRecord, i => Url.Action("GetContent", "Menu", new { page = i }));
                ViewBag.PageInfo = pageInfo;
                ViewBag.CountItem = pageInfo.TotalItems;

                var breadCrumbs = new List<BreadCrumb>();
                var strArrays2 = virtualCategoryId.Split('/');
                for (var i1 = 0; i1 < strArrays2.Length; i1++)
                {
                    var str = strArrays2[i1];
                    var menuLink = _menuLinkService.GetByMenuName(str, title);
                    if (menuLink != null)
                    {
                        breadCrumbs.Add(new BreadCrumb
                        {
                            Title = menuLink.GetLocalized(m => m.MenuName, menuLink.Id),
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
            }

            ViewBag.MenuId = menuId;
            ViewBag.VirtualId = virtualCategoryId;
            ViewBag.Title = title;

            return PartialView(newsLocalized);
        }

        [ChildActionOnly]
        [PartialCache("Short")]
        public ActionResult GetRelativeNews(string virtualId, int newsId)
        {
            var news = new List<News>();
            var newses = _newsService.GetTop(4,
                x => x.Status == (int) Status.Enable && x.VirtualCategoryId.Contains(virtualId) && x.Id != newsId &&
                     !x.Video, x => x.ViewCount);
            if (newses.IsAny())
            {
                news.AddRange(newses);
            }

            return PartialView(news);
        }

        [ChildActionOnly]
        public ActionResult GetVideoSlide()
        {
            var news = new List<News>();
            var newses = _newsService.FindBy(x => x.Video && x.Status == (int)Status.Enable, true);
            if (newses.IsAny())
            {
                news.AddRange(newses);
            }

            return PartialView(news);
        }

        [OutputCache(CacheProfile = "Medium")]
        public ActionResult NewsDetail(string seoUrl)
        {
            var breadCrumbs = new List<BreadCrumb>();
            var news = _newsService.Get(x => x.SeoUrl.Equals(seoUrl), true);

            if (news == null)
            {
                return HttpNotFound();
            }

            var newsLocalized = new News();
            {
                newsLocalized = news.ToModel();

                ViewBag.Title = newsLocalized.MetaTitle;
                ViewBag.KeyWords = newsLocalized.MetaKeywords;
                ViewBag.SiteUrl = Url.Action("NewsDetail", "News", new {seoUrl, area = "" });
                ViewBag.Description = newsLocalized.MetaDescription;
                ViewBag.Image = Url.Content(string.Concat("~/", newsLocalized.ImageMediumSize));
                ViewBag.MenuId = newsLocalized.MenuId;
                ViewBag.VirtualId = newsLocalized.VirtualCategoryId;

                var strArrays = newsLocalized.VirtualCategoryId.Split('/');
                for (var i = 0; i < strArrays.Length; i++)
                {
                    var str = strArrays[i];
                    var menuLink = _menuLinkService.Get(x => x.CurrentVirtualId.Equals(str));

                    //Lấy bannerId từ post để hiển thị banner trên post
                    if (i == 0)
                    {
                        ViewBag.BannerId = menuLink.Id;
                    }

                    breadCrumbs.Add(new BreadCrumb
                    {
                        Title = menuLink.GetLocalized(m => m.MenuName, menuLink.Id),
                        Current = false,
                        Url = Url.Action("GetContent", "Menu", new { area = "", menu = menuLink.SeoUrl })
                    });
                }
                breadCrumbs.Add(new BreadCrumb
                {
                    Current = true,
                    Title = newsLocalized.Title
                });
                ViewBag.BreadCrumb = breadCrumbs;
            }
           
            ViewBag.SeoUrl = newsLocalized.MenuLink.SeoUrl;

            return View(newsLocalized);
        }

        /// <summary>
        /// Lấy tất cả bài viết nhóm theo tháng, năm.
        /// </summary>
        /// <returns></returns>       
        public ActionResult NewsTimeLine()
        {
            var ieNews = _newsService.GetAll();

            if (ieNews == null)
            {
                return HttpNotFound();
            }

            var ieNewsGroup = ieNews.GroupBy(l => l.CreatedDate.Year + 12 + l.CreatedDate.Month)
                .Select(g => g.First().ToModel()).ToList();

            ViewBag.NewsGroup = ieNewsGroup;

            return PartialView(ieNewsGroup);
        }
    }
}