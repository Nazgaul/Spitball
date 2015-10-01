(function() {
    angular.module('app').controller('SearchController', search);


    function search() {
        var s = this;

        s.open = false;
        s.openForm = function() {
            s.open = true;
        }
    }
})();