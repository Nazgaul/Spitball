mBox = angular.module('mBox', ['ngDragDrop']);
mBox.controller('BoxCtrl',
        ['$scope', '$rootScope',
         '$routeParams', '$modal', '$location',
         '$filter', '$q', '$timeout',
         'sBox', 'sItem', 'sQuiz', 'sQnA',
         'sNewUpdates', 'sUserDetails', 'sFacebook',

        function ($scope, $rootScope, $routeParams, $modal, $location, $filter,
                  $q, $timeout, sBox, sItem, sQuiz, sQnA, sNewUpdates, sUserDetails, sFacebook) {

            cd.pubsub.publish('box');//statistics

            var jsResources = window.JsResources;
            $scope.boxId = parseInt($routeParams.boxId, 10);
            $scope.uniName = $routeParams.uniName;
            $scope.boxName = $routeParams.boxName;

            $rootScope.$broadcast('uploadBox', $scope.boxId);

            $scope.action = {};

            $scope.partials = {
                shareEmail: '/Share/MessagePartial/',
                boxSettings: '/Box/SettingsPartial/'
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
                console.log($location);
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
                    inviteUrl: $location.url() + 'invite/'
                };

                $scope.strings = {
                    share: $scope.info.boxType === 'academic' ? jsResources.ShareCourse : jsResources.ShareBox,
                    invite: $scope.info.boxType === 'academic' ? jsResources.InviteCourse : jsResources.InviteBox
                }


                $scope.info.showJoinGroup = !($scope.isUserFollowing());

                $timeout(function () {
                    $rootScope.$broadcast('viewContentLoaded');
                    $rootScope.$broadcast('update-scroll');
                });
            });



            $scope.$on('box:quizzesLength', function (e, length) {
                $scope.info.quizzesLength = length;
            });
            $scope.$on('box:filesLength', function (e, length) {
                $scope.info.itemsLength = length;
            });


            $scope.setTab = function (tab) {
                if ($scope.options.activeTab === tab) {
                    return;
                }
                $scope.options.activeTab = tab;
                $scope.options.loader = true;
            };




            //#region tabs           


            //TODO DRAGANDDROP


            //#endregion

            //#region share
            $scope.shareFacebook = function () {
                sFacebook.share($scope.info.url, //url
                      $scope.info.name, //title
                       $scope.info.boxType === 'academic' ? $scope.info.name + ' - ' + $scope.info.ownerName : $scope.info.name, //caption
                       jsResources.IShared + ' ' + $scope.info.name + ' ' + jsResources.OnCloudents + '<center>&#160;</center><center></center>' + jsResources.CloudentsJoin,
                        null //picture
                     );

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
                });

                $scope.$on('$destroy', function () {
                    if (modalInstance) {
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

                    }, function () {
                        //dismiss
                    });

                    $scope.$on('$destroy', function () {
                        if (modalInstance) {
                            modalInstance.close();
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
                $scope.info.userType = 'subscrie';

                var member = {
                    uid: sUserDetails.getDetails().id,
                    name: sUserDetails.getDetails().name,
                    image: sUserDetails.getDetails().image,
                    url: sUserDetails.getDetails().url
                };

                if ($scope.info.members.length < 7) {
                    $scope.info.members.unshift(member);
                } else {
                    $scope.info.members.pop();
                    $scope.info.members.unshift(member);
                }
                $scope.info.membersLength++;
                if ($scope.info.allMembers) {
                    $scope.info.allMembers.push(member);
                };

                $timeout(function () {
                    $scope.info.showJoinGroup = false;
                }, 3300);

                sFacebook.postFeed($filter('stringFormat')(jsResources.IJoined, [$scope.info.name]), $scope.info.url);

                if (nonAjax) { //if user uploaded a file he automatically join the box
                    return;
                }

                sBox.follow({ BoxId: $scope.boxId }).then(function () {

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

            $scope.$on('openUpload', function (qna) {
                $scope.openUploadPopup(qna);

            });

            $scope.partials = {
                uploader: '/Box/UploadPartial/',
                uploadAddLink: '/Box/UploadLinkPartial/'
            };
            //#region upload
            $scope.openUploadPopup = function (qna) {
                if (!sUserDetails.isAuthenticated()) {
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                var modalInstance = $modal.open({
                    windowClass: "uploader",
                    templateUrl: $scope.partials.uploader,
                    controller: 'UploadCtrl',
                    backdrop: 'static'
                });

                $scope.$on('$destroy', function () {
                    if (modalInstance) {
                        modalInstance.close();
                    }
                });

                modalInstance.result.then(function (response) {
                    if (response.url) {
                        modalInstance = $modal.open({
                            windowClass: "uploadLink",
                            templateUrl: $scope.partials.uploadAddLink,
                            controller: 'UploadLinkCtrl',
                            backdrop: 'static'
                        });

                        modalInstance.result.then(function (url) {
                            $scope.$broadcast('linkAdded', { name: url, url: url, type: 'link', ajax: 'link', timeout: 1000, length: 1 });
                        });
                        return;
                    }

                    if (response.dropbox) {
                        var files = response.files;
                        for (var i = 0, l = files.length; i < l; i++) {
                            (function (file, index) {
                                $scope.$broadcast('linkAdded', {
                                    name: file.name,
                                    size: file.bytes,
                                    url: file.link,
                                    type: 'dropbox',
                                    ajax: 'dropbox',
                                    timeout: 0,
                                    index: index,
                                    length: files.length

                                });
                            })(files[i], i);
                        }
                        return;
                    }

                    if (response.googleDrive) {
                        var files = response.files;
                        for (var i = 0, l = files.length; i < l; i++) {
                            (function (file, index) {
                                $scope.$broadcast('linkAdded', {
                                    name: file.name,
                                    size: file.size,
                                    url: file.link,
                                    type: 'googleLink',
                                    ajax: 'link',
                                    timeout: 1000,
                                    index: index,
                                    length: files.length

                                });


                            })(files[i], i);
                        }
                        return;
                    }
                }, function () {
                    //dismiss
                });
            };
            //#endregion
        }

        ]);