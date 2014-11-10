﻿mBox.controller('BoxItemsCtrl',
		['$scope', '$rootScope', '$analytics','sModal', '$filter', '$timeout', 'sItem', 'sBox', 'sNewUpdates',
            'sUserDetails', 'sFacebook',
function ($scope, $rootScope,$analytics, sModal, $filter, $timeout, sItem, sBox, sNewUpdates, sUserDetails, sFacebook) {
    "use strict";
    var jsResources = window.JsResources;

    var consts = {
        view: {
            thumb: 'thumb',
            list: 'list'
        },
        itemsLimit: 50
    };


    $scope.iOptions = {
        currentView: consts.view.thumb,
        itemsLimit: consts.itemsLimit,
        starsLength: 5,
        starsWidth: 69,
        currentTab: null
    };

    sBox.items({ id: $scope.boxId }).then(function (boxItems) {
        $scope.items = _.map(boxItems, function (item) {
            sNewUpdates.isNew($scope.boxId, 'items', item.id, function (isNew) {
                item.isNew = isNew;
            });
            return item;
        });

        $scope.items.sort(sortItems);
        $scope.filteredItems = $filter('filter')($scope.items, filterItems);
        isSponsoredView();
        $scope.options.loader = false;
        $rootScope.$broadcast('BoxItemsLoaded');
    });
    //#region upload

    $scope.openUploadPopup = function () {
        if (!sUserDetails.isAuthenticated()) {
            cd.pubsub.publish('register', { action: true });
            return;
        }

        $analytics.eventTrack('Box Items', {
            category: 'Upload',
            label: 'User opened the upload popup'
        });

        sModal.open('upload', {
            data: {
                boxId: $scope.boxId,
                tabId: $scope.tabId,
                boxUrl: $scope.info.url
            }
        });
    };

    $scope.$on('ItemUploaded', function (e, data) {
        if (data.boxId !== $scope.boxId) {
            return;
        }

        $scope.followBox(true);

        sFacebook.postFeed($filter('stringFormat')(jsResources.IUploaded, [data.itemDto.name]), $scope.info.url);

        if ($scope.iOptions.currentTab && ($scope.iOptions.currentTab.id !== data.tabId)) {
            return;
        }

        $scope.info.itemsLength++;
        $scope.items.unshift(data.itemDto);
        $scope.filteredItems.unshift(data.itemDto);
        $scope.items.sort(sortItems);
        $scope.filteredItems.sort(sortItems);

    });

    //#endregion

    //#region view
    $scope.changeView = function (view) {
        if ($scope.iOptions.currentView === view) {
            return;
        }
        $scope.iOptions.itemsLimit = consts.itemsLimit;
        $scope.iOptions.lastView = $scope.iOptions.currentView;
        $scope.iOptions.currentView = view;

        $analytics.eventTrack('Box Items', {
            category: 'Change view',
            label: 'User changed view to ' + view
        });
    };

    $scope.getView = function () {
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
            sItem.delete(data).then(removeItem, function () {
                alert('error deleting this item'); //translate
            });

            $analytics.eventTrack('Box Items', {
                category: 'Delete'                
            });
        });

        function removeItem(response) {
            var index = $scope.items.indexOf(item);
            $scope.items.splice(index, 1);
            index = $scope.filteredItems.indexOf(item);
            if (index > -1) {
                $scope.filteredItems.splice(index, 1);
            }
            $scope.info.itemsLength--;

        }
    };



    $scope.deleteAllow = function (item) {
        return ($scope.info.userType === 'subscribe' || $scope.info.userType === 'owner') &&
               ($scope.info.userType === 'owner' || item.ownerId === sUserDetails.getDetails().id || sUserDetails.getDetails().isAdmin);
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

        var filteredItems = $filter('filter')($scope.items, filterItems);

        if (tab) {            
            $scope.filteredItems = $filter('orderBy')(filteredItems, 'name');
        } else {
            $scope.filteredItems = filteredItems;
        }

        isSponsoredView();
    });

    $scope.$on('tabItemAdded', function (e, data) {
        var item = _.find($scope.items, function (i) {
            return i.id === data.item.id;
        });

        item.tabId = data.tabId;

        saveItemsToTab([item.id], data.tabId);

    });

    $scope.$on('manageTab', function () {
        var filteredItems = $filter('filter')($scope.items, filterManageItems);
        isSponsoredView();
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

    $scope.manageSave = function () {
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

        $analytics.eventTrack('Box Items', {
            category: 'Manage Save',
            label: 'User saved manage changes'
        });
    };

    $scope.manageCancel = function () {
        resetLastView();

        $analytics.eventTrack('Box Items', {
            category: 'Manage Cancel',
            label: 'User cancelled manage changes'
        });
    };

    function saveItemsToTab(items, tabId) {
        var data = {
            boxId: $scope.boxId,
            tabId: tabId || $scope.iOptions.currentTab.id, //tabId from draganddrop
            itemId: items,
            nDelete: !tabId //delete is false if only one item added from draganddrop
        };

        sBox.addItemsToTab(data).then(function () { }, function (response) {
            alert(jsResources.FolderItemError);

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
