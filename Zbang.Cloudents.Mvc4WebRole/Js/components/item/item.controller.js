(function () {
    angular.module('app.item').controller('ItemController', item);
    item.$inject = ['$stateParams', 'itemService', '$sce', '$location'];

    function item($stateParams, itemService, $sce, $location) {
        var i = this, boxid = $stateParams.boxId, itemId = $stateParams.itemId;
        var index = 0, needLoadMore = false;

        i.state = {
            regular: 0,
            rename: 1,
            flag: 2
        };

        i.preview = '';
        itemService.getDetails(boxid, itemId).then(function (response) {
            i.details = response;
            i.details.downloadUrl = $location.url() + 'download/';
            i.details.printUrl = $location.url() + 'print/';
            getPreview();
        });


       

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

        function getPreview() {
            i.loader = true;
            itemService.getPreview(
                 i.details.blob,
                 index,
                itemId,
                boxid
            ).then(function (data) {
                data = data || {};
                //$scope.load.contentLoading = $scope.load.contentLoadMore = false;
                if (data.preview) {
                    if (data.preview.indexOf('iframe') > 0
                        || data.preview.indexOf('audio') > 0
                        || data.preview.indexOf('video') > 0) {
                        //i.preview = data.preview;
                        i.preview = $sce.trustAsHtml(data.preview);
                    } else {
                        i.preview += data.preview;
                        //$analytics.eventTrack('Get Prview', {
                        //    category: 'Item'
                        //});
                        needLoadMore = true;
                    }
                    i.loader = false;
                    //$scope.$broadcast('update', data.preview); //for fullscreen
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
            if (needLoadMore) {
                needLoadMore = false;
                ++index;
                getPreview();
            }
        };
        function renameItem() {
            itemService.renameItem(i.renameText, itemId).then(function (response) {
                i.details.name = response.name;
                $location.path(response.url).replace();
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
            i.details.rate = !i.details.rate;
        }
    }


})();


