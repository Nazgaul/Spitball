(function () {
    'use strict';
    angular.module('app.box.items').controller('ItemsController', items);
    items.$inject = ['boxService', '$stateParams', '$rootScope',
        'itemThumbnailService', '$mdDialog',
        '$scope', 'user', '$q', 'resManager', '$state', "$window", "$timeout"];

    function items(boxService, $stateParams, $rootScope, itemThumbnailService,
        $mdDialog, $scope, user, $q, resManager, $state, $window, $timeout) {
        var i = this,
        boxId = $stateParams.boxId;
        i.items = [];
        i.uploadShow = true;
        $scope.stateParams = $stateParams;
        var page = 0, needToBringMore = true, disablePaging = false;


        i.myPagingFunction = function() {
            if (i.term) {
                return $q.when();
            }
            return getItems(true);
        };
        i.filter = filter;
        i.openUpload = openUpload;
        i.deleteItem = deleteItem;
        i.addItemToTab = addItemToTab;
        //i.dropToTabSuccess = dropToTabSuccess;
        i.downloadItem = followBox;
        i.removeItemFromTab = removeItemFromTab;
        //$scope.setTab = setTab;

        if ($stateParams.tabId && $stateParams.q) {
            $state.go('box.items', { tabId: $stateParams.tabId, q: null });
            return;
        }
        if ($stateParams.tabId) {
            getItems();
            scrollToPosition();
        }
        else if ($stateParams.q) {
            getFilter();
            scrollToPosition();
        } else {
            getItems();
            scrollToPosition();
        }



        function followBox() {
            $scope.$emit('follow-box');
        }

        function removeItemFromTab(item) {
            boxService.addItemToTab(boxId, null, item.id);
            $scope.$broadcast('tab-item-remove');
            var index = i.items.indexOf(item);
            i.items.splice(index, 1);
        }
        function addItemToTab($data, tab) {
            if (!user.id) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            var item = i.items.find(function (x) {
                return x.id === $data.id;
            });
            if (item) {
                var index = i.items.indexOf(item);
                i.items.splice(index, 1);
            }
            tab.count++;
            followBox();
            boxService.addItemToTab(boxId, tab.id, $data.id);

        }


        $rootScope.$on('disablePaging', function () {
            disablePaging = true;
        });
        $rootScope.$on('enablePaging', function () {
            disablePaging = false;
        });


        //$scope.$on('resetParams', resetParams);
        $scope.$on('update-thumbnail', function (e, args) {
            var item = i.items.find(function (x) {
                return x.id === args;
            });
            if (item) {
                item.thumbnail += '&1=1';
            }
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
            $rootScope.$broadcast('open_upload', $stateParams.tabId);
            i.uploadShow = false;
        }

        function deleteItem(ev, item) {
            disablePaging = true;
            var confirm = $mdDialog.confirm()
                 .title(resManager.get('deleteItem'))
                 .targetEvent(ev)
                 .ok(resManager.get('dialogOk'))
                 .cancel(resManager.get('dialogCancel'));

            $mdDialog.show(confirm).then(function () {
                var index = i.items.indexOf(item);
                boxService.deleteItem(item.id).then(function () {
                    $scope.$broadcast('tab-item-remove');
                    i.items.splice(index, 1);
                });
            }).finally(function () {
                disablePaging = false;
            });
        }


        function getItems() {
            if (!needToBringMore) {
                return $q.when();
            }
            if (disablePaging) {
                return $q.when();
            }
            return boxService.items(boxId, $stateParams.tabId, page).then(function (response) {
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
                $state.go('box.items', { tabId: null, q: null });
            }
            $state.go('box.items', { tabId: null, q: i.term });

        }

        function buildItem(value) {
            value.downloadLink = value.url + 'download/';
            var retVal = itemThumbnailService.assignValue(value.source);
            value.thumbnail = retVal.thumbnail;
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
                // ReSharper disable once CoercedEqualsUsing
                if (response.boxId !== $stateParams.boxId) { 
                    return;
                }
                if (response.item.tabId !== $stateParams.tabId) {
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
            //TODO: use https://developer.mozilla.org/en/docs/Web/JavaScript/Reference/Global_Objects/Array/findIndex
            var item = i.items.find(function (x) {
                return x.id === itemId;
            });
            if (item) {
                var index = i.items.indexOf(item);
                i.items.splice(index, 1);
            }
        });

        $scope.$watchCollection(function () {
            return [$state.params.tabId, $state.params.q];
        }, function (newParams, oldParams) {
            if ($state.current.name !== 'box.items') {
                return; //happen upon link
            }
            if (newParams[0] !== oldParams[0]) {
                if ($stateParams.tabId && $stateParams.q) {
                    $state.go('box.items', { tabId: $stateParams.tabId, q: null });
                    return;
                }
                resetParams();
                getItems();
            }
            if (newParams[1] !== oldParams[1]) {
                resetParams();
                getFilter();
            }

        });
        function getFilter() {
            i.term = $stateParams.q;
            boxService.filterItem($stateParams.q, boxId, 0).then(function (response) {
                angular.forEach(response, buildItem);
                i.items = response;
            });
        }

        function scrollToPosition() {
            var yOffsetParam = $stateParams.pageYOffset;
            if (yOffsetParam) {
                $timeout(function() {
                    $window.scrollTo(0, yOffsetParam);
                });
            }
        }
    }
})();