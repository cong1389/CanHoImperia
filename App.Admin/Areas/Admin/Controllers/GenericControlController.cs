using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Admin.Helpers;
using App.Aplication;
using App.Aplication.Extensions;
using App.Core.Caching;
using App.Core.Utils;
using App.Domain.Entities.GenericControl;
using App.Domain.Entities.Menu;
using App.FakeEntity.GenericControl;
using App.FakeEntity.Menu;
using App.Framework.Ultis;
using App.Service.GenericControl;
using App.Service.Language;
using App.Service.LocalizedProperty;
using App.Service.Menu;
using AutoMapper;
using Resources;

namespace App.Admin.Controllers
{
    public class GenericControlController : BaseAdminController
    {
        private const string CacheGenericcontrolKey = "db.GenericControl";
        private readonly ICacheManager _cacheManager;

        private readonly IGenericControlService _genericControlService;

        private readonly ILanguageService _languageService;

        private readonly ILocalizedPropertyService _localizedPropertyService;

        private readonly IGenericControlValueService _genericControlValueService;
        private readonly IMenuLinkService _menuLinkService;

        public GenericControlController(IGenericControlService genericControlService
              , ILanguageService languageService
             , ILocalizedPropertyService localizedPropertyService
            , IGenericControlValueService genericControlValueService
            , IMenuLinkService menuLinkService
            , ICacheManager cacheManager)
        {
            _genericControlService = genericControlService;
            _languageService = languageService;
            _localizedPropertyService = localizedPropertyService;
            _genericControlValueService = genericControlValueService;
            _menuLinkService = menuLinkService;
            _cacheManager = cacheManager;

            //Clear cache
            _cacheManager.RemoveByPattern(CacheGenericcontrolKey);
        }

        [RequiredPermisson(Roles = "CreateEditGenericControl")]
        public ActionResult Create()
        {
            var model = new GenericControlViewModel
            {
                OrderDisplay = 1,
                Status = 1
            };

            //Add locales to model
            AddLocales(_languageService, model.Locales);

            return View(model);
        }

        [HttpPost]
        [RequiredPermisson(Roles = "CreateEditGenericControl")]
        public ActionResult Create(GenericControlViewModel model, string returnUrl)
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

                var modelMap = Mapper.Map<GenericControlViewModel, GenericControl>(model);
                _genericControlService.Create(modelMap);

