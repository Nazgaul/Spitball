var dashboard = angular.module('mDashboard', ['apiService', 'ngModal', 'Filters']);

//dashboard.config(function (ngModalDefaultsProvider) {
//    return ngModalDefaultsProvider.set({
//        closeButtonHtml: "<i class='fa fa-times'></i>"
//    });
//});

dashboard.controller('DashboardController', ['$scope', 'Dashboard', 'Box','User', function ($scope, Dashboard, Box,User) {
    $scope.academicBoxes = [];
    $scope.groupBoxes = [];
    $scope.params = {
        privateBoxDialogShown: false,
        showAllDialogShown: false,
        showAllPartial: false,
        createBoxPartial : false
    }
    $scope.partials = {
        friends: '/Dashboard/FriendsPartial',
        createBox : '/Dashboard/PrivateBoxPartial'
    }
    $scope.createBoxFormData = {
        privacySettings: 'AnyoneWithUrl'
    };

    $scope.title = JsResources.CoursesFollow;

    Dashboard.boxList().then(function (data) {
        $scope.wall = data.payload.wall;
        $scope.friends = data.payload.friends;

        mapBoxes(data.payload.boxes);

        if (data.payload.friends.length > 4) {
            $scope.twoLinesFriends = true;
        }

        $scope.contentLoaded = true;

        $scope.$broadcast('update-scroll');

        document.getElementById('dashboard').style.display = 'block';
        document.getElementById('mLoading').style.display = 'none';
    });

    $scope.removeBox = function (box) {
        cd.confirm2($scope.removeConfirm(box)).then(function(){
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

    $scope.toggleShowCreateBox = function () {        
        $scope.params.privateBoxDialogShown = !$scope.params.privateBoxDialogShown;
        $scope.params.createBoxPartial = true;
    };

    $scope.createBox = function (isValid) {
        if (!isValid) {
            return;
        }
        Box.create($scope.createBoxFormData).then(function (box) {            
            $scope.groupBoxes.unshift(box.payload);
            $scope.toggleShowCreateBox();
        });

        $scope.createBoxFormData = {
            privacySettings: 'AnyoneWithUrl'
        };


    };

    $scope.toggleShowFriends = function () {
        User.friends().then(function (data) {
                $scope.users = data.payload.my;
                $scope.params.showAllPartial = true;
                $scope.params.showAllDialogShown = !$scope.params.showAllDialogShown;
        });

    };

    $scope.resetCreateBox = function () {        
        $scope.params.createBoxPartial = false;
    };

    $scope.sendUserMessage = function (user) {
        $scope.params.showAllDialogShown = false;
        var friend = [{
            id: user.uid, name: user.name, userImage: user.image
        }];
        cd.pubsub.publish('messageFromPopup', {
            id: ''.id, data: friend
        });

    };


    $scope.resetShowAll = function () {
        $scope.params.searchInput = null;            
    };

    $scope.removeConfirm = function(box) {
        if (box.userType === 'none') {
            return;
        }

        if (box.userType === 'owner') {
            return JsResources.SureYouWant + ' ' + JsResources.ToDeleteBox;
        }

        if (box.membersCount <= 2 && box.commentCount < 2 && !box.itemCount) {
            return 'You have created an empty course, if you unfollow this course it will be deleted. Do you want to delete the course?';
        }

        return JsResources.SureYouWant + ' ' + JsResources.ToLeaveGroup;
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

    $scope.showRemove = function (box) {
        if (box.userType === 'owner' || box.userType === 'subscribe') {
            return true;
        }

        return false;
    }

    function mapBoxes(boxes) {
        var academic = [], group = [];
        for (var i = 0, l = boxes.length; i < l; i++) {
            boxes[i].boxPicture = boxes[i].boxPicture || '/images/emptyState/my_default3.png';
            if (boxes[i].boxType === 'academic') {
                academic.push(boxes[i]);
            } else {
                group.push(boxes[i]);
            }
        }

        $scope.academicBoxes = academic;
        $scope.groupBoxes = group;

    }


}]);

//dashboard.controller('DashboardAsideController', ['$scope', 'User', function ($scope, User) {
//    var dashWall = document.getElementById('dash_Wall'),
//        dashWallTextItem = dashWall.getAttribute('data-itemText'),
//        dashWallTextComment = dashWall.getAttribute('data-commenttext');

//    var activityAction = function (activity) {
//        switch (activityAction.action) {
//            case 'item': return dashWallTextItem;
//            case 'question': return dashWallTextComment;
//            case 'answer': return dashWallTextComment;
//            default:
//                return '';

//        }
//    };
//}]);