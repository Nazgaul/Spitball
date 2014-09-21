mBox.controller('BoxItemsCtrl',
		['$scope', '$rootScope', '$modal', '$filter', '$timeout', 'sItem', 'sBox', 'sNewUpdates', 'sUserDetails', 'sUpload', 'sFacebook', '$templateCache',
function ($scope, $rootScope, $modal, $filter, $timeout, sItem, sBox, sNewUpdates, sUserDetails, sUpload, sFacebook, $templateCache) {
    var jsResources = window.JsResources;

    var consts = {
        view: {
            thumb: 'thumb',
            list: 'list'
        },
        itemsLimit: 21
    };

    $scope.partials = {
        uploader: '/Box/UploadPartial/',
        uploadAddLink: '/Box/UploadLinkPartial/'
    };



    $scope.iOptions = {
        currentView: consts.view.thumb,
        itemsLimit: consts.itemsLimit,
        starsLength: 5,
        starsWidth: 69,
        currentTab: null
    };

    sBox.items({ id: $scope.boxId, pageNumber: 0 }).then(function (response) {
        var data = response.success ? response.payload : [];
        $scope.items = _.map(data, function (item) {
            item.isNew = sNewUpdates.isNew($scope.boxId, 'items', item.id);
            return item;
        });

        $scope.items.sort(sortItems);
        $scope.filteredItems = $filter('filter')($scope.items, filterItems);
        isSponsoredView();
        $scope.options.loader = false;

    });

    //#region upload
    $scope.$on('FileAdded', function (event, data) {
        $scope.$apply(function () {
            if (data.boxId !== $scope.boxId) {
                return;
            }

            if ($scope.iOptions.currentTab && ($scope.iOptions.currentTab.id !== data.item.tabId)) {
                return;
            }

            $scope.items.unshift(data.item);
            $scope.filteredItems.unshift(data.item);
            $scope.items.sort(sortItems);
            $scope.filteredItems.sort(sortItems);

            $scope.followBox(true);
        });
    });

    $scope.openUploadPopup = function (qna) {
        if (!sUserDetails.isAuthenticated()) {
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
                    saveItem({ name: url, url: url, type: 'link', ajax: 'link', timeout: 1000, length: 1 });
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
            //TODO: what is that
            var formData = {
                boxId: $scope.boxId, //
                boxUid: $scope.boxId,
                boxName: $scope.boxName,
                uniName: $scope.uniName,
                tabId: $scope.iOptions.currentTab ? $scope.iOptions.currentTab.id : null, //
                url: data.url,
                fileName: data.name, //
                fileUrl: data.url //
            }

            var uploaded = 0;
            $timeout(function () {
                sUpload[data.ajax](formData).then(function (response) {
                    uploaded++;

                    if (uploaded === 1) {
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
                    if (qna) {
                        fileList.push(responseItem);
                        if (data.type === 'link') {
                            cd.pubsub.publish('addPoints', { type: 'itemUpload', amount: 1 });
                        } else if (uploaded === data.length) {
                            cd.pubsub.publish('addPoints', { type: 'itemUpload', amount: fileList.length });
                        }
                        defer.resolve(fileList);

                    }

                    if ((!$scope.iOptions.currentTab) || ($scope.iOptions.currentTab.id === responseItem.tabId)) {
                        $scope.items.unshift(responseItem);
                        $scope.filteredItems.unshift(responseItem);
                        $scope.filteredItems.sort(sortItems);
                    }

                    $scope.followBox(true);


                }).catch(function () {
                    uploaded++;
                });
            }, data.timeout);
        }

    };

    //#endregion

    //#region view
    $scope.changeView = function (view) {
        if ($scope.iOptions.currentView === view) {
            return;
        }
        $scope.iOptions.itemsLimit = consts.itemsLimit;
        $scope.iOptions.lastView = $scope.iOptions.currentView;
        $scope.iOptions.currentView = view;
    };

    $scope.getView = function (item) {
        return $scope.iOptions.currentView === consts.view.thumb ? 'itemThumbView' : 'itemListView';
    };

    function resetLastView() {
        if ($scope.iOptions.lastView) {
            $scope.changeView($scope.iOptions.lastView);
        }

        $scope.filteredItems = $filter('filter')($scope.items, filterItems);
        isSponsoredView();

        $scope.iOptions.manageTab = false;
    }
    //#endregion

    //#region items
    $scope.selectItem = function (e, item) {
        if ($scope.iOptions.manageTab) {
            e.preventDefault();
            item.isCheck = !item.isCheck;
            return;
        }

        item.isNew = false;
        sNewUpdates.setOld($scope.boxId, 'items', item.id);
    };

    $scope.deleteItem = function (item) {
        cd.confirm2(jsResources.SureYouWantToDelete + ' ' + item.name + "?").then(function () {
            var data = {
                itemId: item.id,
                boxId: $scope.boxId
            }
            sItem.delete(data).then(removeItem);
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

        }
    };

   

    $scope.deleteAllow = function (item) {
        return ($scope.info.userType === 'subscribe' || $scope.info.userType === 'owner') &&
               ($scope.info.userType === 'owner' || item.ownerId === sUserDetails.getDetails().id || sUserDetails.getDetails().score > 1000000);
    };

    $scope.downloadItem = function (item) {
        cd.pubsub.publish('item_Download', { id: item.id });
    };

    function filterItems(item) {
        if (!$scope.iOptions.currentTab) {
            return true;
        }

        if ($scope.iOptions.currentTab.id === item.tabId) {
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
        //if (a.name < b.name) {
        //    return 1;
        //}
        //else {
        //    return -1;
        //}
        //return 0;
    }
    //#endregion

    //#region scroll
    $scope.addItems = function () {
        $scope.iOptions.itemsLimit += 7;
    };
    //#endregion            

    //#region tabs

    $scope.$on('selectTab', function (e, tab) {
        $scope.iOptions.currentTab = tab;

        $scope.iOptions.itemsLimit = consts.itemsLimit;
        $scope.filteredItems = $filter('filter')($scope.items, filterItems);
        isSponsoredView();
    });

    $scope.$on('tabItemAdded', function (e, data) {       
        var item = _.find($scope.items, function (i) {
            return i.id === data.item.id;
        });

        item.tabId = data.tabId;
        
        saveItemsToTab([item.id], data.tabId);

    });

    $scope.$on('manageTab', function (e) {
        var filteredItems = $filter('filter')($scope.items, filterManageItems);
        isSponsoredView()
        if (!filteredItems.length) {
            return;
        }
        $scope.filteredItems = filteredItems;
        $scope.changeView(consts.view.thumb);
        $scope.iOptions.itemsLimit = consts.itemsLimit;


        for (var i = 0, l = $scope.filteredItems.length; i < l; i++) {
            $scope.filteredItems[i].isCheck = ($scope.iOptions.currentTab.id === $scope.filteredItems[i].tabId);
        }

        $scope.iOptions.manageTab = true;
    });

    $scope.manageSave = function (e) {
        var savedItems = [],
            item;

        for (var i = 0, l = $scope.filteredItems.length; i < l; i++) {
            item = $scope.filteredItems[i];
            if (item.isCheck) {
                savedItems.push(item.id);
                item.tabId = $scope.iOptions.currentTab.id;
                continue;
            }
            item.tabId = null;
        }

        saveItemsToTab(savedItems);
        resetLastView();
    };

    $scope.manageCancel = function (e) {
        resetLastView();
    };

    function saveItemsToTab(items, tabId) {
        var data = {
            boxId: $scope.boxId,
            tabId: tabId || $scope.iOptions.currentTab.id, //tabId from draganddrop
            itemId: items,
            nDelete: !tabId //delete is false if only one item added from draganddrop
        };

        sBox.addItemsToTab(data).then(function (response) {
            if (!response.success) {
                alert(jsResources.FolderItemError);
            }
        });

    }

    function filterManageItems(item) {
        if (item.sponsored) {
            $scope.iOptions.sponsored = true;
        }

        return true;
    }

    function isSponsoredView() {
        $scope.iOptions.sponsored = false;
        _.forEach($scope.filteredItems, function (item) {
            if (item.sponsored) {
                $scope.iOptions.sponsored = true;
            }
        });
    }
    //#endregion
}
		]);
