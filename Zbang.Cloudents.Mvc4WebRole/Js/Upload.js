/// <reference path="/Js/Resource/ZboxResources.js" />
(function (mmc, $) {
    "use strict";
    var popUpElement,
    $uploadNext = $('#uploadNext'),
    $upldBoxName = $('#BoxName'),
    firstTimeFired = true,
    //stage = {
    //    upload: 3
    //},
    boxToUpload = {
        name: '',
        id: '',
    },
    itemTemplate = '<li id="{1}"><span class="percent"></span><span class="fileName elps">{0}</span><button class="rFloat pending">{2}</button><span class="rFloat finished uploadSprite"></span></li>',
    uploader = new plupload.Uploader({
        runtimes: 'html5,flash,silverlight,html4',
        browse_button: 'AddFiles',
        drop_element: 'fileZone',
        chunk_size: '3mb',
        url: '/Upload/File',
        unique_names: true,
        flash_swf_url: '/Scripts/plupload/plupload.flash.swf',
        silverlight_xap_url: '/Scripts/plupload/plupload.silverlight.xap',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    });

    mmc.uploadV2 = function (elem) {
        popUpElement = $(elem);
    };
    mmc.uploadpopUp = function () {
        boxToUpload.id = Zbox.getParameterByName('boxuid');
        boxToUpload.name = $('#boxName').text();
        if (firstTimeFired) {
            firstTimeFired = false;
            registerEvents();
        }
        popUpElement.show();
        showState();
        mmc.modal(function () {
            if (uploader.state === plupload.UPLOADING) {
                if (!mmc.confirm('You still uploading files. are you sure you want to close?')) {
                    return false;
                }
                uploader.stop();
            }
            popUpElement.hide();
            cleanUp();
            
        }, 'upload');
    };

    function registerEvents() {
        $uploadNext.click(function () {
            if (uploader.state === plupload.UPLOADING) {
                if (!mmc.confirm('You still uploading files. are you sure you want to close?')) {
                    return;
                }
                uploader.stop();
            }
            $('.modal').trigger('close');
            mmc.invitePopUp(boxToUpload.id, boxToUpload.name);
        });
        uploadFiles();
    }
    function cleanUp() {
        $upldBoxName.val('');
        var forms = popUpElement.find('form');
        for (var i = 0; i < forms.length; i++) {
            mmc.clearErrors(forms[i]);
        }
        $('.field-validation-error').each(function () {
            $(this).text('');
        });
        $('#Url').val('');
        $('#fileZone').find('ul').empty().hide();

        $('#DragDropText').show();

    }

    function itemUploaded(id) {
        var elem = $('#' + id);
        elem.find('.finished').show();
        elem.find('button').hide();
        elem.find('.percent').hide();
    }
    function uploadFiles() {
        uploader.bind('Init', function (up) {
            up.settings.multipart_params = {};
            up.settings.multipart_params.uploadId = plupload.guid();
        });

        uploader.bind('BeforeUpload', function (up, file) {
            try {
                up.settings.multipart_params.BoxUid = boxToUpload.id;
                up.settings.multipart_params.fileId = file.id;
                up.settings.multipart_params.fileName = file.name;
                up.settings.multipart_params.fileSize = file.size;
            }
            catch (err) {
            }

        });
        uploader.init();
        if (!uploader.features.dragdrop) {
            $('#DragDropText').remove();
        }
        $('#fileZone').on('click', 'button.pending', function () {
            var li = $(this).parents('li');
            var file = uploader.getFile(li[0].id);
            uploader.removeFile(file);
            li.remove();
            var ul = $('#fileZone').find('ul');
            if (!ul.children('li').length) {
                ul.hide();
                //$('#DragDropText').removeClass('dragDropText');
                //$('#DragDropText').show();
            }
        });

        uploader.bind('FilesAdded', function (up, files) {
            var html = '';
            for (var i = 0; i < files.length; i++) {
                if (files[i].name.indexOf('.') === -1 && files[i].size === 4096) {
                    mmc.notification(ZboxResources.CantUploadDirectory);
                    uploader.removeFile(files[i]);
                    continue;
                }
                if (files[i].size === 0) {
                    mmc.notification(ZboxResources.CantUploadEmptyFile);
                    uploader.removeFile(files[i]);
                    continue;
                }
                html += itemTemplate.format(files[i].name, files[i].id, ZboxResources.Cancel);
            }
            addItemToUploadList(html);
            if (uploader.state === plupload.STOPPED) {
                uploader.start();
            }
        });

        var progressBarMaxwidth = $('#fileZone').width();
        uploader.bind('UploadProgress', function (up, file) {
            $('#' + file.id).find('span.percent').width(progressBarMaxwidth * (file.percent / 100));
        });

        //uploader.bind('Error', function (up, err) {
        //});

        uploader.bind('FileUploaded', function (up, file, data) {
            var itemData = JSON.parse(data.response);
            itemUploaded(file.id);
            mmc.doUpdate('addItem', itemData.Payload);
        });
    }
    function addItemToUploadList(html) {
        $('#fileZone').find('ul').append(html).show();
        $('#DragDropText').hide();
    }
    function showState() {
        $uploadNext.val(ZboxResources.Next);
        uploader.refresh();
        $('#upldstep3BoxName').text('‎“' + boxToUpload.name + '”‎').css('overflow', 'hidden'); // overflow is for the ellipsis (property set in css caused problems);
        $('#addLinkBox').val(boxToUpload.id);
    }

    mmc.addLinkSuccess = function (data) {
        if (data.Success) {
            var $url = $('#Url');
            var id = plupload.guid();
            addItemToUploadList(itemTemplate.format($url.val(), id, ZboxResources.Cancel));
            $url.val('');
            itemUploaded(id);
            mmc.clearErrors($('#fAddLink'));
            mmc.doUpdate('addItem', data.Payload);
        }
        else {
            mmc.modelErrors($('#fAddLink'), data.Payload);
        }
    };
}(window.mmc = window.mmc || {}, jQuery))