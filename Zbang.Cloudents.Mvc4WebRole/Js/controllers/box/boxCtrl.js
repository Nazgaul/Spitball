﻿mBox = angular.module('mBox', ['ngDragDrop', 'angular-plupload']);
mBox.controller('BoxCtrl',
        ['$scope', '$rootScope',
         '$routeParams', '$modal', '$location',
         '$filter', '$q', '$timeout',
         'sBox', 'sItem', 'sQuiz', 'sQnA',
         'sNewUpdates', 'sUserDetails', 'sFacebook', 'sUpload',

        function ($scope, $rootScope, $routeParams, $modal, $location, $filter,
                  $q, $timeout, sBox, sItem, sQuiz, sQnA, sNewUpdates, sUserDetails, sFacebook, sUpload) {

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
                uploader: '/Box/UploadPartial/'
            };

            $scope.popup = {
                share: false
            }

            $scope.options = {
                loader: true,
                activeTab: 'feed'
            };

            sFacebook.loginStatus(); //check if user is authenticated so user can use facebook properly


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



                $timeout(function () {
                    $rootScope.$broadcast('viewContentLoaded');
                    $rootScope.$broadcast('update-scroll');
                    $rootScope.$broadcast('uploadBox', $scope.boxId);

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

                var modalInstance = $modal.open({
                    windowClass: "invite",
                    templateUrl: $scope.partials.shareEmail,
                    controller: 'ShareCtrl',
                    backdrop: 'static',
                    resolve: {
                        data: function () {
                            return null;
                        }
                    }
                });

                modalInstance.result.then(function () {
                }, function () {
                    //dismiss
                })['finally'](function () {
                    modalInstance = undefined;
                });

                $scope.$on('$destroy', function () {
                    if (modalInstance) {
                        modalInstance = undefined;
                        modalInstance.close();
                    }
                });
            };

            $scope.inviteFriends = function (e) {
                if (!sUserDetails.isAuthenticated()) {
                    e.preventDefault();
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                if ($scope.info.userType === 'none' || $scope.info.userType === 'invite') {
                    e.preventDefault();
                    alert(jsResources.NeedToFollowBox);
                    return;
                }
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

                    var modalInstance = $modal.open({
                        windowClass: "boxSettings",
                        templateUrl: $scope.partials.boxSettings,
                        controller: 'SettingsCtrl',
                        backdrop: 'static',
                        resolve: {
                            data: function () {
                                return {
                                    info: $scope.info,
                                    notification: notification,
                                    boxId: $scope.boxId,
                                    tab: tab
                                }
                            }
                        }
                    });

                    modalInstance.result.then(function (result) {
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

                    })['finally'](function () {
                        modalInstance = undefined;
                    });

                    $scope.$on('$destroy', function () {
                        if (modalInstance) {
                            modalInstance.dismiss();
                            modalInstance = undefined;
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
