(function() {
    angular.module('app').directive('tabDrop', tabDrop);

    function tabDrop() {
        return {
            restrict: 'A',
            link: function (scope, element) {
                element.tabdrop();
            }
        };
    }

})()