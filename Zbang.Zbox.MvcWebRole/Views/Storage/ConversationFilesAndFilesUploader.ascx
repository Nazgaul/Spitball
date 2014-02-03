<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div id="conversations-container">
    <div id="conversationsHeader">
        <div class="style11 txt-24 blue-2" id="selectedBoxName">
            &nbsp;</div>
        <div class="style11 style11-no-bold txt-24 blue-2">
            &nbsp;|&nbsp;</div>
        <div class="style12 txt-24 blue-2">
            owner:</div>
        <div class="style13 txt-24 blue-2" id="ownerLabel">
            you</div>
    </div>
    <br class="clear" />
    <div class="comments-top-menu">
        <div class="menu-item">
            <a id="addCommentTop">
                <img src="/content/images/commentPanel/icon-add-comment.png" alt="comment" />
                <span class="comment-menu-text tViolet">comment</span> </a>
        </div>
        <div class="menu-item">
            <a id="addLinkTop">
                <img src="/content/images/commentPanel/icon-add-link.png" alt="add link" />
                <span class="comment-menu-text tBlue">add link</span></a>
        </div>
        <div class="menu-item">
            <a id="addFileTop">
                <img src="/content/images/commentPanel/icon-add-file.png" alt="add file" />
                <span class="comment-menu-text tGreen">add file</span></a>
        </div>
    </div>
    <br class="clear" />
    <div id="comments-top">
        <div class="bubble">
            <img src="/content/images/commentPanel/add-comment-arrow.png" alt="" id="bubble-arrow"
                style="margin: 0 0 -5px 32px;" />
            <div class="hor-line">
            </div>
            <div class="contentMain">
                <span id="ConversationSubmitError"></span>
                <div id="bubble-default" class="bubble-item pad5">
                    <span class="tGray">Add your voice to the conversation...</span>
                </div>
                <div id="bubble-add-comment" class="bubble-item" style="display: none;">
                    <a class="close-top-bubble" title="cancel">&nbsp;</a>
                    <%: Html.TextArea("newCommentTxt", new{ rows="2", cols="20"}) %>
                    <%--<textarea name="newCommentTxt" rows="2" cols="20" id="newCommentTxt" ></textarea>--%>
                </div>
                <div id="bubble-add-link" class="bubble-item" style="display: none;">
                    <a class="close-top-bubble" title="cancel">&nbsp;</a>
                    <div class="add-link-container">
                        <input name="newLinkTxt" type="text" id="newLinkTxt" />
                    </div>
                    <div id="bubble-add-comment-link">
                        <textarea name="newCommentLinkTxt" rows="2" cols="20" id="newCommentLinkTxt"></textarea>
                    </div>
                </div>
                <div id="bubble-add-file" class="bubble-item" style="display: none;">
                    <a class="close-top-bubble" title="cancel">&nbsp;</a>
                    <div class="file-counter">
                        <a id="AddFiles" href="#" class="rounded btnGreen txt-shadow txt-13 white">Select Files
                            <%--<img src="/Content/Images/commentPanel/select-files.png" alt="select files" />--%></a>
                        <span id="filesCounter" class="tGreen bold counter">0</span><span class="tGreen bold counter">Files</span></div>
                    <div id="add-file-container" class="add-file-container">
                       <%-- <input type="hidden" id="flashcookie" value="<%: Request.Cookies[FormsAuthentication.FormsCookieName]==null ? string.Empty : Request.Cookies[FormsAuthentication.FormsCookieName].Value %>" />--%>
                       
                        
                        <ul id="divFilesToUploadConversation">
                        </ul>
                    </div>
                    <div id="bubble-add-comment-file">
                        <textarea name="newCommentFileTxt" rows="2" cols="20" id="newCommentFileTxt"></textarea>
                    </div>
                </div>
            </div>
            <div class="hor-line">
            </div>
            <div class="btn-container">
                <a class="txt-13 txt-shadow rounded  btnBlue white btnPadding" id="share" style="display:none">Share
                  <%-- <img src="/content/images/commentPanel/btn-share.png" alt="share" id="share" style="display: none" />--%>
                </a>
            </div>
                    </div>
    </div>
</div>
<% Html.RenderPartial("~/Views/Storage/Conversations.ascx"); %>
