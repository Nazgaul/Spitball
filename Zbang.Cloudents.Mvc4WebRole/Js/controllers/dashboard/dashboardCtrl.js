﻿
var mDashboard = angular.module('mDashboard', ['wizard', 'InviteEmail', 'angular-plupload']);
mDashboard.controller('DashboardCtrl',
     ['$scope', '$rootScope', '$timeout',
       'sModal', '$document',
      '$window', 'sDashboard', 'sBox',
      'sUser', 'sNewUpdates', '$location', '$analytics',

function ($scope, $rootScope, $timeout, sModal, $document, $window, sDashboard, sBox, sUser, sNewUpdates, $location, $analytics) {
    "use strict";
    var jsResources = window.JsResources;
    $scope.title = 'Dashboard';
    $scope.academicBoxes = [];
    $scope.groupBoxes = [];
    $scope.params = {};
    //cd.pubsub.publish('dash_boxes');//statistics
    cd.analytics.setLibrary($('.uniText').text());
    //$analytics

    $scope.myCourses = jsResources.CoursesFollow;
    $scope.openCreateBoxWizard = function () {
        $rootScope.params.createBoxWizard = true;

        sModal.open('createBoxWizard', {
            data: { 
                isPrivate: true 
            },
            callback: {
                close: function (response) {
                    $rootScope.params.createBoxWizard = false;
                    if (response) {
                        $scope.newUniversity = 0;
                        $location.path(response.url);
                        if (response.isItems) {
                            $location.hash('items');
                        }
                    }
                },
                always: function () {
                    $rootScope.params.createBoxWizard = false; //user cancelled
                }
            }
        });              
    };
    function firstTimeDashboard() {
        sDashboard.recommendedCourses().then(function (response) {
            var data = response.success ? response.payload : {};
            $scope.recommendedCourses = data;
        });
        sDashboard.disableFirstTime();
    }


    sDashboard.boxList().then(function (data) {
        $scope.wall = data.payload.wall;
        $scope.friends = data.payload.friends;

        mapBoxes(data.payload.boxes);
        if (!data.payload.boxes.length) {
            firstTimeDashboard();
        }

        $scope.contentLoaded = true;
        $scope.$broadcast('update-scroll');
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
                if ($scope.academicBoxes.length === 0 && $scope.groupBoxes.length === 0) {
                    firstTimeDashboard();
                }
            });
        });
    };


    $scope.inviteFriends = function () {
        sModal.open('cloudentsInvite');        
    };

    //$scope.openCreateBox = function () {
    //    var modalInstance = $modal.open({
    //        windowClass: "privateBox",
    //        templateUrl: $scope.partials.createBox,
    //        controller: 'CreateBoxCtrl',
    //        backdrop: 'static'
    //    });

    //    modalInstance.result.then(function (box) {
    //        $location.path(box.url);
    //    }, function () {
    //    });
    //};

    //$scope.openShowFriends = function () {
    //    sUser.friends().then(function (data) {
    //        var modalInstance2 = $modal.open({
    //            windowClass: "boxSettings dashMembers",
    //            templateUrl: $scope.partials.friends,
    //            controller: 'ShowFriendsCtrl',
    //            backdrop: 'static',
    //            resolve: {
    //                friends: function () {
    //                    return data.payload.my;
    //                }
    //            }
    //        });

    //        modalInstance2.result.then(function (response) {
    //            if (response.invite) {
    //                $scope.inviteCloudents();
    //            }
    //        })['finally'](function () {
    //            modalInstance = undefined;
    //        });

    //        $scope.$on('$destroy', function () {
    //            if (modalInstance) {
    //                modalInstance.dismiss();
    //                modalInstance = undefined;
    //            }
    //        });
    //    });
    //};

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
            (function (box) {
                sNewUpdates.getBoxUpdates(boxes[i].id, function (count) {
                    box.numOfUpdates = count;

                });
            })(boxes[i]);
        }


        $scope.academicBoxes = academic;
        $scope.groupBoxes = group;

    }

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
