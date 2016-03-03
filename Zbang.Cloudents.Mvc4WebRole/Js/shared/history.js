(function () {
    angular.module('app').service('history', h);
    h.$inject = ['$rootScope', '$location'];
    function h($rootScope, $location) {
        var self = this, arr = [] , skipState;
        //self.arr = [];
        var url;
        $rootScope.$on('$stateChangeStart', function () {
            url = $location.url();
        });
        $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
            if (fromState.name === toState.name) {
                return;
            }
            if (toParams.fromBack) {
                return;
            }
            if (skipState) {
                skipState = false;
                return;
            }
            // to be used for back button //won't work when page is reloaded.
            arr.push({
                name: fromState.name,
                params: fromParams

            });
            //arr.push($location.url());
        });

        $rootScope.$on('from-back', function() {
            skipState = true;
        });

        //self.pushState = function () {
        //    arr.push($location.url());
        //}

        self.popElement = function () {
            if (arr.length === 1) {
                return;
            }
            return arr.pop();
        }
        self.firstState = function() {
            return arr.length === 0;
        }
        
    }
})();