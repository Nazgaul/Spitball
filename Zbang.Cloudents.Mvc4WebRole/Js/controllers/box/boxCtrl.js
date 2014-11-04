"use strict";
var mBox = angular.module('mBox', ['ngDragDrop', 'angular-plupload']).
    controller('BoxCtrl',
        ['$scope', '$rootScope', '$routeParams',
         'sModal', '$location','$filter', '$timeout',
         'sBox','sNewUpdates', 'sUserDetails', 'sFacebook',
        function ($scope, $rootScope, $routeParams, sModal, $location, $filter,
                  $timeout, sBox, sNewUpdates, sUserDetails, sFacebook) {

            var jsResources = window.JsResources;
            $scope.boxId = parseInt($routeParams.boxId, 10);
            $scope.uniName = $routeParams.uniName;
            $scope.boxName = $routeParams.boxName;


            $scope.action = {};

            $scope.states = {
                feed: 'feed',
                items: 'items',
                quizzes: 'quizzes',
                members: 'members'
            };

            $scope.partials = {
                shareEmail: '/Share/MessagePartial/',
                boxSettings: '/Box/SettingsPartial/',
                uploader: '/Box/UploadPartial/',
                boxSocialInvite:'/Box/SocialInvitePartial/'
            };

            $scope.popup = {
                share: false
            }

            $scope.options = {
                loader: true,
                activeTab: 'feed'
            };

            sBox.info({ id: $scope.boxId }).then(function (response) {
                var info = response.success ? response.payload : null;
                $scope.info = {
                    name: info.name,
                    courseId: info.courseId,
                    boxType: info.boxType,
                    date: info.date,
                    itemsLength: info.items || 0,
                    membersLength: info.members || 0,
                    quizLength: info.quizes || 0,
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
                    share: $scope.info.boxType === 'academic' ? jsResources.ShareCourse : jsResources.ShareBox,
                    invite: $scope.info.boxType === 'academic' ? jsResources.InviteCourse : jsResources.InviteBox
                }


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

                $scope.tabId = tab.id;

            });

            //#region tabs           

            //#endregion

            //#region share
            $scope.shareFacebook = function () {
                sFacebook.share($scope.info.url, //url
                      $scope.info.name, //title
                       $scope.info.boxType === 'academic' ? $scope.info.name + ' - ' + $scope.info.ownerName : $scope.info.name, //caption
                       jsResources.IShared + ' ' + $scope.info.name + ' ' + jsResources.OnCloudents + '<center>&#160;</center><center></center>' + jsResources.CloudentsJoin,
                        null //picture
                     );
                $scope.popup.share = false;
            };

            $scope.shareEmail = function () {
                $scope.popup.share = false;

                sModal.open('shareEmail');
            };

            $scope.inviteFriends = function () {
                if (!sUserDetails.isAuthenticated()) {
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                if ($scope.info.userType === 'none' || $scope.info.userType === 'invite') {
                    alert(jsResources.NeedToFollowBox);
                    return;
                }


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
                    alert(jsResources.NeedToFollowBox);
                    return;
                }


                sBox.notification({ boxId: $scope.boxId }).then(function (response) {
                    var notification = response.success ? response.payload : '';

                    sModal.open('boxSettings', {
                        data: {
                            info: $scope.info,
                            notification: notification,
                            boxId: $scope.boxId,
                            tab: tab
                        },
                        callback: {
                            close: function(result) {
                                if (result.invite) { //invite popup
                                    $scope.inviteFriends();
                                    return;
                                }

                                $scope.info.name = result.name;
                                $scope.info.privacy = result.boxPrivacy;

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

                $scope.action = {
                    userFollow: true
                }
                $scope.info.userType = 'subscribe';

                sFacebook.postFeed($filter('stringFormat')(jsResources.IJoined, [$scope.info.name]), $scope.info.url);

                $rootScope.$broadcast('followedBox', $scope.boxId);


                if (nonAjax) { //if user uploaded a file he automatically join the box
                    return;
                }

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
            $scope.$on('selectTab', function (e, tab) {
                $scope.options.currentTab = tab;
            });         
        }


        ]);
