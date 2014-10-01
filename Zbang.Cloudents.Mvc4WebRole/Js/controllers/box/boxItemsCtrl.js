mBox.controller('BoxItemsCtrl',
		['$scope', '$rootScope', '$modal', '$filter', '$timeout', 'sItem', 'sBox', 'sBoxData', 'sNewUpdates', 'sUserDetails', 'sUpload', 'sFacebook',
function ($scope, $rootScope, $modal, $filter, $timeout, sItem, sBox, sBoxData, sNewUpdates, sUserDetails, sUpload, sFacebook) {
    var jsResources = window.JsResources;

    var consts = {
        view: {
            thumb: 'thumb',
            list: 'list'
        },
        itemsLimit: 21
    };

    $scope.iOptions = {
        currentView: consts.view.thumb,
        itemsLimit: consts.itemsLimit,
        manageTab: false,
        starsLength: 5,
        starsWidth: 69,
    }

    sBox.items({ id: $scope.boxId, pageNumber: 0 }).then(function (response) {
        var data = response.success ? response.payload : [];
        $scope.items = _.map(data, function (item) {
            item.isNew = sNewUpdates.isNew($scope.boxId, 'items', item.id);
            return item;
        });

        $scope.items.sort(sortItems);
        $scope.filteredItems = $filter('filter')($scope.items, filterItems);
        $scope.options.loader = false;

    });

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
            $scope.items.sort(sortItems);
            $scope.filteredItems.sort(sortItems);

            sBoxData.addFile(data.item);
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
                tabId: $scope.info.currentTab ? $scope.info.currentTab.id : null, //
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
                    $scope.items.unshift(responseItem);
                    $scope.filteredItems.unshift(responseItem);
                    $scope.items.sort(sortItems);
                    $scope.filteredItems.sort(sortItems);

                    sBoxData.addFile(responseItem);

                    if (qna) {
                        fileList.push(responseItem);
                        if (data.type === 'link') {
                            cd.pubsub.publish('addPoints', { type: 'itemUpload', amount: 1 });
                        } else if (uploaded === data.length) {
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

    //#region view
    $scope.changeView = function (view) {
        if ($scope.iOptions.currentView === view) {
            return;
        }
        $scope.iOptions.itemsLimit = consts.itemsLimit;
        $scope.iOptions.lastView = $scope.options.currentView;
        $scope.iOptions.currentView = view;
    };

    $scope.getView = function (item) {
        return $scope.options.currentView === consts.view.thumb ? 'itemThumbView' : 'itemListView';
    };

    //function resetLastView() {
    //    if ($scope.options.lastView) {
    //        $scope.changeView($scope.options.lastView);
    //    }

    //    $scope.filteredItems = $filter('filter')($scope.items, filterItems);

    //    $scope.options.manageTab = false;
    //}
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
            sBoxData.removeFile(item);

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

            sBoxData.removeFile(item);
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
               ($scope.info.userType === 'owner' || item.ownerId === sUserDetails.getDetails().id || sUserDetails.getDetails().score > 1000000);
    };

    $scope.downloadItem = function (item) {
        cd.pubsub.publish('item_Download', { id: item.id });
    };

    function filterItems(item) {
        if (item.sponsored) {
            $scope.iOptions.sponsored = true;
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


}
		]);
