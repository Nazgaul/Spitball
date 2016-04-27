﻿(function () {
    angular.module('app.item').controller('ItemController', item);
    item.$inject = ['$stateParams', 'itemService', '$sce', '$location', '$q', 'user',
        'itemData', '$scope'];

    function item($stateParams, itemService, $sce, $location, $q,
        user, itemData, $scope) {
        var i = this, boxid = $stateParams.boxId, itemId = $stateParams.itemId;
        var index = 0, needLoadMore = false;

        i.state = {
            regular: 0,
            rename: 1,
            flag: 2
        };

        i.preview = '';


        i.details = itemData;


        i.details.downloadUrl = $location.path() + 'download/';
        i.details.printUrl = $location.path() + 'print/';
        i.details.boxUrl = i.details.boxUrl + 'items/';
        getPreview();
        // i.firstPage = history2.firstState();
        i.showRawText = false;
        $scope.app.showMenu = false;
        $scope.app.showSearch = true;
        $scope.app.fixedBgColor = true;

        //i.renameOn = true;
        i.loadMore = loadMore;
        i.selectedState = i.state.regular;


        i.renameItem = renameItem;
        i.flagItem = flagItem;
        i.cancelFlag = cancelFlag;
        i.like = like;

        i.showRename = showRename;

        i.swipeLeft = swipeLeft;
        i.swipeRight = swipeRight;

        i.document = itemData.fileContent;
        //if (i.firstPage && i.details.type === "File") {
        //    itemService.content(boxid, itemId).then(function (response) {
        //        i.document = response;
        //    });
        //}
        function getPreview() {
            i.loader = true;
            return itemService.getPreview(i.details.blob, index, itemId, boxid).then(function (data) {
                data = data || {};
                i.loader = false;

                if (data.preview) {
                    if (data.preview.indexOf('iframe') > 0
                        || data.preview.indexOf('audio') > 0
                        || data.preview.indexOf('video') > 0
                        || data.preview.indexOf('previewFailed') > 0) {
                        i.preview = $sce.trustAsHtml(data.preview);
                    } else {
                        var element = angular.element(data.preview);
                        i.preview += data.preview;
                        if (element.find('img,svg').length === 3) {
                            needLoadMore = true;
                        }

                    }

                }
            });
        }
        function swipeLeft() {
            if (i.details.next) {
                $location.url(i.details.next);
            }
        }


        function swipeRight() {
            if (i.details.previous) {
                $location.url(i.details.previous);
            }
        }

        function showRename() {
            i.selectedState = i.state.rename;
            i.renameText = i.details.name;
        }

        function loadMore() {
            if (needLoadMore && user.id) {
                needLoadMore = false;
                ++index;
                return getPreview();
            }
            return $q.when();
        };
        function renameItem() {
            if (i.renameText === i.details.name) {
                i.selectedState = i.state.regular;
                return;
            }
            i.submitDisabled = true;
            itemService.renameItem(i.renameText, itemId).then(function (response) {
                i.details.name = response.name;
                $location.path(response.url).replace();
            }).finally(function () {
                i.submitDisabled = false;
            });
        }
        function flagItem() {
            itemService.flag(i.flag, i.customFlag, itemId);
            cancelFlag();
        }

        function cancelFlag() {
            i.flag = '';
            i.selectedState = i.state.regular;
        }


        function like() {
            itemService.like(itemId, boxid);
            if (i.details.like) {
                i.details.likes--;
            } else {
                //TODO: bring back the toaster when item page is no longer pop up
                //$scope.app.showToaster(resManager.get('toasterLikeItem'));
                i.details.likes++;
            }
            i.details.like = !i.details.like;
        }
    }


})();


