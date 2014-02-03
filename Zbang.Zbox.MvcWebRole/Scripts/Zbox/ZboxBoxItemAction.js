/// <reference path="/Scripts/jquery-1.6.2-vsdoc.js" />
var currentActiveElement = 'default';   // for comment-creating menu
$(document).ready(function () {
    if ($('#conversations-container').length > 0) { //this is trigger also when we are on the unreg pages which sometime doesnt have this parts of the html
        $("#newCommentLinkTxt").watermark('Say something about this link...', { className: 'watermarkStyle', useNative: false });
        $("#newCommentFileTxt").watermark('Say something about these files...', { className: 'watermarkStyle', useNative: false });
        $('#newLinkTxt').watermark('http://', { className: 'watermarkStyle', useNative: false });
        var $divFilesToUploadConversation = $('#divFilesToUploadConversation');
        if ($divFilesToUploadConversation.length > 0) {
            $divFilesToUploadConversation.simplyScroll({ height: 68 });
            Zbox.BoxItemAction.InitFile();

        }
        Zbox.BoxItemAction.bindEvents();
        $("#share").click(function () {
            Zbox.BoxItemAction.SubmitAction();
        });
    }



});

Zbox.BoxItemAction = {}

Zbox.BoxItemAction.CheckForPermission = function (permissionValue) {
    var permission = parseInt($('#selectedBoxIdPermission').val());
    return permission > permissionValue;
}

Zbox.BoxItemAction.ActionPermissionAllowed = function (permissionValue, ActionToComple) {
    if ($('#emailVerificationStatus').data('verifed')) {
        if (this.CheckForPermission(permissionValue)) {
            ActionToComple();
        }
        else {
            Zbox.InsufficentPermission();
        }
    }
    else {
        Zbox.VerifyAccount();
    }
}

Zbox.BoxItemAction.bindEvents = function () {
    var self = this;
    var $commentTop = $('#comments-top');
    var $plupload = $commentTop.find('.plupload');
    var $bubbleItem = $commentTop.find('.bubble-item');
    // add new comment
    $("#addCommentTop").bind('click', function (e) {
        e.stopPropagation();
        self.ActionPermissionAllowed(1, function () {
            $plupload.css('z-index', -1);
            $('#CommentReplyBubble').slideUp(200); // close all replies
            if (currentActiveElement != "comment") {
                $bubbleItem.slideUp(200);
                $("#bubble-add-comment").slideDown(200, function () {
                    resizeContext($(document));
                });
                $("#newCommentTxt").focus();
                $("#share").show();
                $("#bubble-arrow").css('margin', '0 0 -5px 32px');
                currentActiveElement = "comment";

            }
        });
    });
    $("#bubble-default").bind('click', function (e) {
        //e.stopPropagation();
        $("#addCommentTop").click();
    });
    // add link
    $("#addLinkTop").bind('click', function (e) {
        e.stopPropagation();
        self.ActionPermissionAllowed(1, function () {
            $plupload.css('z-index', -1)
            $('#CommentReplyBubble').slideUp(200); // close all replies
            if (currentActiveElement != "link") {
                $bubbleItem.slideUp(200);
                $("#bubble-add-link").slideDown(200, function () {
                    resizeContext($(document));
                });
                $("#share").show();
                $("#bubble-arrow").css('margin', '0 0 -5px 131px');
                currentActiveElement = "link";
            }
        });
    });
    // add file
    $("#addFileTop").bind('click', function (e) {
        e.stopPropagation();
        self.ActionPermissionAllowed(1, function () {
            $('#CommentReplyBubble').slideUp(200); //close all replies
            if (currentActiveElement != "file") {
                $bubbleItem.slideUp(200);
                //$(".bubble-item").slideUp(200);
                $("#bubble-add-file").slideDown(200, function () {

                    if (!Zbox.BoxItemAction.IsSupportHtml5FileSystem())
                    // need to refresh the uploader for it to work - its need to be visible
                        $plupload.css('z-index', 9999)
                    self.uploader.refresh();
                    resizeContext($(document));
                });

                $("#AddFiles").focus();

                $("#share").show();
                $("#bubble-arrow").css('margin', '0 0 -5px 239px');
                currentActiveElement = "file";
            }
        });
    });

    $('#add-file-container').click(function (e) {
        if (Zbox.HasClass(e.target, 'add-file-scroll-delete')) {
            var $e = $(e.target);
            self.removeFileFromUploadQueue(e.target.getAttribute('data-id'));
            $e.parent().remove();
            self.UpdateStatusUploadFiles();
        }
    });

    $commentTop.find('a.close-top-bubble').click(function () {
        //$("a.close-top-bubble", $commentTop).click(function () { // when clicking on "x" inside the top bubble - add comment/link/file
        self.resetCommentsTop($commentTop);
    });

    $("#newCommentTxt").keyup(function (e) {
        if (e.which == jQuery.ui.keyCode.ENTER) {
            var $this = $(this);
            var numOfRows = $this.attr('rows');
            $this.attr('rows', ++numOfRows);
        }
    });

}
Zbox.BoxItemAction.resetCommentsTop = function (commentTop) {
    commentTop.find('div.bubble-item').slideUp(200, function () {
        //$(".bubble-item", "#comments-top").slideUp(200, function () {
        $("#bubble-default").slideDown(200);
        resizeContext($(document));
    });

    $("#share").hide();
    currentActiveElement = "default";
}

