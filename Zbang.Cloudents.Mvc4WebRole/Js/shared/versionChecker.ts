module app {
    "use strict";
    // clientVersion = window["version"],
      var  timeInterval = 900000; //fifteen minutes
    class VerionChecker {

        constructor(
            private $http: angular.IHttpService,
            private cacheFactory: CacheFactory.ICacheFactory,
            private $mdToast: angular.material.IToastService,
            private resManager: IResManager,
            private $interval: angular.IIntervalService) {

            if (document.readyState === "complete") {
                $interval(this.checkVersion, timeInterval);
                //this.checkVersion();
            } else {
                window.addEventListener("load", () => {
                    this.checkVersion();
                    $interval(this.checkVersion, timeInterval);
                }, false);
            }

        }
        checkVersion() {
            this.$http.get("/home/version/").then(response => {
                var retVal: any = response.data;
                if (retVal.success) {
                    if (window["version"] === retVal.payload) {
                        return;
                    }
                    this.cacheFactory.clearAll();
                    const toast = this.$mdToast.simple().textContent(this.resManager.get('spitballUpdate')).action(this.resManager.get('dialogOk'))
                        .highlightAction(false)
                        .position('top');
                    this.$mdToast.show(toast).then(() => {
                        window.location.reload(true);
                    });


                }
            });
        }
        static factory() {
            const factory = ($http, cacheFactory, $mdToast, resManager, $interval) => {
                return new VerionChecker($http, cacheFactory, $mdToast, resManager, $interval);
            };

            factory["$inject"] = ["$http", "CacheFactory", "$mdToast", "resManager","$interval"];
            return factory;
        }
    }
    angular.module('app').run(VerionChecker.factory());
}
//'use strict';
//(function () {
//    angular.module('app').run(verionChecker);
//    verionChecker.$inject = ['$http', 'CacheFactory', '$mdToast', 'resManager', '$document'];

//    function verionChecker($http, cacheFactory, $mdToast, resManager, $document) {
//        "use strict";
//        var clientVersion = window.version,
//            timeInterval = 900000; //fifteen minutes

//        $document.ready(function () {
//            setInterval(checkVersion, timeInterval);
//        });
//        //window.setTimeout(function() {
//        //    setInterval(checkVersion, timeInterval);
//        //}, 10000);
//        //return {
//        //    checkVersion: checkVersion
//        //    //currentVersion: clientVersion
//        //};

//        function checkVersion() {
//            $http.get('/home/version/').then(function (response) {
//                var retVal = response.data;
//                if (retVal.success) {
//                    if (clientVersion === retVal.payload) {
//                        return;
//                    }
//                    cacheFactory.clearAll();
//                    var toast = $mdToast.simple().textContent(resManager.get('spitballUpdate')).action(resManager.get('dialogOk'))
//                        .highlightAction(false)
//                        .position('top');
//                    $mdToast.show(toast).then(function () {
//                        window.location.reload(true);
//                    });


//                }
//            });
//        }
//    }

//})()