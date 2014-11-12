var mDashboard = angular.module('mDashboard', ['wizard', 'InviteEmail', 'angular-plupload']);
mDashboard.controller('DashboardCtrl',
     ['$scope', '$rootScope', '$timeout',
       'sModal','sDashboard', 'sBox','resManager',
      'sUser', 'sNewUpdates', '$location', '$analytics',

function ($scope, $rootScope, $timeout, sModal, sDashboard, sBox, resManager, sUser, sNewUpdates, $location, $analytics) {
    "use strict";
    $scope.title = 'Dashboard';
    $scope.academicBoxes = [];
    $scope.groupBoxes = [];   
    //cd.pubsub.publish('dash_boxes');//statistics

    //cd.analytics.setLibrary($('.uniText').text()); //TODO

    $scope.myCourses = resManager.get('CoursesFollow');
    $scope.openCreateBoxWizard = function () {
        $rootScope.params.createBoxWizard = true;

        sModal.open('createBoxWizard', {
            data: { 
                isPrivate: true 
            },
            callback: {
                close: function (response) {
                    $analytics.eventTrack('Dashboard', {
                        category: 'Finish Wizard'
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
    function firstTimeDashboard() {        
        sDashboard.disableFirstTime();
    }

    sDashboard.sideBar().then(function(sideBar){
        $scope.params = {
            name:sideBar.info.name,
            image:sideBar.info.img
        };

        $scope.leaderBoard = {
            first: sideBar.leaderBoard[0],
            second: sideBar.leaderBoard[1],
            third: sideBar.leaderBoard[2],
            length: sideBar.leaderBoard.length
        };
        
        $scope.recommendedCourses = sideBar.recommended;
        $analytics.setVariable('dimension1', sideBar.info.uniName);

    });
        
    sDashboard.boxList().then(function (boxList) {
        if (!boxList.length) {
            firstTimeDashboard();
        }
        mapBoxes(boxList);

        $scope.contentLoaded = true;
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
            $analytics.eventTrack('Dashboard', {
                category: 'Remove Box'
            });
        });
    };


    $scope.inviteFriends = function () {
        sModal.open('cloudentsInvite');
        $analytics.eventTrack('Dashboard', {
            category: 'Invite Friends'
        });
    };

  
    $scope.removeConfirm = function (box) {
        if (box.userType === 'none') {
            return;
        }

        if (box.userType === 'owner') {
            return resManager.get('SureYouWantTo') + ' ' + resManager.get('ToDeleteBox');
        }

        if (box.membersCount <= 2 && box.commentCount < 2 && !box.itemCount) {
            return 'You have created an empty course, if you unfollow this course it will be deleted. Do you want to delete the course?';
        }

        return resManager.get('SureYouWantTo') + ' ' + resManager.get('ToLeaveGroup');
    };

    $scope.getTitle = function (box) {
        if (box.userType === 'none') {
            return '';
        }

        if ((box.userType === 'owner') || (box.membersCount <= 2 && box.commentCount < 2 && !box.itemCount)) {
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
