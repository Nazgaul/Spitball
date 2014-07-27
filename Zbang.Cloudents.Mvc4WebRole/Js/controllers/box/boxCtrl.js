//define('boxCtrl', ['app'], function (app) {
mBox = angular.module('mBox', ['ngDragDrop']);
mBox.controller('BoxCtrl',
        ['$scope', '$rootScope',
         '$routeParams', '$modal', '$location',
         '$filter', '$q', '$timeout',
         'sBox', 'sItem', 'sQuiz', 'sQnA', 'sUpload',
         'sNewUpdates', 'sUserDetails', 'sFacebook',

        function ($scope, $rootScope, $routeParams, $modal, $location, $filter,
                  $q, $timeout, Box, Item, Quiz, QnA, Upload, NewUpdates, UserDetails, Facebook) {

            cd.pubsub.publish('box');//statistics

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
            $scope.action = {};

            $scope.partials = {
                createTab: '/Box/CreateTabPartial/',
                uploader: '/Box/UploadPartial/',
                uploadAddLink: '/Box/UploadLinkPartial/',
                shareEmail: '/Share/MessagePartial/',
                boxSettings: '/Box/SettingsPartial/'
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

                $scope.items = items.map(function (item) {
                    var type = item.type === 'Quiz' ? 'quizzes' : 'items';
                    item.isNew = NewUpdates.isNew($scope.boxId, type, item.id);
                    return item;
                });

                $scope.items.sort(sortItems);
                $scope.filteredItems = $filter('filter')($scope.items, filterItems);

                $scope.$broadcast('qna', qna);

                $scope.info.showJoinGroup = $scope.isUserFollowing();

                $timeout(function () {
                    $rootScope.$broadcast('viewContentLoaded');
                });
            });

            //#region quiz
            $scope.addQuiz = function () {
                if (!UserDetails.isAuthenticated()) {
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                if ($scope.info.userType === 'invite' || $scope.info.userType === 'none') {
                    alert(jsResources.NeedToFollowBox);
                    return;
                }

                $rootScope.$broadcast('initQuiz', { boxId: $scope.boxId, boxName: $scope.info.name });
                $timeout(function () {
                    $rootScope.options.quizOpen = true;
                });
            };

            $scope.$on('QuizAdded', function (e, quizItem) {
                if (quizItem.boxId !== $scope.boxId) {
                    return;
                }

                var quiz, index, filteredIndex;
                for (var i = 0, l = $scope.items.length; i < l && !quiz; i++) {
                    if ($scope.items[i].id === quizItem.id) {
                        quiz = $scope.items[i];
                        index = i;
                    }
                }

                if (quiz) {
                    if (!quizItem.publish) {
                        quiz.name = quizItem.name;
                        quiz.publish = quizItem.publish;
                        return;
                    }

                    filteredIndex = $scope.filteredItems.indexOf(quiz);

                    $scope.items.splice(index, 1);
                    $scope.filteredItems.splice(filteredIndex, 1);
                }

                $scope.items.unshift(quizItem);

                if (!$scope.currentTab) {
                    $scope.filteredItems.unshift(quizItem);
                }

            });

            $scope.$on('QuizDeleted', function (e, data) {
                if (data.boxId !== $scope.boxId) {
                    return;
                }

                var quiz, index, filteredIndex;
                for (var i = 0, l = $scope.items.length; i < l && !quiz; i++) {
                    if ($scope.items[i].id === data.quizId) {
                        quiz = $scope.items[i];
                        index = i;
                    }
                }

                if (!quiz) {
                    return;
                }

                filteredIndex = $scope.filteredItems.indexOf(quiz);

                $scope.items.splice(index, 1);
                $scope.filteredItems.splice(filteredIndex, 1);

            });


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

                    $scope.followBox(true);
                });
            });

            $scope.openUploadPopup = function (qna) {
                if (!UserDetails.isAuthenticated()) {
                    cd.pubsub.publish('register', { action: true });
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

                    var uploaded = 0;
                    $timeout(function () {
                        Upload[data.ajax](formData).then(function (response) {
                            uploaded++;

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

                            if (qna) {
                                fileList.push(responseItem);
                                if (data.type === 'link') {
                                    cd.pubsub.publish('addPoints', { type: 'itemUpload', amount: 1 });
                                    return;
                                }
                                if (uploaded === data.length) {
                                    cd.pubsub.publish('addPoints', { type: 'itemUpload', amount: fileList.length });
                                }
                                defer.resolve(fileList);

                            }
                            $scope.followBox(true);


                        }).catch(function () {
                            uploaded++;
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
                $scope.info.currentTab = null;
                $scope.options.manageTab = false;
                $scope.filteredItems = $filter('filter')($scope.items, filterItems);


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
                if (!UserDetails.isAuthenticated()) {
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                if ($scope.info.userType === 'invite' || $scope.info.userType === 'none') {
                    alert(jsResources.NeedToFollowBox);
                    return;
                }

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

            $scope.manageTab = function () {
                var filteredItems = $filter('filter')($scope.items, filterManageItems);
                if (!filteredItems.length) {
                    return;
                }
                $scope.filteredItems = filteredItems;
                $scope.changeView(consts.view.thumb);
                $scope.options.itemsLimit = consts.itemsLimit;


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
                if (item.sponsored) {
                    $scope.options.sponsored = true;
                }
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
                       cd.pubsub.publish('addPoints', { type: 'shareFb' });
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

            $scope.inviteFriends = function (e) {
                if (!UserDetails.isAuthenticated()) {
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

                if (item.type === 'Quiz' && !item.publish) {
                    $rootScope.$broadcast('initQuiz', { boxId: $scope.boxId, boxName: $scope.boxName, quizId: item.id });
                    $timeout(function () {
                        $rootScope.options.quizOpen = true;
                    });
                    return;
                }

                item.isNew = false;
                var type = item.type === 'Quiz' ? 'quizzes' : 'items';
                NewUpdates.setOld($scope.boxId, type, item.id);
            };

            $scope.deleteItem = function (item) {
                cd.confirm2(jsResources.SureYouWantToDelete + ' ' + item.name + "?").then(function () {
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
                });

                function removeItem(response) {
                    if (!(response.Success || response.success)) {
                        alert('error deleting ' + item.type.toLowerCase()); //translate
                        return;
                    }
                    var index = $scope.items.indexOf(item);
                    $scope.items.splice(index, 1);
                    index = $scope.filteredItems.indexOf(item);
                    if (index > -1) {
                        $scope.filteredItems.splice(index, 1);
                    }

                    if (item.type === 'Quiz' && !item.publish) {
                        $rootScope.$broadcast('closeQuizCreate', item.id);
                    }


                }
            };

            $scope.addDraggedItem = function (item, tabId) {
                if (item.type === 'Quiz') {
                    return;
                }

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
                if (item.sponsored) {
                    $scope.options.sponsored = true;
                }

                if (!$scope.info.currentTab) {
                    return true;
                }

                if ($scope.info.currentTab.id === item.tabId) {
                    return true;
                }

                return false;

            }

            function sortItems(a, b) {
                if (a.sponsored) {
                    return -1;
                }
                if (b.sponsored) {
                    return 1;
                }
                if (a.date > b.date) {
                    return -1;
                } else {
                    return 1;
                }
                if (a.name < b.name) {
                    return 1;
                }
                else {
                    return -1;
                }
                return 0;
            }
            //#endregion

            //#region scroll
            $scope.addItems = function () {
                $scope.options.itemsLimit += 7;
            };
            //#endregion            

            //#region settings

            $scope.openBoxSettings = function (tab) {

                if (!UserDetails.isAuthenticated()) {
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                if ($scope.info.userType === 'none' || $scope.info.userType === 'invite') {
                    alert(jsResources.NeedToFollowBox);
                    return;
                }

                var memberPromise = Box.members({ boxUid: $scope.boxId }),
                   notificationPromise = Box.notification({ boxUid: $scope.boxId }),
                   settingsAll = $q.all([memberPromise, notificationPromise]),
                   notification;

                settingsAll.then(function (response) {
                    $scope.info.allMembers = response[0].success ? response[0].payload : [];
                    notification = response[1].success ? response[1].payload : '';

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
                                    members: $scope.info.allMembers
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
                    uid: UserDetails.getDetails().id,
                    name: UserDetails.getDetails().name,
                    image: UserDetails.getDetails().image,
                    url: UserDetails.getDetails().url
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

                if (nonAjax) {
                    return;
                }

                Box.follow({ BoxUid: $scope.boxId }).then(function () {

                });
            };

            $scope.isUserLoggedIn = function () {
                return UserDetails.isAuthenticated();
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
