using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Admin.Helpers;
using App.Aplication;
using App.Core.Caching;
using App.Core.Utils;
using App.Domain.Entities.Payments;
using App.FakeEntity.Payments;
using App.Framework.Ultis;
using App.Service.Language;
using App.Service.LocalizedProperty;
using App.Service.PaymentMethodes;
using AutoMapper;
using Resources;

namespace App.Admin.Controllers
{
    public class PaymentMethodController : BaseAdminController
    {
        private const string CachePaymentMethod = "db.PaymentMethod";
        private readonly ICacheManager _cacheManager;

        private readonly IPaymentMethodService _paymentMethodService;

        private readonly IImagePlugin _imagePlugin;

        private readonly ILocalizedPropertyService _localizedPropertyService;
        private readonly ILanguageService _languageService;

        public PaymentMethodController(IPaymentMethodService paymentMethodService, IImagePlugin imagePlugin
            , ICacheManager cacheManager
            , ILocalizedPropertyService localizedPropertyService
            , ILanguageService languageService)
        {
            _paymentMethodService = paymentMethodService;
            _imagePlugin = imagePlugin;
            _cacheManager = cacheManager;
            _languageService = languageService;
            _localizedPropertyService = localizedPropertyService;

            //Clear cache
            _cacheManager.RemoveByPattern(CachePaymentMethod);

        }

        [RequiredPermisson(Roles = "CreateEditPaymentMethod")]
        public ActionResult Create()
        {
            var model = new PaymentMethodViewModel
            {
                Status = 1,
                OrderDisplay = 1
            };

            //Add locales to model
            AddLocales(_languageService, model.Locales);

            return View(model);
        }

        [HttpPost]
        [RequiredPermisson(Roles = "CreateEditPaymentMethod")]
        public ActionResult Create(PaymentMethodViewModel model, string returnUrl)
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
                    var fileExtension = Path.GetExtension(model.Image.FileName);
                    var fileNameOriginal = Path.GetFileNameWithoutExtension(model.Image.FileName);

                    var fileName = fileNameOriginal.FileNameFormat(fileExtension);

                    _imagePlugin.CropAndResizeImage(model.Image, $"{Contains.PaymentMethodFolder}", fileName, ImageSize.PaymentMethodWithMediumSize, ImageSize.PaymentMethodHeightMediumSize);

                    model.ImageUrl = string.Concat(Contains.PaymentMethodFolder, fileName);
                }

                var modelMap = Mapper.Map<PaymentMethodViewModel, PaymentMethod>(model);
                _paymentMethodService.Create(modelMap);

                //Update Localized   
                foreach (var localized in model.Locales)
                {
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.PaymentMethodSystemName, localized.PaymentMethodSystemName, localized.LanguageId);
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.Description, localized.Description, localized.LanguageId);
                }

                Response.Cookies.Add(new HttpCookie("system_message", string.Format(MessageUI.CreateSuccess, FormUI.PaymentMethod)));
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
                ExtentionUtils.Log(string.Concat("PaymentMethod.Create: ", ex.Message));
                ModelState.AddModelError("", ex.Message);

                return View(model);
            }

            return action;
        }

        [RequiredPermisson(Roles = "CreateEditPaymentMethod")]
        public ActionResult Delete(int[] ids)
        {
            try
            {
                if (ids.Length != 0)
                {
                    var paymentMethod =
                        from id in ids
                        select _paymentMethodService.Get(x => x.Id == id);
                    _paymentMethodService.BatchDelete(paymentMethod);
                }
            }
            catch (Exception ex)
            {
                ExtentionUtils.Log(string.Concat("PaymentMethod.Delete: ", ex.Message));
            }

            return RedirectToAction("Index");
        }

        [RequiredPermisson(Roles = "CreateEditPaymentMethod")]
        public ActionResult Edit(int id)
        {
            var modelMap = Mapper.Map<PaymentMethod, PaymentMethodViewModel>(_paymentMethodService.Get(x => x.Id == id));

            //Add Locales to model
            AddLocales(_languageService, modelMap.Locales, (locale, languageId) =>
            {
                locale.Id = modelMap.Id;
                locale.LocalesId = modelMap.Id;
                locale.PaymentMethodSystemName = modelMap.GetLocalized(x => x.PaymentMethodSystemName, id, languageId, false, false);
                locale.Description = modelMap.GetLocalized(x => x.Description, id, languageId, false, false);
            });

            return View(modelMap);
        }

        [HttpPost]
        [RequiredPermisson(Roles = "CreateEditPaymentMethod")]
        public ActionResult Edit(PaymentMethodViewModel model, string returnUrl)
        {
            ActionResult action;
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", MessageUI.ErrorMessage);
                    return View(model);
                }

                var byId = _paymentMethodService.Get(x => x.Id == model.Id);

                if (model.Image != null && model.Image.ContentLength > 0)
                {
                    var fileExtension = Path.GetExtension(model.Image.FileName);
                    var fileNameOriginal = Path.GetFileNameWithoutExtension(model.Image.FileName);

                    var fileName = fileNameOriginal.FileNameFormat(fileExtension);

                    //var fileExtension = Path.GetExtension(model.Image.FileName);
                    //var fileName = titleNonAccent.FileNameFormat(fileExtension);

                    _imagePlugin.CropAndResizeImage(model.Image, $"{Contains.PaymentMethodFolder}", fileName, ImageSize.PaymentMethodWithMediumSize, ImageSize.PaymentMethodHeightMediumSize);

                    model.ImageUrl = string.Concat(Contains.PaymentMethodFolder, fileName);
                }

                var modelMap = Mapper.Map(model, byId);
                _paymentMethodService.Update(modelMap);

                //Update Localized   
                foreach (var localized in model.Locales)
                {
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.PaymentMethodSystemName, localized.PaymentMethodSystemName, localized.LanguageId);
                    _localizedPropertyService.SaveLocalizedValue(modelMap, x => x.Description, localized.Description, localized.LanguageId);
                }


                Response.Cookies.Add(new HttpCookie("system_message", string.Format(MessageUI.UpdateSuccess, FormUI.PaymentMethod)));
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
                ExtentionUtils.Log(string.Concat("PaymentMethod.Edit: ", ex.Message));
                return View(model);
            }

            return action;
        }

        [RequiredPermisson(Roles = "ViewPaymentMethod")]
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

            var manufacturers = _paymentMethodService.PagedList(sortingPagingBuilder, paging);
            if (manufacturers.IsAny())
            {
                var pageInfo = new Helper.PageInfo(ExtentionUtils.PageSize, page, paging.TotalRecord, i => Url.Action("Index", new { page = i, keywords }));
                ViewBag.PageInfo = pageInfo;
            }

            return View(manufacturers);
        }
    }
}