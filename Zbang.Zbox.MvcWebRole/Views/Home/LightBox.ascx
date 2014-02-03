<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="generalRoundCorPopup footerFiles-lightbox" id="LightBox" style="display: none;">
    <div class="lb-mainImg float-left">
        <div class="lb-mainFileNam-box">
            <h3 class="arial bold txt-18 float-left" id="LightBoxBoxName">
            </h3>
            <span class="arial txt-18 float-left">&nbsp;|&nbsp;</span><h3 class="arial txt-15 float-left normal lb-mainFileNam"
                id="LightBoxItemName">
            </h3>
            <div class="float-right lb-actions GradiantActions">
                <ul>
                    <li class="lb-actions-del lb-actions-bor1" id="LightBoxDelete">Delete</li>
                    <li class="actSep"></li>
                    <li class="lb-actions-dl lb-actions-bor1 lb-actions-bor2" id="LightBoxDownload">Download</li>
                    <li class="actSep"></li>
                    <li class="lb-actions-share lb-actions-bor2" id="LightBoxShare">
                        <div id="d_clip_container" style="position: relative">
                            <div id="d_clip_button">
                                Share</div>
                        </div>
                    </li>
                </ul>
                <br class="clear" />
            </div>
            <br class="clear" />
        </div>
        <!-- lb-mainFileNam-box END -->
        <div class="lb-mainFile-box">
            <div class="lb-mainFile-box-content" id="LightBoxMainPic">
            </div>
        </div>
    </div>
    <!-- lb-mainImg END  -->
    <div class="lb-comments  float-right">
        <div class="lb-userInfo">
            <div class="inside">
                <img class="float-left" src="/Content/Images/default-user-avatar.png" alt="" />
                <div class="arial txt-13 lb-User-details float-left">
                    <span class="bold">Added by: <span id="LightBoxUploader"></span></span>
                    <br />
                    <span id="LightBoxFileCreated"></span>
                    <br />
                    <span id="LightBoxfileSize"></span>
                </div>
                <br class="clear" />
            </div>
        </div>
        <!-- lb-userInfo end -->
        <img src="../../Content/Images/footerFiles-lightbox/dotttedLine.gif" />
        <div class="lb-comment-bg">
            <input type="text" id="lb-comment" class="style5" /></div>
        <div class="txt-13 txt-shadow rounded  btnBlue white btnPadding btnShareLighbx float-right"
            id="btnShareLighbxComment" style="">
            Share</div>
        <br class="clear " />
        <img src="../../Content/Images/footerFiles-lightbox/dotttedLine.gif" />
        <div class="lb-comments-content" id="LightHouseCommentSection">
        </div>
    </div>
    <br class="clear" />
    <div class="lb-thumbs GradiantThumbs">
        <img id="mycarousel-prev" class="float-left" src="/Content/Images/footerFiles-lightbox/thumbs-arrowLeft.png" />
        <div id="mycarousel" class="jcarousel-skin-tango float-left">
            <ul id="">
            </ul>
        </div>
        <img class="float-left" id="mycarousel-next" src="/Content/Images/footerFiles-lightbox/thumbs-arrowRight.png" />
        <br class="clear" />
    </div>
    <img class="lb-arrowLeft" src="/Content/Images/footerFiles-lightbox/arrowLeft.png"
        id="LightBoxMoveLeft" />
    <img class="lb-arrowRight" src="/Content/Images/footerFiles-lightbox/arrowRight.png"
        id="LightBoxMoveRight" />
    <img id="btnLbClose" src="/Content/Images/close.png" />
</div>
<div class="fadeMe" id="LightBoxFadeMe" style="display: none;">
</div>
