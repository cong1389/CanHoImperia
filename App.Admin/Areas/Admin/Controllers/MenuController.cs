using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Admin.Helpers;
using App.Aplication;
using App.Core.Caching;
using App.Domain.Entities.Menu;
using App.FakeEntity.Menu;
using App.Framework.Ultis;
using App.Service.Language;
using App.Service.LocalizedProperty;
using App.Service.Menu;
using AutoMapper;
using Resources;

namespace App.Admin.Controllers
{
    public class MenuController : BaseAdminController
    {
        private const string CacheMenuKey = "db.MenuLink";
        private readonly ICacheManager _cacheManager;

        private readonly IMenuLinkService _menuLinkService;

        private readonly ILocalizedPropertyService _localizedPropertyService;

        private readonly ILanguageService _languageService;
        private readonly IImagePlugin _imagePlugin;

        public MenuController(IMenuLinkService menuLinkService, ILocalizedPropertyService localizedPropertyService
            , ILanguageService languageService
           , ICacheManager cacheManager, IImagePlugin imagePlugin)
        {
            _menuLinkService = menuLinkService;
            _localizedPropertyService = localizedPropertyService;
            _languageService = languageService;
            _cacheManager = cacheManager;
            _imagePlugin = imagePlugin;

            //Clear cache
            _cacheManager.RemoveByPattern(CacheMenuKey);
        }

        [RequiredPermisson(Roles = "CreateEditMenu")]
        public ActionResult Create()
        {
            var model = new MenuLinkViewModel
            {
                OrderDisplay = 1,
                Status = 1
            };

            //Add locales to model
            AddLocales(_languageService, model.Locales);

            return View(model);
        }

