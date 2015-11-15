(function() {
    angular.module('app').directive('animClass', animClass);
    animClass.$inject = ['$state'];

    function animClass($state) {
        return {
            link: function(scope, elm) {
                if ($state.current.data) {
                    var enterClass = $state.current.data.animateClass;

                    elm.addClass(enterClass);
                }
                scope.$on('$destroy', function() {
                    elm.removeClass(enterClass);
                    if ($state.current.data) {
                        elm.addClass($state.current.data.animateClass);
                    }
                });
            }
        }
    };
})();