Zbox.BoxItemAction.removeFileFromUploadQueue = function (fileId) {
    var file = this.uploader.getFile(fileId);
    this.uploader.removeFile(file);
    $('#' + fileId).remove();
    this.IsRemoveFileListTitle();
}

Zbox.BoxItemAction.resetLayout = function () {

    //function resetLayout() {
    $('#comments-top').find("div.bubble-item").slideUp(200);
    $("#bubble-default").slideDown(200, function () {
        resizeContext($(document));
    });
    $("#share").hide();
    $("#bubble-arrow").css('margin', '0 0 -5px 32px');
    currentActiveElement = "default";
    $("#instructions").show();

    this.resetCommentText();
    this.resetLinkText();
    this.resetFileText();
}

Zbox.BoxItemAction.resetLinkText = function () {
    //function resetLinkText() {
    $('#newCommentLinkTxt').val('');
    $('#newLinkTxt').val('');

}
Zbox.BoxItemAction.resetFileText = function () {
    $('#newCommentFileTxt').val('');
    $('#divFilesToUploadConversation').empty();
    this.UpdateStatusUploadFiles();
}

Zbox.BoxItemAction.resetCommentText = function () {
    //function resetCommentText() {
    $("#newCommentTxt").val("");
}

Zbox.BoxItemAction.SubmitAction = function () {
    //function SubmitAction() {
    var boxId = $('#selecteBoxId').val();
    if (currentActiveElement == "link") {
        var newComment = $('#newCommentLinkTxt').val();
        var newLink = jQuery.trim($('#newLinkTxt').val());
        this.SubmitLink(newLink, boxId, newComment);
    }
    if (currentActiveElement == 'file') {

        this.commentText = $('#newCommentFileTxt').val();
        $('#eastPaneAccordion').accordion('activate', 2);
        if (!this.IsSupportHtml5FileSystem()) {
            if (Zbox.BoxItemAction.uploader.total.size > plupload.parseSize('20mb')) {
                Zbox.BoxItemAction.uploader.runtime = 'flash';
            }
            this.BatchSize = $('#divFilesToUploadConversation').children().length;
            this.uploader.start();

        }
        this.resetLayout();
    }
    if (currentActiveElement == "comment") {

        var commentText = $("#newCommentTxt").val();
        Zbox.Comments.PostComment(Zbox.Comments.CommentTarget.Box, boxId, commentText, this.onNewBoxCommentSuccess, this.ShowError);
        this.resetLayout();
    }
}


Zbox.BoxItemAction.ShowError = function (errorText) {
    //function ShowError(errorText) {
    var $ConversationSubmitError = $('#ConversationSubmitError');
    $ConversationSubmitError.text(errorText);
    $ConversationSubmitError.fadeOut(5000, function () {
        $ConversationSubmitError.text('');
        $ConversationSubmitError.toggle();
    });
}

