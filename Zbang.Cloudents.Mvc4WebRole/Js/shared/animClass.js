(function() {
    angular.module('app').directive('animClass', animClass);
    animClass.$inject = ['$state', '$rootScope'];

    function animClass($state, $rootScope) {
        return {
            link: function(scope, elm) {
                //if ($state.current.data) {
                //    var enterClass = $state.current.data.animateClass;
                //    //elm.addClass(enterClass);
                //    scope.app.extra = enterClass
                //}
                ////scope.$on('$destroy', function () {
                ////    console.log('on desktroy')
                ////    elm.removeClass(enterClass);
                ////    if ($state.current.data) {
                ////        elm.addClass($state.current.data.animateClass);
                ////    }
                ////});
                //$rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState) {
                //    console.log(event);
                //    if (toState.data) {
                //        scope.app.extra = toState.data.animateClass;
                //    }
                //});

                var enterClass = $state.current.data.animateClass;
                elm.addClass(enterClass);
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