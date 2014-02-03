(function (mmc, $) {
    "use strict";
    function changeState() {
        $settings.toggle();
        $boxElements.toggle();
    }
    if (!mmc.page.box) {
        return;
    }

    var $boxElements = $('#boxMetaData').children(':not(.boxSettings)'), $settings,
       $name,
       $privacy,
       $notification,
       nameVal, privacyVal, notificationVal,
           boxid = Zbox.getParameterByName('BoxUid');
    mmc.beforeBoxSettings = function () {
        if ($settings !== undefined) {
            changeState();
            return false;
        }

    };
    mmc.boxSettings = function () {
        $settings = $('.boxSettings'),
        $name = $('#Name'),
        $privacy = $('#Privacy'),
        $notification = $('#Notification'),
        nameVal = $name.val(), privacyVal = $privacy.val(), notificationVal = $notification.val();
        $.validator.unobtrusive.parseDynamicContent($settings);
        $settings.submit(function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (!$(this).valid()) {
                return false;
            }
            if (!isChanged()) {
                settingComplete();
                return;
            }
            var request = new ZboxAjaxRequest({
                url: "/Box/Settings",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    Uid: boxid,
                    Privacy: $privacy.val(),
                    Name: $name.val(),
                    Notification: $notification.val()
                }),
                success: function () {
                    settingComplete();
                },
                error: function (msg) {
                    mmc.modelErrors2($settings, msg);
                }
            });
            request.Post();

            function isChanged() {
                if (nameVal === $name.val() && privacyVal === $privacy.val() && notificationVal === $notification.val()) {
                    return false;
                }
                return true;
            }
        });

        $settings.find('.btn2').click(function (e) {
            changeState();
            resetForm();
        });
        $boxElements.hide();
        function resetForm() {
            $name.val(nameVal);
            $privacy.val(privacyVal);
            $notification.val(notificationVal);
            mmc.resetForm($settings);
        }
        function settingComplete() {
            changeState();
            nameVal = $name.val();
            $('#boxName').text(nameVal);
            //$('.boxSettings').validate().resetForm();
        }
    };

    /*Box settings section*/
    mmc.members = function (elem) {
        if (members) {
            return;
        }
        members = true;
        $(elem).click(function () {
            var boxMembers = document.getElementById('boxMembers');
            $(boxMembers).show();
            ko.applyBindings(new membersViewModel(), boxMembers);
            mmc.modal(function () {
                $(boxMembers).hide();
            }, 'members');
        });
    };
    var members = false;
    function membersViewModel() {
        function Member(data,owner) {
            var self = this;
            self.name = data.Name;
            self.image = data.Image;
            self.id = data.Uid;
            self.isOwner = owner === data.Uid;
        }
        var self = this;

        self.members = ko.observableArray([]);

        var request = new ZboxAjaxRequest({
            url: "/Box/Members",
            data: { BoxUid: boxid },
            success: function (data) {
                var ownerId = $('#boxOwner').find('button').data('uid');
                var temp = [];
                for (var i = 0, len = data.length ; i < len; i++) {
                    
                    temp.push(new Member(data[i], ownerId));
                }
                self.members(temp);
            }
        });
        request.Post();

        self.delete = function (member) {
            if (member.isOwner) {
                return;
            }
            if (!confirm(ZboxResources.DeleteThisMember)) {
                return;
            }
            self.members.remove(member);
            var request = new ZboxAjaxRequest({
                url: "/Dashboard/RemoveUser",
                data: { BoxUid: boxid, UserUid: member.uid },
                success: function () {
                    self.members.remove(member);
                }
            });
            request.Post();
        };


    }

}(window.mmc = window.mmc || {}, jQuery))


