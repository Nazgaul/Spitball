var mDashboard = angular.module('mDashboard', []);
mDashboard.controller('DashboardCtrl',
     ['$scope', '$rootScope', '$timeout',
       '$modal', '$document',
      '$window', 'sDashboard', 'sBox',
      'sUser', 'sNewUpdates', '$location',

function ($scope, $rootScope, $timeout, $modal, $document, $window, sDashboard, sBox, sUser, sNewUpdates, $location) {
    var jsResources = window.JsResources;
    $scope.title = 'Dashboard';
    $scope.academicBoxes = [];
    $scope.groupBoxes = [];

    maxVisible();

    $scope.options = {
        maxAcademicVisible: $scope.rows * $scope.cols - 1,//-1 is browse button             
        addBoxPerScroll: $scope.rows
    };
    $scope.partials = {
        friends: '/Dashboard/FriendsPartial/',
        createBox: '/Dashboard/PrivateBoxPartial/'
    };

    $scope.myCourses = jsResources.CoursesFollow;

    sDashboard.boxList().then(function (data) {
        $scope.wall = data.payload.wall;
        $scope.friends = data.payload.friends;

        mapBoxes(data.payload.boxes);

        $scope.contentLoaded = true;

        $scope.$broadcast('update-scroll');



        calculateGroupsVisible();
        $timeout(function () {
            $rootScope.$broadcast('viewContentLoaded');
        });

    });

    $scope.removeBox = function (box) {
        cd.confirm2($scope.removeConfirm(box)).then(function () {
            sBox.remove({ id: box.id }).then(function () {
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
            windowClass: "privateBox",
            templateUrl: $scope.partials.createBox,
            controller: 'CreateBoxCtrl',
            backdrop: 'static'
        });

        modalInstance.result.then(function (box) {
            //cd.pubsub.publish('nav', box.url);
            $location.url(box.url);
            //location.path();
        }, function () {
            //dismiss
        });
    };

    $scope.openShowFriends = function () {
        sUser.friends().then(function (data) {
            var modalInstance = $modal.open({
                windowClass: "boxSettings",
                templateUrl: $scope.partials.friends,
                controller: 'ShowFriendsCtrl',
                backdrop: 'static',
                resolve: {
                    friends: function () {
                        return data.payload.my;
                    }
                }
            });

            modalInstance.result.then(function (/*box*/) {
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
            return jsResources.SureYouWantTo + ' ' + jsResources.ToDeleteBox;
        }

        if (box.membersCount <= 2 && box.commentCount < 2 && !box.itemCount) {
            return 'You have created an empty course, if you unfollow this course it will be deleted. Do you want to delete the course?';
        }

        return jsResources.SureYouWantTo + ' ' + jsResources.ToLeaveGroup;
    };

    $scope.getTitle = function (box) {
        if (box.userType === 'none') {
            return '';
        }

        if ((box.userType === 'owner') || (box.membersCount <= 2 && box.commentCount < 2 && !box.itemCount)) {
            return jsResources.Delete;
        }

        return jsResources.LeaveGroup;
    };

    $scope.showRemove = function (box) {
        if (box.userType === 'owner' || box.userType === 'subscribe') {
            return true;
        }

        return false;
    };

    $scope.addBoxes = function () {
        var academicRemain = $scope.academicBoxes.length - $scope.options.maxAcademicVisible,
           groupRemain;

        if (academicRemain >= $scope.options.addBoxPerScroll) {
            $scope.options.maxAcademicVisible += $scope.options.addBoxPerScroll;
            return;
        }

        if (academicRemain < $scope.options.addBoxPerScroll) {
            $scope.options.maxAcademicVisible += academicRemain;
            groupRemain = $scope.options.addBoxPerScroll - academicRemain;
            $scope.options.maxGroupVisible += groupRemain;
            return;
        }
        groupRemain = $scope.groupBoxes.length - $scope.options.maxGroupVisible;
        if (groupRemain > 0) {
            $scope.options.maxGroupVisible += groupRemain;
        }
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
            boxes[i].numOfUpdates = sNewUpdates.getBoxUpdates(boxes[i].id);
        }



        $scope.academicBoxes = academic;
        $scope.groupBoxes = group;

    }
    function maxVisible() {
        var height = document.body.clientHeight - 130,//130 is top bar and title
            width = document.body.clientWidth - 324, //sidebar + margins
            boxWidth = 238, boxHeight = 120; //include margins

        $scope.cols = Math.floor(width / boxWidth);
        $scope.rows = Math.ceil(height / boxHeight);
    }

    function calculateGroupsVisible() {
        if ($scope.academicBoxes.length > $scope.options.maxAcademicVisible) {
            $scope.options.maxGroupVisible = 0;
            return;
        }

        $scope.options.maxGroupVisible = $scope.options.maxAcademicVisible - $scope.academicBoxes.length;
    }

    $window.onresize = maxVisible;

    $scope.productStore = function () {
        var dualScreenLeft = window.screenLeft != undefined ? window.screenLeft : screen.left;
        var dualScreenTop = window.screenTop != undefined ? window.screenTop : screen.top;

        var w = 935, h = 600,
         left = (screen.width / 2) - (w / 2) + dualScreenLeft,
         top = (screen.height / 2) - (h / 2) + dualScreenTop;
        cd.analytics.trackEvent('Dashboard', 'AdvertismentClick');
        window.open($scope.options.storeUrl, '', 'height=' + h + ',menubar=0,status=0,toolbar=0,scrollbars=1,width=' + w + ',left=' + left + ',top=' + top + '');
    }
}
     ]);
