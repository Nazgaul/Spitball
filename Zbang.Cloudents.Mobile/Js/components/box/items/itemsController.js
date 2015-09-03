angular.module('boxItems', ['ajax']).
    controller('ItemsController',
    ['itemsService', '$stateParams', function (itemsService, $stateParams) {
        var boxItems = this;

        var page = 0,
            endResult,
            isFetching,
            boxId = $stateParams.boxId;
       
        getItemsPage();
        getTabs();

        boxItems.getItems = function (isAppend) {
            if (!isAppend) {
                page = 0;
                boxItems.items = [];
                endResult = false;
            }

            getItemsPage(isAppend);
        };

        function getItemsPage(isAppend) {
            if (isFetching) {
                return;
            }

            if (endResult) {
                return;
            }

            boxItems.loading = true;
            isFetching = true;


            itemsService.getItems(boxId, page, boxItems.currentTabId).then(function (items) {
                items = items || [];
                for (var i = 0; i < items.length; i++) {
                    items[i].thumbnail = 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(items[i].source) + '.jpg?width=148&height=188&mode=crop';

                }
                page++;

                if (!items.length) {
                    if (!isAppend) {
                        boxItems.noResults = true;
                        return;
                    }
                    endResult = true;
                    return;
                }

                if (!isAppend) {
                    boxItems.items = items;
                    return;
                }

                boxItems.items = boxItems.items.concat(items);



            }).finally(function () {
                isFetching = false;
                boxItems.loading = false;

            });
        }

        function getTabs() {
            itemsService.getTabs(boxId).then(function (tabs) {
                boxItems.tabs = tabs;
            });

        }

    }]);
