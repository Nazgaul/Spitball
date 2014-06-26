define(['app'], function (app) {
    app.controller('BoxCtrl',
        ['$scope', '$rootScope',
         '$routeParams', '$modal',
         '$filter', '$q','$window',
         'Box', 'Item', 'QnA',
         'NewUpdates','UserDetails',

        function ($scope, $rootScope, $routeParams, $modal, $filter, $q, $window, Box, Item, QnA, NewUpdates, UserDetails) {
            $scope.boxId = $routeParams.boxId;
            $scope.uniName = $routeParams.uniName;
            $scope.boxName = $routeParams.boxName;

            $scope.init = function (backUrl, backTitle) {
                $scope.back = {
                    title: backTitle,
                    url: backUrl
                };
                if (!$rootScope.previousTitle) {
                    $scope.previousTitle = backTitle                   
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
                createTab: '/Box/CreateTabPartial'
            };

            $scope.options = {
                currentView: consts.view.thumb,
                itemsLimit: consts.itemsLimit,
                manageTab: false
            };

            var infoPromise = Box.info({ boxUid: $scope.boxId }), //uid
                itemsPromise = Box.items({ boxUid: $scope.boxId, pageNumber: 0 }), //uid
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
                    uniCountry: info.uniCountry
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

                $scope.qna = 'qna'
                //answers: [{id:233b4bdc-3fb3-4be7-8ec5-a2f400c5063e,…}]
                //content: "asdsadas"
                //creationTime: "2014-03-20T09:54:27Z"

                //files: []
                //id: "59d599bb-34f5-4692-ac84-a2f400c43b74"
                //url: "/user/1/ram"
                //userImage: "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/userpic9.jpg"
                //userName: "guy golan"
                //userUid: 18372 //uid

                document.getElementById('mLoading').style.display = 'none';
                document.getElementById('box').style.display = 'block';
                document.getElementById('box').style.opacity = 1;

            });
            $scope.addQuiz = function () {
                $scope.params.quizOpen = true;
                $scope.$emit('initQuiz');
            };

            $scope.$on('addedItem', function (event, item) {

            });

            $scope.openUploadPopup = function () {

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
                    boxUid: $scope.boxId,  //uid
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
                $scope.changeView(consts.view.thumb);

                $scope.filteredItems = $filter('filter')($scope.items, filterManageItems);

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

            function saveItemsToTab(items) {
                var data = {
                    boxId: $scope.boxId,
                    tabId: $scope.info.currentTab.id,
                    itemId: items,
                    nDelete: true
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
                switch (item.type.toLowerCase()) {
                    case 'file':
                    case 'link':
                        return $scope.options.currentView === consts.view.thumb ? 'itemThumbView' : 'itemListView';
                    case 'quiz':
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

            $scope.deleteAllow = function (item) {
                return 
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
                console.log($scope.options.itemsLimit);
            };
            //#endregion            
        }
        ]);
});