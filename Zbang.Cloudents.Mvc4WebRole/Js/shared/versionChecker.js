(function () {
    angular.module('app').factory('versionCheckerFactory', verionChecker);
    verionChecker.$inject = ['$http', 'CacheFactory', '$mdToast'];

    function verionChecker($http, cacheFactory, $mdToast) {
        "use strict";
        var clientVersion = window.version,
            //currentVersion,
            //timeInterval = 300000; //five minutes
            timeInterval = 30000;

        setInterval(checkVersion, timeInterval);
        //checkVersion();
        return {
            checkVersion: checkVersion,
            //currentVersion: clientVersion
        };

        function checkVersion() {
            $http.get('/home/version/').then(function (response) {
                var retVal = response.data;
                if (retVal.success) {
                    if (clientVersion === retVal.payload) {
                        return;
                    }
                    cacheFactory.clearAll();

                    var toast = $mdToast.simple().textContent('Spitball has updated').action('OK')
                        .highlightAction(false)
                        .position('top');
                    $mdToast.show(toast).then(function () {
                        window.location.reload(true);
                    });


                }
            });
        }
    }

})()