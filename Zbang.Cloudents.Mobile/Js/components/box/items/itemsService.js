angular.module('box')
    .service('itemsService',
    ['box', function (box) {
        var service = this;

        service.getItems = function(boxId, page) {
            return box.items({ id: boxId, page: page });
        };

        service.getTabs = function (boxId) {
            return box.tabs{id: boxId});
     }
    }]
);