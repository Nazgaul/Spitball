﻿(function() {
    angular.module('app.account').controller('AccountSettingsDepartmentController', department);
    department.$inject = ['accountService', '$scope'];

    function department(accountService, $scope) {
        var ad = this;
        ad.departments = [];
        accountService.closeddepartment().then(function(response) {
            ad.departments = response;
        });
        $scope.$on('open-dep', function (e, dep) {
            accountService.closedMembers(dep.id).then(function (response) {
                dep.users = response;
            });
        });
    }
})();
(function() {
    angular.module('app.account').controller('accountSettingsDepController', department);

    department.$inject = ['$scope'];
    function department($scope) {
        // This LoadingController is attached to the DOM inside the ng-repeat
        // so it has access to the "group" object, which is provided by the ng-repeat

        // We are going to watch the current scope for changes to group.anyOldValue
        $scope.$watch(

          // This is the expression (on $scope) to watch
          'dep.isOpen',

          // This is the handler function that will be run "only" when the watched expression changes
          function (value) {
              // This first parameter is the changed value of group.anyOldValue
              // A second parameter would hold the previous value if you wanted it
              if (value) {
                  $scope.$emit('open-dep', $scope.dep);
              }
          }

        );
    }
})();