(function (cd, dataContext, ko) {
    if (window.scriptLoaded.isLoaded('mIvm')) {
        return;
    }

    ko.applyBindings(new InviteViewModel(), document.getElementById('invites'));
    function InviteViewModel() {
        function Invite(data) {
            var that = this;
            that.id = data.boxid;
            that.name = data.boxName;
            that.owner = data.userName;
            that.picture = data.image || '/images/EmptyState/my_default3.png';
            that.url = data.url;
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
            dataContext.getNotifications({
                success: function (data) {
                    var mapped = $.map(data, function (d) {
                        if (!d.message) {
                            return new Invite(d);
                        }
                    });
                    self.invites(mapped);
                    if (mapped.length) {
                        $('#f_invites,[data-id="f_invites"]').text(mapped.length);
                    }
                    else {
                        $('#f_invites,[data-id="f_invites"]').hide();
                    }
                }
            });
        }
    }

})(cd, cd.data, ko);