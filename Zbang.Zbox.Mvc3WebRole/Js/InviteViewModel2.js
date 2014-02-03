(function () {
    "use strict";
    //mmc.pendingInvites = function () {
    //    ko.applyBindings(new InviteViewModel(), document.getElementById('headerNav'));
    //}
    ko.applyBindings(new InviteViewModel(), document.getElementById('invites'));

    function InviteViewModel() {
        function Invite(data) {
            var self = this;
            self.id = data.BoxUid;
            self.name = data.BoxName;
            self.owner = data.BoxOwner;
        }
        var self = this;

        self.invites = ko.observableArray([]);
        //self.cinvites = ko.observable(0);
        getInvite();
        //if (mmc.register) {
        //    getInvite();
        //}
        function getInvite() {
            $.ajaxRequest.get({
                url: "/Share/Invites",
                done: function (data) {
                    if (data.length) {
                        var mapped = $.map(data, function (invite) { return new Invite(invite); });
                        self.invites(mapped);
                    }
                }
            });
        }
    }
})();