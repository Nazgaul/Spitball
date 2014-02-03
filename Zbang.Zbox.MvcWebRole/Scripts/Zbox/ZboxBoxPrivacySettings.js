/// <reference path="/Scripts/jquery-1.6.2.js" />
$(function () {
    if ($('#BoxPrivacySettings').length > 0) {
        Zbox.BoxPrivacySettings.BoxPrivacySettingsRegisterEvents();
    }

});

Zbox.BoxPrivacySettings = {
    CurrentBoxPrivacySettings: '',
    UserTemporaryCheckPrivacySettings: '',
}

Zbox.BoxPrivacySettings.BoxPrivacySettingsRegisterEvents = function () {
    var self = this;
    var $privacySettingsOptionContainer = $('#BoxPrivacySettings').find('div.privacy-settings-option-container');
    //function BoxPrivacySettingsRegisterEvents() {
    //hideCancel();
    $('#selectedBoxSUID').change(function () {
        self.getBoxPrivacy($(this).val());
        $('#privacy-buttons-container').css('visibility', 'hidden');
    });
    $privacySettingsOptionContainer.find('input:radio').change(function () {
        //$('div.privacy-settings-option-container>input:radio').change(function () {
        $privacySettingsOptionContainer.children('div').hide();
        if ($('#selectedBoxIdPermission').val() >= 8) {
            $('#privacy-buttons-container').css('visibility', 'visible');
        }
        $(this).parent().children('div').show();
        self.UserTemporaryCheckPrivacySettings = $(this).val();
    });

    $('#share-password-confirm').click(function () {
        self.showpasswordBoxPrivacy(this);
    });


    $('#btnPrivacyOk').click(function () {
        // has anything changed?
        if (self.UserTemporaryCheckPrivacySettings != '') {

            var passwordField = $('#share-password');
            //var passwd = '';
            var passwd = Zbox.GetPassword('#share-password-confirm', '#share-password1', '#share-password');

            // check if we changed to "password protected"
            if (passwordField.closest('div.privacy-settings-option-container').find('input:radio').attr('checked')) {
                //passwd = passwordField.val();
                if (passwd == '' || passwd.replace(' ', '') == '') {
                    self.privacyFeedBack('must provide a password');
                    self.bindFeedbackReset()
                    return false;
                }
            }
            self.saveBoxPrivacy($('#selectedBoxSUID').val(), self.UserTemporaryCheckPrivacySettings, passwd);
            var text = $('#privacy-settings-' + self.UserTemporaryCheckPrivacySettings).find('span').text();
            $('#privacy-buttons-container').css('visibility', 'hidden');

        }

        return false;
    });

    $('#btnPrivacyCancel').click(function () {
        $('#box-privacy-settings').find('input:radio[value="' + self.CurrentBoxPrivacySettings + '"]').click();
        $('#privacy-buttons-container').css('visibility', 'hidden');
    });
}

Zbox.BoxPrivacySettings.bindFeedbackReset = function () {
    //function bindFeedbackReset() {
    var self = this;
    $('#share-password').keydown(function () {
        self.privacyFeedBack('');
        $(this).unbind('keydown');
    });
}

Zbox.BoxPrivacySettings.getBoxPrivacy = function (boxId) {
    var self = this;
    var privacySettings = Zbox.Box.boxes.GetBox(boxId).PrivacySettings;
    var privacyPassword = Zbox.Box.boxes.GetBox(boxId).PrivacyPassword;

    self.CurrentBoxPrivacySettings = privacySettings;
    self.changeCurrentPrivacySettingsDisplay(privacySettings);

    $('#PrivacyId').val(privacySettings);

    var text = $('#privacy-settings-' + privacySettings).find('span').text();

    $('#share-password').val(privacyPassword);
    $('#share-password1').val(privacyPassword);
}

Zbox.BoxPrivacySettings.changeCurrentPrivacySettingsDisplay = function (privacySettingsCode) {
    //function changeCurrentPrivacySettingsDisplay(privacySettingsCode) {
    var label = 'error';
    if (privacySettingsCode != 'error') {
        $('#box-privacy-settings').find('div.privacy-settings-option-container').children('div').hide();
        var container = $('#privacy-settings-' + privacySettingsCode);

        label = container.find('span').first().text();

        container.find('input:radio').click();

        container.children('div').show();
    }
    $('#privacy-settings-header').text(label);
}

Zbox.BoxPrivacySettings.saveBoxPrivacy = function (boxId, settingsCode, passwd) {
    //function saveBoxPrivacy(boxId, settingsCode, passwd) {
    var self = this;
    var privacySettingsUrl = '/Collaboration/ChangeBoxPrivacySettings';

    var privacySettingsRequest = new ZboxAjaxRequest({
        data: { boxId: boxId, newSettingsString: settingsCode, passwd: passwd },
        url: privacySettingsUrl,
        success: function (data) {

            self.CurrentBoxPrivacySettings = settingsCode;

            Zbox.Box.boxes.GetBox(boxId).PrivacySettings = settingsCode;
            Zbox.Box.boxes.GetBox(boxId).PrivacyPassword = passwd;

            Zbox.toaster('privacy settings changed');
            self.changeCurrentPrivacySettingsDisplay(settingsCode);
            $('#PrivacyId').val(data.privacySettings);

            $('#share-password').val(data.password);
            $('#share-password1').val(data.password);
            $('#eastPaneAccordion').accordion('activate', 0);
        },
        error: function (err) {
            self.changeCurrentPrivacySettingsDisplay('error');
            self.privacyFeedBack(err);
        }
    });

    privacySettingsRequest.Post();
}

Zbox.BoxPrivacySettings.privacyFeedBack = function (message) {
    //function privacyFeedBack(message) {
    var $privacyChangeFeedback = $('#privacy-change-feedback');
    $privacyChangeFeedback.text(message);
    $privacyChangeFeedback.delay(5000).fadeOut(1000, function () {
        $privacyChangeFeedback.empty();
        $privacyChangeFeedback.css('display', 'inline');
    });
}


Zbox.BoxPrivacySettings.showpasswordBoxPrivacy = function (elem) {
    //function showpasswordBoxPrivacy(elem) {
    Zbox.PasswordShow(elem, '#share-password1', '#share-password', 'inline-block');
}