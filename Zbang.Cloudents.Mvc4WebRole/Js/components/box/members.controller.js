﻿(function () {
    angular.module('app.box.members').controller('MembersController', members);
    members.$inject = ['boxService', '$stateParams'];

    function members(boxService, $stateParams) {


        var m = this;
        var boxData;
        boxService.getMembers($stateParams.boxId).then(function (response) {
            m.members = response;
        });
    }
})();