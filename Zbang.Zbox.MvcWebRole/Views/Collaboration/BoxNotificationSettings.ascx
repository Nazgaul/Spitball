<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Zbang.Zbox.Domain.Common" %>
<%@ Import Namespace="Zbang.Zbox.Infrastructure.Enums" %>
<%/*%><script src="~/Scripts/jquery-1.6.2-vsdoc.js" type="text/javascript"></script><%*/%>
<script type="text/javascript">
    $(function () {
        $('#selectedBoxSUID').change(function () {
            getBoxNotification($(this).val());

        });

        $('input:radio[name="notification-settings"]').change(function () {

            var currentNotificationSettings = $(this).val();

            saveBoxNotification($('#selecteBoxId').val(), currentNotificationSettings);
            var text = $('div#notification-settings-' + currentNotificationSettings).find('span').text();
            $('span#notification-settings-header').text(text);
            $('#notification-buttons-container').css('visibility', 'hidden');

            //Notificationfeedback('');
        });
    });

    function getBoxNotification(boxId) {

        var notificationSettings = Zbox.Box.boxes.GetBox(boxId).NotificationSettings;

        changeCurrentNotificationSettingsDisplay(notificationSettings);
        $('#NotificationSelect').val(notificationSettings);

        var text = $('#notification-settings-' + notificationSettings).find('span').text();

        $('#notification-settings-header').text(text);

    }

    function changeCurrentNotificationSettingsDisplay(NotificationSettingsCode) {
        var label = 'error';
        if (NotificationSettingsCode != 'error') {

            $('div.notification-settings-option-container').children('div').hide();

            var container = $('#notification-settings-' + NotificationSettingsCode);
            container.find('input:radio').attr('checked', true);
            container.children('div').show();
        }
    }

    function saveBoxNotification(boxId, settingsCode) {
        var NotificationSettingsUrl = '/Collaboration/ChangeBoxNotificationSettings';

        var NotificationSettingsRequest = new ZboxAjaxRequest({
            data: { boxId: boxId, newSettingsString: settingsCode },
            url: NotificationSettingsUrl,
            success: function (data) {
                Notificationfeedback('notification settings changed');
                Zbox.Box.boxes.GetCurrentBox().NotificationSettings = settingsCode;

            },
            error: function (err) {
                changeCurrentNotificationSettingsDisplay('error');
                Notificationfeedback(err);
            }
        });

        NotificationSettingsRequest.Post();
    }

    function Notificationfeedback(message) {
        var $notification = $('#notification-change-feedback')
        $notification.text(message)
                            .fadeOut(2500, function () {
                                $notification.text('');
                                $notification.show();
                            });

    }
</script>
<div>
    <div class="notification-settings-option-container" id="notification-settings-<%:NotificationSettings.On %>">
        <input type="radio" name="notification-settings" value="<%:NotificationSettings.On %>" />
        <span class="style25">On</span>
    </div>
    <div class="notification-settings-option-container" id="notification-settings-<%:NotificationSettings.Off %>">
        <input type="radio" name="notification-settings" value="<%:NotificationSettings.Off %>" />
        <span class="style25">Off</span>
    </div>
    <div id="notification-change-feedback">
    </div>
</div>