        [HttpPost]
        [RequiredPermisson(Roles = "CreateEditMenu")]
        public ActionResult Create(MenuLinkViewModel model, string returnUrl)
        {
            ActionResult action;
            try
            {
                if (!ModelState.IsValid)
                {
                    var messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                            .Select(v => v.ErrorMessage + " " + v.Exception));
                    ModelState.AddModelError("", messages);
                    return View(model);
                }

                var menuName = model.MenuName.NonAccent();
                var bySeoUrl = _menuLinkService.GetListSeoUrl(menuName);
                model.SeoUrl = model.MenuName.NonAccent();

                if (bySeoUrl.Any(x => x.Id != model.Id))
                {
                    var menuLinkViewModel = model;
                    var seoUrl = menuLinkViewModel.SeoUrl;
                    var num = bySeoUrl.Count();
                    menuLinkViewModel.SeoUrl = string.Concat(seoUrl, "-", num.ToString());
                }

                var folderName = Utils.FolderName(model.MenuName);
                if (model.ImageBigSizeFile != null && model.ImageBigSizeFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(model.ImageBigSizeFile.FileName);
                    var fileExtension = Path.GetExtension(model.ImageBigSizeFile.FileName);
                    var fileNameFormat = fileName.FileNameFormat(fileExtension);

                    _imagePlugin.CropAndResizeImage(model.ImageBigSizeFile, $"{Contains.MenuFolder}{folderName}/", fileNameFormat, ImageSize.MenuWithBigSize, ImageSize.MenuHeightBigSize, true);
                    model.ImageBigSize = $"{Contains.MenuFolder}{folderName}/{fileNameFormat}";
                }

                if (model.ImageMediumSizeFile != null && model.ImageMediumSizeFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(model.ImageMediumSizeFile.FileName);
                    var fileExtension = Path.GetExtension(model.ImageMediumSizeFile.FileName);
                    var fileNameFormat = fileName.FileNameFormat(fileExtension);

                    _imagePlugin.CropAndResizeImage(model.ImageMediumSizeFile, $"{Contains.MenuFolder}{folderName}/", fileNameFormat, ImageSize.MenuWithMediumSize, ImageSize.MenuHeightMediumSize, true);
                    model.ImageMediumSize = $"{Contains.MenuFolder}{folderName}/{fileNameFormat}";
                }

                if (model.ImageSmallSizeFile != null && model.ImageSmallSizeFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(model.ImageSmallSizeFile.FileName);
                    var fileExtension = Path.GetExtension(model.ImageSmallSizeFile.FileName);
                    var fileNameFormat = fileName.FileNameFormat(fileExtension);

                    _imagePlugin.CropAndResizeImage(model.ImageSmallSizeFile, $"{Contains.MenuFolder}{folderName}/", fileNameFormat, ImageSize.MenuWithSmallSize, ImageSize.MenuHeightSmallSize, true);
                    model.ImageSmallSize = $"{Contains.MenuFolder}{folderName}/{fileNameFormat}";
                }

                var guid = Guid.NewGuid().ToString();
                if (model.ParentId.HasValue)
                {
                    model.CurrentVirtualId = guid;
                    var byId = _menuLinkService.GetById(model.ParentId.Value);
                    model.VirtualId = $"{byId.VirtualId}/{guid}";
                    model.VirtualSeoUrl = $"{byId.SeoUrl}/{model.SeoUrl}";
                }
                else
                {
                    model.VirtualId = guid;
                    model.CurrentVirtualId = guid;
                }

                var modelMap = Mapper.Map<MenuLinkViewModel, MenuLink>(model);
                _menuLinkService.Create(modelMap);

                //Update Localized   
                foreach (var localized in model.Locales)
                {
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.MenuName, localized.MenuName, localized.LanguageId);
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.SeoUrl, localized.MenuName.NonAccent(), localized.LanguageId);
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.MetaTitle, localized.MetaTitle, localized.LanguageId);
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.MetaKeywords, localized.MetaKeywords, localized.LanguageId);
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.MetaDescription, localized.MetaDescription, localized.LanguageId);
                }

                Response.Cookies.Add(new HttpCookie("system_message", string.Format(MessageUI.CreateSuccess, FormUI.MenuLink)));
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
                ExtentionUtils.Log(string.Concat("MenuLink.Create: ", ex.Message));

                return View(model);
            }
            return action;
        }

        [RequiredPermisson(Roles = "DeleteMenu")]
        public ActionResult Delete(string[] ids)
        {
            try
            {
                if (ids.Length != 0)
                {
                    var menuLinks =
                        from id in ids
                        select _menuLinkService.GetById(int.Parse(id));
                    _menuLinkService.BatchDelete(menuLinks);

                    //Delete localize
                    for (var i = 0; i < ids.Length; i++)
                    {
                        var ieLocalizedProperty
                            = _localizedPropertyService.GetByEntityId(int.Parse(ids[i]));
                        _localizedPropertyService.BatchDelete(ieLocalizedProperty);
                    }
                }
            }
            catch (Exception ex)
            {
                ExtentionUtils.Log(string.Concat("MenuLink.Delete: ", ex.Message));
                ModelState.AddModelError("", ex.Message);

            }
            return RedirectToAction("Index");
        }


        [RequiredPermisson(Roles = "DeleteMenu")]
        public ActionResult DeleteOne(int id)
        {
            try
            {
                var menuLink = _menuLinkService.GetById(id);

                _menuLinkService.Delete(menuLink);
            }
            catch (Exception ex)
            {
                ExtentionUtils.Log(string.Concat("MenuLink.DeleteById: ", ex.Message));

                Response.Cookies.Add(new HttpCookie("system_message", string.Format(MessageUI.ErrorMessageWithFormat, ex.Message)));

            }
            return RedirectToAction("Index");
        }

        [RequiredPermisson(Roles = "CreateEditMenu")]
        public ActionResult Edit(int id)
        {
            var modelMap = Mapper.Map<MenuLink, MenuLinkViewModel>(_menuLinkService.GetById(id));

            //Add Locales to model
            AddLocales(_languageService, modelMap.Locales, (locale, languageId) =>
            {
                locale.Id = modelMap.Id;
                locale.LocalesId = modelMap.Id;
                locale.MenuName = modelMap.GetLocalized(x => x.MenuName, id, languageId, false, false);
                locale.MetaTitle = modelMap.GetLocalized(x => x.MetaTitle, id, languageId, false, false);
                locale.MetaKeywords = modelMap.GetLocalized(x => x.MetaKeywords, id, languageId, false, false);
                locale.MetaDescription = modelMap.GetLocalized(x => x.MetaDescription, id, languageId, false, false);
                locale.SeoUrl = modelMap.GetLocalized(x => x.SeoUrl, id, languageId, false, false);
            });

            return View(modelMap);
        }

        [HttpPost]
        [RequiredPermisson(Roles = "CreateEditMenu")]
        public ActionResult Edit(MenuLinkViewModel model, string returnUrl)
        {
            ActionResult action;
            try
            {
                if (!ModelState.IsValid)
                {
                    var messages = string.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                             .Select(v => v.ErrorMessage + " " + v.Exception));
                    ModelState.AddModelError("", messages);

                    return View(model);
                }

                var byId = _menuLinkService.GetById(model.Id);
                var menuName = model.MenuName.NonAccent();

                var bySeoUrl = _menuLinkService.GetListSeoUrl(menuName);
                model.SeoUrl = model.MenuName.NonAccent();
                if (bySeoUrl.Any(x => x.Id != model.Id))
                {
                    var menuLinkViewModel = model;
                    var seoUrl = menuLinkViewModel.SeoUrl;
                    var num = bySeoUrl.Count();
                    menuLinkViewModel.SeoUrl = string.Concat(seoUrl, "-", num.ToString());
                }

                var folderName = Utils.FolderName(model.MenuName);
                if (model.ImageBigSizeFile != null && model.ImageBigSizeFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(model.ImageBigSizeFile.FileName);
                    var fileExtension = Path.GetExtension(model.ImageBigSizeFile.FileName);
                    var fileNameFormat = fileName.FileNameFormat(fileExtension);

                    _imagePlugin.CropAndResizeImage(model.ImageBigSizeFile, $"{Contains.MenuFolder}{folderName}/", fileNameFormat, ImageSize.MenuWithBigSize, ImageSize.MenuHeightBigSize, true);
                    model.ImageBigSize = $"{Contains.MenuFolder}{folderName}/{fileNameFormat}";
                }

                if (model.ImageMediumSizeFile != null && model.ImageMediumSizeFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(model.ImageMediumSizeFile.FileName);
                    var fileExtension = Path.GetExtension(model.ImageMediumSizeFile.FileName);
                    var fileNameFormat = fileName.FileNameFormat(fileExtension);

                    _imagePlugin.CropAndResizeImage(model.ImageMediumSizeFile, $"{Contains.MenuFolder}{folderName}/", fileNameFormat, ImageSize.MenuWithMediumSize, ImageSize.MenuHeightMediumSize, true);
                    model.ImageMediumSize = $"{Contains.MenuFolder}{folderName}/{fileNameFormat}";
                }
                if (model.ImageSmallSizeFile != null && model.ImageSmallSizeFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(model.ImageSmallSizeFile.FileName);
                    var fileExtension = Path.GetExtension(model.ImageSmallSizeFile.FileName);
                    var fileNameFormat = fileName.FileNameFormat(fileExtension);

                    _imagePlugin.CropAndResizeImage(model.ImageSmallSizeFile, $"{Contains.MenuFolder}{folderName}/", fileNameFormat, ImageSize.MenuWithSmallSize, ImageSize.MenuHeightSmallSize, true);
                    model.ImageSmallSize = $"{Contains.MenuFolder}{folderName}/{fileNameFormat}";
                }

                var parentId = model.ParentId;
                if (!parentId.HasValue)
                {
                    model.ParentId = null;
                    model.VirtualId = byId.CurrentVirtualId;
                    model.VirtualSeoUrl = null;
                }
                else
                {
                    var byId1 = _menuLinkService.GetById(parentId.Value);
                    model.VirtualId = $"{byId1.VirtualId}/{byId.CurrentVirtualId}";
                    model.VirtualSeoUrl = $"{byId1.SeoUrl}/{model.SeoUrl}";
                }

                var modelMap = Mapper.Map(model, byId);
                _menuLinkService.Update(byId);

                //Update Localized   
                foreach (var localized in model.Locales)
                {
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.MenuName, localized.MenuName, localized.LanguageId);
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.SeoUrl, localized.MenuName.NonAccent(), localized.LanguageId);
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.MetaTitle, localized.MetaTitle, localized.LanguageId);
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.MetaKeywords, localized.MetaKeywords, localized.LanguageId);
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.MetaDescription, localized.MetaDescription, localized.LanguageId);
                }

                Response.Cookies.Add(new HttpCookie("system_message", string.Format(MessageUI.UpdateSuccess, FormUI.MenuLink)));
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
                ExtentionUtils.Log(string.Concat("MenuLink.Create: ", ex.Message));

                return View(model);
            }

            return action;
        }

        [RequiredPermisson(Roles = "ViewMenu")]
        public ActionResult Index(int page = 1, string keywords = "")
        {
            var lstMenuNav = new List<MenuNavViewModel>();

            var menuLinks = _menuLinkService.GetAll();
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
                lstMenuNav = CreateMenuNav(null, menuNav);
            }

            return View(lstMenuNav);
        }

        private List<MenuNavViewModel> CreateMenuNav(int? parentId, IEnumerable<MenuNavViewModel> source)
        {
            var ieMenuNavViewModel = (from x in source
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

            return ieMenuNavViewModel;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.RouteData.Values["action"].Equals("create") || filterContext.RouteData.Values["action"].Equals("edit"))
            {
                ViewBag.MenuList = _menuLinkService.GetAll();
            }
        }
    }
}