(function () {
    'use strict';
    angular.module('app.item').controller('ItemController', item);
    item.$inject = ['$stateParams', 'itemService', '$sce', '$location', '$q', 'user',
        'itemData', '$scope', '$rootScope', 'resManager', '$timeout', '$mdMenu'];

    function item($stateParams, itemService, $sce, $location, $q,
        user, itemData, $scope, $rootScope, resManager, $timeout, $mdMenu) {
        var i = this, boxid = $stateParams.boxId, itemId = $stateParams.itemId, disablePaging = false;
        var index = 0, needLoadMore = false;

        i.state = {
            regular: 0,
            rename: 1,
            flag: 2
        };

        i.preview = '';

        //$('[ui-view].class').hide();
        //$scope.$on('$destroy', function () {
        //    $('[ui-view].class').show()
        //})
        i.details = itemData;


        i.details.downloadUrl = $location.path() + 'download/';
        i.details.printUrl = $location.path() + 'print/';
        i.details.boxUrl = i.details.boxUrl + 'items/';
        getPreview();
        // i.firstPage = history2.firstState();
        i.showRawText = false;

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
        i.followBox = followBox;
        i.document = itemData.fileContent;

        //i.back = back;

        //function back() {
        //    if ($previousState.get()) {
        //        $previousState.go();
        //        return;
        //    }
        //    $location.url(i.details.boxUrl);
        //}

        function followBox() {
            itemService.followbox();
            //cacheFactory.clearAll();//autofollow issue
        }
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

        $rootScope.$on('disablePaging', function () {
            disablePaging = true;
        });
        $rootScope.$on('enablePaging', function () {
            disablePaging = false;
        });
        function loadMore() {
            if (!disablePaging && needLoadMore && user.id) {
                needLoadMore = false;
                ++index;
                return getPreview();
            }
            return $q.when();
        }
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
                $scope.app.showToaster(resManager.get('toasterLikeItem'), 'main-nav');
                i.details.likes++;
            }
            i.details.like = !i.details.like;

            $timeout(function () {
                $mdMenu.hide();
            }, 2000);
        }

    }


})();


