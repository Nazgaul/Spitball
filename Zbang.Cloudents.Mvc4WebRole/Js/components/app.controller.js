(function () {
    angular.module('app').controller('AppController', AppController);
    dashboard.$inject = ['$scope'];

    function AppController($scope) {
        $scope.$on('$viewContentLoaded', function () {
            Metronic.init(); // init core components
            //Layout.init(); //  Init entire layout(header, footer, sidebar, etc) on page load if the partials included in server side instead of loading with ng-include directive 
        });
    }
})();