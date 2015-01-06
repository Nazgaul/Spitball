angular.module('boxItems',['ajax']).
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
            getItemsPage(isAppend);
        };

        function getItemsPage(isAppend) {
            if (isFetching) {
                return;
            }

            isFetching = true;

            itemsService.getItems(boxId, page,boxItems.currentTabId).then(function (items) {
                items = items || [];

                if (!items.length) {
                    endResult = true;
                    return;
                }

                boxItems.items = items;
                page++;

            }).finally(function () {
                isFetching = false;
            });
        }

        function getTabs() {
            itemsService.getTabs(boxId).then(function (tabs) {                
                boxItems.tabs = tabs;          
            });

        }

    }]);
