(function () {
    angular.module('app').factory('versionCheckerFactory', verionChecker);
    verionChecker.$inject = ['$http', 'CacheFactory', '$mdToast', 'resManager'];

    function verionChecker($http, cacheFactory, $mdToast, resManager) {
        "use strict";
        var clientVersion = window.version,
            timeInterval = 300000; //five minutes

        setInterval(checkVersion, timeInterval);
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
                    var toast = $mdToast.simple().textContent(resManager.get('spitballUpdate')).action(resManager.get('dialogOk'))
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