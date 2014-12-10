(function (dataContext, ZboxResources, $, cd, plupload) {
    if (window.scriptLoaded.isLoaded('mAc')) {
        return;
    }
    //$('#h_back').show().addClass('left').attr('href', '/');
    //$('[data-navigation]').removeAttr('data-navigation');
    var smallimg, largeimg;


    function registerUploadImage() {

        var uploader = new plupload.Uploader({
            runtimes: 'html5,html4',
            browse_button: 'uplImage',
            //max_file_size: '4mb',
            multi_selection: false,
            url: '/Upload/ProfilePicture',
            //resize: { width: 100, height: 100, quality: 80 },
            unique_names: true,
            //flash_swf_url: '/Scripts/plupload/plupload.flash.swf',
            //silverlight_xap_url: '/Scripts/plupload/plupload.silverlight.xap',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            //multipart: false,
            filters: [
                { title: "Image files", extensions: "jpg,gif,png" }
            ]
        });

        uploader.bind('Init', function (up) {
            up.settings.multipart_params = {};
        });
        uploader.init();
        uploader.bind('FilesAdded', function () {
            uploader.start();

            $('#accountSettingsThumb').attr('src', '/Images/loader2.gif');
        });

        uploader.bind('Error', function (up, err) {
            if (err.code === plupload.FILE_EXTENSION_ERROR) {
                cd.notification(JsResources.IncorrectExtension);
            }
            if (err.status === 401) {
                document.location.href = '/';
            }
            if (err.status === 403) {
                document.location.href = '/';
            }
            up.start();
            up.refresh(); // Reposition Flash/Silverlight
        });

        uploader.bind('FileUploaded', function (up, file, data) {
            var result = JSON.parse(data.response);
            if (result.Success) {
                smallimg = result.urlSmall;
                largeimg = result.urlLarge;
                $('#accountSettingsThumb').attr('src', largeimg);
                $('#Profile').submit();
            }
            else {
                cd.notification(JsResources.CouldNotProcess);
            }
        });
    }

    function accountSettings() {
        var languageElem = $('#language');
        $('#ChangePwd').click(function () {
            var prev = $(this).prev(), css = 'show', oldpswElem = $('#oldpsw'),
                oldpsw = oldpswElem.val(),
                regex = new RegExp(oldpswElem.data('valRegexPattern')),
                newpswElem = $('#newpsw'),
                 newpsw = newpswElem.val(), emailerr = $('#passerr');


            if (oldpsw === '') {
                emailerr.text(oldpswElem.data('valRequired'));
                return;
            }
            if (!regex.test(oldpsw)) {
                emailerr.text(oldpswElem.data('valRegex'));
                return;
            }
            if (newpsw === '') {
                emailerr.text(newpswElem.data('valRequired'));
                return;
            }
            if (!regex.test(newpsw)) {
                emailerr.text(newpswElem.data('valRegex'));
                return;
            }
            dataContext.changePassword({
                data: { CurrentPassword: oldpsw, NewPassword: newpsw },
                success: function () {
                    emailerr.text('');
                    newpswElem.val('');
                    oldpswElem.val('');
                    window.scrollTo(0, 0); //iphone issue bug 477
                    cd.notification(JsResources.PwdChanged);
                    location.hash = ''; // close the form
                    $('#changePwd').removeClass('selected');
                },
                error: function (msg) {
                    emailerr.text(msg);
                }
            });

        });

        $('#profileName').focusout(function () {
            $('#Profile').submit();
        });

        var codeRequired = false;
        $('#AddBtn').click(function () {
            var $newEmail = $('#newEmail'),
                emailRegEx = new RegExp($newEmail.data('valRegexPattern')),
                emailerr = $('#emailerr');
            if (!codeRequired) {
                submitEmail();
            }
            else {
                submitCode();

            }

            function submitEmail() {
                var newEmail = $newEmail.val();
                if (newEmail === '') {
                    emailerr.text($newEmail.data('valRequired'));
                    return;
                }
                if (!emailRegEx.test(newEmail)) {
                    emailerr.text($newEmail.data('valRegex'));
                    return;
                }
                dataContext.changeEmail({
                    data: { Email: newEmail },
                    success: function () {
                        $newEmail.val('').attr('placeholder', JsResources.TypeCode).focus().next().text(JsResources.Save);
                        codeRequired = true;
                    },
                    error: function (msg) {
                        emailerr.text(msg);
                    }
                });


            }
            function submitCode() {
                var newEmail = $newEmail.val();
                if (newEmail === '') {
                    emailerr.text(JsResources.FieldRequired);
                    return;
                }
                if (!$.isNumeric(newEmail)) {
                    emailerr.text(JsResources.FieldNeedsNumber);
                    return;
                }
                dataContext.submitCode({
                    data: { code: newEmail },
                    success: function (data) {
                        $newEmail.attr('placeholder', JsResources.EnterEmail).next().text(JsResources.Change);
                        $newEmail.parent().toggle().prev().toggle();
                        codeRequired = false;
                        $('.changeEmail').find('span:first').text(data);
                        cd.notification(JsResources.EmailChanged);
                    },
                    error: function (msg) {
                        emailerr.text(msg);
                    }
                });

            }
        });
        languageElem.change(function () {
            dataContext.changeLanguage({
                data: { Language: languageElem.val() },
                success: function () {
                    window.location.reload();
                }
            });
        });
    }
    $(document).on('submit', '#Profile', function (e) {
        //$('#Profile').submit(function (e) {
        var $this = $(this);
        e.preventDefault();
        if (!$this.valid()) {
            return false;
        }
        var values = $this.serializeArray();
        values.push({ name: 'Image', value: smallimg });
        values.push({ name: 'LargeImage', value: largeimg });
        values.push({ name: "X-Requested-With", value: "XMLHttpRequest" });
        dataContext.accountSettings({
            data: values,
            success: function () {
                $('#userName').text($('#profileName').val().trim().split(' ', 1)[0]);
                $('#userDetails').find('img').attr('src', smallimg);
                cd.notification(JsResources.SettingsSaved);
            },
            error: function (msg) {
                cd.displayErrors($this, msg);
            }
        });

    });
    cd.loadModel('account', 'AccountContext', function () {
        if (getIfFileCanBeUpload()) {
            registerUploadImage();
        }
        else {
            $('#uplImage').click(function () {
                cd.notification('your browser doesnt support upload');
            })
        }
        accountSettings();
    });
    //cd.autocomplete($('#University'), null, '/Library/University');
   

    function getIfFileCanBeUpload() {
        var elem = document.createElement('input');
        elem.type = 'file';
        return !elem.disabled;
    }
})(cd.data, JsResources, jQuery, cd, plupload);