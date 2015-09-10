(function () {
    angular.module('app.item').controller('ItemController', item);
    item.$inject = ['$stateParams', 'itemService'];

    function item($stateParams, itemService) {
        var i = this;

        i.preview = '';
        itemService.getDetails($stateParams.boxId, $stateParams.itemId).then(function (response) {
            console.log(response);
            i.details = response;

            itemService.getPreview(i.details.blob, 0, $stateParams.itemId, $stateParams.boxId).then(function (response) {
                i.preview += response.preview;
            });
        });

    }
})();


(function () {
    angular.module('app.item').service('itemService', library);
    library.$inject = ['ajaxService'];

    function library(ajaxservice) {
        var d = this;

        d.getDetails = function (boxId, itemId) {
            return ajaxservice.get('/item/load/', { boxId: boxId, itemId: itemId }, 1800000);
        }
        d.getPreview = function (blobName, index, itemId, boxId) {
            return ajaxservice.get('/item/preview/', {
                blobName: blobName,
                index: index,
                id: itemId,
                boxId: boxId
            });
        }

    }
})();