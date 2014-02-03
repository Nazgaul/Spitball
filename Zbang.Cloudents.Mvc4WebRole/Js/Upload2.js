(function ($, plupload, cd, dataContext, ZboxResources, analytics) {    
    "use strict";
    if (window.scriptLoaded.isLoaded('upload2')) {
        return;
    }
     //var itemTemplate = '<li id="{1}"><span class="percent"></span><span class="fileName elps">{0}</span><button class="rFloat pending">{2}</button><span class="rFloat finished uploadSprite"></span></li>',
    var uploader,
        $uploads = $('#uploads'),
    $uploadDialog = $('#uploadDialog'),
    boxToUpload = {
        id: '',
        tabid: null,
        question: false,
    },
    firstTimeFired = true;


    cd.loadModel('box', 'BoxContext', function () {
        $uploadDialog = $('#uploadDialog');
        $('#up_dropZone').hide();
        //uploader.destroy();
        if (!uploader) {
            uploader = uploadFiles();
            registerDnd();
        }
    });

    cd.pubsub.subscribe('box', function (d) {
        boxToUpload.id = parseInt(d.id, 10);
        uploader.refresh();
    });
    cd.pubsub.subscribe('itemTab', function (d) {
        boxToUpload.tabid = $.isEmptyObject(d) ? null : d;
    });

    cd.pubsub.subscribe('upload', function (d) {
        $uploadDialog.show();
        uploader.refresh();
        //uploader.destroy();
        //uploader = uploadFiles();

        uploadVisible();
        boxToUpload.question = $.isEmptyObject(d) ? false : d.question;
    });
    function trackUpload(action, label) {
        analytics.trackEvent('Upload', action, label);
    }
    function uploadVisible() {
        if (firstTimeFired) {
            firstTimeFired = false;
            registerEvents();
        }

    }
    function registerEvents() {
        cd.loader.registerDropBox();

        var d = cd.loader.registerGoogleDrive();//, s = cd.loader.registerSkyDrive();
        $.when(d).done(function () {
            $('#up_Drive').removeAttr('disabled title');
        });
        //$.when(s).done(function () {
        //    $('#up_Sky').removeAttr('disabled').removeAttr('title');
        //})
        var interval = window.setInterval(function () {
            if (window.Dropbox !== undefined) {
                $('#up_DropBox').removeAttr('disabled title');
                window.clearInterval(interval);
            }
        }, 50);

        var $AddLink = $('#AddLink');
        $('#up_AddLink').click(function () {
            closeDialog();
            $AddLink.show().find('input.addUrl').focus();
        });
        var $fAddLink = $('#fAddLink');

        $fAddLink.submit(function (e) {
            var $url = $('#Url');
            /// <summary></summary>
            e.preventDefault();
            var $form = $(this);
            if (!$form.valid || $form.valid()) {
                var fdata = $form.serializeArray();
                fdata.push({ name: 'TabId', value: boxToUpload.tabid });
                fdata.push({ name: 'BoxId', value: boxToUpload.id });
                fdata.push({ name: 'UniName', value: cd.getParameterFromUrl(1) });
                fdata.push({ name: 'BoxName', value: cd.getParameterFromUrl(3) });
                var guid = fakeUpload($url.val(), null, 0);
                dataContext.addLink({
                    data: fdata,
                    success: function (itemData) {
                        $url.val('');
                        cd.resetForm($fAddLink);
                        $AddLink.hide();
                        $uploads.show();
                        finishFakeUpload(guid);
                        itemUploaded(itemData, boxToUpload.id);

                        trackUpload('upload link', '');
                    },
                    error: function (data) {
                        cd.displayErrors($fAddLink, data);
                    }
                });
            }



        });

        $('#up_DropBox').click(function () {
            Dropbox.choose({
                success: function (files) {
                    for (var i = 0; i < files.length; i++) {
                        uploadDropBox(files[i].link, files[i].name, files[i].bytes);
                    }
                },
                linkType: "direct",
                multiselect: true
            });
            function uploadDropBox(link, name, size) {
                var guid = fakeUpload(link, name, size);
                dataContext.dropBox({
                    data: {
                        boxUid: boxToUpload.id,
                        fileUrl: link,
                        fileName: name,
                        tabId: boxToUpload.tabid,
                        uniName: cd.getParameterFromUrl(1),
                        boxName: cd.getParameterFromUrl(3)
                    },
                    success: function (itemData) {
                        trackUpload('upload by dropbox', 'Number of uploads by dropbox');
                        closeDialog();
                        finishFakeUpload(guid);
                        itemUploaded(itemData, boxToUpload.id);
                    },
                    error: function () {
                        closeDialog();
                    }
                });
            }
        });
        //$('#up_Sky').click(function () {
        //WL.login({ "scope": "wl.skydrive wl.signin" }).then(
        //     function (response) {
        //         openFromSkyDrive();
        //     },
        //     function (response) {
        //         log("Failed to authenticate.");
        //     }
        // );
        //function openFromSkyDrive() {
        //    WL.fileDialog({
        //        mode: 'open',
        //        select: 'single'
        //    })
        //}
        //    WL.fileDialog({
        //        mode: "open",
        //        select: "multi"
        //    });
        //});
        // cd.innerScroll($('#uploadList'), parseInt($('#uploadList').css('maxHeight').replace('px', ''), 10));
        document.getElementById('up_Drive').addEventListener('click', function () {
            var picker = new google.picker.PickerBuilder().
                addView(google.picker.ViewId.DOCS).
                enableFeature(google.picker.Feature.NAV_HIDDEN).
                enableFeature(google.picker.Feature.MULTISELECT_ENABLED).
                setDeveloperKey('AIzaSyBqnR38dm9S2E-eQWRj-cTgup2kGA7lmlg').
                setCallback(pickerCallback).
                build();
            picker.setVisible(true);


            // A simple callback implementation.
            function pickerCallback(data) {
                if (data[google.picker.Response.ACTION] !== google.picker.Action.PICKED) {
                    return;
                }

                var doc, url, name,
                    fdata = [{ name: 'TabId', value: boxToUpload.tabid },
                        { name: 'BoxId', value: boxToUpload.id },
                        { name: 'Url', value: '' },
                        { name: 'UniName', value: cd.getParameterFromUrl(1) },
                        { name: 'BoxName', value: cd.getParameterFromUrl(3) }];
                for (var i = 0, l = data[google.picker.Response.DOCUMENTS].length; i < l ; i++) {
                    doc = data[google.picker.Response.DOCUMENTS][i];
                    url = doc[google.picker.Document.URL];
                    name = doc.name;
                    if (!url) {
                        continue;
                    }

                    fdata[2].value = url;
                    var guid = fakeUpload(url, name, 0);
                    (function (fileData, name, fGuid) {
                        dataContext.addLink({
                            data: fileData,
                            success: function (itemData) {
                                finishFakeUpload(fGuid);
                                itemUploaded(itemData, boxToUpload.id);
                            }
                        });
                    })(fdata, name, guid);

                }
                closeDialog();
                trackUpload('upload by google drive', 'Number of uploads by Google drive');

                //var doc = ;
                //url = ;
                //if (!url) {
                //    return;
                //}
                //var fdata = [];
                //fdata.push({ name: 'TabId', value: boxToUpload.tabid });
                //fdata.push({ name: 'BoxId', value: boxToUpload.id });
                //fdata.push({ name: 'Url', value: url });
                //dataContext.addLink({
                //    data: fdata,
                //    success: function (itemData) {
                //        trackUpload('upload by google drive', 'Number of uploads by Google drive');
                //        itemUploaded(itemData, boxToUpload.id);
                //        closeDialog();
                //    }
                //});

            }
        }, false);

    }

    function fakeUpload(url, name, size) {
        var guid = cd.guid(), $progressBarMaxwidth, $fileId;

        var t = cd.attachTemplateToData('templateUpload', {
            name: name !== null ? name : url,
            size: size > 0 ? plupload.formatSize(size) : '',
            id: guid
        });
        t = t.replace('()', '');
        addItemToUploadList(t);
        $fileId = $('#' + guid);
        $progressBarMaxwidth = $('.progress').width();
        $fileId.find('.progress').data('percentage', 50);
        $fileId.find('span.progressFill').width($progressBarMaxwidth * (0.5));
        updateTitle(guid, 50);

        return guid;
    }
    function finishFakeUpload(guid) {
        var $fileId = $('#' + guid),
            $progressBarMaxwidth = $('.progress').width();

        $fileId.find('.progress').data('percentage', 100);
        $fileId.find('span.progressFill').width($progressBarMaxwidth * (1));
        updateTitle(guid, 100);
        fileUploaded(guid);
    }

    function closeDialog() {
        $uploadDialog.hide();
    }

    function uploadFiles() {
        var uploader = new plupload.Uploader({
            runtimes: 'html5,flash,silverlight,html4',
            browse_button: 'AddFiles',
            drop_element: 'fileZone',

            chunk_size: '3mb',
            url: '/Upload/File/',
            unique_names: true,
            flash_swf_url: '/Scripts/plupload/plupload.flash.swf',
            silverlight_xap_url: '/Scripts/plupload/plupload.silverlight.xap',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        uploader.bind('Init', function (up) {
            up.settings.multipart_params = {};
        });
        uploader.bind('BeforeUpload', function (up, file) {
            try {
                up.settings.multipart_params.fileId = file.id;
                up.settings.multipart_params.fileName = file.name;
                up.settings.multipart_params.fileSize = file.size;


                up.settings.multipart_params.BoxUid = file.boxid;
                up.settings.multipart_params.tabId = file.tabid;
                up.settings.multipart_params.boxName = file.boxName;
                up.settings.multipart_params.uniName = file.uniName;
            }
            catch (err) {
            }

        });
        uploader.init();
        if (!uploader.features.dragdrop) {
            $('#DragDropText').css('visibility', 'hidden');
        }
        //$('#fileZone').on('click', 'button.pending', function () {
        //    var li = $(this).parents('li');
        //    var file = uploader.getFile(li[0].id);
        //    uploader.removeFile(file);
        //    li.remove();
        //    var ul = $('#fileZone').find('ul');
        //    if (!ul.children('li').length) {
        //        ul.hide();
        //        //$('#DragDropText').removeClass('dragDropText');
        //        //$('#DragDropText').show();
        //    }
        //});

        uploader.bind('FilesAdded', function (up, files) {
            $(window).trigger('dragreset');
            var filesobj = [];
            var cloneBoxToUpload = cd.clone(boxToUpload);
            for (var i = 0; i < files.length; i++) {
                if (files[i].name.indexOf('.') === -1 && files[i].size === 4096) {
                    cd.notification(ZboxResources.CantUploadDirectory);
                    uploader.removeFile(files[i]);
                    continue;
                }
                if (files[i].size === 0) {
                    cd.notification(ZboxResources.CantUploadEmptyFile);
                    uploader.removeFile(files[i]);
                    continue;
                }
                files[i].boxid = cloneBoxToUpload.id;
                files[i].tabid = cloneBoxToUpload.tabid;
                files[i].boxName = cd.getParameterFromUrl(1);
                files[i].uniName = cd.getParameterFromUrl(3);
                filesobj.push({ name: files[i].name, size: plupload.formatSize(files[i].size), id: files[i].id });
            }
            addItemToUploadList(cd.attachTemplateToData('templateUpload', filesobj));

            //try {
            //    up.settings.multipart_params.BoxUid = cloneBoxToUpload.id;
            //    up.settings.multipart_params.tabId = cloneBoxToUpload.tabid;
            //}
            //catch (err) {
            //}
            if (uploader.state === plupload.STOPPED) {
                uploader.start();
            }
            closeDialog();

        });

        var progressBarMaxwidth = 0;
        uploader.bind('UploadProgress', function (up, file) {
            if (!progressBarMaxwidth) {
                progressBarMaxwidth = $('.progress').width();
            }
            var $fileId = $('#' + file.id);
            $fileId.find('.progress').data('percentage', file.percent);
            $fileId.find('span.progressFill').width(progressBarMaxwidth * (file.percent / 100));
            updateTitle(file.id, file.percent);
        });



        //uploader.bind('Error', function (up, err) {
        //});

        uploader.bind('FileUploaded', function (up, file, data) {
            var itemData = JSON.parse(data.response);
            if (!itemData.success) {
                return;
            }
            trackUpload('total upload', 'total number of uploads in the system');
            fileUploaded(file.id);
            itemUploaded(itemData.payload.fileDto, itemData.payload.boxid);
            cd.pubsub.publish('clear_cache');
        });

        return uploader;

    }

    function addItemToUploadList(html) {
        $('#uploadList').append(html);
        generatePreviewBaseOnState();
        $uploads.show();
    }
    function fileUploaded(id) {
        var elem = $('#' + id).attr('data-done', 1);
        generatePreviewBaseOnState();
        elem.find('.fileDone').show();
        elem.find('.fileCancel').hide();
        elem.find('.progress').hide();
    }

    function registerDnd() {
        var dndAvailable = true;

        if (!'draggable' in document.createElement('span')) {
            return;
        }
        if (!cd.register()) {
            return;
        }
        $(document).bind('dragover', function (e) {
            // e.originalEvent.stopPropagation();
            e.originalEvent.preventDefault();

            var elem = $(e.target);
            if (
                elem.is('#up_dropZone')
                || elem.parents('#up_dropZone').length
                || elem.is('#fileZone')
                || elem.parents('#fileZone').length) {
                e.originalEvent.dataTransfer.dropEffect = 'copy';
            }
            else {
                e.originalEvent.dataTransfer.dropEffect = 'none';
            }
            return false;
        });

        $('#up_dropZone')

        .bind('drop', function (e) {
            e.originalEvent.preventDefault();
            e.originalEvent.stopPropagation();
            var files = e.originalEvent.dataTransfer.files;
            trackUpload('upload by drag&drop', '');
            var x, w, y = [], A, v = {};
            for (w = 0; w < files.length; w++) {
                x = files[w];
                if (v[x.name] && plupload.ua.safari && plupload.ua.windows) {
                    continue;
                }
                v[x.name] = true;
                A = plupload.guid();
                plupload.test[A] = x;
                y.push(new plupload.File(A, x.fileName || x.name, x.fileSize || x.size));
            }
            if (y.length) {
                uploader.trigger("FilesAdded", y);
            }
        });

        function containsFiles(event) {
            if (event.dataTransfer.types) {
                for (var i = 0; i < event.dataTransfer.types.length; i++) {
                    if (event.dataTransfer.types[i] == "Files") {
                        return true;
                    }
                }
            }
            return false;
        }
        $.fn.draghover = function () {
            return this.each(function () {

                var collection = $(),
                    self = $(this);

                self.on('dragenter', function (e) {
                    if (!containsFiles(e.originalEvent)) {
                        return;
                    }

                    if (collection.length === 0) {
                        self.trigger('draghoverstart');
                    }
                    collection = collection.add(e.target);
                });

                self.on('dragleave', function (e) {
                    collection = collection.not(e.target);
                    if (collection.length === 0) {
                        self.trigger('draghoverend');
                    }
                });
                self.on('dragreset', function () {
                    collection = collection.not('*');
                    self.trigger('draghoverend');
                });
            });
        };

        // Now that we have a plugin, we can listen for the new events 
        $(window).draghover().on({
            'draghoverstart': function () {
                if ($('.popupWrppr:visible').length > 0)
                    return;
                $('#up_dropZone').show();
            },
            'draghoverend': function () {
                $('#up_dropZone').hide();
            }
        });
        cd.pubsub.subscribe('boxclear', function () {
            $('#up_dropZone').hide();
        });
    }

    $('#uploads_title').click(function () {
        $uploads.toggleClass('collapsed');
        generatePreviewBaseOnState();
    });
    $('#uploads_close').click(function () {
        $uploads.hide();
        $('#uploadList').empty();
        $(this).hide();
    });
    $('#uploadList').on('click', '.fileCancel', function () {
        var fileElem = $(this).parents('.uploadItem'), fileId = fileElem.attr('id');
        var file = uploader.getFile(fileId);

        if (file.status === plupload.UPLOADING) {
            uploader.stop();
        }
        uploader.removeFile(file);
        fileElem.remove();
        uploader.start();
        generatePreviewBaseOnState();
    });
    var $uploadsTT = $('#uploadsTT'),
        $uploadsTF = $('#uploadsTF'),
        $uploadsTP = $('#uploadsTP');
    function generatePreviewBaseOnState() {

        var hasClass = $uploads.hasClass('collapsed');
        var currentElemUploaded = $('#uploadList').find('li:not([data-done])');
        $uploadsTT.text(function () {
            if (!currentElemUploaded.length) {
                return $(this).data('finish');
            }
            return hasClass ? $(this).data('minimizetile') : $(this).data('maintitle');
        });
        if (!currentElemUploaded.length) {
            $uploadsTF.hide();
            $uploadsTP.hide();
            $('#uploads_close').show();
            return;
        }
        $uploadsTF.toggle(hasClass);
        $uploadsTP.toggle(hasClass);
    }
    function updateTitle(fileId, percent) {
        var elem = $('#' + fileId);
        $uploadsTF.find('q').text(elem.find('span.fileName').text());
        $uploadsTP.text('(' + percent + '%)');
    }


    function itemUploaded(d, boxid) {
        if (boxToUpload.id === boxid) {
            if (boxToUpload.question) {
                cd.pubsub.publish('qnaAttacment', d);
            }
            cd.pubsub.publish('addItem', d);
        }
        d.ownerUrl = cd.userDetail().url; //need this for activity feed
        d.ownerImg = cd.userDetail().img; //need this for activity feed
        cd.pubsub.publish('addItemNoti', { BoxUid: boxid, item: d });
    }
})(jQuery, window.plupload, window.cd, window.cd.data, window.ZboxResources, window.cd.analytics);