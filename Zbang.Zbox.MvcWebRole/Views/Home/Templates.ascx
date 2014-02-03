<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="templates">
    <div id="boxTemplate" style="display: none;">
        <div id="boxEntry{shortUid}" data-boxid="{shortUid}" class="boxEntry" title="{BoxName}">
            <img alt="" src="/Content/Images/my_box.png" style="vertical-align: middle;" />
            <span class="boxName black333 txt-13" style="white-space: nowrap;">{BoxName} </span>
            <input class="boxName" type="text" value="{BoxName}" />
            <img alt="" src="/Content/Images/list_delete_icon.png" class="deleteListIcon" />
        </div>
    </div>
    <div id="subscribedBoxTemplate" style="display: none;">
        <div id="boxEntry{shortUid}" data-boxid="{shortUid}" class="boxEntry" title="{BoxName}">
            <img alt="" src="/Content/Images/subscribed_box.png" style="vertical-align: middle;" />
            <p style="width: 135px; overflow: hidden; display: inline;">
                <span class="boxName  border-right black333 txt-13" style="white-space: nowrap;">{BoxName}</span>
                <span class="boxOwnerName black333 txt-13" title="{BoxOwner}" style="white-space: nowrap;">
                    {BoxOwner}</span></p>
            <img alt="" src="/Content/Images/list_delete_icon.png" class="deleteListIcon" />
        </div>
    </div>
    <div id="InvitedBoxTemplate" style="display: none;">
        <div id="InvitedBox{shortUid}" data-boxid="{shortUid}" class="boxEntry invited" title="{BoxName}">
            <img alt="" src="/Content/Images/subscribed_box.png" style="vertical-align: middle;" />
            <p style="width: 135px; overflow: hidden; display: inline;">
                <span class="boxName  border-right black333 txt-13" style="white-space: nowrap;">{BoxName}</span>
                <span class="boxOwnerName black333 txt-13" title="{BoxOwner}" style="white-space: nowrap;">
                    {BoxOwner}</span></p>
            <img alt="" src="/Content/Images/list_delete_icon.png" class="deleteListIcon" />
        </div>
    </div>
    <%--<script id="fileEntryTemplate" type="text/html">--%>
    <div id="fileEntryTemplate" style="display: none;">
        <div class="boxItem fileEntry" data-itemid="{ItemId}">
            <%--<span class="popup-box-item-menu"><span class="box-item-menu-top"><span class="menu-item">
                <a href="{BlobUrl}Download" target="_blank"><span class="iconWidth">
                    <img src="/Content/Images/boxItem/box-icon-download.png" alt="Download" />
                </span>Download</a></span> <span id="LocationmenuItemShareTemplate1"></span></span>
            </span>--%>
            <span id="LocationBoxItemActionDeleteTemplate1"></span>
            <div class="item-thumb">
                <a href="#">
                    <img alt="" original="{ThumbnailBlobUrl}" data-lightbox="true" data-name="{Name}"
                        data-href="{BlobUrl}" data-itemid="{ItemId}" data-filesize="{fileLength}"
                        data-fileuploaded="{CreationTime}" data-uploadername="{UploaderName}" /></a>
            </div>
            <div class="item-label txt-13 black333" title="{Name}">
                {cropFileName}</div>
            <div class="style19AA  txt-11 black333">
                {fileLength}</div>
            <span id="LocationBoxItemDetailTemplate1" class="LocationBoxItemDetailTemplate">
            </span>
        </div>
    </div>
    <%--</script>--%>
    <%--<script id="BoxItemDetailTemplate" type="text/html">--%>
    <div id="BoxItemDetailTemplate" style="display: none;">
        <div class="boxItemDetails">
            <div class="style20">
                <img class="timeIcon" src="/Content/Images/time.png" alt="time:"><span class="item-time"><input
                    type="hidden" value="{CreationTime}"><span>[time]</span></span></div>
        </div>
    </div>
    <%--</script>--%>
    <%--<script type="text/html" id="BoxItemCopyToClipboardMenuTemplate">--%>
    <div id="BoxItemCopyToClipboardMenuTemplate" style="display: none;">
        <div id="div{ItemId}" style="position: relative">
            <div class="copyToClipboard" data-blob="{copyToClipboardData}" id="divsub{ItemId}"
                style="height: 16px;">
                <img src="/Content/Images/boxItem/file-box-divider-small.png" alt="" class="divider" />
                <span class="iconWidth">
                    <img src="/Content/Images/boxItem/file-box-menu-link.png" alt="link" />
                </span><span class="cplink">Copy link</span>
            </div>
        </div>
    </div>
    <%--</script>--%>
    <%--<script type="text/html" id="BoxItemActionDeleteTemplate">--%>
    <div id="BoxItemActionDeleteTemplate" style="display: none;">
        <span class="popup-box-delete"><a class="{removeBoxItem}" id="removeItem_{ItemId}"
            data-isuserallowed="{IsUserDeleteAllowed}" href="#">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        </a></span>
    </div>
    <%--</script>--%>
    <div id="linkEntryTemplate" style="display: none;">
        <div class="boxItem linkEntry" data-itemid="{ItemId}">
            <%--            <span class="popup-box-item-menu">&nbsp; <span class="box-item-menu-top"><span class="menu-item">
                <a href="{Url}" target="_blank"><span class="iconWidth">
                    <img src="/Content/Images/boxItem/box-icon-download.png" alt="Download" />
                </span><span>Download</span> </a></span>
                <img src="/Content/Images/boxItem/file-box-divider-small.png" alt="" class="divider" />
                <span id="LocationmenuItemShareTemplate2"></span></span></span>
            --%>
            <span id="LocationBoxItemActionDeleteTemplate2"></span>
            <div class="item-thumb">
                <img alt="" src="{encodedUrlThumbnail}" data-lightbox="colorboxCombine" data-name="{Name}"
                    data-href="{encodedUrl}" data-itemid="{ItemId}" data-filesize="0" data-fileuploaded="{CreationTime}"
                    data-uploadername="{UploaderName}" />
            </div>
            <div class="item-label" title="{Url}">
                <a href="{Url}" target="_blank" class="txt-13 black333">{cropLinkName}</a></div>
            <span id="LocationBoxItemDetailTemplate2" class="LocationBoxItemDetailTemplate">
            </span>
        </div>
    </div>
    <div id="commentTemplate" style="display: none;">
        <div class="comment" id="comment{CommentId}" data-commentid="{CommentId}">
            <span id="LocationdeleteCommentTemplate"></span>
            <div class="style70  time">
                <span class="item-time">
                    <input type="hidden" value="{CreationTime}" /><span>[time]</span></span>
            </div>
            <div class="comment-avatar">
                <img class="comment-avatar" src="/Content/Images/default-user-avatar.png" alt="avatar" />
            </div>
            <div class="comment-content">
                <div class="commentCreate">
                    <span class="blue-3 txt-13 txt-capitl">{AuthorName}</span>
                </div>
                <span class="comment-text txt-13 gray">{CommentText}</span>
                <div class="comment-content-footer">
                    <a href="#" class="reply style70Reply">reply</a>
                </div>
            </div>
            <br class="clear" />
            <div class="replies">
            </div>
        </div>
    </div>
    <div class="bubble" id="CommentReplyBubble" style="display: none">
        <img src="/content/images/commentPanel/add-comment-arrow.png" alt="" id="Img1" style="margin: 0 0 -5px 26px;" />
        <div class="hor-line">
        </div>
        <div class="contentMain">
            <div class="bubble-item reply-bubble-comment">
                <a class="close-top-bubble" title="cancel">&nbsp;</a>
                <textarea class="reply-bubble-text" rows="2" cols="20"></textarea>
            </div>
        </div>
        <div class="hor-line">
        </div>
        <div class="btn-container">
            <a id="share" class=" submitReply txt-13 txt-shadow rounded  btnBlue white btnPadding">
                Share</a>
        </div>
        <br class="clear" />
    </div>
    <div id="deleteCommentTemplate" style="display: none;">
        <%--<a href="#" class="delete-comment">--%>
        <img alt="X" src="/Content/Images/list_delete_icon.png" class="deleteListIcon border-left delete-comment" />
        <%--</a>--%>
    </div>
    <div id="menuItemShareTemplate" style="display: none;">
        <span class="menu-item menu-item-share">
            <div class="popupMenu">
                <span id="LocationCopyToClipboardTemplate"></span>
                <div class="popupMenuIn">
                    <img src="/Content/Images/boxItem/file-box-divider-small.png" alt="" class="divider" />
                    <span class="iconWidth">
                        <img src="/Content/Images/boxItem/file-box-menu-email.png" alt="email" />
                    </span><span>E-mail</span>
                    <img src="/Content/Images/boxItem/file-box-divider-small.png" alt="" class="divider" />
                    <span class="iconWidth">
                        <img src="/Content/Images/boxItem/file-box-menu-facebook.png" alt="facebook" />
                    </span><span>Facebook</span>
                    <img src="/Content/Images/boxItem/file-box-divider-small.png" alt="" class="divider" />
                    <span class="iconWidth">
                        <img src="/Content/Images/boxItem/file-box-menu-twitt.png" alt="twitter" />
                    </span><span>Twitter</span>
                </div>
            </div>
        </span>
    </div>
    <div id="LightBoxItemComment" style="display: none;">
        <div class="lb-comments-item">
            <img class="float-left" src="/Content/Images/default-user-avatar.png" />
            <div class="float-left lb-comments-item-comment">
                <span class="blue-3 txt-13 arial">{AuthorName} | <span class="item-time">
                    <input type="hidden" value="{CreationTime}" /><span>[time]</span></span><br />
                <p class="gray txt-13 arial">
                    {CommentText}
                </p>
            </div>
            <br class="clear" />
        </div>
    </div>
    <%-- <div id="seachItemTemplate" style="display: none;">
        <div class="boxItem">
            <input type="hidden" value="{Id}" class="item-id"><div class="item-thumb">
                <a title="{FileName}" href="{BlobAddressUri}">
                    <img src="{ThumbnailBlobAddressUri}" alt=""></a></div>
            <div title="{FileName}" class="item-label style18AAA  txt-13 black333">
                {FileName}</div>
            <div class="style19AAA  txt-11 black333">
                ramy763 KB</div>
            <div class="boxItemDetails">
                <div class="style20">
                    <img alt="time:" src="/Content/Images/time.png" class="timeIcon"><span class="item-time"><input
                        type="hidden" value="{CreationTimeEpochMillis}"><span>[time]</span></span></div>
            </div>
        </div>
    </div>--%>
</div>
