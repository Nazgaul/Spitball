(function (mmc, $) {
    "use strict";
    (function () {
        var $headerNav = $('#headerNav');
        if ($headerNav.length) {// user not sign up doesnt have this element
            ko.applyBindings(new inviteViewModel(), $headerNav[0]);
        }
    })();
    
    function inviteViewModel() {
        function invite(data) {
            var that = this;
            that.id = data.BoxUid;
            that.name = data.BoxName;
            that.owner = data.BoxOwner;
        }
        var self = this;

        self.invites = ko.observableArray([]);
        self.cinvites = ko.observable(0);

        if (mmc.register){
            getInvite();
            registerEvents();
        }
        function getInvite() {
            var request = new ZboxAjaxRequest({
                url: "/Share/Invites",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    if (data.length) {
                        var mapped = $.map(data, function (d) { return new invite(d); });
                        self.invites(mapped);
                        self.cinvites(data.length);
                    }
                    
                }
            });
            request.Get();
        }

        function registerEvents() {
            var $invtesList = $('#invitesList');
            $('#invites').click(function (e) {
                $('ul.userMenu').slideUp(150);//close settings - maybe should be in class
                if ($invtesList.is(':visible')) {
                    $invtesList.slideUp(150);
                    return;
                }
                if (self.cinvites() === 0) {
                    return;
                }
                e.stopPropagation();
                $invtesList.slideDown(150);

            });
            $('body').click(function () {
                $invtesList.slideUp(150);
            });
        }
    }

}(window.mmc = window.mmc || {} , jQuery));