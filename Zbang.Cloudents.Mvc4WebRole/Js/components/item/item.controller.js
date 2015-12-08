(function () {
    angular.module('app.item').controller('ItemController', item);
    item.$inject = ['$stateParams', 'itemService', '$sce', '$location'];

    function item($stateParams, itemService, $sce, $location) {
        var i = this;
        var index = 0, needLoadMore = false;

        i.state = {
            regular: 0,
            rename: 1,
            flag: 2
        };

        i.preview = '';
        itemService.getDetails($stateParams.boxId, $stateParams.itemId).then(function (response) {
            i.details = response;
            i.details.downloadUrl = $location.url() + 'download/';
            i.details.printUrl = $location.url() + 'print/';
            getPreview();
        });


        function getPreview() {
            itemService.getPreview(
                 i.details.blob,
                 index,
                 $stateParams.itemId,
                $stateParams.boxId
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
                    //$scope.$broadcast('update', data.preview); //for fullscreen
                }
            });
        }

        //i.renameOn = true;
        i.loadMore = loadMore;
        i.selectedState = i.state.regular;

        i.flagItemOn = true;

        i.renameItem = renameItem;
        i.flagItem = flagItem;
        i.like = like;
        i.showRename = showRename;

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
            itemService.renameItem(i.renameText, $stateParams.itemId).then(function (response) {
                i.details.name = response.name;
                $location.path(response.url).replace();
            });
        }

        function flagItem() {
            if (!i.flagItemOn) {
                i.flagItemOn = true;
                return;
            }

            console.log(i.flag, i.customFlag);
            //TODO: flag the the item
        }

        
        function like() {
            itemService.like($stateParams.itemId, $stateParams.boxId);
        }
    }


})();


