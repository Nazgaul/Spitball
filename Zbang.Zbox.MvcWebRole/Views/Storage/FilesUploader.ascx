<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%/*%><script src="~/Scripts/jquery-1.6.2-vsdoc.js" type="text/javascript"></script><%*/%>
<script type="text/javascript">

    $(document).ready(function () {

        $('#selectedBoxIdPermission').change(function () {
            if ($(this).val() > 1) {
                $('#pickfiles').button("option", "disabled", false);
                // uploader.init();

            }
            else {
                $('#pickfiles').button("option", "disabled", true);
                // uploader.destroy();

            }
        });
        $('#pickfiles').button({ disabled: true });

        var textToComment = [];
        var uploader = new plupload.Uploader({
            runtimes: 'html5,flash,silverlight,browserplus',
            browse_button: 'pickfiles',
            container: 'fileUploadContainer',
            /*max_file_size: '2048mb',*/
            chunk_size: '4mb',
            url: '<%: Url.Action("UploadFile","Storage")%>',
            unique_names: true,
            flash_swf_url: '<%: Url.Content("~/Scripts/plupload/plupload.flash.swf") %>',
            silverlight_xap_url: '<%: Url.Content("~/Scripts/plupload/plupload.silverlight.xap") %>'

        });
        
        uploader.bind('Init', function (up, params) {
            var flashCookie = "<% = Request.Cookies[FormsAuthentication.FormsCookieName]==null ? string.Empty : Request.Cookies[FormsAuthentication.FormsCookieName].Value %>";
            up.settings.multipart_params = { boxId: $('#selecteBoxId').val(), flashCookie: flashCookie };
        });
        uploader.bind('BeforeUpload', function (up, file) {
            up.settings.multipart_params['boxId'] = file.boxId;
            up.settings.multipart_params['fileid'] = file.id;
            up.settings.multipart_params['fileSize'] = file.size;
        });

        uploader.init();
        uploader.bind('FilesAdded', function (up, files) {
            var uploadsCounter = $('span#uploadsCounter');
            var boxName = $('#selectedBoxName').text();
            var filelist = $('#filelist');
            var showTitle = false;
            var title = filelist.append('<h2 class="upload-box-header">' + boxName + '</h2>');
            
            $.each(files, function (i, file) {
                if (file.size > 0) {
                    file.boxId = $('#selecteBoxId').val();
                    file.boxName = boxName;

                    filelist.append([
                        '<div id="', file.id, '" class="style19">',
                            file.name, ' (', plupload.formatSize(file.size), ') <b></b>',
                        '</div>'
                        ].join(''));

                    uploadsCounter.text(parseInt(uploadsCounter.text()) + 1);
                    showTitle = true;
                }
                else {
                    uploader.removeFile(uploader.getFile(file.id));
                    title.remove();
                }
            });
            if (!showTitle)
                title.remove();

            up.refresh(); // Reposition Flash/Silverlight  
            //uploader.start();

            // selectedEastAccordionIndex is defined at SiteMaster.js
            if (selectedEastAccordionIndex != 2)
                $('#eastPaneAccordion').accordion('activate', 2);
        });

        uploader.bind('UploadProgress', function (up, file) {

            $('#' + file.id + " b").html(file.percent + "%");
        });

        uploader.bind('Error', function (up, err) {
            $('#filelist').append("<div>Error: " + err.code +
            ", Message: " + err.message +
            (err.file ? ", File: " + err.file.name : "") +
            "</div>");

            up.refresh(); // Reposition Flash/Silverlight
        });

        uploader.bind('FileUploaded', function (up, file, data) {


            var result = JSON.parse(data.response);
            if (result.success == false) {
                if (result.data == 'Unauthorized') {
                    document.location.href = '/';
                }
                else {
                    $('#filelist').append("<div>Error: " + result.data);
                }
            }
            else {
                if (result.Payload != null) {
                    $('#' + file.id + " b").html("100%");

                    Zbox.LoadQuota();

                    var uploadEntry = $('#' + file.id);

                    uploadEntry.prepend('<img src="/Content/Images/check-icon.png" class="upload-ok-icon" />');

                    uploadEntry.delay(2000).fadeOut('slow', function () {
                        var next = uploadEntry.next();
                        // take care to remove the box header if needed
                        if (next.hasClass('upload-box-header') || next.length == 0) {
                            uploadEntry.prev().remove();

                            //We uploading a different box now

                            Zbox.Comments.PostComment(Zbox.Comments.CommentTarget.Box, file.boxId, textToComment.join("<br/>"), onNewBoxCommentSuccess, onCommentSubmissionError);
                            //Clear string            
                            textToComment = [];
                        }

                        uploadEntry.remove();
                    });
                    Zbox.toaster(file.name + ' was uploaded to box');
                    var permission = parseInt($('input#selectedBoxIdPermission').val());

                    if ($('#selecteBoxId').val() == file.boxId) {
                        result.Payload.ItemType = 'file';
                        Zbox.BoxItem.AddBoxItemEntry(result.Payload, permission);
                        UpdateBoxItemRegion();
                    }

                    textToComment.push('+added a file: <a href="' + result.Payload.BlobAddressUri + '">' + result.Payload.FileName + '</a>');

                    var uploadsCounter = $('span#uploadsCounter');

                    var remaining = parseInt(uploadsCounter.text()) - 1;
                    uploadsCounter.text(remaining);
                }
            }

            up.refresh();


        });

    });

    
   
</script>
<div>
    <div id="fileUploadContainer">
        <%-- <div class="rounded fixed-button style7">
            <div id="addFile" class="my-button">--%>
        <button id="pickfiles">
            + Add File</button>
        <%--<a id="pickfiles" href="#">+ Add File</a>--%>
        <%--   </div>
        </div>--%>
    </div>
</div>
