(function () {
    if (window.scriptLoaded.isLoaded('mU')) {
        return;
    }
    var boxToUpload = {
        id: '',
        tabid: null
    }, firstTime = true, uploader = {};
    cd.loadModel('box', 'BoxContext', function () {
        if (!cd.register()) {
            $('#AddFiles').click(function () {
                cd.pubsub.publish('register');
            });
            return;
        }
        if (firstTime) {
            firstTime = false;
            function getIfFileCanBeUpload() {
                var elem = document.createElement('input');
                elem.type = 'file';
                return !elem.disabled;
            }
            if (getIfFileCanBeUpload()) {
                uploadObj();
            }
            else {
                $('#AddFiles').click(function () {
                    cd.notification('your browser doesnt support upload');
                })
            }
        }
    });
   

    //cd.pubsub.subscribe('nav_hash', function (hash) {
    //    if (hash !== 'showitems') {
    //        if (uploader && uploader.destroy) {
    //            uploader.destroy();
    //        }
    //    }
    //});
    cd.pubsub.subscribe('upload', function (d) {

        boxToUpload.id = d.id;
        //uploadObj();
        //uploader.refresh();
        //boxToUpload.tabid = d.tabid;
        //$uploadDialog.show();
        //uploadVisible();
    });

    function uploadObj() {
        uploader = new plupload.Uploader({
            runtimes: 'html5,html4',
            container: 'box',
            browse_button: 'AddFiles',
            chunk_size: '3mb',
            url: '/Upload/File',
            unique_names: true,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });
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
                up.settings.multipart_params.tabId = boxToUpload.tabid;
                up.settings.multipart_params.UniName = cd.getParameterFromUrl(1);
                up.settings.multipart_params.BoxName = cd.getParameterFromUrl(3);
            }
            catch (err) {
            }

        });
        uploader.init();


        uploader.bind('FilesAdded', function (up, files) {
            if (uploader.state === plupload.STOPPED) {
                uploader.start();
            }
        });
        uploader.bind('FileUploaded', function (up, file, data) {
            var itemData = JSON.parse(data.response);
            cd.pubsub.publish('addItem', itemData.payload.fileDto);
            cd.pubsub.publish('clear_cache');
        });

        $('#box').on('click', '#AddFilesEmptyState', function () {
            if (!cd.register()) {
                cd.pubsub.publish('register');
                return;
            }
            if (uploader.features.triggerDialog) {
                var input = document.getElementById(uploader.settings.browse_button);
                $(input).click();
            }
        });

    }
})();