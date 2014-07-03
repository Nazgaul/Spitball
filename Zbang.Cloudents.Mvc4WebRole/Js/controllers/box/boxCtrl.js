define('boxCtrl',['app'], function (app) {
    app.controller('BoxCtrl',
        ['$scope', '$rootScope',
         '$routeParams', '$modal',
         '$filter', '$q', '$timeout',
         'Box', 'Item', 'Quiz', 'QnA', 'Upload',
         'NewUpdates', 'UserDetails',

        function ($scope, $rootScope, $routeParams, $modal, $filter,
                  $q, $timeout, Box, Item, Quiz, QnA, Upload, NewUpdates, UserDetails) {

            $scope.boxId = $routeParams.boxId;
            $scope.uniName = $routeParams.uniName;
            $scope.boxName = $routeParams.boxName;

            $scope.init = function (backUrl, backTitle) {
                $scope.back = {
                    title: backTitle,
                    url: backUrl
                };
                if (!$rootScope.previousTitle) {
                    $scope.previousTitle = backTitle;
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
                createTab: '/Box/CreateTabPartial',
                uploader: '/Box/UploadPartial',
                uploadAddLink: '/Box/UploadLinkPartial'
            };

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
                var info = data[0].success ? data[0].payload : {},
                    items = data[1].success ? data[1].payload : {},
                    qna = data[2].success ? data[2].payload : {};

                $scope.info = {
                    name: info.name,
                    comments: info.comments,
                    courseId: info.courseId,
                    itemsLength: info.items,
                    membersLength: info.members,
                    members: info.subscribers,
                    ownerName: info.ownerName,
                    ownerId: info.ownerUid, //uid
                    privacy: info.privacySetting,
                    tabs: info.tabs,
                    userType: info.userType,
                    uniCountry: info.uniCountry,
                    image: info.image
                };

                $scope.info.currentTab = null;

                $scope.items = items;
                $scope.filteredItems = $filter('filter')($scope.items, filterItems);

                //commentsCount: 0
                //date: "2014-06-05T14:54:29Z"
                //id: 2966
                //numOfViews: 0
                //owner: "guy golan"
                //ownerId: 18372
                //publish: false
                //rate: 0
                //type: "Quiz"
                //userUrl: "/user/1/ram"

                $scope.qna = 'qna';
                //answers: [{id:233b4bdc-3fb3-4be7-8ec5-a2f400c5063e,…}]
                //content: "asdsadas"
                //creationTime: "2014-03-20T09:54:27Z"

                //files: []
                //id: "59d599bb-34f5-4692-ac84-a2f400c43b74"
                //url: "/user/1/ram"
                //userImage: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/userpic9.jpg"
                //userName: "guy golan"
                //userUid: 18372 //uid

                $timeout(function () {
                    $rootScope.$broadcast('viewContentLoaded');
                });
            });
            $scope.addQuiz = function () {
                $scope.params.quizOpen = true;
                $scope.$emit('initQuiz');
            };

            $scope.$on('addedItem', function (event, item) {

            });

            $scope.openUploadPopup = function () {
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
                            saveItem({url: url,type: 'link',timeout:1000});                            
                        }); //save url
                        return;
                    }

                    if (response.dropbox) {
                        var files = response.files, uploadData;
                        for (var i = 0, l = files.length; i < l; i++) {
                            saveItem({
                                name: files[i].name, 
                                size: files[i].bytes,
                                url: files[i].link,
                                type: 'dropbox',
                                timeout:5000
                            });
                                               
                        }
                        return;
                    }

                    if (response.googleDrive) {

                    }

                  
                }, function () {
                    //dismiss
                });
                function saveItem(data) {
                    var item = {
                        file: {
                            name: data.name || data.link,
                            size: data.size || 1e6
                        },
                        progress: 50,
                        isUploading: true,

                    },
                    formData = {
                        boxId: $scope.boxId,
                        boxUid: $scope.boxId,
                        boxName: $scope.boxName,
                        uniName: $scope.uniName,
                        tabId: $scope.info.currentTab ? $scope.info.currentTab.id : null,
                        url: data.url,
                        fileName: data.name,
                        fileUrl : data.url
                    }
                    item.remove = function () {
                        $scope.uploader.removeFromQueue(this);
                    };
                    item.cancel = function () {
                        $timeout.cancel(saveFile);
                    };
                    
                    $rootScope.uploader.queue.push(item);

                    var saveFile = $timeout(function () {
                        Upload[data.type](formData).then(function (response) {
                            if (!response.success) {
                                alert('error saving link');
                                return;
                            }
                            item.progress = 100;
                            item.isSuccess = item.isUploaded = true;
                            item.isUploading = false;
                            var responseItem = response.payload;
                            $scope.items.unshift(responseItem);
                            $scope.filteredItems.unshift(responseItem);
                        });
                    }, data.timeout);                    
                }
            };

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
                        alert('Error deleting tab');
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
                        alert('Error saving items to tab');
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
            };

            $scope.shareEmail = function () {
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
                        alert('error deleting ' + item.type.toLowerCase());
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
        }
        ]);
});