var appf = angular.module('myApp', []);
module myApp {
    "use strict";
    export interface IStoreListController {
        isOpen: boolean;
        sec: string;
    }

export class HomeController {
    static $inject = ["$scope", "$location"];
    isOpen:false;
    sec:'ask-copy';
    constructor(private $scope: angular.IScope, $location: angular.ILocationService) {
        this.isOpen= false;
        this.sec= 'ask-copy';
    }
   
    changeSection(item) {
        this.isOpen = false;
        this.sec=item;
        console.log("change");
    }
    }
appf.controller('HomeController', HomeController as any);
}

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
