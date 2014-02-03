(function (mmc, $) {
    "use strict";
    //function FriendViewModelWrapper() {
    if (mmc.page.dashboard) {
        mmc.dashboardfriend = function () {
            var self = this;
            ko.applyBindings(new FriendViewModel(), document.getElementById('FriendsSection'));
        };
    }
    function FriendViewModel() {
        var self = this;

        function UserFriends(data) {
            var self = this;
            self.userName = data.Name;
            self.userImage = data.Image ? data.Image : '/images/usrPic.png';
            self.id = data.Uid;
        }

        self.friends = ko.observableArray([]);
        userFriends();
        function userFriends() {
            var request = new ZboxAjaxRequest({
                url: "/User/Friends",
                success: function (data) {
                    var friends = [];
                    for (var i = 0; i < data.length; i++) {
                        var friend = new UserFriends(data[i]);
                        if (data[i].Type === 'university') {
                            var $school = $('.school');
                            $school.find('span').text(friend.userName);
                            $school.find('a.link').attr('href', '/Friend/' + friend.id);
                            $school.show();
                        }
                        friends.push(friend);
                    }
                   // var mappedfriends = $.map(data, function (friend) { return new UserFriends(friend); });
                    self.friends(friends);
                }
            });
            request.Post();
        }
    }
}(window.mmc = window.mmc || {}, jQuery));