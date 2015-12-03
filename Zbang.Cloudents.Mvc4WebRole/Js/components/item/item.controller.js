(function () {
    angular.module('app.item').controller('ItemController', item);
    item.$inject = ['$stateParams', 'itemService', '$sce', '$location'];

    function item($stateParams, itemService, $sce, $location) {
        var i = this;
        var index = 0, loadMore = false;

        i.preview = '';
        itemService.getDetails($stateParams.boxId, $stateParams.itemId).then(function (response) {
            i.details = response;
            i.details.downloadUrl = $location.url() + 'download/';
            i.details.printUrl = $location.url() + 'print/';
            getPreview();
        });

        function getPreview() {
            //if (index > 0) {
            //    $scope.load.contentLoadMore = true;
            //} else {
            //    $scope.load.contentLoading = true;
            //}
            //blobName, index, itemId, boxId
            //string blobName, int imageNumber, long id, string boxId, int width = 0, int height = 0
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
                        loadMore = true;
                    }
                    //$scope.$broadcast('update', data.preview); //for fullscreen
                }
            });
        }

       

        i.loadMore = function () {
            if (loadMore) {
                loadMore = false;
                ++index;
                getPreview();
            }
        };


        i.renameOn = false;

        i.renameItem = function () {
            if (!i.renameOn) {
                i.renameOn = true;
                return;
            }
            itemService.renameItem(i.details.name, $stateParams.itemId).then(function (response) {
                i.details.name = response.name;
                $location.path(response.url).replace();
            });
        }

        i.like = function() {
            itemService.like($stateParams.itemId, $stateParams.boxId);
        }
    }

    
})();


