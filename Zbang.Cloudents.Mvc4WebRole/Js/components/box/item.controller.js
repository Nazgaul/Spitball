(function () {
    angular.module('app.box.items').controller('ItemsController', feed);
    feed.$inject = ['boxService', '$stateParams'];

    function feed(boxService, $stateParams) {
        var f = this;
        console.log('here')
        //boxService.getFeed($stateParams.boxId).then(function (response) {
        //    f.data = response;
        //});
    }
})();