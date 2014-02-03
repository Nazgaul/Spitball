(function (mmc, $) {
    "use strict";
    if (mmc.page.account) {
        accountSettingsProfile();
        registerEvents();
        storage();
        notification();
        accountSettings();
    }
    function accountSettings() {
        var languageElem = $('#language'), language = languageElem.val();
        $('#ChangePwd').click(function () {
            var prev = $(this).prev(), css = 'show', oldpswElem = $('#oldpsw'),
                oldpsw = oldpswElem.val(),
                regex = new RegExp(oldpswElem.data('valRegexPattern')),
                newpswElem = $('#newpsw'),
                 newpsw = newpswElem.val(), emailerr = $('#passerr');

            if (!prev.hasClass(css)) {
                prev.toggleClass(css);
                emailerr.hide();
                return;
            }
            emailerr.show();
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
            var request = new ZboxAjaxRequest({
                url: '/Account/ChangePassword',
                data: JSON.stringify({ CurrentPassword: oldpsw, NewPassword: newpsw }),
                contentType: 'application/json; charset=utf-8',
                success: function () {
                    emailerr.text('');
                    newpswElem.val('');
                    oldpswElem.val('');
                    prev.toggleClass(css);
                },
                error: function (msg) {
                    emailerr.text(msg);
                }
            });
            request.Post();
        });

        $('#ChangeEmail').click(function () {
            $(this).toggle().next().toggle();
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
                var request = new ZboxAjaxRequest({
                    url: '/Account/ChangeEmail',
                    data: JSON.stringify({ Email: newEmail }),
                    contentType: 'application/json; charset=utf-8',
                    success: function () {
                        $newEmail.val('').attr('placeholder', ZboxResources.TypeCode).focus().next().text(ZboxResources.Save);
                        codeRequired = true;
                    },
                    error: function (msg) {
                        emailerr.text(msg);
                    }
                });
                request.Post();

            }
            function submitCode() {
                var newEmail = $newEmail.val();
                if (newEmail === '') {
                    emailerr.text('this field is required');
                    return;
                }
                if (!$.isNumeric(newEmail)) {
                    emailerr.text('this field should contain number');
                    return;
                }
                var request = new ZboxAjaxRequest({
                    url: '/Account/EnterCode',
                    data: { code: newEmail },
                    success: function (data) {
                        $newEmail.attr('placeholder', ZboxResources.EnterEmail).next().text(ZboxResources.Change)
                        $newEmail.parent().toggle().prev().toggle();
                        codeRequired = false;
                        $('.changeEmail').find('span:first').text(data);
                        mmc.notification('Your email changed');
                    },
                    error: function (msg) {
                        emailerr.text(msg);
                    }
                });
                request.Post();
            }
        });

        $('#ChangeLanguage').click(function () {
            if (languageElem.val() === language) {
                return;
            }
            var request = new ZboxAjaxRequest({
                url: '/Account/ChangeLanguage',
                data: JSON.stringify({ Language: languageElem.val() }),
                contentType: 'application/json; charset=utf-8',
                success: function () {
                    window.location.reload();
                }
            });
            request.Post();
        });

    }
    function notification() {

        ko.applyBindings(new NotificationViewModel(), document.getElementById('Notification'));

        function NotificationViewModel() {
            var self = this, run = false;
            function model(data) {
                this.name = data.Name;
                this.owner = data.Owner;
                this.notification = data.Notifications;
                this.id = data.Uid;
            }
            self.boxes = ko.observableArray([]);
            self.selectChange = function (e, b) {
                if (!run) {
                }
                new ZboxAjaxRequest({
                    url: "/Box/ChangeNotification",
                    data: { BoxUid: e.id, notification: e.notification }
                }).Post();
            }
            var $notificationTab = $('#notifcationTab');
            if ($notificationTab.hasClass('selected')) {
                getModel();
            }
            $notificationTab.click(function () {
                getModel();

            });
            function getModel() {
                if (run) {
                    return;
                }
                new ZboxAjaxRequest({
                    url: "/Dashboard/Notification",
                    success: function (data) {
                        var array = [];
                        for (var i = 0, len = data.length; i < len; i++) {
                            array.push(new model(data[i]));
                        }
                        self.boxes(array);
                        run = true;
                    }
                }).Post();
            }
        }
    }
    function storage() {
        var used = parseInt($('#your').data('used'), 10);
        var $bar = $('.bar'), percent = $bar.width() / 100, $your = $bar.find('.your');
        $your.width(used * percent);
    }

    function registerEvents() {
        var tabConst = 'Profile';
        changetab();
        $('#settingsMenu').click(function (e) {
            var $li = $(e.target);
            if ($li.hasClass('selected') || !$li.is('li')) //don't do anything if user clicks currently selected li or a parent of the li
            {
                return;
            }
            window.location.hash = $li.data('form');
        });

        $(window).bind('hashchange', function () {
            changetab();
        });

        function changetab() {
            var tab = window.location.hash;
            if (tab.charAt(0) === '#') {
                tab = tab.substr(1);
            }
            var now = $('#settingsMenu').find('li[data-form="' + tab + '"]').addClass('selected');
            if (!now.length) {
                return;
            }
            var previous = $('#settingsMenu').find('li.selected').not(now).removeClass('selected').data('form');

            $('#' + previous).toggle();
            $('#' + tab).toggle();
        }
    }
    //TODO: add change event to suppress save without change
    function accountSettingsProfile() {
        var smallimg, largeimg;
        registerUploadImage();

        function registerUploadImage() {
            var uploader = new plupload.Uploader({
                runtimes: 'html5,flash,silverlight,browserplus,html4',
                browse_button: 'uplImage',
                max_file_size: '4mb',
                multi_selection: false,
                url: '/Upload/ProfilePicture',
                unique_names: true,
                flash_swf_url: '/Scripts/plupload/plupload.flash.swf',
                silverlight_xap_url: '/Scripts/plupload/plupload.silverlight.xap',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                },
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
                $('#accountSettingsThumb').attr('src', '/Images/328.gif');
            });

            uploader.bind('Error', function (up, err) {
                if (err.code === plupload.FILE_EXTENSION_ERROR) {
                    mmc.notification(ZboxResources.IncorrectExtension);
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
                }
                else {
                    mmc.notification(ZboxResources.CouldNotProcess);
                }
            });


        }

        $('#Profile').submit(function (e) {
            var $this = $(this);
            e.preventDefault();
            e.stopPropagation();
            if (!$this.valid()) {
                return false;
            }
            var values = $this.serializeArray();
            values.push({ Image: smallimg }, { LargeImage: largeimg });
            values.push({ name: "X-Requested-With", value: "XMLHttpRequest" });
            var request = new ZboxAjaxRequest({
                url: $this.attr('action'),
                data: values,
                success: function () {
                    $('#userName').text($('#Name').val().split(' ', 1)[0]);
                    $('#userDetails').find('img').attr('src', smallimg);
                    mmc.notification('You settings are saved');
                },
                error: function (msg) {
                    mmc.modelErrors2($this, msg);
                }
            });
            request.Post();
        });

        mmc.autocomplete($('#University'), null, '/User/University');
        


    }


}(window.mmc = window.mmc || {}, jQuery));