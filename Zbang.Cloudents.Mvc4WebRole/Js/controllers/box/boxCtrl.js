mBox = angular.module('mBox', ['ngDragDrop']);
mBox.controller('BoxCtrl',
        ['$scope', '$rootScope',
         '$routeParams', '$modal', '$location',
         '$filter', '$q', '$timeout',
         'sBox', 'sItem', 'sQuiz', 'sQnA',
         'sNewUpdates', 'sUserDetails', 'sFacebook','sUpload',

        function ($scope, $rootScope, $routeParams, $modal, $location, $filter,
                  $q, $timeout, sBox, sItem, sQuiz, sQnA, sNewUpdates, sUserDetails, sFacebook, sUpload) {

            //cd.pubsub.publish('box');//statistics

            var jsResources = window.JsResources;
            $scope.boxId = parseInt($routeParams.boxId, 10);
            $scope.uniName = $routeParams.uniName;
            $scope.boxName = $routeParams.boxName;

            $rootScope.$broadcast('uploadBox', $scope.boxId);

            $scope.action = {};

            $scope.partials = {
                shareEmail: '/Share/MessagePartial/',
                boxSettings: '/Box/SettingsPartial/',
                uploader: '/Box/UploadPartial/',
                uploadAddLink: '/Box/UploadLinkPartial/'
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
                    inviteUrl: $location.url() + 'invite/'
                };

                $scope.strings = {
                    share: $scope.info.boxType === 'academic' ? jsResources.ShareCourse : jsResources.ShareBox,
                    invite: $scope.info.boxType === 'academic' ? jsResources.InviteCourse : jsResources.InviteBox
                }



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
            $scope.$on('openUpload', function (qna) {
                $scope.openUploadPopup(qna);

            });

           
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
                            saveItem({ name: url, url: url, type: 'link', ajax: 'link', timeout: 1000, length: 1, qna: qna });
                        });
                        return;
                    }

                    if (response.dropbox) {
                        var files = response.files;
                        for (var i = 0, l = files.length; i < l; i++) {
                            (function (file, index) {
                                saveItem({
                                    name: file.name,
                                    size: file.bytes,
                                    url: file.link,
                                    type: 'dropbox',
                                    ajax: 'dropbox',
                                    timeout: 0,
                                    index: index,
                                    length: files.length,
                                    qna: qna
                                });
                            })(files[i], i);
                        }
                        return;
                    }

                    if (response.googleDrive) {
                        var files = response.files;
                        for (var i = 0, l = files.length; i < l; i++) {
                            (function (file, index) {
                                saveItem({
                                    name: file.name,
                                    size: file.size,
                                    url: file.link,
                                    type: 'googleLink',
                                    ajax: 'link',
                                    timeout: 1000,
                                    index: index,
                                    length: files.length,
                                    qna: qna
                                });


                            })(files[i], i);
                        }
                        return;
                    }
                }, function () {
                    //dismiss
                });
            };


            var uploaded = 0;
            var fileList = [];
            function saveItem(data) {
                if (data.qna) {
                    if (!fileList.length) {
                        fileList = [];
                    }
                }

                if (data.type === 'link') {
                    $rootScope.$broadcast('linkUpload', data.url);
                } else if (data.type === 'dropbox') {
                    $rootScope.$broadcast('dropboxUpload', { file: { url: data.url, name: data.name, size: data.size }, index: data.index });
                } else if (data.type === 'googleLink') {
                    $rootScope.$broadcast('googleUpload', { file: { url: data.url, name: data.name, size: data.size }, index: data.index });
                }
                //TODO: what is that
                var formData = {
                    boxId: $scope.boxId, //
                    boxName: $scope.boxName,
                    uniName: $scope.uniName,
                    tabId: $scope.options.currentTab ? $scope.options.currentTab.id : null, //
                    url: data.url,
                    fileName: data.name, //
                    fileUrl: data.url //
                }

                $timeout(function () {
                    sUpload[data.ajax](formData).then(function (response) {
                        uploaded++;

                        if (uploaded === 1) {
                            $scope.followBox(true);
                            sFacebook.postFeed($filter('stringFormat')(jsResources.IUploaded, [formData.fileName]), $scope.info.url);
                        }
                        if (!response.success) {
                            alert((data.name || data.url) + ' - ' + response.payload);
                            return;
                        }

                        if (data.type === 'link') {
                            $rootScope.$broadcast('linkUploaded');
                        } else if (data.type === 'dropbox') {
                            $rootScope.$broadcast('dropboxUploaded', data.index);
                        } else if (data.type === 'googleLink') {
                            $rootScope.$broadcast('googleUploaded', data.index);
                        }

                        var responseItem = response.payload;

                        if ($scope.options.activeTab === 'items') {
                            $scope.$broadcast('itemAdded', responseItem);
                            uploaded = 0;
                            return;
                        }

                        if (data.qna) {
                            fileList.push(responseItem);
                            if (data.type === 'link') {
                                cd.pubsub.publish('addPoints', { type: 'itemUpload', amount: 1 });
                                $rootScope.$broadcast('qna:upload', fileList);
                                fileList = [];
                                uploaded = 0;
                            } else if (uploaded === data.length) {
                                cd.pubsub.publish('addPoints', { type: 'itemUpload', amount: fileList.length });
                                $rootScope.$broadcast('qna:upload', fileList);
                                fileList = [];
                                uploaded = 0;
                            }
                        }
                    }).catch(function () {
                        uploaded++;
                    });
                }, data.timeout);
            }
            //#endregion
        }


        ]);
