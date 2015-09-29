(function () {
    angular.module('app.item').service('itemService', library);
    library.$inject = ['ajaxService'];

    function library(ajaxservice) {
        var d = this;

        d.getDetails = function (boxId, itemId) {
            return ajaxservice.get('/item/load/', { boxId: boxId, itemId: itemId }, 1800000);
        };
        d.getPreview = function (blobName, index, itemId, boxId) {
            return ajaxservice.get('/item/preview/', {
                blobName: blobName,
                index: index,
                id: itemId,
                boxId: boxId
            });
        };


        d.addLink = function (url, boxid, tabId, question, name) {
            return ajaxservice.post('upload/link', {
                url: url,
                boxid: boxid,
                tabId: tabId,
                quesion: question,
                name: name
            });
        }
        d.addFromDropBox = function (boxId, url, name,  tabId, question) {
            return ajaxservice.post('upload/dropbox/', {
                boxId: boxId,
                url: url,
                name: name,
                tabId: tabId,
                question: question
            });
        }

    }
})();