/// <reference path="/Js/Resource/ZboxResources.js" />
(function (mmc, $) {
    "use strict";
    var popUpElement, stageIndex = 1,
    $uploadPrev = $('#uploadPrev'),
    $uploadNext = $('#uploadNext'),
    $upldBoxName = $('#BoxName'),
    firstTimeFired = true,
    stage = {
        //start: 1,
        newBox: 2,
        upload: 3
    },
    boxToUpload = {
        name: '',
        id: '',
        isNew: true//,
        //choose: false
    },
    itemTemplate = '<li id="{1}"><span class="percent"></span><span class="fileName elps">{0}</span><button class="pending">{2}</button><span class="finished upldSprite"></span></li>',
    //tagHtml = '<li><span class="tagNameSelected">{0}</span><button class="tagSelectedX"></button> <input type="hidden" value="{0}" name="Tags"/></li>',
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
    mmc.uploadpopUp = function (elem) {
        var initialState = chooseStage(elem);
        if (firstTimeFired) {
            firstTimeFired = false;
            registerEvents();
        }
        popUpElement.show();
        changeState(initialState);
        mmc.modal(function () {
            popUpElement.hide();
            cleanUp();
        }, 'upload');
    };
    function chooseStage(elem) {
        //header
        var page = mmc.page;
        if ($(elem).parents('header').length) {
            return stage.newBox;
        }
        //if (page.friend) {
        //    return stage.newBox;
        //}
        //if (page.dashboard) {
        //    return stage.start;
        //}
        if (page.box || page.item) {
            boxToUpload.id = Zbox.getParameterByName('boxuid');
            boxToUpload.name = $('#boxName').text();
            //boxToUpload.choose = true;
            return stage.upload;
        }
        return stage.newBox;
    }

    function registerEvents() {
        $('.toggleType').click(function (e) {
            var span = $(e.target);
            if (span.hasClass('selected')) //don't do anything if user clicks currently selected button
            {
                return;
            }
            var $academic = $('#Academic'), $personal = $('#personal');
            $(this).toggleClass('toggle'); // switch background image
            $(this).find('.selected').removeClass('selected');
            span.addClass('selected');

            $academic.toggle();
            $personal.toggle();
        });

        $('#cancel').click(function () {
            $('.modal').click();
        });
        $uploadNext.click(function () {
            if (stageIndex === stage.newBox) {
                if (boxToUpload.isNew) {
                    $('.newBoxForm').find('form:visible').submit();
                }
                else {
                    changeState(stage.upload);
                }
                return;
            }
            if (stageIndex === stage.upload) {
                $('.modal').click();
                if (boxToUpload.isNew) {
                    mmc.invitePopUp(boxToUpload.id, boxToUpload.name);
                }
            }
        });
        $('#upldCreateNewBox').click(function () {
            changeState(stage.newBox);
        });

        $uploadPrev.click(function () {
            changeState(stage.newBox);
        });
        boxAutoComplete();
        professorAutoComplete();
        departmentAutoComplete();
        CourseAutoComplete();
        uploadFiles();

        $('.newBoxForm').find('input[type="text"],textarea').change(function () {
            var $this = $(this);
            if ($this.is(':hidden')) {
                return;
            }
            if (this.value !== $this.data(this.id)) {
                //if (this.value !== boxToUpload.description) {
                boxToUpload.isNew = true;
                return;
            }
            boxToUpload.isNew = false;

        });

    }
    function cleanUp() {
        //boxToUpload.choose = false;
        $upldBoxName.val('');
        var forms = popUpElement.find('form');
        for (var i = 0; i < forms.length; i++) {
            mmc.clearErrors(forms[i]);
        }
        $('.field-validation-error').each(function () {
            $(this).text('');
        });
        //$upldSlctdTgs.empty();
        $('#BoxName').val('');
        $('#Url').val('');
        $('#fileZone').find('ul').empty();
    }
    function boxAutoComplete() {
        autoComplete($upldBoxName);
        autoComplete($('#academicBoxName'), 'Academic');

    }
    function autoComplete(elem, type) {
        var url = "/Box/Names";
        if (type) {
            url += "?type=" + type;
        }
        mmc.autocomplete(elem, {
            appendTo: '.step2',
            select: function (event, ui) {
                boxToUpload.id = ui.item.value;
                elem.val(ui.item.label);
                popludateBoxData(type);
                return false;
            },
            change: function (event, ui) {
                if (ui.item) {
                    return;
                }
                boxToUpload.isNew = true;
                $('#Description').val('');
            }
        }, url);
    }
    function CourseAutoComplete() {
        var elem = $('#CourseId');
        mmc.autocomplete(elem, {
            appendTo: '.step2',
            select: function (e, ui) {
                boxToUpload.id = ui.item.value;
                elem.val(ui.item.label);
                popludateBoxData('Academic');
                return false;
            }
        }, '/User/Course');
    }
    function popludateBoxData(type) {
        new ZboxAjaxRequest({
            url: "/Box/UploadBoxData",
            data: { BoxUid: boxToUpload.id, type: type },
            success: function (data) {
                function setValues(elemId, value) {
                    $('#' + elemId).val(value).data(elemId, value);
                }
                if (type === 'Academic') {
                    setValues('CourseId', data.Course);
                    setValues('Professor', data.Proffesor);
                    setValues('Department', data.Department);
                    setValues('academicDescription', data.Description);
                    setValues('academicBoxName', data.Name);
                }
                else {
                    setValues('Description', data.Description);
                    setValues('BoxName', data.Name);
                }
                
                boxToUpload.name = data.Name;
                boxToUpload.isNew = false;
            }
        }).Get();
    }
    function professorAutoComplete() {
        mmc.autocomplete($('#Professor'), { appendTo: '.step2' }, '/User/Professor');
    }

    function departmentAutoComplete() {
        mmc.autocomplete($('#Department'), { appendTo: '.step2' }, '/User/Department');
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
                $('#DragDropText').removeClass('dragDropText');
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

        uploader.bind('Error', function (up, err) {
        });

        uploader.bind('FileUploaded', function (up, file, data) {
            var itemData = JSON.parse(data.response);
            itemUploaded(file.id);
            mmc.doUpdate('addItem', itemData.Payload);
        });
    }
    function addItemToUploadList(html) {
        $('#fileZone').find('ul').append(html).show();
        $('#DragDropText').addClass('dragDropText');
    }
    function showState(state) {
        popUpElement.children('div').hide();
        $('div.step' + state).show();
        previousBtnShow();
        uploadNextBtnShow();

        if (state === stage.upload) {
            uploader.refresh();
            //if (!boxToUpload.choose) {
            //    changeState(stage.newBox);
            //}
            $('#upldstep3BoxName').text(boxToUpload.name);
            $('#addLinkBox').val(boxToUpload.id);
        }

        function uploadNextBtnShow() {
            //if (state === stage.start) {
            //    $uploadNext.val(ZboxResources.Next);
            //}
            if (state === stage.newBox) {
                $uploadNext.val(ZboxResources.Next);
            }
            if (state === stage.upload) {
                if (!boxToUpload.isNew) {
                    $uploadNext.val(ZboxResources.Done);
                }
                else {
                    $uploadNext.val(ZboxResources.Next);
                }
            }
        }
        function previousBtnShow() {
            if (state === stage.newBox) {
                $uploadPrev.hide();
            }
            else {
                $uploadPrev.show();
            }
        }
    }
    function changeState(state) {
        stageIndex = state;
        showState(stageIndex);
    }
    mmc.createBoxSuccess = function (data, status, xhr) {
        var form = $('.newBoxForm').find('form:visible');
        if (data.Success) {
            boxToUpload.id = data.Payload.Uid;
            boxToUpload.name = data.Payload.Name;
            //boxToUpload.choose = true;
            boxToUpload.isNew = true;
            changeState(stage.upload);
            mmc.doUpdate('createbox', data.Payload);
            mmc.clearErrors(form);
        }
        else {
            mmc.modelErrors(form, data.Payload);
        }
    };
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