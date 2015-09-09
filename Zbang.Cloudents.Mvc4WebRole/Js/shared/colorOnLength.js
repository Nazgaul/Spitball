(function() {
    angular.module('app').directive('dColor',
        function() {
            return {
                restrict: 'A',
                link: function(scope, element, attrs) {
                    var length = attrs.dColor % 17;
                    element.addClass('color' + length);
                }
            };
        }
    );
})();


(function() {
    angular.module('app').directive('mixitup', ['$timeout', function ($timeout) {
        var linker = function (scope, element, attrs) {

            scope.$on("mixItUp", function () {
                $timeout(function () {
                    $(element).mixitup();
                });
            });
        };
        return {
            restrict: 'A',
            link: linker
        };
    }]);
})();


(function () {
    angular.module('app').directive('tableScroller', ['$timeout', function ($timeout) {
        var linker = function (scope, element, attrs) {

            scope.$on("tableScroll", function () {
                $timeout(function () {
                    $(element).dataTable({
                        paging: false,
                        scrollY: 400,
                        searching: false,
                        info:false,
                       // "dom": "<'row'<'col-md-6 col-sm-12'l><'col-md-6 col-sm-12'f>r>t<'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>", // datatable layout without  horizobtal scroll
                        //"scrollY": "300",
                        //"deferRender": true,
                        //"order": [
                        //    [0, 'asc']
                        //],
                        //"lengthMenu": [
                        //    [5, 10, 15, 20, -1],
                        //    [5, 10, 15, 20, "All"] // change per page values here
                        //],
                        //"pageLength": 10 // set the initial value            
                    });
                });
            });
        };
        return {
            restrict: 'A',
            link: linker
        };
    }]);
})();
