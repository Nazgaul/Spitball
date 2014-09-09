var libChoose = mLibrary.controller('LibChooseCtrl',
        ['$scope',
            '$timeout',
            '$filter',
           '$modal',
           '$location',
           'debounce',
         'sLibrary',
         'sFacebook',
         '$rootScope',

         function ($scope, $timeout, $filter, $modal, $location, debounce, sLibrary, sFacebook, $rootScope) {

             $scope.formData = {};
             $scope.display = {
                 searchUniversity: true
             };


             sFacebook.getToken().then(function (token) {
                 sLibrary.facebookFriends({ authToken: token }).then(function (response) {
                     var data = response.success ? response.payload : {};
                     $scope.FBUniversities = data;
                     if (!$scope.display.search || $scope.formData.searchInput) {
                         $scope.display.facebook = true;
                     }


                 });

             });
             $timeout(function () {
                 $scope.$emit('viewContentLoaded');
             });
             //#endregion

             //#region search
             var lastQuery;
             $scope.search = debounce(function () {
                 var query = $scope.formData.searchInput;
                 if (!query) {
                     return;
                 }
                 if (query.length < 2) {
                     $scope.display.search = false;
                     $scope.display.facebook = true;
                     $scope.universities = null;
                     lastQuery = null;
                     return;
                 }


                 if (query === lastQuery) {
                     return;
                 }

                 lastQuery = query;

                 sLibrary.searchUniversities({ term: query }).then(function (response) {
                     var data = response.success ? response.payload : [];
                     $scope.display.search = true;
                     $scope.display.facebook = false;
                     $scope.universities = data;
                 });
             }, 200);


             $scope.selectUniversity = function (university) {
                 $scope.selectedUni = university;


                 sLibrary.updateUniversity(
                     {
                         UniversityId: university.id
                     }

                     ).then(function (response) {
                         var data = response.success ? response.payload : [];
                         if (!data) {
                             $scope.display.searchUniversity = $scope.display.search = $scope.display.facebook = false;
                             $scope.display.complete = $scope.display.choose = true;
                         } else {
                             var modalInstance = $modal.open({
                                 windowClass: 'libChoosePopUp',
                                 template: data.html,
                                 controller: 'restrictionPopUpCtrl',
                                 resolve: {
                                     data: function () {
                                         return {
                                             university: university
                                         };
                                     }
                                 }
                             });
                             modalInstance.result.then(function() {
                                 $scope.display.searchUniversity = $scope.display.search = $scope.display.facebook = false;
                                 $scope.display.complete = $scope.display.choose = true;
                             });
                         }
                     });
             }

             //#endregion

             //#region facebook


             //#region create department

             $scope.createDepartment = function () {
                 $scope.display.createDep = true;
                 $scope.display.choose = false;
             };

             $scope.createDepartmentSubmit = function (isValid) {
                 if (!isValid) {
                     return;
                 }
                 $scope.createDepartmentForm.$invalid = true;
                 sLibrary.createDepartment($scope.formData.createDepartment).then(function (response) {
                     if (response.success) {
                         $location.path('/dashboard/');
                     }
                 });
             };

             //#endregion

             //#region choose department
             $scope.searchDepartment = debounce(function () {
                 if (!$scope.params.departmentSearch) {
                     $scope.departments = null;
                     return;
                 }

                 sLibrary.searchDepartment({ term: $scope.params.departmentSearch }).then(function (response) {
                     var data = response.success ? response.payload : {};
                     var departments = data;
                     $scope.departments = departments; //$filter('orderByFilter')(departments, { field: 'name', input: $scope.params.departmentSearch });
                 });
             }, 200);

             $scope.selectDepartment = function (department) {
                 $scope.selectedDepartment = department;
                 $scope.params.departmentSearch = department.name;
                 $scope.departments = null;

             };

             $scope.chooseDepartment = function () {
                 sLibrary.chooseDeparment({ id: $scope.selectedDepartment.id }).then(function (response) {
                     if (response.success) {
                         $location.path('/dashboard/');
                     }
                 });
             };

             $scope.backDepartment = function () {
                 $scope.selectedDepartment = null;
                 $scope.departments = $scope.params.departmentSearch = null;
                 $scope.display.choose = $scope.display.complete = false;
                 $scope.display.searchUniversity = $scope.display.facebook = true;
             };

             $scope.backCreateDepartment = function () {
                 $scope.formData.createDepartment = {};
                 $scope.display.createDep = false;
                 $scope.display.choose = true;
             };
             //#endregion



             $scope.createUniversity = function () {
                 $scope.display.createUniversity = true;
                 $scope.display.search = $scope.display.searchUniversity = $scope.display.facebook
                     = $scope.display.complete = $scope.display.choose = false;

             };

             $scope.createUniversitySubmit = function (isValid) {
                 if (!isValid) {
                     return;
                 }
                 sLibrary.createUniversity($scope.formData.createUniversity).then(function (response) {
                     var university = response.success ? response.payload : null;
                     if (!university) {
                     }
                     $scope.selectedUni = university;
                     $scope.display.createUniversity = $scope.display.searchUniversity = $scope.display.search = $scope.display.facebook = false;
                     $scope.display.complete = $scope.display.createDep = true;
                 });
             };

             $scope.backUniversity = function () {
                 $scope.formData.createUniversity = {};
                 $scope.display.createUniversity = false;
                 $scope.display.search = $scope.display.searchUniversity = true;
             };

             //#endregion 


             //cd.analytics.trackEvent('Library Choose', 'Search', term);
         }
        ]);
