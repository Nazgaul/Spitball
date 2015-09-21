(function () {
    angular.module('app.dashboard').controller('CreateClass', createClass);
    createClass.$inject = ['dashboardService', '$scope'];

    function createClass(dashboardService, $scope) {
        var cc = this;
        var boxType = {
            groupType: 1,
            classType: 2
        };

        var steps = {
            type: 1,
            name: 2,
            department: 3,
            upload: 4,
            invite: 5
        }
        cc.step = steps.type;

        cc.boxType = boxType.groupType;

        cc.goToStep = function(step) {
            cc.step = step;
        }
    }
})();