angular.module('box').
    controller('ItemsController',
    ['itemsService', '$stateParams', function (itemsService, $stateParams) {
        var boxItems = this;

        var page = 0,
            endResult,
            isFetching,
            boxId = $stateParams.boxId;


        getItemsPage();
        getTabs();

        boxItems.getItems = function () {
            getItemsPage(true);
        };

        function getItemsPage(isAppend) {
            if (isFetching) {
                return;
            }

            isFetching = true;

            itemsService.getItems(boxId, page).then(function (items) {
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
                boxItems.tabs = [{ id: null, name: boxItems.AllItems }];
                boxItems.tabs.push(tabs);
            });

        }

    }]);
