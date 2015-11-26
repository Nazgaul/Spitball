(function () {
    angular.module('app.box.items').controller('ItemsController', items);
    items.$inject = ['boxService', '$stateParams', '$scope', '$q', 'itemThumbnail'];

    function items(boxService, $stateParams, $scope, $q, itemThumbnail) {
        var i = this;
        i.items = [];
        var page = 0, loading = false, needToBringMore = true;

        boxService.items($stateParams.boxId, page).then(function (data) {
            i.items = data;
            iterateItem();
            page++;
        });

        i.myPagingFunction = function () {
            getItems(true);
        }


       

        function getItems(needToAppend) {
            if (!loading && needToBringMore) {
                loading = true;

                boxService.items($stateParams.boxId,  page).then(function (response) {
                    if (needToAppend) {
                        i.items = i.items.concat(response);
                    } else {
                        i.items = response;
                    }
                    iterateItem();
                    if (!response.length) {
                        needToBringMore = false;
                    }
                    page++;
                    loading = false;
                });
            }
        }

        //i.filterItems = function (item) {
        //    if (!i.tabSelectedId) {
        //        return true;
        //    }
        //    if (i.tabSelectedId === item.tabId) {
        //        return true;
        //    }
        //    return false;
        //}

        //upload
        $scope.$on('item_upload', function (event, response) {
            if (response.boxId != $stateParams.boxId) { // string an int comarison
                return;
            }
            var item = response.fileDto;
            item.thumbnail = buildThumbnailUrl(item.source);
            i.items.unshift(item);
        });

        function iterateItem() {
            for (var j = 0; j < i.items.length; j++) {
                if (i.items[j].thumbnail) {
                    continue;
                }
                i.items[j].thumbnail = buildThumbnailUrl(i.items[j].source);
            }
        }
        function buildThumbnailUrl(name) {
            return itemThumbnail.get(name, 368, 520);
        }
    }
})();