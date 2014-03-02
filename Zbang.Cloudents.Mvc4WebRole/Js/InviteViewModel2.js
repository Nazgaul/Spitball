(function (cd, $, ko,analytics) {
    "use strict";
    if (window.scriptLoaded.isLoaded('invm')) {
        return;
    }

    if (!cd.register()) {
        return;
    }
    //ko.applyBindings(new InviteViewModel(), document.getElementById('headerInvites'));

    var invites = document.getElementById('invites');
    var invitesCounter = document.getElementById('invitesCounter');
    var invitesList = document.getElementById('invitesList');
    var headerInvites = document.getElementById('headerInvites');
    
    function Invite(data) {
        var that = this;
        that.id = data.boxUid;
        that.name = data.boxName;
        that.owner = data.boxOwner;
        that.url = data.url + '?r=siteheader&s=invite';
    }


    //function InviteViewModel() {
    //var self = this;



    //self.invites = ko.observableArray([]);
    //self.cinvites = ko.observable(0);
    getInvites(true);
    registerEvents();
      
    cd.pubsub.subscribe('dinvite', function (d) {
        //var x = ko.utils.arrayFirst(self.invites(), function (i) {
        //    return i.id === d;
        //});
        var invite = document.getElementById('invite_' + d);
        if (invite) {
            invite.parentNode.removeChild(invite);
        }
        //self.invites.remove(x);
        updateInviteHeader(invitesList.children.length);
    });

    cd.pubsub.subscribe('inviteuser', function (d) {
        getInvites(true);
    });


    function getInvites(appendItems) {
        cd.data.getInvites({
            success: function (data) {
                mapInvites(data,appendItems);
            }
        });
    }

    function mapInvites(data,appendItems) {
        if (data.length) {
            var mapped = $.map(data, function (d, i) {return new Invite(d);});
            cd.appendData(invitesList, 'invitesTemplate', mapped, 'afterbegin', appendItems);
            updateInviteHeader(invitesList.children.length);
            
            //self.invites(mapped);
        }
    }

    function updateInviteHeader(numOfInvites) {
        if (numOfInvites > 0) {
            invites.classList.remove('noInvites');
            invitesCounter.classList.add('invitesCounterShow');
            //invites.className = invites.className.replace(' noInvites', '');
            //invitesCounter.className += ' invitesCounterShow';            
            invitesCounter.textContent = numOfInvites;
        } else {
            invitesCounter.classList.remove('invitesCounterShow');
            invites.classList.add('noInvites');
            //invites.className += ' noInvites';
            //invitesCounter.className.replace(' invitesCounterShow', '');
            invitesCounter.textContent = '';
        }        
    }

    function registerEvents() {
        $('#invites').click(function (e) {
            if (!invitesList.children.length) {
                return;
            }
            $('ul.userMenu').slideUp(150);//close settings - maybe should be in class
            if ($(invitesList).is(':visible')) {
                $(invitesList).slideUp(150);
                return;
            }            
            e.stopPropagation();
            $(invitesList).slideDown(150);

        });
        $('body').click(function () {
            $(invitesList).slideUp(150);
        });
        $(headerInvites).on('click', '[data-navigation="Box"]', function () {
            analytics.trackEvent('Library', 'Accept Message', 'The number of clicks on te inner  invite and messages icon');
        });
    }
//}
})(cd, jQuery, ko, cd.analytics);