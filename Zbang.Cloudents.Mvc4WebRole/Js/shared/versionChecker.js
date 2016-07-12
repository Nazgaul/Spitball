'use strict';
(function () {
    angular.module('app').run(verionChecker);
    verionChecker.$inject = ['$http', 'CacheFactory', '$mdToast', 'resManager', '$document'];

    function verionChecker($http, cacheFactory, $mdToast, resManager, $document) {
        "use strict";
        var clientVersion = window.version,
            timeInterval = 900000; //fifteen minutes

        $document.ready(function() {
            setInterval(checkVersion, timeInterval);
        });
        //window.setTimeout(function() {
        //    setInterval(checkVersion, timeInterval);
        //}, 10000);
        //return {
        //    checkVersion: checkVersion
        //    //currentVersion: clientVersion
        //};

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