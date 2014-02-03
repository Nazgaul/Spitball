(function (cd, dataContext,ko) {
    if (window.scriptLoaded.isLoaded('mIvm')) {
        return;
    }

    ko.applyBindings(new InviteViewModel(), document.getElementById('invites'));
    function InviteViewModel() {
        function Invite(data) {
            var that = this;
            that.id = data.boxUid;
            that.name = data.boxName;
            that.owner = data.boxOwner;
            that.picture = data.image || '/images/EmptyState/my_default3.png';
        }
        var self = this;
        self.invites = ko.observableArray([]);
        //self.cinvites = ko.observable(0);
        getInvite();

        cd.pubsub.subscribe('dinvite', function (d) {
            var elem = $('#f_invites,[data-id="f_invites"]'),
            x = ko.utils.arrayFirst(self.invites(), function (i) {
                return i.id === d;
            });
            self.invites.remove(x);
            var l = self.invites().length;
            elem.text(l);
            if (!l) {
                elem.hide();
            }



        });

        function getInvite() {
            dataContext.getInvites({
                success: function (data) {
                    if (data.length) {
                        $('#f_invites,[data-id="f_invites"]').text(data.length);
                        var mapped = $.map(data, function (d) { return new Invite(d); });
                        self.invites(mapped);
                    }
                    else {
                        $('#f_invites,[data-id="f_invites"]').hide();
                    }
                }
            });
        }
    }

})(cd, cd.data,ko);