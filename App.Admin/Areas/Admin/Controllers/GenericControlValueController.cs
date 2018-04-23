using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Admin.Helpers;
using App.Aplication;
using App.Core.Caching;
using App.Core.Utils;
using App.Domain.Entities.GenericControl;
using App.FakeEntity.GenericControl;
using App.Framework.Ultis;
using App.Service.GenericControl;
using App.Service.Menu;
using AutoMapper;
using Resources;

namespace App.Admin.Controllers
{
    public class GenericControlValueController : BaseAdminController
    {
        private const string CacheGenericcontrolvalueKey = "db.GenericControlValue";
        private readonly ICacheManager _cacheManager;

        private readonly IGenericControlValueService _genericControlValueService;
        private readonly IGenericControlService _genericControlService;
        private readonly IMenuLinkService _menuLinkService;

        public GenericControlValueController(IGenericControlValueService genericControlValueService
            , IGenericControlService genericControlService
            , IMenuLinkService menuLinkService
            , ICacheManager cacheManager)
        {
            _genericControlValueService = genericControlValueService;
            _genericControlService = genericControlService;
            _menuLinkService = menuLinkService;
            _cacheManager = cacheManager;

            //Clear cache
            _cacheManager.RemoveByPattern(CacheGenericcontrolvalueKey);
        }

        [RequiredPermisson(Roles = "CreateEditGenericControlValue")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [RequiredPermisson(Roles = "CreateEditGenericControlValue")]
        public ActionResult Create(GenericControlValueViewModel model, string returnUrl)
        {
            ActionResult action;
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", MessageUI.ErrorMessage);
                    return View(model);
                }

                var modelMap = Mapper.Map<GenericControlValueViewModel, GenericControlValue>(model);
                _genericControlValueService.Create(modelMap);

                Response.Cookies.Add(new HttpCookie("system_message", string.Format(MessageUI.CreateSuccess, FormUI.Name)));
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
                ExtentionUtils.Log(string.Concat("GenericControlValue.Create: ", exception.Message));
                return View(model);
            }
            return action;
        }

        [RequiredPermisson(Roles = "DeleteGenericControlValue")]
        public ActionResult Delete(string[] ids)
        {
            try
            {
                if (ids.Length != 0)
                {
                    var genericControlValues =
                        from id in ids
                        select _genericControlValueService.GetById(int.Parse(id));
                    _genericControlValueService.BatchDelete(genericControlValues);
                }
            }
            catch (Exception exception1)
            {
                var exception = exception1;
                ExtentionUtils.Log(string.Concat("GenericControlValue.Delete: ", exception.Message));
            }
            return RedirectToAction("Index");
        }

        [RequiredPermisson(Roles = "CreateEditGenericControlValue")]
        public ActionResult Edit(int id)
        {
            var genericControlValueViewModel = Mapper.Map<GenericControlValue, GenericControlValueViewModel>(_genericControlValueService.GetById(id));

            //_genericControlValueService.GetById(Id)


            return View(genericControlValueViewModel);
        }

        [HttpPost]
        [RequiredPermisson(Roles = "CreateEditGenericControlValue")]
        public ActionResult Edit(GenericControlValueViewModel model, string returnUrl)
        {
            ActionResult action;
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", MessageUI.ErrorMessage);
                    return View(model);
                }

                var modelMap = Mapper.Map<GenericControlValueViewModel, GenericControlValue>(model);

                _genericControlValueService.Update(modelMap);

                Response.Cookies.Add(new HttpCookie("system_message", string.Format(MessageUI.UpdateSuccess, FormUI.Name)));

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
                ExtentionUtils.Log(string.Concat("GenericControlValue.Edit: ", exception.Message));
                return View(model);
            }
            return action;
        }

        [RequiredPermisson(Roles = "ViewGenericControlValue")]
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
            var list = _genericControlValueService.PagedList(sortingPagingBuilder, paging).ToList();
            list.ForEach(item => item.GenericControl = _genericControlService.GetById(item.GenericControlId));
            if (list != null && list.Any())
            {
                var pageInfo = new Helper.PageInfo(ExtentionUtils.PageSize, page, paging.TotalRecord, i => Url.Action("Index", new { page = i, keywords }));
                ViewBag.PageInfo = pageInfo;
            }
            return View(list);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.RouteData.Values["action"].Equals("create") || filterContext.RouteData.Values["action"].Equals("edit"))
            {
                var all = _genericControlService.GetAll();
                ViewBag.GenericControls = all;
            }
        }

        [HttpPost]
        public JsonResult GetByMenuId(int menuId, int entityId)
        {
            var lstValueResponse = new List<ControlValueItemResponse>();

            var menuLink = _menuLinkService.GetById(menuId);
            if (menuLink != null)
            {
                IEnumerable<GenericControl> ieGc = menuLink.GenericControls;
                if (ieGc.IsAny())
                {
                    foreach (var item in ieGc)
                    {
                        var gCvDefault = item.GenericControlValues.Where(m => m.Status == 1);
                        //if (gCVDefault.IsAny())
                        //    break;                       

                        foreach (var gcValue in gCvDefault)
                        {
                            var objValueResponse = new ControlValueItemResponse();

                            objValueResponse.GenericControlValueId = gcValue.Id;
                            objValueResponse.Name = gcValue.ValueName;
                            objValueResponse.ValueName = gcValue.GetValueItem(entityId);

                            lstValueResponse.Add(objValueResponse);
                        }
                    }
                }
            }

            var jsonResult = Json(
                 new
                 {
                     success = lstValueResponse.Count() > 0,
                     list = RenderRazorViewToString("_CreateOrUpdate.GenericControlValue", lstValueResponse)
                 },
                 JsonRequestBehavior.AllowGet);

            return jsonResult;
        }

    }
}