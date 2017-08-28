var app;
(function (app) {
    "use strict";
    var homeController = (function () {
        function homeController($scope) {
            this.$scope = $scope;
            this.isOpen = false;
            this.sec = "ask-copy";
        }
        homeController.prototype.changeSection = function (item) {
            this.isOpen = false;
            this.sec = item;
        };
        homeController.$inject = ["$scope"];
        return homeController;
    }());
    angular
        .module("app")
        .controller("ctrl", homeController);
})(app || (app = {}));
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