﻿//define('boxCtrl', ['app'], function (app) {
mBox = angular.module('mBox', ['ngDragDrop']);
mBox.controller('BoxCtrl',
        ['$scope', '$rootScope',
         '$routeParams', '$modal', '$location',
         '$filter', '$q', '$timeout',
         'sBox', 'sItem', 'sQuiz', 'sQnA', 'sUpload',
         'sNewUpdates', 'sUserDetails', 'sFacebook',

        function ($scope, $rootScope, $routeParams, $modal, $location, $filter,
                  $q, $timeout, Box, Item, Quiz, QnA, Upload, NewUpdates, UserDetails, Facebook) {
            var jsResources = window.JsResources;
            $scope.boxId = parseInt($routeParams.boxId, 10);
            $scope.uniName = $routeParams.uniName;
            $scope.boxName = $routeParams.boxName;

            $rootScope.$broadcast('uploadBox', $scope.boxId);

            $scope.init = function (backUrl, backTitle) {
                if (angular.equals($rootScope.back, {})) {
                    $rootScope.back.title = backTitle;
                    $rootScope.back.url = backUrl;
                }
            };

            var consts = {
                view: {
                    thumb: 'thumb',
                    list: 'list'
                },
                itemsLimit: 21,
                starsLength: 5,
                starsWidth: 69
            };

            $scope.partials = {
                createTab: '/Box/CreateTabPartial/',
                uploader: '/Box/UploadPartial/',
                uploadAddLink: '/Box/UploadLinkPartial/',
                shareEmail: '/Share/MessagePartial/',
                boxSettings: '/Box/SettingsPartial'
            };

            $scope.popup = {
                share: false
            }

            $scope.options = {
                currentView: consts.view.thumb,
                itemsLimit: consts.itemsLimit,
                manageTab: false
            };

            var infoPromise = Box.info({ id: $scope.boxId }),
                itemsPromise = Box.items({ id: $scope.boxId, pageNumber: 0 }),
                qnaPromise = QnA.list({ boxId: $scope.boxId, uniName: $scope.uniName, boxName: $scope.boxName }),
                all = $q.all([infoPromise, itemsPromise, qnaPromise]);

            all.then(function (data) {
                var info = data[0].success ? data[0].payload : null,
                    items = data[1].success ? data[1].payload : null,
                    qna = data[2].success ? data[2].payload : null;

                $scope.info = {
                    name: info.name,
                    comments: info.comments,
                    courseId: info.courseId,
                    boxType: info.boxType,
                    itemsLength: info.items,
                    membersLength: info.members,
                    members: info.subscribers,
                    ownerName: info.ownerName,
                    ownerId: info.ownerUid, //uid
                    privacy: info.privacySetting,
                    professor: info.professorName,
                    tabs: info.tabs,
                    userType: info.userType,
                    uniCountry: info.uniCountry,
                    image: info.image,
                    url: decodeURI($location.absUrl())
                };

                $scope.strings = {
                    share: $scope.info.boxType === 'academic' ? jsResources.ShareCourse : jsResources.ShareBox,
                    invite: $scope.info.boxType === 'academic' ? jsResources.InviteCourse : jsResources.InviteBox
                }

                $scope.info.currentTab = null;

                $scope.items = items;
                $scope.filteredItems = $filter('filter')($scope.items, filterItems);
                $scope.$broadcast('qna', qna);

                $timeout(function () {
                    $rootScope.$broadcast('viewContentLoaded');
                });
            });

            //#region quiz
            $scope.addQuiz = function () {
                $scope.options.quizOpen = true;
                $rootScope.$broadcast('initQuiz', { boxId: $scope.boxId, boxName: $scope.info.name });
            };
            //#endregion

            //#region upload
            $scope.$on('FileAdded', function (event, data) {
                $scope.$apply(function () {
                    if (data.boxId !== $scope.boxId) {
                        return;
                    }

                    if ($scope.info.currentTab && ($scope.info.currentTab.id !== data.item.tabId)) {
                        return;
                    }

                    $scope.items.unshift(data.item);
                    $scope.filteredItems.unshift(data.item);
                });
            });

            $scope.openUploadPopup = function (qna) {
                if (!UserDetails.isAuthenticated()) {
                    cd.pubsub.publish('register');
                    return;
                }
                var defer, fileList;
                if (qna) {
                    defer = $q.defer();
                    fileList = [];
                }

                var modalInstance = $modal.open({
                    windowClass: "uploader",
                    templateUrl: $scope.partials.uploader,
                    controller: 'UploadCtrl',
                    backdrop: 'static'
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
                            saveItem({ url: url, type: 'link', ajax: 'link', timeout: 1000, length: 1 });
                        }); //save url
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
                                saveItem({
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

                if (defer) {
                    return defer.promise;
                }


                function saveItem(data) {
                    if (data.type === 'link') {
                        $rootScope.$broadcast('linkUpload', data.url);
                    } else if (data.type === 'dropbox') {
                        $rootScope.$broadcast('dropboxUpload', { file: { url: data.url, name: data.name, size: data.size }, index: data.index });
                    } else if (data.type === 'googleLink') {
                        $rootScope.$broadcast('googleUpload', { file: { url: data.url, name: data.name, size: data.size }, index: data.index });
                    }

                    var formData = {
                        boxId: $scope.boxId,
                        boxUid: $scope.boxId,
                        boxName: $scope.boxName,
                        uniName: $scope.uniName,
                        tabId: $scope.info.currentTab ? $scope.info.currentTab.id : null,
                        url: data.url,
                        fileName: data.name,
                        fileUrl: data.url
                    }

                    $timeout(function () {
                        Upload[data.ajax](formData).then(function (response) {
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
                            $scope.items.unshift(responseItem);
                            $scope.filteredItems.unshift(responseItem);
                            fileList.push(responseItem);
                            if (data.type === 'link') {
                                defer.resolve(fileList);
                                return;
                            }
                            if (fileList.length === data.length) {
                                defer.resolve(fileList);
                            }

                        });
                    }, data.timeout);
                }

            };

            //#endregion

            //#region tabs
            $scope.manageSave = function () {
                var savedItems = [],
                    item;

                for (var i = 0, l = $scope.filteredItems.length; i < l; i++) {
                    item = $scope.filteredItems[i];
                    if (item.isCheck) {
                        savedItems.push(item.id);
                        item.tabId = $scope.info.currentTab.id;
                        continue;
                    }
                    item.tabId = null;
                }

                saveItemsToTab(savedItems);
                resetLastView();
            };

            $scope.manageCancel = function () {
                resetLastView();
            };

            $scope.deleteTab = function (tab) {
                var data = {
                    boxId: $scope.boxId,
                    TabId: tab.id
                }
                Box.deleteTab(data).then(function (response) {
                    if (!response.Success) {
                        alert(jsResources.DeleteError);
                    }
                });

                var index = $scope.info.tabs.indexOf(tab);
                $scope.info.tabs.splice(index, 1);
            };

            $scope.renameTab = function (tab) {
                var modalInstance = $modal.open({
                    windowClass: "createTab",
                    templateUrl: $scope.partials.createTab,
                    controller: 'createTabCtrl',
                    backdrop: 'static',
                    resolve: {
                        data: function () {
                            return {
                                boxId: $scope.boxId,
                                tabName: tab.name,
                                tabId: tab.id
                            };
                        }
                    }
                });

                modalInstance.result.then(function (name) {
                    tab.name = name;
                }, function () {
                    //dismiss
                });
            };

            $scope.createTab = function () {
                var modalInstance = $modal.open({
                    windowClass: "createTab",
                    templateUrl: $scope.partials.createTab,
                    controller: 'createTabCtrl',
                    backdrop: 'static',
                    resolve: {
                        data: function () {
                            return {
                                boxId: $scope.boxId
                            };
                        }
                    }
                });

                modalInstance.result.then(function (tab) {
                    $scope.info.tabs.push(tab);
                }, function () {
                    //dismiss
                });
            };

            $scope.manageTab = function (tab) {
                var filteredItems = $filter('filter')($scope.items, filterManageItems);
                if (!filteredItems.length) {
                    return;
                }
                $scope.filteredItems = filteredItems;
                $scope.changeView(consts.view.thumb);

                for (var i = 0, l = $scope.filteredItems.length; i < l; i++) {
                    $scope.filteredItems[i].isCheck = ($scope.info.currentTab.id === $scope.filteredItems[i].tabId);
                }

                $scope.options.manageTab = true;
            };

            $scope.selectTab = function (tab) {
                $scope.info.currentTab = tab;

                $scope.options.itemsLimit = consts.itemsLimit;

                $scope.filteredItems = $filter('filter')($scope.items, filterItems);

                if (!tab) { //all
                    return;
                }
                $rootScope.$broadcast('selectTab', tab.id);
            };

            //TODO DRAGANDDROP

            function saveItemsToTab(items, tabId) {
                var data = {
                    boxId: $scope.boxId,
                    tabId: tabId || $scope.info.currentTab.id, //tabId from draganddrop
                    itemId: items,
                    nDelete: !tabId //delete is false if only one item added from draganddrop
                };

                Box.addItemsToTab(data).then(function (response) {
                    if (!response.Success) {
                        alert(jsResources.FolderItemError);
                    }
                });

            }

            function filterManageItems(item) {
                if (item.type === 'Quiz') {
                    return false;
                }

                return true;
            }
            //#endregion

            //#region share
            $scope.shareFacebook = function () {
                $scope.popup.share = false;

                Facebook.share($scope.info.url, //url
                      $scope.info.name, //title
                       $scope.info.boxType === 'academic' ? $scope.info.name + ' - ' + $scope.info.ownerName : $scope.info.name, //caption
                       jsResources.IShared + ' ' + $scope.info.name + ' ' + jsResources.OnCloudents + '<center>&#160;</center><center></center>' + jsResources.CloudentsJoin,
                        null //picture
                   ).then(function () {
                       //TODO: add points
                   });
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
            };
            //#endregion

            //#region view
            $scope.changeView = function (view) {
                if ($scope.options.currentView === view) {
                    return;
                }
                $scope.options.itemsLimit = consts.itemsLimit;
                $scope.options.lastView = $scope.options.currentView;
                $scope.options.currentView = view;
            };

            $scope.getView = function (item) {
                switch (item.type) {
                    case 'File':
                    case 'Link':
                        return $scope.options.currentView === consts.view.thumb ? 'itemThumbView' : 'itemListView';
                    case 'Quiz':
                        return $scope.options.currentView === consts.view.thumb ? 'quizThumbView' : 'quizListView';
                }
            };

            function resetLastView() {
                if ($scope.options.lastView) {
                    $scope.changeView($scope.options.lastView);
                }

                $scope.filteredItems = $filter('filter')($scope.items, filterItems);

                $scope.options.manageTab = false;
            }
            //#endregion

            //#region items
            $scope.selectItem = function (e, item) {
                if ($scope.options.manageTab) {
                    e.preventDefault();

                    item.isCheck = !item.isCheck;
                    return;
                }
            };

            $scope.deleteItem = function (item) {
                switch (item.type) {
                    case 'File':
                    case 'Link':
                        var data = {
                            itemId: item.id,
                            boxId: $scope.boxId
                        }
                        Item.delete(data).then(removeItem);
                        break;
                    case 'Quiz':
                        var data = {
                            id: item.id,
                        }
                        Quiz.delete(data).then(removeItem);
                        break;

                };

                function removeItem(response) {
                    if (!(response.Success || response.success)) {
                        alert('error deleting ' + item.type.toLowerCase()); //translate
                        return;
                    }
                    var index = $scope.items.indexOf(item);
                    $scope.items.splice(index, 1);
                    index = $scope.filteredItems.indexOf(item);
                    if (index > -1) {
                        index = $scope.filteredItems.splice(index, 1);
                    }

                }
            };

            $scope.addDraggedItem = function (item, tabId) {
                var setTabId = false;
                for (var i = 0, l = $scope.items.length; i < l && !setTabId; i++) {
                    if ($scope.items[i].id === item.id) {
                        $scope.items[i].tabId = tabId;
                        setTabId = true;
                    }
                }

                saveItemsToTab([item.id], tabId);
            };

            $scope.deleteAllow = function (item) {
                return ($scope.info.userType === 'subscribe' || $scope.info.userType === 'owner') &&
                       ($scope.info.userType === 'owner' || item.ownerId === UserDetails.getDetails().id);
            };

            function filterItems(item) {
                if (!$scope.info.currentTab) {
                    return true;
                }

                if ($scope.info.currentTab.id === item.tabId) {
                    return true;
                }

                return false;

            }
            //#endregion

            //#region scroll
            $scope.addItems = function () {
                $scope.options.itemsLimit += 7;
            };
            //#endregion            

            //#region settings
            var memberPromise = Box.members({ boxUid: $scope.boxId }),
                notificationPromise = Box.notification({ boxUid: $scope.boxId }),
                all = $q.all([memberPromise, notificationPromise]),
                notification;


            all.then(function (response) {
                $scope.members = response[0].success ? response[0].payload : [];
                notification = response[1].success ? response[1].payload : '';


                $scope.openBoxSettings = function (tab) {
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
                                    tab: tab,
                                    members: $scope.members
                                }
                            }
                        }
                    });

                    modalInstance.result.then(function (result) {
                        $scope.info.name = result.name;

                        $scope.info.url = $scope.info.url.lastIndexOf('/') + result.queryString + '/';
                        var path = $location.path(),
                            boxName = '/' + path.split('/')[4] + '/';//boxName

                        path = path.replace(boxName, '/' + result.queryString + '/');
                        $location.url(path, '', path).replace();
                        if ($scope.info.boxType === 'academic') {

                        }
                    }, function () {
                        //dismiss
                    });
                };
            });
        }
            //#endregion 

        ]);