Zbox.BoxItemAction.onNewBoxCommentSuccess = function (comment, boxid) {

    //Now that we upload comment when file is upload and user change box in the middle we can render comment in the wrong box
    if (boxid == $('#selecteBoxId').val()) {
        Zbox.Comments.RenderComment(comment, function (html) {
            // $('#box-comments').prepend(html);
            $(html).hide().prependTo('#box-comments').fadeIn();
        });
        Zbox.UpdateScreenTime();
    }
}

Zbox.BoxItemAction.SubmitLink = function (newLink, boxId, comment) {
    //var url_pattern = new RegExp("((http|https)(:\/\/))?([a-zA-Z0-9]+[.]{1}){2}[a-zA-z0-9]+(\/{1}[a-zA-Z0-9]+)*\/?", "i");
    // var url_pattern = /http:\/\/[A-Za-z0-9\.-]{3,}\.[A-Za-z]{3}/

    if (typeof (newLink) == 'undefined' || newLink == null || newLink.length == 0) {
        this.ShowError('link cannot be empty');
        return;
    }
    //    if (!url_pattern.test(newLink)) {
    //        this.ShowError('invalid link');
    //        return;
    //    }
    Zbox.BoxItemAction.resetLayout();
    var addLinkRequest = new ZboxAjaxRequest({
        url: '/Storage/AddLink',
        data: { url: encodeURI(newLink), boxId: boxId, linkComment: comment },
        success: function (data) {
            var permission = parseInt($('input#selectedBoxIdPermission').val());
            Zbox.BoxItem.AddBoxItemEntry(data.link, permission, true);
            Zbox.BoxItem.UpdateBoxItemRegion();
            Zbox.BoxItemAction.onNewBoxCommentSuccess(data.comment, data.boxId);

            Zbox.toaster('Link was Uploaded to box');


        },
        error: function (error) {
            Zbox.BoxItemAction.ShowError(error);
        }
    });

    addLinkRequest.Post();
}





//files section
Zbox.BoxItemAction.uploader = '';
Zbox.BoxItemAction.commentText = '';
Zbox.BoxItemAction.BatchSize = '';

Zbox.BoxItemAction.IsSupportHtml5FileSystem = function () {
    //return window.File && window.FileReader && window.FileList && window.Blob;
    return false;
}
Zbox.BoxItemAction.DeleteFileFromQueue = function () {
    $('#filelist').delegate('img.deleteFile', 'click', function () {

        var uploader = Zbox.BoxItemAction.uploader;
        var fileId = this.parentNode.id;
        var file = uploader.getFile(fileId);
        if (file.status === plupload.UPLOADING) {
            uploader.stop();
            uploader.removeFile(file);
            uploader.start();
        }
        else if (file.status === plupload.QUEUED) {
            uploader.removeFile(file);
        }
        $(this).parent().remove();
        var $h2 = $('#filelist').find('h2');
        $.each($h2, function (i, h2) {
            var $title = $(h2);
            if ($title.next('div').length === 0)
                $title.remove();
        });
        Zbox.BoxItemAction.UpdateStatusUploadFiles()


    });
}
Zbox.BoxItemAction.InitFile = function () {

    //    $('a#AddFiles').click(function () {
    //        $('input#inputAddFiles').click();
    //        return false;
    //    });
    //    if (this.IsSupportHtml5FileSystem()) {
    //        // regular input
    //        document.getElementById('inputAddFiles').addEventListener('change', handleFileSelect, false);

    //        //drag & drop
    //        //var dropZone = document.getElementById('divFilesToUploadConversation').parentNode;
    //        //dropZone.addEventListener('dragover', handleDragOver, false);
    //        //dropZone.addEventListener('drop', handleFileSelectDrop, false);




    //        // File API is supported.
    //    }
    //    else {
    this.DeleteFileFromQueue();
    Zbox.BoxItemAction.InitPlUpload();
    //}

}

//Zbox.BoxItemAction.AjaxPending = {
//    comment: false,
//    item: false,
//    friends:false,
//    AjaxGet : []
//}

//Zbox.BoxItemAction.FireAjax = function () {
//    var uploader = Zbox.BoxItemAction.uploader;
//    return uploader.state === plupload.STARTED;
////    if (uploader.state === plupload.STARTED) {

////    }
//}

