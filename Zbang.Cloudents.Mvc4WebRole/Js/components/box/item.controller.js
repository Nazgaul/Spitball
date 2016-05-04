﻿'use strict';
(function () {
    angular.module('app.box.items').controller('ItemsController', items);
    items.$inject = ['boxService', '$stateParams', '$rootScope', 'itemThumbnailService', '$mdDialog',
        '$scope', 'user', '$q', 'resManager'];

    function items(boxService, $stateParams, $rootScope, itemThumbnailService, $mdDialog, $scope, user, $q, resManager) {
        var i = this,
        boxId = $stateParams.boxId;
        i.items = [];
        i.uploadShow = true;
        i.tabSelected = {};
        var page = 0, needToBringMore = true;

        i.myPagingFunction = function () {
            if (i.term) {
                var defer = $q.defer();
                defer.resolve();
                return defer.promise;
            }
            return getItems(true);
        }
        i.filter = filter;
        i.openUpload = openUpload;
        i.deleteItem = deleteItem;
        i.addItemToTab = addItemToTab;
        i.dropToTabSuccess = dropToTabSuccess;
        i.downloadItem = followBox;
        i.removeItemFromTab = removeItemFromTab;
        $scope.setTab = setTab;

        getItems();

        function setTab(tab) {
            i.tabSelected = tab;
        }

        function followBox() {
            $scope.$emit('follow-box');
        }

        function removeItemFromTab(item) {
            boxService.addItemToTab(boxId, null, item.id);
            i.tabSelected.count--;
            var index = i.items.indexOf(item);
            i.items.splice(index, 1);
        }
        function addItemToTab($data, tab) {
            if (!user.id) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            tab.count++;
            followBox();
            boxService.addItemToTab(boxId, tab.id, $data.id);

        }
        function dropToTabSuccess(index) {
            i.items.splice(index, 1);
        }

        $scope.$on('resetParams', function () {
            resetParams();
        });
        $scope.$on('getItems', function () {
            getItems();
        });
        function resetParams() {
            page = 0;
            needToBringMore = true;
        }
        
        function openUpload() {
            if (!user.id) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            $rootScope.$broadcast('open_upload', i.tabSelected.id);
            i.uploadShow = false;
        }

        function deleteItem(ev, item) {
            var confirm = $mdDialog.confirm()
                 .title(resManager.get('deleteItem'))
                 .targetEvent(ev)
                 .ok(resManager.get('dialogOk'))
                 .cancel(resManager.get('dialogCancel'));

            $mdDialog.show(confirm).then(function () {
                var index = i.items.indexOf(item);
                i.tabSelected.count--;
                i.items.splice(index, 1);
                boxService.deleteItem(item.id);
            });
        }


        function getItems() {

            if (!needToBringMore) {
                var defer = $q.defer();
                defer.resolve();
                return defer.promise;
            }
            return boxService.items(boxId, i.tabSelected.id, page).then(function (response) {
                //response = itemThumbnailService.assignValues(response);
                angular.forEach(response, buildItem);
                if (page > 0) {
                    i.items = i.items.concat(response);
                } else {
                    i.items = response;
                }

                if (!response.length) {
                    needToBringMore = false;
                }
                page++;
            });
        }
        function filter() {
            if (!i.term) {
                i.tabSelected = {};
                resetParams();
                getItems();
            }
            boxService.filterItem(i.term, boxId, 0).then(function (response) {
                //response = itemThumbnailService.assignValues(response);
                angular.forEach(response, buildItem);
                i.items = response;
            });
        }

        function buildItem(value) {
            value.downloadLink = value.url + 'download/';
            var retVal = itemThumbnailService.assignValue(value.source);
            value.thumbnail = retVal.thumbnail;
            //value.icon = retVal.icon;
            value.nameExtension = value.name.replace(/\.[^/.]+$/, "");
        }


        //upload
        $rootScope.$on('item_upload', function (event, response2) {
            if (angular.isArray(response2)) {
                for (var j = 0; j < response2.length; j++) {
                    pushItem(response2[j]);
                }
                return;
            }
            pushItem(response2);
            function pushItem(response) {
                if (!response) {
                    return;
                }
                if (response.boxId != $stateParams.boxId) { // string an int comarison
                    return;
                }
                if (response.item.tabId != i.tabSelected.id) {
                    return; //not the same tab
                }
                followBox();
                var item = response.item;
                buildItem(item);
                i.items.unshift(item);
            }
        });
        $rootScope.$on('close_upload', function () {
            i.uploadShow = true;
        });
        $rootScope.$on('item_delete', function (e, itemId) {
            var item = i.items.find(function (x) {
                return x.id === itemId;
            });
            if (item) {
                var index = i.items.indexOf(item);
                i.items.splice(index, 1);
            }
        });
    }
})();