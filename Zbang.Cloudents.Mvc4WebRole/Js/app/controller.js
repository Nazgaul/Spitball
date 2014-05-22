var dashboard = angular.module('mDashboard', ['apiService', 'ngModal', 'Filters', 'ngScrollbar']);

dashboard.config(function (ngModalDefaultsProvider) {
    return ngModalDefaultsProvider.set({
        closeButtonHtml: "<i class='fa fa-times'></i>"
    });
});

dashboard.controller('DashboardController', ['$scope', 'Dashboard', 'Box', function ($scope, Dashboard, Box) {
    $scope.academicBoxes = [];
    $scope.groupBoxes = [];

    var boxList = Dashboard.boxList({
        success: function (data) {
            $scope.wall = data.payload.wall;
            $scope.friends = data.payload.friends;

            mapBoxes(data.payload.boxes);


            if (data.payload.friends.length > 4) {
                $scope.twoLinesFriends = true;
            }

            $scope.contentLoaded = true;
            document.getElementById('dashboard').style.display = 'block';
            document.getElementById('mLoading').style.display = 'none';


            //rebuildscrollbar
            $scope.$broadcast('rebuild:me');
        }
    });

    //var friends = Dashboard.friends.(function (data) {
    //    console.log(friends);
    //    console.log(data);
    //})
    //$scope.title = 'CoursesFollow';
    $scope.title = JsResources.CoursesFollow;

    $scope.removeBox = function (box) {
        Box.create.query(function () { });

    };

    $scope.toggleShowCreateBox = function () {
        $scope.privateBoxDialogShown = !$scope.privateBoxDialogShown;
        $scope.createBoxPartial = true;
    };
    
    //$scope.creatingBox = false;
    //$scope.createBox = function () {
    //    analytics.trackEvent('Dashboard', 'Create privte box', 'Number of clicks on "create" after writing down the box name');
    //    $scope.creatingBox = true;
    //    Box.create({
    //        data: data
    //    });
    //    dataContext.createBox({
    //        data: data,
    //        success: function (result) {
    //            var box = new Box(result);
    //            cd.resetForm($('#createBoxDialog'));
    //            $privateBoxDialog.hide();
    //            $('#BoxName').val('');
    //            isSubmit = false;
    //            cd.pubsub.publish('nav', box.boxUrl/* + '?r=dashboard'*/);
    //        },
    //        error: function (msg) {
    //            isSubmit = false;
    //            cd.notification(msg[0].Value[0]);
    //        }
    //    });
    //};


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

    };

}]);

dashboard.controller('DashboardAsideController', ['$scope', 'User', function ($scope, User) {
    var dashWall = document.getElementById('dash_Wall'),
        dashWallTextItem = dashWall.getAttribute('data-itemText'),
        dashWallTextComment = dashWall.getAttribute('data-commenttext');

    var activityAction = function (activity) {
        switch (activityAction.action) {
            case 'item': return dashWallTextItem;
            case 'question': return dashWallTextComment;
            case 'answer': return dashWallTextComment;
            default:
                return '';

        };
    }
   
    $scope.toggleShowFriends = function () {
        User.friends({
            success: function (data) {
                $scope.users = data.payload.my;
                $scope.showAllDialogShown = !$scope.showAllDialogShown;
                $scope.showAllPartial = true;

            }
        });

    };

    $scope.sendUserMessage = function (user) {
        $scope.showAllDialogShown = false;
        var friend = [{ id: user.uid, name: user.name, userImage: user.image }];
        cd.pubsub.publish('messageFromPopup', { id: ''.id, data: friend });

    }

}]);

dashboard.controller('CreateBoxController', ['$scope', 'Box', function ($scope, Box) {
    $scope.formData = {
        privacySettings: 'AnyoneWithUrl'
    };

    $scope.createBox = function (isValid) {
        if (!isValid) {
            return;
        }

        Box.create({
            data: $scope.formData,
            success: function (data) {
                $scope.formData = {};
                $scope.toggleShowCreateBox();
            }
        });
    }

    $scope.reset = function () {
        $scope.formData = {};
        $scope.toggleShowCreateBox();

    };

}]);