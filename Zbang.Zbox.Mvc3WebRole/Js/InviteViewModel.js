/// <reference path="/Scripts/knockout.js" />
//function InviteVMW(isauthnticated) {
(function (mmc, $) {
    "use strict";
    mmc.pendingInvites = function () {
        ko.applyBindings(new InviteViewModel(), document.getElementById('headerNav'));
    }
    function InviteViewModel() {
        function Invite(data) {
            var self = this;
            self.id = data.BoxUid;
            self.name = data.BoxName;
            self.owner = data.BoxOwner;
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
                        var mapped = $.map(data, function (invite) { return new Invite(invite); });
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