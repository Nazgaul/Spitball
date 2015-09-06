(function () {
    angular.module('app').controller('AppController', appController);
    appController.$inject = ['$scope'];

    function appController($scope) {
        $scope.$on('$viewContentLoaded', function () {
            //TODO: maybe this is no good.
            Metronic.init(); // init core components
            
        });
    }
})();