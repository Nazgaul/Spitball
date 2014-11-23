var mBox = angular.module('mBox', ['ngDragDrop', 'angular-plupload']).
    controller('BoxCtrl',
        ['$scope', '$rootScope', '$routeParams',
         'sModal', '$location', 'resManager', '$timeout', '$analytics',
         'sBox', 'sNewUpdates', 'sUserDetails', 'sFacebook', 'sNotify',
        function ($scope, $rootScope, $routeParams, sModal, $location, resManager,
                  $timeout, $analytics, sBox, sNewUpdates, sUserDetails, sFacebook, sNotify) {
            "use strict";
            $scope.boxId = parseInt($routeParams.boxId, 10);
            $scope.uniName = $routeParams.uniName;
            $scope.boxName = $routeParams.boxName;


            $scope.action = {};

            $scope.states = {
                feed: 'feed',
                items: 'items',
                quizzes: 'quizzes',
                members: 'members'
            }

            $scope.popup = {
                share: false
            };

            $scope.options = {
                loader: true,
                activeTab: 'feed'
            };

            $scope.info = {
                userType: 'none'
            };

            sBox.info({ id: $scope.boxId }).then(function (info) {

                $scope.info = {
                    name: info.name,
                    courseId: info.courseId,
                    boxType: info.boxType,
                    date: info.date,
                    itemsLength: info.items || 0,
                    membersLength: info.members || 0,
                    quizLength: info.quizes || 0,
                    feedLength: info.feeds || 0,
                    ownerName: info.ownerName,
                    ownerId: info.ownerId,
                    privacy: info.privacySetting,
                    professor: info.professorName,
                    tabs: info.tabs,
                    userType: info.userType,
                    image: info.image,
                    url: decodeURI($location.absUrl()),
                    inviteUrl: $location.path() + 'invite/'
                };

                $scope.strings = {
                    share: $scope.info.boxType === 'academic' ? resManager.get('ShareCourse') : resManager.get('ShareBox'),
                    invite: $scope.info.boxType === 'academic' ? resManager.get('InviteCourse') : resManager.get('InviteBox')
                };


                sNewUpdates.getUpdatesCount($scope.boxId, function (updatesCount) {
                    $scope.params.newFeed = updatesCount.feed;
                    $scope.params.newItems = updatesCount.items;
                    $scope.params.newQuizzes = updatesCount.quizzes;
                });


                $timeout(function () {
                    $rootScope.$broadcast('viewContentLoaded');
                    $rootScope.$broadcast('update-scroll');
                });


            });

            $scope.setTab = function (tab) {
                if ($scope.options.activeTab === tab) {
                    return;
                }
                $location.hash(tab);
                $scope.options.activeTab = tab;
                $scope.options.loader = true;

            };

            if ($location.hash()) {
                if ($scope.states.hasOwnProperty($location.hash())) {
                    $scope.setTab($location.hash());
                } else {
                    $location.hash('');
                }
            }


            $scope.$on('selectTab', function (e, tab) {
                if (!tab) {
                    $scope.tabId = null;
                    return;
                }

                $scope.options.currentTab = tab;
                $scope.tabId = tab.id;


            });

            //#region tabs           

            //#endregion

            //#region share
            $scope.shareFacebook = function () {
                sFacebook.share($scope.info.url, //url
                      $scope.info.name, //title
                       $scope.info.boxType === 'academic' ? $scope.info.name + ' - ' + $scope.info.ownerName : $scope.info.name, //caption
                       resManager.get('IShared') + ' ' + $scope.info.name + ' ' + resManager.get('OnCloudents') + '<center>&#160;</center><center></center>' + resManager.get('CloudentsJoin'),
                        null //picture
                     );
                $scope.popup.share = false;

                $analytics.eventTrack('Box', {
                    category: 'Facebook Share',
                    label: 'User shared a box on facebook ' + $scope.boxId + ' ' + $scope.info.name
                });
            };

            $scope.shareEmail = function () {
                $scope.popup.share = false;

                sModal.open('shareEmail');

                $analytics.eventTrack('Box', {
                    category: 'Email Share',
                    label: 'User clicked on share by email'
                });
            };

            $scope.inviteFriends = function () {
                if (!sUserDetails.isAuthenticated()) {
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                if ($scope.info.userType === 'none' || $scope.info.userType === 'invite') {
                    sNotify.tAlert('NeedToFollowBox');
                    return;
                }

                $analytics.eventTrack('Box', {
                    category: 'Invite Friends',
                    label: 'User clicked on invite friends'
                });

                sModal.open('boxInvite', {
                    data: {
                        id: $scope.boxId,
                        name: $scope.info.name,
                        image: $scope.info.image,
                        url: $scope.info.url
                    }
                });
            };
            //#endregion



            //#region settings

            $scope.openBoxSettings = function (tab) {

                if (!sUserDetails.isAuthenticated()) {
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                if ($scope.info.userType === 'none' || $scope.info.userType === 'invite') {
                    sNotify.tAlert('NeedToFollowBox');
                    return;
                }

                $analytics.eventTrack('Box', {
                    category: 'Settings',
                    label: 'User clicked on box settings'
                });


                sBox.notification({ boxId: $scope.boxId }).then(function (notification) {
                    sModal.open('boxSettings', {
                        data: {
                            info: $scope.info,
                            notification: notification,
                            boxId: $scope.boxId,
                            tab: tab
                        },
                        callback: {
                            close: function (result) {
                                if (result.invite) { //invite popup
                                    $scope.inviteFriends();
                                    return;
                                }

                                $scope.info.name = result.name;
                                $scope.info.privacy = result.boxPrivacy;
                                $scope.info.professor = result.professor;
                                $scope.info.courseId = result.courseCode;

                                if (!result.queryString) {
                                    return;
                                }
                                $scope.info.url = $scope.info.url.lastIndexOf('/') + result.queryString + '/';
                                var path = $location.path(),
                                    boxName = '/' + path.split('/')[4] + '/';//boxName

                                path = path.replace(boxName, '/' + result.queryString + '/');
                                $location.url(path, '', path).replace();
                            }
                        }
                    });
                });
            };

            //#endregion 

            //#region user
            $scope.followBox = function (nonAjax) {
                if ($scope.info.userType === 'owner' || $scope.info.userType === 'subscribe') {
                    return;
                }

                if ($scope.action.userFollow) {
                    return;
                }

                sFacebook.postFeed(resManager.getParsed('IJoined', [$scope.info.name]), $scope.info.url);

                $scope.action = {
                    userFollow: true
                };
                $scope.info.userType = 'subscribe';


                $rootScope.$broadcast('followedBox', $scope.boxId);


                if (nonAjax) { //if user uploaded a file he automatically join the box
                    return;
                }

                $analytics.eventTrack('Box', {
                    category: 'Follow',
                    label: 'User followd a box ' + $scope.boxId + ' ' + $scope.info.name
                });

                sBox.follow({ BoxId: $scope.boxId }).then(function () {
                    $scope.info.membersLength++;
                });
            };

            $scope.isUserLoggedIn = function () {
                return sUserDetails.isAuthenticated();
            };

            $scope.isUserFollowing = function () {
                if (!$scope.info) {
                    return false;
                }
                return ($scope.info.userType === 'owner' || $scope.info.userType === 'subscribe');
            };

            //#endregion
        }


        ]);