                //Update Localized   
                foreach (var localized in model.Locales)
                {
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.Name, localized.Name, localized.LanguageId);
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.Description, localized.Description, localized.LanguageId);
                }

                Response.Cookies.Add(new HttpCookie("system_message", string.Format(MessageUI.CreateSuccess, FormUI.GenericControl)));
                if (!Url.IsLocalUrl(returnUrl) || returnUrl.Length <= 1 || !returnUrl.StartsWith("/") || returnUrl.StartsWith("//") || returnUrl.StartsWith("/\\"))
                {
                    action = RedirectToAction("Index");
                }
                else
                {
                    action = Redirect(returnUrl);
                }
            }
            catch (Exception exception1)
            {
                var exception = exception1;
                ExtentionUtils.Log(string.Concat("GenericControl.Create: ", exception.Message));
                return View(model);
            }
            return action;
        }

        [RequiredPermisson(Roles = "DeleteGenericControl")]
        public ActionResult Delete(string[] ids)
        {
            try
            {
                if (ids.Length != 0)
                {
                    var genericControls =
                        from id in ids
                        select _genericControlService.GetById(int.Parse(id));
                    _genericControlService.BatchDelete(genericControls);

                    //Delete localize
                    for (var i = 0; i < ids.Length; i++)
                    {
                        var ieLocalizedProperty
                           = _localizedPropertyService.GetByEntityId(int.Parse(ids[i]));
                        _localizedPropertyService.BatchDelete(ieLocalizedProperty);
                    }
                }
            }
            catch (Exception exception1)
            {
                var exception = exception1;
                ExtentionUtils.Log(string.Concat("ServerGenericControl.Delete: ", exception.Message));
            }
            return RedirectToAction("Index");
        }

        [RequiredPermisson(Roles = "CreateEditGenericControl")]
        public ActionResult Edit(int id)
        {
            var modelMap = Mapper.Map<GenericControl, GenericControlViewModel>(_genericControlService.GetById(id));
            
            //Add Locales to model
            AddLocales(_languageService, modelMap.Locales, (locale, languageId) =>
            {
                locale.Id = modelMap.Id;
                locale.LocalesId = modelMap.Id;
                locale.Name = modelMap.GetLocalized(x => x.Name, id, languageId, false, false);
                locale.Description = modelMap.GetLocalized(x => x.Description, id, languageId, false, false);
            });

            return View(modelMap);
        }

        [HttpPost]
        [RequiredPermisson(Roles = "CreateEditGenericControl")]
        public ActionResult Edit(GenericControlViewModel model, string returnUrl)
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

                var objGenericControl = _genericControlService.GetById(model.Id);

                var lstMenuLink = new List<MenuLink>();
                var lstMenuId = new List<int>();

                var menuLink = Request["MenuLink"];
                if (!string.IsNullOrEmpty(menuLink))
                {
                    foreach (var item in menuLink.Split(',').ToList())
                    {
                        var menuId = int.Parse(item);
                        lstMenuId.Add(menuId);

                        lstMenuLink.Add(_menuLinkService.GetById(menuId,false));
                    }

                    if (lstMenuId.IsAny())
                    {
                        (from x in objGenericControl.MenuLinks
                                where !lstMenuId.Contains(x.Id)
                                select x).ToList()
                            .ForEach(m =>
                                objGenericControl.MenuLinks.Remove(m)
                            );
                    }
                }
                objGenericControl.MenuLinks = lstMenuLink;
                    
                var modelMap = Mapper.Map<GenericControlViewModel, GenericControl>(model);
                _genericControlService.Update(objGenericControl);


                //Update Localized   
                foreach (var localized in model.Locales)
                {
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.Name, localized.Name, localized.LanguageId);
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.Description, localized.Description, localized.LanguageId);
                }

                Response.Cookies.Add(new HttpCookie("system_message", string.Format(MessageUI.UpdateSuccess, FormUI.GenericControl)));
                if (!Url.IsLocalUrl(returnUrl) || returnUrl.Length <= 1 || !returnUrl.StartsWith("/") || returnUrl.StartsWith("//") || returnUrl.StartsWith("/\\"))
                {
                    action = RedirectToAction("Index");
                }
                else
                {
                    action = Redirect(returnUrl);
                }
            }
            catch (Exception exception1)
            {
                var exception = exception1;
                ExtentionUtils.Log(string.Concat("GenericControl.Create: ", exception.Message));
                return View(model);
            }
            return action;
        }

        [RequiredPermisson(Roles = "ViewGenericControl")]
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
            var genericControls = _genericControlService.PagedList(sortingPagingBuilder, paging);
            if (genericControls != null && genericControls.Any())
            {
                var pageInfo = new Helper.PageInfo(ExtentionUtils.PageSize, page, paging.TotalRecord, i => Url.Action("Index", new { page = i, keywords }));
                ViewBag.PageInfo = pageInfo;
            }
            return View(genericControls);
        }


        public JsonResult GetGenericControlByMenuId(int menuId)
        {
            IEnumerable<GenericControl> ieGenericControls = null;
            try
            {
                ieGenericControls = _genericControlService.GetByMenuId(menuId);

                //if (ieGenericControls.Count() == 0)
                //    return null;

                //int genericControlId = ieGenericControls.FirstOrDefault().Id;

                //genericControlValue = _genericControlValueService.FindBy((GenericControlValue x) => x.GenericControlId == genericControlId && x.Status == 1, false);

            }
            catch 
            {

            }

            var jsonResult = Json(
                new
                {
                    success = ieGenericControls.Count(),
                    list = RenderRazorViewToString("_CreateOrUpdate.GenericControl", ieGenericControls)
                },
                JsonRequestBehavior.AllowGet);

            return jsonResult;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.RouteData.Values["action"].Equals("create"))
            {
                var lstMenuLink = _menuLinkService.GetAll();

                //var menuLinkViewModel = new MenuLinkViewModel();
                //var model = lstMenuLink.Select(m =>
                //{
                //    return m.ToModel(menuLinkViewModel);
                //});

                //ViewBag.MenuLink = model;
            }

            if (filterContext.RouteData.Values["action"].Equals("edit"))
            {
                var id = int.Parse(filterContext.RouteData.Values["id"].ToString());

                var lstMenuLink = _menuLinkService.GetAll();

                var model = from x in lstMenuLink
                                                       select new MenuLinkViewModel
                                                       {
                                                           Id = x.Id,
                                                           ParentId = x.ParentId,
                                                           Status = x.Status,
                                                           TypeMenu = x.TypeMenu,
                                                           Position = x.Position,
                                                           MenuName = x.MenuName,
                                                           VirtualId = x.VirtualId,
                                                           Selected = x.SelectedMenu(id)
                                                       };

                ViewBag.GCMenuLink = model;
            }
        }
    }
}