mDashboard.controller('ShowFriendsCtrl',
   ['$scope',
    '$modalInstance',
    '$filter',
    'friends',

    function ($scope, $modalInstance, $filter,friends) {
        $scope.formData = {};
        $scope.params = {
            search: '',
            maxFriends: 30,
            scrollToTop: true
        };

        $scope.friends = $filter('orderByFilter')(friends, { field: 'name', input:''});
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

        $scope.toggleLimit = function (e) {

            $scope.params.scrollToTop = true;

            if (!$scope.params.search.length) {
                $scope.friends = friends;
                
                $scope.params.maxFriends = 30;
                
               return;
            }

            $scope.friends = $filter('orderByFilter')(friends, { field: 'name', input: $scope.params.search });
        };

        $scope.addFriendsLimit = function () {            
            $scope.params.maxFriends += 30;
        };
    }
   ]);