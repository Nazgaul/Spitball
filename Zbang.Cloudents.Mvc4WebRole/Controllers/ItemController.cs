﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs.ItemDtos;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [NoUniversity]
    public class ItemController : BaseController
    {
        private readonly IBlobProvider m_BlobProvider;
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private readonly IQueueProvider m_QueueProvider;
        private readonly Lazy<IIdGenerator> m_IdGenerator;


        public ItemController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IFormsAuthenticationService formsAuthenticationService,
            IBlobProvider blobProvider,
            IFileProcessorFactory fileProcessorFactory,
            IQueueProvider queueProvider,
            Lazy<IIdGenerator> idGenerator
            )
            : base(zboxWriteService, zboxReadService,
            formsAuthenticationService)
        {
            m_BlobProvider = blobProvider;
            m_FileProcessorFactory = fileProcessorFactory;
            m_QueueProvider = queueProvider;
            m_IdGenerator = idGenerator;
        }

        /// <summary>
        /// Item Page
        /// </summary>
        /// <returns></returns>
        //[ZboxAuthorize(IsAuthenticationRequired = false)]
        //[UserNavNWelcome]
        //[Route("Item/{BoxUid:length(11)}/{ItemUid:length(11)}/{itemName?}")]
        //public ActionResult Index(string boxUid, string itemUid, string itemName)
        //{
        //    var itemId = m_ShortToLongCode.Value.ShortCodeToLong(itemUid, ShortCodesType.Item);
        //    var boxId = m_ShortToLongCode.Value.ShortCodeToLong(boxUid);
        //    var query = new GetItemQuery(GetUserId(false), itemId, boxId);
        //    var item = m_ZboxReadService.GetItem(query);

        //    var builder = new UrlBuilder(HttpContext);
        //    var url = builder.BuildItemUrl(boxId, item.BoxName, itemId, item.Name, item.UniName);
        //    return RedirectPermanent(url);
        //}

        //[ZboxAuthorize(IsAuthenticationRequired = false)]
        //[UserNavNWelcome]
        //[Route("Item/{BoxUid:length(11)}/{itemid:long:min(0)}")]
        //public ActionResult Index(string boxUid, long itemid)
        //{
        //    var boxId = m_ShortToLongCode.Value.ShortCodeToLong(boxUid);
        //    var query = new GetItemQuery(GetUserId(false), itemid, boxId);
        //    var item = m_ZboxReadService.GetItem(query);

        //    var builder = new UrlBuilder(HttpContext);
        //    var url = builder.BuildItemUrl(boxId, item.BoxName, itemid, item.Name, item.UniName);
        //    return RedirectPermanent(url);
        //}

        [ActionName("Index"), Ajax]
        [AjaxCache(TimeConsts.Hour)]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public ActionResult Index2(long boxUid, long itemId, string uniName)
        {
            try
            {
                var userId = GetUserId(false); // not really needs it

                var query = new GetItemQuery(userId, itemId, boxUid);
                var item = m_ZboxReadService.GetItem(query);
                if (item.BoxId != boxUid)
                {
                    throw new ItemNotFoundException();
                }
                var serializer = new JsonNetSerializer();
                ViewBag.data = serializer.Serialize(item);
                
                return PartialView();
            }
            catch (BoxAccessDeniedException)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }
            catch (ItemNotFoundException)
            {
                return HttpNotFound();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On item load boxid = " + boxUid + " ,itemid = " + itemId, ex);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [UserNavNWelcome]
        [NonAjax]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [Route("Item/{universityName}/{boxId:long}/{boxName}/{itemid:long:min(0)}/{itemName}", Name = "Item")]
        public ActionResult Index(long boxId, long itemid, string itemName, string universityName, string boxName)
        {
            try
            {
                var userId = GetUserId(false); // not really needs it

                var query = new GetItemQuery(userId, itemid, boxId);
                var item = m_ZboxReadService.GetItem(query);
                if (item.BoxId != boxId)
                {
                    throw new ItemNotFoundException();
                }
                if (itemName != UrlBuilder.NameToQueryString(item.Name))
                {
                    throw new ItemNotFoundException();
                }

                if (!string.IsNullOrEmpty(item.Country))
                {
                    var culture = Languages.GetCultureBaseOnCountry(item.Country);
                    BaseControllerResources.Culture = culture;
                    var seoItemName = item.Name;
                    var file = item as FileWithDetailDto;
                    if (file != null)
                    {
                        seoItemName = file.NameWOExtension;
                    }
                    ViewBag.title = string.Format("{0} {1} | {2} | {3}", BaseControllerResources.TitlePrefix, item.BoxName, seoItemName, BaseControllerResources.Cloudents);
                }
                if (!string.IsNullOrEmpty(item.Description))
                {
                    var metaDescription = item.Description.RemoveEndOfString(197);
                    ViewBag.metaDescription = metaDescription.Length == 197 ? metaDescription + "..." : metaDescription;
                }

                var serializer = new JsonNetSerializer();
                ViewBag.data = serializer.Serialize(item);
                //ViewBag.title = item.Name;
                ViewBag.Description = item.Description;
                if (Request.IsAjaxRequest())
                {
                    return PartialView();
                }
                return View();
            }
            catch (BoxAccessDeniedException)
            {
                return RedirectToAction("MembersOnly", "Error", new { returnUrl = Request.Url.AbsolutePath });
            }
            catch (ItemNotFoundException)
            {
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On item load boxid = " + boxId + " ,itemid = " + itemid, ex);
                return RedirectToAction("Index", "Error");
            }

        }

        /// <summary>
        /// Ajax Request - user press next prev icon on item page
        /// </summary>
        /// <param name="boxUid"></param>
        /// <param name="itemId"></param>
        /// <param name="uniName"></param>
        /// <returns></returns>
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [HttpGet]
        [Ajax]
        [AjaxCache(TimeConsts.Minute * 15)]
        public ActionResult Load(long boxUid, long itemId, string uniName)
        {
            try
            {
                var userId = GetUserId(false); // not really needs it

                var query = new GetItemQuery(userId, itemId, boxUid);
                var item = m_ZboxReadService.GetItem(query);
                if (item.BoxId != boxUid)
                {
                    throw new ItemNotFoundException();
                }
                return this.CdJson(new JsonResponse(true, item));
            }
            catch (BoxAccessDeniedException)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }
            catch (ItemNotFoundException)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On item load boxid = " + boxUid + " ,itemid = " + itemId, ex);
                return this.CdJson(new JsonResponse(false));
            }

        }

        /// <summary>
        /// Download Item
        /// </summary>
        /// <param name="boxUid"></param>
        /// <param name="itemId"></param>
        /// <returns>if link redirect to link if file download</returns>
        [ZboxAuthorize]
        [Route("D/{BoxUid:long:min(0)}/{itemid:long:min(0)}", Name = "ItemDownload")]
        public ActionResult Download(long boxUid, long itemId)
        {
            const string defaultMimeType = "application/octet-stream";

            var query = new GetItemQuery(GetUserId(false), itemId, boxUid);

            var item = m_ZboxReadService.GetItem(query);


            var filedto = item as FileWithDetailDto;
            if (filedto == null) // link
            {
                return Redirect(item.Blob);
            }
            var blob = m_BlobProvider.GetFile(filedto.Blob);
            var contentType = defaultMimeType;
            if (!string.IsNullOrWhiteSpace(blob.Properties.ContentType))
            {
                contentType = blob.Properties.ContentType;
            }
            return new BlobFileStream(blob, contentType, item.Name, true);
        }

        /// <summary>
        /// Used to rename file name - item name cannot be changed
        /// </summary>
        /// <param name="newFileName"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [ZboxAuthorize]
        [HttpPost]
        [Ajax]
        public JsonResult Rename(string newFileName, long itemId)
        {
            if (string.IsNullOrWhiteSpace(newFileName))
            {
                return Json(new JsonResponse(false, "need file name"));
            }
            if (newFileName.Length > Item.NameLength)
            {
                return Json(new JsonResponse(false, "File name to long"));
            }
            var userId = GetUserId();
            try
            {
                var command = new ChangeFileNameCommand(itemId, newFileName, userId);
                var result = m_ZboxWriteService.ChangeFileName(command);
                return Json(new JsonResponse(true, new
                {
                    name = Path.GetFileNameWithoutExtension(result.Name),
                    queryString = UrlBuilder.NameToQueryString(result.Name)
                }));
            }

            catch (UnauthorizedAccessException)
            {
                return Json(new JsonResponse(false, "You need to follow this box in order to change file name"));
            }
            catch (ArgumentException ex)
            {
                return Json(new JsonResponse(false, ex.Message));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("ChangeFileName newFileName {0} ItemUid {1} userId {2}", newFileName, itemId, userId), ex);
                return Json(new JsonResponse(false, "Error"));
            }


        }

        /// <summary>
        /// Print selected file
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="itemId"></param>
        /// <param name="otakim"></param>
        /// <returns>View with no layout and print command in javascript</returns>
        [ZboxAuthorize]
        public async Task<ActionResult> Print(long boxId, long itemId, bool otakim = false)
        {

            var query = new GetItemQuery(GetUserId(false), itemId, boxId);

            var item = m_ZboxReadService.GetItem(query);



            var filedto = item as FileWithDetailDto;
            if (filedto != null)
            {
                var uri = new Uri(m_BlobProvider.GetBlobUrl(filedto.Blob));


                if (otakim)
                {
                    var bloburl = m_BlobProvider.GenerateSharedAccressReadPermissionInStorage(uri, 60);
                    var url = string.Format("{3}?ReferrerBaseURL=cloudents.com&ReferrerUserName={2}&ReferrerUserToken={2}&FileURL={0}&FileName={1}", Server.UrlEncode(bloburl), filedto.Name, User.Identity.Name,
                        Zbox.Infrastructure.Extensions.ConfigFetcher.Fetch("otakimUrl"));
                    return Redirect(url);
                }
                IEnumerable<string> retVal = null;

                // var model = GenerateItemView(boxid, userId, itemId);
                var processor = m_FileProcessorFactory.GetProcessor(uri);
                if (processor != null)
                {
                    var result = await processor.ConvertFileToWebSitePreview(uri, int.MaxValue, int.MaxValue, 0);
                    retVal = result.Content;

                }
                return View(retVal);
            }
            return Redirect(item.Blob);

        }


        [ZboxAuthorize]
        [HttpPost]
        public ActionResult Delete(long itemId, long boxUid)
        {
            try
            {
                var userEmailId = GetUserId(false);

                var command = new DeleteItemCommand(itemId, userEmailId, boxUid);
                m_ZboxWriteService.DeleteItem(command);

                return Json(new JsonResponse(true, itemId));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("DeleteItem user: {0} boxid: {1} itemId {2}", GetUserId(), boxUid, itemId), ex);
                return Json(new JsonResponse(false));
            }
        }



        [ZboxAuthorize]
        [HttpPost]
        [Ajax]
        public JsonResult Rate(long itemId, int rate)
        {
            try
            {
                var id = m_IdGenerator.Value.GetId();
                var command = new RateItemCommand(itemId, GetUserId(), rate, id);
                m_ZboxWriteService.RateItem(command);

                return Json(new JsonResponse(true, itemId));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rate user: {0} itemId {1}", GetUserId(), itemId), ex);
                return Json(new JsonResponse(false));
            }
        }

        [ZboxAuthorize]
        [HttpGet, Ajax]
        public async Task<JsonResult> Rate(long itemId)
        {

            var query = new GetItemRateQuery(GetUserId(), itemId);
            var retVal = await m_ZboxReadService.GetRate(query) ?? new Zbox.ViewModel.DTOs.ItemRateDto();

            return Json(new JsonResponse(true, retVal), JsonRequestBehavior.AllowGet);

        }



        #region Preview
        [HttpGet, Ajax]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [AjaxCache(TimeConsts.Minute * 15)]
        public async Task<ActionResult> Preview(string blobName, int imageNumber, long uid, string boxUid, int width = 0, int height = 0)
        {
            Uri uri;
            if (!Uri.TryCreate(blobName, UriKind.Absolute, out uri))
            {
                uri = new Uri(m_BlobProvider.GetBlobUrl(blobName));
            }
            //this will not work due to ie9
            //if (!Request.Headers["Referer"].Contains(boxUid))
            //{
            //    return Json(new JsonResponse(false), JsonRequestBehavior.AllowGet);
            //}
            if (!User.Identity.IsAuthenticated && imageNumber > 0)
            {
                return Json(new JsonResponse(true), JsonRequestBehavior.AllowGet);
            }
            var processor = m_FileProcessorFactory.GetProcessor(uri);
            if (processor == null)
                return
                    Json(
                        new JsonResponse(true,
                            new
                            {
                                preview =
                                    RenderRazorViewToString("_PreviewFailed",
                                        Url.ActionLinkWithParam("Download", new {BoxUid = boxUid, ItemId = uid}))
                            }),
                        JsonRequestBehavior.AllowGet);
            try
            {
                var retVal = await processor.ConvertFileToWebSitePreview(uri, width, height, imageNumber);
                if (string.IsNullOrEmpty(retVal.ViewName))
                {
                    return Json(new JsonResponse(true, new { preview = retVal.Content.First() }), JsonRequestBehavior.AllowGet);
                }

                return Json(new JsonResponse(true, new { preview = RenderRazorViewToString("_Preview" + retVal.ViewName, retVal.Content.Take(3)) }), JsonRequestBehavior.AllowGet);
                //if (retVal.Content.Count() == 0 && imageNumber == 0) // this is happen due failed preview at the start
                //{
                //    return Json(new JsonResponse(true, new { preview = RenderRazorViewToString("_PreviewFailed", Url.ActionLinkWithParam("Download", new { BoxUid = boxUid, ItemId = uid })) }), JsonRequestBehavior.AllowGet);
                //}
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("GeneratePreview filename: {0}", blobName), ex);
                if (imageNumber == 0)
                {
                    return Json(new JsonResponse(true, new { preview = RenderRazorViewToString("_PreviewFailed", Url.ActionLinkWithParam("Download", new { BoxUid = boxUid, ItemId = uid })) }), JsonRequestBehavior.AllowGet);
                }
                return Json(new JsonResponse(true), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        #region flagItem
        [Ajax]
        [HttpPost]
        [ZboxAuthorize]
        public ActionResult Flag()
        {
            return PartialView("_FlagItem", new FlagBadItem());
        }

        [HttpPost]
        [ZboxAuthorize]
        [Ajax]
        public ActionResult FlagRequest(FlagBadItem model)
        {

            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }

            m_QueueProvider.InsertMessageToTranaction(new BadItemData(model.BadItem.GetEnumDescription(), model.Other, GetUserId(), model.ItemId));
            return Json(new JsonResponse(true));
        }

        #endregion
    }
}