Zbox.BoxItemAction.InitPlUpload = function () {
    var guid = null;
    this.uploader = new plupload.Uploader({
        runtimes: 'html5,flash,silverlight,browserplus',
        browse_button: 'AddFiles',
        container: 'comments-top',
        //drop_element: 'add-file-container',
        chunk_size: '100kb',
        url: 'AStorage/UploadFile',
        unique_names: true,
        flash_swf_url: '/Scripts/plupload/plupload.flash.swf',
        silverlight_xap_url: '/Scripts/plupload/plupload.silverlight.xap',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }


    });
    var uploader = this.uploader;
    var nativeFiles = {};
    this.uploader.bind('PostInit', function (up, params) {
        // Initialize Preview.
        if (uploader.runtime == "html5") {
            var inputFile = document.getElementById(uploader.id + '_html5');
            var oldFunction = inputFile.onchange;

            inputFile.onchange = function () {
                nativeFiles = this.files;
                oldFunction.call(inputFile);
            }
        }
    });

    this.uploader.bind('Init', function (up, params) {

        //var flashCookie = $('input#flashcookie').val(); //"<% = Request.Cookies[FormsAuthentication.FormsCookieName]==null ? string.Empty : Request.Cookies[FormsAuthentication.FormsCookieName].Value %>";
        up.settings.multipart_params = { boxId: $('#selecteBoxId').val()/*, flashCookie: flashCookie */ };
    });
    this.uploader.bind('BeforeUpload', function (up, file) {
        try {
            up.settings.multipart_params['boxId'] = file.boxId;
            up.settings.multipart_params['fileid'] = file.id;
            up.settings.multipart_params['fileName'] = file.name;
            up.settings.multipart_params['fileSize'] = file.size;
            up.settings.multipart_params['uploadBatch'] = file.uploadBatch;
            up.settings.multipart_params['userComment'] = Zbox.BoxItemAction.commentText;
            up.settings.multipart_params['fileindex'] = file.index;
            up.settings.multipart_params['batchSize'] = Zbox.BoxItemAction.BatchSize;
        }
        catch (err) {
        }

    });

    this.uploader.init();



    this.uploader.bind('UploadProgress', function (up, file) {
        $('#' + file.id + " b").html(file.percent + "%");
    });

    this.uploader.bind('Error', function (up, err) {
        if (err.status = 401) {
            document.location.href = '/';
        }
        $('#filelist').append("<div>Error: " + err.code +
            ", Message: " + err.message +
            (err.file ? ", File: " + err.file.name : "") +
            "</div>");

        up.refresh(); // Reposition Flash/Silverlight
    });


    this.uploader.bind('FileUploaded', function (up, file, data) {

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

                Zbox.LoadUserStatus();

                var uploadEntry = $('#' + file.id);

                uploadEntry.prepend('<img src="/Content/Images/check-icon.png" class="upload-ok-icon" />');

                uploadEntry.delay(2000).fadeOut('slow', function () {
                    var next = uploadEntry.next();
                    // take care to remove the box header if needed
                    if (next.hasClass('upload-box-header') || next.length == 0) {
                        uploadEntry.prev().remove();
                    }

                    uploadEntry.remove();
                });
                Zbox.toaster(file.name + ' was uploaded to box');
                var permission = parseInt($('#selectedBoxIdPermission').val());

                if ($('#selecteBoxId').val() == file.boxId) {
                    result.Payload.ItemType = 'file';
                    Zbox.BoxItem.AddBoxItemEntry(result.Payload.fileItem, permission, true);
                    var $divFiles = $("#divFiles");
                    var $imgInDiv = $divFiles.find('.item-thumb').find('img');

                    if (result.Payload.comment != null) {
                        Zbox.BoxItemAction.onNewBoxCommentSuccess(result.Payload.comment, result.Payload.boxId);
                    }
                    Zbox.BoxItem.UpdateBoxItemRegion();
                    Zbox.LazyLoadOfImages($divFiles, $imgInDiv);
                }
                //Zbox.BoxItemAction.commentText.push('<a href="' + result.Payload.BlobAddressUri + '">' + result.Payload.FileName + '</a>');
                var uploadsCounter = $('#uploadsCounter');

                var remaining = parseInt(uploadsCounter.text()) - 1;
                //$('span#filesCounter').text(parseInt($('span#filesCounter').text()) - 1);
                uploadsCounter.text(remaining);
                if (remaining === 0) {
                    Zbox.BoxItemAction.commentText = '';
                    $('#eastPaneAccordion').accordion('activate', 2);
                }

            }

            up.refresh();



        }
    });

    this.uploader.bind('FilesAdded', function (up, files) {
        var uploadsCounter = $('#uploadsCounter');
        var boxName = $('#selectedBoxName').text();
        var filelist = $('#filelist');

        if ($('#divFilesToUploadConversation').children().length == 0) {
            var title = filelist.append('<h2 class="upload-box-header">' + boxName + '</h2>');
            guid = plupload.guid();
        }



        $.each(files, function (i, file) {
            var FILEID = file.id;
            if (file.size > 0) {
                file.boxId = $('#selecteBoxId').val();
                file.boxName = boxName;
                file.uploadBatch = guid;
                file.index = i;
                filelist.append([
                        '<div id="', file.id, '" class="style19">',
                            file.name, ' (', plupload.formatSize(file.size), ') <b></b><img alt="" src="/Content/Images/list_delete_icon.png" class="deleteFile"  />',
                        '</div>'
                        ].join(''));

                //For now
                if (uploader.runtime == "html5" && window.File && window.FileReader && window.FileList && window.Blob) {
                    var fileObject = uploader.getFile(file.id);
                    if (nativeFiles[i].type.match('image.*')) {
                        var reader = new FileReader();

                        reader.onload = (function (file, id) {
                            return function (e) {
                                var li = $('<li style="width:136px;" class="add-file-thumb"></li>');
                                li.html('<a class="add-file-scroll-delete" data-id="' + FILEID + '">&nbsp;</a><div class="thumb-container"><img class="thumb" src="' + e.target.result + '" /></div><span class="style18addFiles">' + file.name + '<br/>' + plupload.formatSize(file.size) + '</span>');


                                $('#divFilesToUploadConversation').prepend(li);

                                Zbox.BoxItemAction.UpdateStatusUploadFiles();

                            };
                        })(nativeFiles[i], file.id);

                        reader.readAsDataURL(nativeFiles[i]);
                    }
                    else {
                        $('#divFilesToUploadConversation').prepend('<li style="width:136px;" class="add-file-thumb"><a class="add-file-scroll-delete" id="RemoveFilesFromUpload" data-id="' + file.id + '">&nbsp;</a><div class="thumb-container"><img src="/Content/Images/icons/file_placeholder.png" alt="" /></div><span class="style18addFiles">' + file.name + '<br/>' + plupload.formatSize(file.size) + '</span></li>');
                    }
                }
                else {
                    $('#divFilesToUploadConversation').prepend('<li style="width:136px;" class="add-file-thumb"><a class="add-file-scroll-delete" id="RemoveFilesFromUpload" data-id="' + file.id + '">&nbsp;</a><div class="thumb-container"><img src="/Content/Images/icons/file_placeholder.png" alt="" /></div><span class="style18addFiles">' + file.name + '<br/>' + plupload.formatSize(file.size) + '</span></li>');
                }
                Zbox.BoxItemAction.UpdateStatusUploadFiles();
            }
            else {
                uploader.removeFile(uploader.getFile(file.id));
            }

        });


        Zbox.BoxItemAction.IsRemoveFileListTitle();

        up.refresh();


    });
}

Zbox.BoxItemAction.UpdateStatusUploadFiles = function () {

    var uploadsCounter = $('#uploadsCounter');
    var $divFilesToUploadConversation = $('#divFilesToUploadConversation');
    $divFilesToUploadConversation.trigger('elementChanged');
    uploadsCounter.text(this.uploader.total.queued);
    $('#filesCounter').text($divFilesToUploadConversation.children().length);
}

Zbox.BoxItemAction.IsRemoveFileListTitle = function () {
    var headers = $('#filelist').children(':header')
    $.each(headers, function (i, header) {
        if (!$(header).next().is('div')) {
            $(header).remove();
            if (Zbox.BoxItemAction.uploader.features.html5) {
                Zbox.BoxItemAction.uploader.runtime = 'html5'
            }
        }
    });
}



