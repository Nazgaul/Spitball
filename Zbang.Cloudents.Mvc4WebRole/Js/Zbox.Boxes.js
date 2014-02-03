
(function (Boxes, $, undefined) {
    Boxes.ChangeBoxPrivacy = function () {
        var request = new ZboxAjaxRequest({
            url: "/Box/ChangePrivacySettings",
            data: { BoxUid: Zbox.getParameterByName('boxuid') },
            success: function (data) {
                if (data) {
                    //Zbox.ShowNotification(ZboxResources.PrivacySettingChanged);
                }
            }
        });
        request.Post();
    };   
}(window.Boxes = window.Boxes || {}, jQuery));