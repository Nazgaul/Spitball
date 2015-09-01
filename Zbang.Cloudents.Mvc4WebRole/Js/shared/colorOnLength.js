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