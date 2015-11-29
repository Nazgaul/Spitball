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


        d.addLink = function (url, boxid, question, name) {
            return ajaxservice.post('upload/link', {
                url: url,
                boxid: boxid,
                
                quesion: question,
                name: name
            });
        }
        d.addFromDropBox = function (boxId, url, name,  question) {
            return ajaxservice.post('upload/dropbox/', {
                boxId: boxId,
                url: url,
                name: name,
                question: question
            });
        }

        d.renameItem = function (name, id) {
            return ajaxservice.post('item/rename', {
                id: id,
                newName: name
            }).then(function (response) {
                return { name: response.name, url: response.url }
            });
        }

        //no need to wipe cache
        d.like = function (id, boxId) {
            return ajaxservice.post('item/like', {
                itemId: id,
                boxId: boxId
            }, true);
        };

    }
})();