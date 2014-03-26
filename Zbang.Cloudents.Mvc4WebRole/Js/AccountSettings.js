(function (dataContext, $, ko, cd, ZboxResources, plupload, analytics) {
    "use strict";
    if (window.scriptLoaded.isLoaded('as')) {
        return;
    }    
    cd.loadModel('account', 'AccountContext', function () {
        $('[data-navigation]').removeAttr('data-navigation'); // force postback        
        accountSettingsProfile();
        registerEvents();
        storage();
        notification();
        accountSettings();
        document.title = "Account settings | Cloudents"
        //});
        cd.pubsub.subscribe('accountSettings_load', function () {
            //changetab();
        });
    });
    //}
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
            dataContext.changePassword({
                data: { CurrentPassword: oldpsw, NewPassword: newpsw },
                success: function () {
                    emailerr.text('');
                    newpswElem.val('');
                    oldpswElem.val('');
                    prev.toggleClass(css);
                    alert(ZboxResources.PwdChanged);
                },
                error: function (msg) {
                    emailerr.text(msg);
                }
            });
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
                dataContext.changeEmail({
                    data: { Email: newEmail },
                    success: function () {
                        $newEmail.val('').attr('placeholder', ZboxResources.TypeCode).next().text(ZboxResources.Save).addClass('checkMail');
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
                    emailerr.text(ZboxResources.FieldRequired);
                    return;
                }
                if (!$.isNumeric(newEmail)) {
                    emailerr.text(ZboxResources.FieldNeedsNumber);
                    return;
                }
                dataContext.submitCode({
                    data: { code: newEmail },
                    success: function (data) {
                        $newEmail.attr('placeholder', ZboxResources.EnterEmail).next().text(ZboxResources.Change);
                        $newEmail.parent().toggle().prev().toggle();
                        codeRequired = false;
                        $('.changeEmail').find('span:first').text(data);
                        cd.notification(ZboxResources.EmailChanged);
                    },
                    error: function (msg) {
                        emailerr.text(msg);
                    }
                });
            }
        });

        $('#ChangeLanguage').click(function () {
            if (languageElem.val() === language) {
                return;
            }
            dataContext.changeLanguage({
                data: { Language: languageElem.val() },
                success: function () {
                    cd.pubsub.publish('clear_cache', null, function () {                        
                        cd.sessionStorageWrapper.clear();
                        window.location.reload();
                    });
                }
            });
        });

    }
    function notification() {

        ko.applyBindings(new NotificationViewModel(), document.getElementById('Notification'));

        function NotificationViewModel() {
            var self = this, run = false;
            function Model(data) {
                this.name = data.name;
                this.owner = data.owner;
                this.notification = data.notifications;
                this.id = data.id;
                this.url = data.url;
            }
            self.boxes = ko.observableArray([]);
            self.selectChange = function (e) {
                if (!run) {
                }
                dataContext.changeNotification({
                    data: { BoxUid: e.id, notification: e.notification }
                });
            };
            var $notificationTab = $('#notifcationTab');
            if ($notificationTab.hasClass('selected')) {
                getModel();
            }
            $notificationTab.click(function () {
                analytics.trackEvent('Follow', 'Notifications', 'Nuber of clicks on the notifications under user settings');
                getModel();

            });
            function getModel() {
                if (run) {
                    return;
                }
                dataContext.userNotification({
                    success: function (data) {
                        var array = [];
                        for (var i = 0, len = data.length; i < len; i++) {
                            array.push(new Model(data[i]));
                        }
                        self.boxes(array);
                        run = true;
                    }
                });
            }
        }
    }
    function storage() {
        var used = parseInt($('#your').data('used'), 10);
        var $bar = $('#storageBar'), percent = $bar.width() / 100, $your = $bar.find('.your');
        $your.width(used * percent);
    }

    //function changetab() {
    //    var tab = window.location.hash;
    //    if (tab.charAt(0) === '#') {
    //        tab = tab.substr(1);

    //    }
    //    if (tab === '') {
    //        $('#profile').show();
    //        tab = window.location.hash = 'profile';
    //    }
    //    $('#settingsMenu').find('li').removeClass('selected boldFont').filter('li[data-form="' + tab + '"]').addClass('selected boldFont');     
    //}
    function registerEvents() {

        $("#settingsMenu").on("click", "li", function () {
            if ($(this).hasClass('selected')) //don't do anything if user clicks currently selected li
            {
                return;
            }
            $('#settingsMenu').find('li').removeClass('selected boldFont').eq($(this).index()).addClass('selected boldFont'); 
            $('#settingsContent').find('.settingsTab').removeClass('selected').eq($(this).index()).addClass('selected');
        });
    }
    //TODO: add change event to suppress save without change
    function accountSettingsProfile() {
        var smallimg, largeimg;

        registerUploadImage();
        $('.changeUni').click(function () {
            analytics.trackEvent('Library', 'Change school', 'number of clicks on change school');
        });
        function registerUploadImage() {
            var uploader = new plupload.Uploader({
                runtimes: 'html5,flash,silverlight,browserplus,html4',
                browse_button: 'uplImage',
                max_file_size: '4mb',
                container: 'Profile',
                multi_selection: false,
                url: '/Upload/ProfilePicture/',
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
            window.setTimeout(function () {
                uploader.refresh(); // ie got issue in here
            }, 50);

            uploader.bind('FilesAdded', function () {
                uploader.start();
                $('#accountSettingsThumb').attr('src', '/Images/loader2.gif');
            });

            uploader.bind('Error', function (up, err) {
                if (err.code === plupload.FILE_EXTENSION_ERROR) {
                    cd.notification(ZboxResources.IncorrectExtension);
                }
                if (err.status === 401) {
                    document.location.href = '/';
                }
                if (err.status === 403) {
                    document.location.href = '/';
                }
                //up.start();
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
                    cd.notification(ZboxResources.CouldNotProcess);
                }
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
                    $('#userName').text($('#Name').val().trim().split(' ', 1)[0]);
                    $('#userDetails').find('img').attr('src', smallimg);
                    cd.notification(ZboxResources.SettingsSaved);
                    cd.pubsub.publish('clear_cache');
                },
                error: function (msg) {
                    cd.displayErrors($this, msg);
                }
            });

        });
    }
})(cd.data, jQuery, ko, cd, ZboxResources, plupload, cd.analytics);