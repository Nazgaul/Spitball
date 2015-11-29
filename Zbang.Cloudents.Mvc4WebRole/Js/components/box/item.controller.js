(function () {
    angular.module('app.box.items').controller('ItemsController', items);
    items.$inject = ['boxService', '$stateParams', '$scope', '$q', 'itemThumbnail'];

    function items(boxService, $stateParams, $scope, $q, itemThumbnail) {
        var i = this,
        itemsInBox = [], boxId = $stateParams.boxId;
        i.items = [];
        var page = 0, loading = false, needToBringMore = true;


        boxService.items(boxId, page).then(function (data) {
            i.items = itemsInBox = data;
            iterateItem();
            page++;
        });

        i.myPagingFunction = function () {
            getItems(true);
        }
        i.filter = filter;



        function getItems() {
            if (!loading && needToBringMore) {
                loading = true;

                boxService.items(boxId, page).then(function (response) {
                    i.items = itemsInBox = i.items.concat(response);

                    iterateItem();
                    if (!response.length) {
                        needToBringMore = false;
                    }
                    page++;
                    loading = false;
                });
            }
        }
        function filter() {
            if (!i.term) {
                i.items = itemsInBox;
            }
            boxService.filterItem(i.term, boxId, 0).then(function(response) {
                i.items = response;
                iterateItem();
            });
        }


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