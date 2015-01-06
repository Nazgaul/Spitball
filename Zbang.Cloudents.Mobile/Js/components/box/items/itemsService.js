angular.module('boxItems')
    .service('itemsService',
    ['box', function (box) {
        var service = this;

        service.getItems = function (boxId, page, tabId) {
            tabId = tabId ? tabId : null;
            return box.items({ id: boxId, page: page, tabId: tabId });
        };

        service.getTabs = function (boxId) {
            return box.tabs({ id: boxId });
        };
    }]
);