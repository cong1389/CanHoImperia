using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Aplication;
using App.Core.Caching;
using App.Domain.Entities.GenericControl;
using App.FakeEntity.GenericControl;
using App.Framework.Ultis;
using App.Service.GenericControl;
using App.Service.Menu;
using Newtonsoft.Json;

namespace App.Admin.Controllers
{
    public class GenericControlValueItemController : BaseAdminController
    {
        private const string CacheGenericcontrolvalueitemKey = "db.GenericControlValueItem";

        private readonly IGenericControlValueItemService _genericControlValueItemService;
        private readonly IGenericControlValueService _genericControlValueService;
        private readonly IMenuLinkService _menuLinkService;
        private readonly ICacheManager _cacheManager;

        public GenericControlValueItemController(IGenericControlValueItemService genericControlValueItemService
            , IGenericControlValueService genericControlValueService
            , IMenuLinkService menuLinkService
            , ICacheManager cacheManager)
        {
            _genericControlValueItemService = genericControlValueItemService;
            _genericControlValueService = genericControlValueService;
            _menuLinkService = menuLinkService;
            _cacheManager = cacheManager;

            //Clear cache
            _cacheManager.RemoveByPattern(CacheGenericcontrolvalueitemKey);
        }

        [HttpPost]
        public JsonResult GetByMenuId(int menuId, int entityId)
        {
            var genericControlValueItem = _genericControlValueItemService.GetByOption(menuId, entityId);

            var jsonResult = Json(
                 new
                 {
                     success = genericControlValueItem.IsAny(),
                     list = RenderRazorViewToString("_CreateOrUpdate.GenericControlValue", genericControlValueItem)
                 },
                 JsonRequestBehavior.AllowGet);

            return jsonResult;
        }


        [HttpPost]
        public JsonResult Insert(string param, int entityId)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<GenericControlValueResponse>(param);

                foreach (var item in data.controlValueItemResponse)
                {
                    //Get data
                    var model = _genericControlValueItemService.Get(x => x.EntityId == entityId
                    && x.GenericControlValueId == item.GenericControlValueId);

                    if (model != null)
                    {
                        model.Title = item.Name;
                        model.Value = item.ValueName;
                        model.EntityId = entityId;
                        model.GenericControlValueId = item.GenericControlValueId;

                        _genericControlValueItemService.Update(model);
                    }
                    else
                    {
                        model = new GenericControlValueItem
                        {
                            Title = item.Name,
                            Value = item.ValueName,
                            EntityId = entityId,
                            GenericControlValueId = item.GenericControlValueId,
                            Status = 1,
                            OrderDisplay = 1
                        };


                        _genericControlValueItemService.Create(model);
                    }
                }
            }
            catch (Exception ex)
            {
                ExtentionUtils.Log(string.Concat("District.Edit: ", ex.Message));
            }
            var jsonResult = Json(new { success = true }, JsonRequestBehavior.AllowGet);

            return jsonResult;
        }

        [HttpPost]
        public JsonResult Update(string param)
        {
            try
            {
                //var data = JsonConvert.DeserializeObject<GenericControlValueResponseCollection>(param);

                //foreach (GenericControlValueItem item in data.GenericControlValueItem)
                //{
                //    //Get data
                //    GenericControlValueItem model = _genericControlValueItemService.Get((GenericControlValueItem x) => x.Id == item.Id && x.EntityId == item.EntityId);

                //    if (model != null)
                //    {
                //       // model = new GenericControlValueItem();
                //        model.ColorHex = item.ColorHex;
                //        model.EntityId = entityId;

                //        _genericControlValueItemService.Create(model);
                //    }
                //}
            }
            catch (Exception exception)
            {
                ExtentionUtils.Log(string.Concat("District.Edit: ", exception.Message));
            }
            var jsonResult = Json(new { success = true }, JsonRequestBehavior.AllowGet);

            return jsonResult;
        }

    }
}