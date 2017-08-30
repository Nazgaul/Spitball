var mDashboard = angular.module('mDashboard', ['wizard', 'InviteEmail', 'angular-plupload']);
mDashboard.controller('DashboardCtrl',
     ['$scope', '$rootScope', '$timeout', 'sAccount', 'sUserDetails',
       'sModal', 'sDashboard', 'sBox', 'resManager', 'sNotify',
      'sUser', 'sNewUpdates', '$location', '$analytics',

function ($scope, $rootScope, $timeout, sAccount, sUserDetails, sModal, sDashboard, sBox, resManager, sNotify, sUser, sNewUpdates, $location, $analytics) {
    "use strict";
    $scope.title = 'Dashboard';
    $scope.academicBoxes = [];
    $scope.groupBoxes = [];


    if (sUserDetails.getDetails().firstTimeDashboard) {
        sAccount.disableFirstTime({ firstTime: 'Dashboard' });
    }

    $scope.myCourses = resManager.get('CoursesFollow');
    $scope.openCreateBoxWizard = function () {
        $rootScope.params.createBoxWizard = true;

        sModal.open('createBoxWizard', {
            data: {
                isPrivate: true
            },
            callback: {
                close: function (response) {
                    $analytics.eventTrack( 'Finish Wizard', {
                        category: 'Dashboard'
                    });
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

    sDashboard.sideBar().then(function (sideBar) {
        $scope.params = {
            name: sideBar.info.name,
            image: sideBar.info.img
        };

        $scope.leaderBoard = {
            first: sideBar.leaderBoard[0],
            second: sideBar.leaderBoard[1],
            secondExist: !_.isUndefined(sideBar.leaderBoard[1]),
            third: sideBar.leaderBoard[2],
            thirdExist: !_.isUndefined(sideBar.leaderBoard[2]),
            length: sideBar.leaderBoard.length
        };
        //$analytics.setVariable('dimension1', sideBar.info.uniName);
    });

    sDashboard.recommendedCourses().then(function (courses) {
        $scope.recommendedCourses = courses;
    });

    sDashboard.boxList().then(function (boxList) {
        mapBoxes(boxList);

        $scope.contentLoaded = true;
        $timeout(function () {
            $rootScope.$broadcast('viewContentLoaded');
        });

    });








    $scope.removeBox = function (box) {
        if (sUserDetails.getDetails().isAdmin) {
            sModal.open('leavePrompt', {
                callback: {
                    close: function (response) {
                        if (response.delete) {
                            remove(true);
                            return;
                        }

                        remove();

                    }
                },
                data: {
                    name: box.name
                }
            });
            return;
        }


        sNotify.confirm($scope.removeConfirm(box)).then(function () {
        
            $analytics.eventTrack('Remove Box', {
                category: 'Dashboard'
            });

            remove();

        });

        function remove(isDelete) {
            var index;
            if (box.boxType === 'academic') {
                index = $scope.academicBoxes.indexOf(box);
                $scope.academicBoxes.splice(index, 1);
            } else {
                index = $scope.groupBoxes.indexOf(box);
                $scope.groupBoxes.splice(index, 1);
            }
            sBox.remove({ id: box.id, 'delete': isDelete });
        }
    };


    $scope.inviteFriends = function () {
        sModal.open('cloudentsInvite');
        $analytics.eventTrack( 'Invite Friends', {
            category: 'Dashboard'
        });
    };


    $scope.removeConfirm = function (box) {
        if (box.userType === 'none') {
            return '';
        }

        if (box.userType === 'owner') {
            return resManager.get('SureYouWantTo') + ' ' + resManager.get('ToDeleteBox');
        }

        return resManager.get('SureYouWantTo') + ' ' + resManager.get('ToLeaveGroup');
    };

    $scope.getTitle = function (box) {
        if (box.userType === 'none') {
            return '';
        }

        if (box.userType === 'owner') {
            return resManager.get('Delete');
        }

        return resManager.get('LeaveGroup');
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
            if (boxes[i].boxType === 'academic') {
                academic.push(boxes[i]);
            } else {
                group.push(boxes[i]);
            }
            //show box updates                
            (function (box) {
                sNewUpdates.getBoxUpdates(box.id, function (count) {
                    box.updates = count;

                });
            })(boxes[i]);
        }


        $scope.academicBoxes = academic;
        $scope.groupBoxes = group;

    }

    //$scope.productStore = function () {
    //    var dualScreenLeft = window.screenLeft != undefined ? window.screenLeft : screen.left;
    //    var dualScreenTop = window.screenTop != undefined ? window.screenTop : screen.top;

    //    var w = 935, h = 600,
    //     left = (screen.width / 2) - (w / 2) + dualScreenLeft,
    //     top = (screen.height / 2) - (h / 2) + dualScreenTop;
    //    $analytics.eventTrack('AdvertismentClick', {
    //        category: 'Dashboard'
    //    });
    //    window.open($scope.options.storeUrl, '', 'height=' + h + ',menubar=0,status=0,toolbar=0,scrollbars=1,width=' + w + ',left=' + left + ',top=' + top + '');
    //}
}
     ]);
