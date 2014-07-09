//define("ShowFriendsCtrl", ['app'], function (app) {
mDashboard.controller('ShowFriendsCtrl',
   ['$scope',
    '$modalInstance',
    'friends',

    function ($scope, $modalInstance, friends) {
        $scope.formData = {};
        $scope.params = {
            search: ''
        };

        $scope.friends = friends;
        $scope.close = function () {
            $modalInstance.dismiss();
        };

        $scope.sendMessage = function (friend) {
            var friendObj = [{
                id: friend.uid, name: friend.name, userImage: friend.image
            }];
            cd.pubsub.publish('messageFromPopup', {
                id: '', data: friendObj
            });
        };
    }
   ]);
//});