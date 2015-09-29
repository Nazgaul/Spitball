(function () {
    angular.module('app.dashboard').controller('CreateClass', createClass);
    createClass.$inject = ['dashboardService', '$scope', 'itemService'];

    function createClass(dashboardService, $scope, itemService) {
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
        };

       

        cc.box = {
            name: '123123',
            id: 113760,
            url: '/box/my/113760/123123/'

        };
        cc.step = steps.type;
        

        cc.boxType = boxType.groupType;

        cc.chooseBox = function (type) {
            cc.boxType = type;
            //cc.step = steps.name;
            $scope.$broadcast('uploadPhase');
            cc.step = steps.upload;
        };
        
        cc.alert = null;

        $scope.closeAlert = function () {
            cc.alert = null;
        }

        cc.submitFormProcess = false;

        cc.stepFinish = function () {
            if (cc.step === steps.name) {
                if (cc.boxType === boxType.groupType) {
                    if (!cc.box.name) {
                        cc.alert = 'required field';
                        return;
                    }
                    cc.submitFormProcess = true;
                    dashboardService.createPrivateBox(cc.box.name).then(function (response) {
                        $scope.$broadcast('uploadPhase');
                        cc.box.id = response.id;
                        cc.box.url = response.url;
                        
                        cc.step = steps.upload;
                    }, function (response) {
                        cc.alert = response;
                    }).finally(function () {
                        cc.submitFormProcess = false;
                    });

                }
            }
        }

       
    }
})();