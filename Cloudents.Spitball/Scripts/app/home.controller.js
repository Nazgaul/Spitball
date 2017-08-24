var appf = angular.module('myApp', []);
var myApp;
(function (myApp) {
    "use strict";
    var HomeController = (function () {
        function HomeController($scope, $location) {
            this.$scope = $scope;
            this.isOpen = false;
            this.sec = 'ask-copy';
        }
        HomeController.prototype.changeSection = function (item) {
            this.isOpen = false;
            this.sec = item;
            console.log("change");
        };
        HomeController.$inject = ["$scope", "$location"];
        return HomeController;
    }());
    myApp.HomeController = HomeController;
    appf.controller('HomeController', HomeController);
})(myApp || (myApp = {}));
//}
//    homeController.$inject = ['$location'];
//    function homeController($location) {
//        /* jshint validthis:true */
//        var vm = this;
//        vm.title = 'homeController';
//        vm.sec = 'ask-copy';
//        function changeSection(item) {
//            vm.isOpen = false;
//            vm.sec = item;
//        }
//    }
//}
//# sourceMappingURL=home.controller.js.map