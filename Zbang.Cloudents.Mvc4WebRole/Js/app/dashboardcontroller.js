angular.module('mDashboard', ['apiService']).
    controller('DashboardController', ['$scope', '$modal', 'Dashboard', 'Box', 'User','NewUpdates', function ($scope, $modal, Dashboard, Box, User, NewUpdates) {
        $scope.academicBoxes = [];
        $scope.groupBoxes = [];

        $scope.partials = {
            friends: '/Dashboard/FriendsPartial',
            createBox: '/Dashboard/PrivateBoxPartial'
        }

        $scope.title = JsResources.CoursesFollow;

        Dashboard.boxList().then(function (data) {
            $scope.wall = data.payload.wall;
            $scope.friends = data.payload.friends;

            mapBoxes(data.payload.boxes);                       

            $scope.contentLoaded = true;

            $scope.$broadcast('update-scroll');

            setTimeout(function () {
                document.getElementById('dashboard').style.display = 'block';
                document.getElementById('mLoading').style.display = 'none';
            }, 50)//tempfix

        });

        $scope.removeBox = function (box) {
            cd.confirm2($scope.removeConfirm(box)).then(function () {
                Box.remove({ id: box.id }).then(function () {
                    var index;
                    if (box.boxType === 'academic') {
                        index = $scope.academicBoxes.indexOf(box);
                        $scope.academicBoxes.splice(index, 1);
                    } else {
                        index = $scope.groupBoxes.indexOf(box);
                        $scope.groupBoxes.splice(index, 1);
                    }

                });
            });
        };

        $scope.openCreateBox = function () {
            var modalInstance = $modal.open({
                templateUrl: $scope.partials.createBox,
                controller: 'CreateBoxCtrl',
            });

            modalInstance.result.then(function (box) {
                cd.pubsub.publish('nav', box.url);
                //location.path();
            }, function () {
                //dismiss
            });
        };

        $scope.openShowFriends = function () {
            User.friends().then(function (data) {
                var modalInstance = $modal.open({
                    templateUrl: $scope.partials.friends,
                    controller: 'ShowFriendsCtrl',
                    resolve: {
                        friends: function () {
                            return data.payload.my;
                        }
                    }
                });

                modalInstance.result.then(function (box) {
                    //$scope.groupBoxes.unshift(box);
                    cd.pubsub.publish('nav', box.url);
                }, function () {
                    //dismiss
                });
            });
        };


        $scope.removeConfirm = function (box) {
            if (box.userType === 'none') {
                return;
            }

            if (box.userType === 'owner') {
                return JsResources.SureYouWantTo + ' ' + JsResources.ToDeleteBox;
            }

            if (box.membersCount <= 2 && box.commentCount < 2 && !box.itemCount) {
                return 'You have created an empty course, if you unfollow this course it will be deleted. Do you want to delete the course?';
            }

            return JsResources.SureYouWantTo + ' ' + JsResources.ToLeaveGroup;
        };

        $scope.getTitle = function (box) {
            if (box.userType === 'none') {
                return '';
            }

            if ((box.userType === 'owner') || (box.membersCount <= 2 && box.commentCount < 2 && !box.itemCount)) {
                return JsResources.Delete;
            }

            return JsResources.LeaveGroup;
        };

        $scope.getActionText = function (action) {
            switch (activityAction.action) {
                case 'item': return dashWallTextItem;
                case 'question': return dashWallTextComment;
                case 'answer': return dashWallTextComment;
                default:
                    return '';

            }
        };

        $scope.showRemove = function (box) {
            if (box.userType === 'owner' || box.userType === 'subscribe') {
                return true;
            }

            return false;
        };

        function mapBoxes(boxes) {
            var academic = [], group = [];
            for (var i = 0, l = boxes.length; i < l; i++) {
                boxes[i].boxPicture = boxes[i].boxPicture || '/images/emptyState/my_default3.png';
                if (boxes[i].boxType === 'academic') {
                    academic.push(boxes[i]);
                } else {
                    group.push(boxes[i]);
                }
                //show box updates                
                boxes[i].numOfUpdates = NewUpdates.getBoxUpdates(boxes[i].id);
            }



            $scope.academicBoxes = academic;
            $scope.groupBoxes = group;

        }
    }]).
    controller('CreateBoxCtrl', ['$scope', '$modalInstance', 'Box', function ($scope, $modalInstance, Box) {
        $scope.formData = {
            privacySettings: 'AnyoneWithUrl'
        };

        $scope.create = function (isValid) {
            if (!isValid) {
                return;
            }

            Box.create($scope.formData).then(function (box) {
                $modalInstance.close(box.payload || box.Payload);
            });
        };

        $scope.cancel = function () {
            $modalInstance.dismiss();
        };
    }]).

    controller('ShowFriendsCtrl', ['$scope', '$modalInstance', 'friends', function ($scope, $modalInstance, friends) {

        $scope.formData = {};
        $scope.friends = friends;
        $scope.close = function () {
            $modalInstance.dismiss();
        };

        $scope.sendMessage = function (friend) {
            var friend = [{
                id: friend.uid, name: friend.name, userImage: friend.image
            }];
            cd.pubsub.publish('messageFromPopup', {
                id: ''.id, data: friend
            });
        };
    }]).
    filter('actionText', function () {
        var dashFeed = document.getElementById('dash_feed'),
            dashWallTextItem = dashFeed.getAttribute('data-itemText'),
            dashWallTextComment = dashFeed.getAttribute('data-commenttext');

        return function (action) {

            switch (action) {
                case 'item': return dashWallTextItem;
                case 'question': return dashWallTextComment;
                case 'answer': return dashWallTextComment;
                default:
                    return '';
            }
        };
    });   