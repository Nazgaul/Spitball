<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%/*%><script src="~/Scripts/jquery-1.6.2-vsdoc.js" type="text/javascript"></script><%*/%>
<%--<script type="text/javascript">
       

    $(document).ready(function () {
        var addlinkDialog = $('div#divAddLink');
        addlinkDialog.detach();
        addlinkDialog.insertAfter($('#comment'));

        $('button#btnCancelLink').button();
        $('button#btnOkLink').button();
        $('#txtUrl').watermark(defaultLink);
        $('#btnAddLink').click(function () {
            $('div#new-comment-container').slideUp(function () {
                addlinkDialog.slideDown();
            }); // if comment is open we want to close it.

            return false;
        });

        $('#txtUrl').keypress(function (e) {
            if (e.which == $.ui.keyCode.ENTER) {
                $('button#btnOkLink').trigger('click');
                e.preventDefault();
            }
        });


        $('button#btnOkLink').click(function () {
            var newLink = jQuery.trim($('#txtUrl').val());
            var boxId = $('#selecteBoxId').val();

            if (typeof (newLink) == 'undefined' || newLink == null || newLink.length == 0) {
                showMessage('link cannot be empty');
            }

            var addLinkRequest = new ZboxAjaxRequest({
                url: '/Storage/AddLink',
                data: { url: encodeURI(newLink), boxId: boxId },
                success: function (link) {
                    showMessage("Added");

                    var permission = parseInt($('input#selectedBoxIdPermission').val());
                    Zbox.BoxItem.AddBoxItemEntry(link, permission);


                    Zbox.toaster('Link was Uploaded to box');
                    UpdateBoxItemRegion();
                    ResetLinkDialog();

                    //Add a comment
                    submitCommentAutomatic('+added a link: <a href="' + link.Url + '">' + link.Url + '</a>');

                },
                error: function (error) {
                    showMessage(error);
                }
            });

            addLinkRequest.Post();
            return false;
        });
        $('button#btnCancelLink').click(function () {
            ResetLinkDialog();
            return false;
        });
    });

    function ResetLinkDialog() {
        $('#txtUrl').val('');
        $('#divAddLink').slideUp();
        showMessage('');
    }

    function showMessage(message) {
        $('#AddLinkMessage').text(message);
    }
</script>
<div id="divAddLink" style="display: none;">
    <div class="sub-container rounded black-border shadow curvyIgnore">
        <input type="text" id="txtUrl" name="Url" value="" />
        <div class="button-container">
            <button id="btnCancelLink">
                Cancel</button>
            <button id="btnOkLink">
                Add</button>
        </div>
        <div id="AddLinkMessage" class="baloon-feedback">
        </div>
    </div>
</div>--%>
