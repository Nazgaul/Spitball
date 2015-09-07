(function () {
    angular.module('app.box.items').controller('ItemsController', items);
    items.$inject = ['boxService', '$stateParams', '$scope', '$timeout'];

    function items(boxService, $stateParams, $scope, $timeout) {
        var i = this;
        boxService.getItems($stateParams.boxId).then(function (response) {
            i.data = response;
            for (var j = 0; j < i.data.length; j++) {
                i.data[j].thumbnail = 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(i.data[j].source) + '.jpg?width=240&height=340&mode=crop'; // item.source
            }
            $scope.$broadcast("boxItemsLoaded");
        });
    }
})();