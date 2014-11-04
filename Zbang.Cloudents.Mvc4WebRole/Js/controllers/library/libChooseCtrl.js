
var libChoose = mLibrary.controller('LibChooseCtrl',
        ['$scope',
            '$timeout',
            '$filter',
           '$modal',
           '$location',
           'debounce',
         'sLibrary',
         'sFacebook',
         'sUserDetails',
         '$analytics',
         function ($scope, $timeout, $filter, $modal, $location, debounce, sLibrary, sFacebook, sUserDetails, $analytics) {
             "use strict";
             $scope.formData = {};
             $scope.display = {
                 searchUniversity: true
             };

             //var allDepartments;

             sFacebook.getToken().then(function (token) {
                 if (!token) {
                     return;
                 }
                 sLibrary.facebookFriends({ authToken: token }).then(function (response) {
                     var data = response.success ? response.payload : [];
                     $scope.FBUniversities = data;

                     if (!data.length) {
                         $analytics.eventTrack('no facebook', {
                             category: 'Select university'
                         });
                     }

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
             var lastQuery,
                search = debounce(function (query) {
                    if (query === lastQuery) {
                        return;
                    }

                    lastQuery = query;

                    return

                }, 200);

             $scope.selectSearch = debounce(function () {

                 var query = $scope.formData.searchInput || '';

                 if (query.length < 2) {
                     $scope.display.search = false;
                     $scope.display.facebook = true;
                     $scope.universities = null;
                     lastQuery = null;
                     return;
                 }

                 sLibrary.searchUniversities({ term: query }).then(function (response) {
                     var data = response.success ? response.payload : [];
                     $scope.display.search = true;
                     $scope.display.facebook = false;
                     $scope.universities = data;
                     console.log(data);

                     if (data.length) {
                         $analytics.eventTrack('search ' + $scope.formData.searchInput, {
                             category: 'Select university',
                             label: _.map(data, function (university) {
                                 return university.name;
                             }).toString()
                         });
                         return;
                     }

                     $analytics.eventTrack('empty search', {
                         category: 'Select university',
                         label: $scope.formData.searchInput
                     });
                 });
             });

             $scope.createSearch = function () {
                 var query = $scope.formData.createUniversity.name || '';

                 if (query.length < 2) {
                     $scope.createUniversities = null;
                     lastQuery = null;
                     return;
                 }

                 sLibrary.searchUniversities({ term: query }).then(function (response) {
                     var data = response.success ? response.payload : [];
                     $scope.createUniversities = data;
                 });
             };

             $scope.selectUniversity = function (university, isFacebook) {
                 $scope.selectedUni = university;
                 sLibrary.updateUniversity({ UniversityId: university.id }).then(function (response) {
                     var data = response.success ? response.payload : [];
                     if (!data) {
                         //$scope.display.searchUniversity = $scope.display.search = $scope.display.facebook = false;
                         //$scope.display.choose = true;
                         $analytics.setVariable('dimension1', university.name);
                         if (isFacebook) {
                             $analytics.eventTrack('Facebook choose', {
                                 category: 'Select university',
                                 label: university.name
                             });
                         }
                         window.open('/dashboard/', '_self');
                         //sUserDetails.setDepartment(null);
                         //getDepartments();
                         //sUserDetails.setUniversity(university);

                     } else {
                         sModal.open('uniRestriction', {
                             data: {
                                 university: university
                             },
                             html: data.html,
                             callback: {
                                 close: function () {
                                     $analytics.setVariable('dimension1', university.name);
                                     window.open('/dashboard/', '_self');
                                 }

                             }
                         });
                     }
                 });
             }

             //#endregion

             //#region facebook


             ////#region create department

             //$scope.createDepartment = function () {
             //    $scope.display.createDep = true;
             //    //$scope.display.choose = false;
             //};

             //$scope.createDepartmentSubmit = function (isValid) {
             //    if (!isValid) {
             //        return;
             //    }


             //    $scope.createDepartmentForm.$invalid = true;
             //    sLibrary.createDepartment($scope.formData.createDepartment).then(function (response) {
             //        if (response.success) {

             //            sUserDetails.setDepartment({
             //                name: $scope.formData.createDepartment.name,
             //                id: response.payload.id
             //            });
             //            window.open('/dashboard', '_self');
             //            //$location.path('/dashboard/');

             //        }
             //    });
             //};

             ////#endregion

             ////#region choose department
             //$scope.searchDepartment = debounce(function () {
             //    if (!$scope.params.departmentSearch) {
             //        $scope.departments = $filter('orderBy')(allDepartments, 'name');
             //        $scope.selectedDepartment = null;
             //        return;
             //    }

             //    if ($scope.selectedDepartment && $scope.params.departmentSearch !== $scope.selectedDepartment.name) {
             //        $scope.selectedDepartment = null;
             //    }

             //    if (allDepartments.length) {
             //        $scope.departments = $filter('orderByFilter')(allDepartments, { field: 'name', input: $scope.params.departmentSearch });
             //    }


             //}, 200);

             //$scope.selectDepartment = function (department) {
             //    $scope.selectedDepartment = department;
             //    $scope.params.departmentSearch = department.name;
             //    $scope.departments = null;

             //};
             ////$scope.chooseDepartment = function () {
             ////    sLibrary.chooseDeparment({ id: $scope.selectedDepartment.id }).then(function (response) {
             ////        if (response.success) {
             ////            sUserDetails.setDepartment($scope.selectedDepartment);
             ////            var navUrl = $location.search().returnUrl || '/dashboard/';
             ////            window.open(navUrl, '_self');
             ////            //$location.path('/dashboard/');
             ////        }
             ////    });
             ////};

             //$scope.backDepartment = function () {
             //    $scope.selectedDepartment = null;
             //    $scope.departments = $scope.params.departmentSearch = null;
             //    $scope.display.choose =  false;
             //    $scope.display.searchUniversity = $scope.display.facebook = true;
             //};

             //$scope.backCreateDepartment = function () {
             //    $scope.formData.createDepartment = {};
             //    $scope.display.createDep = false;
             //    $scope.display.choose = true;
             //};
             ////#endregion



             $scope.createUniversity = function () {
                 $scope.display.createUniversity = true;
                 $scope.display.search = $scope.display.searchUniversity = $scope.display.facebook
                      = false;

             };

             $scope.createUniversitySubmit = function (isValid) {
                 if (!isValid) {
                     return;
                 }
                 sLibrary.createUniversity($scope.formData.createUniversity).then(function (response) {
                     //var university = response.success ? response.payload : null;
                     if (response.success) {
                         window.open('/dashboard/', '_self');
                     }
                     //if (!university) {
                     //}
                     //$scope.selectedUni = university;
                     //$scope.display.createUniversity = $scope.display.searchUniversity = $scope.display.search = $scope.display.facebook = false;
                     //$scope.display.createDep = true;
                 });
             };

             //$scope.backUniversity = function () {
             //    $scope.formData.createUniversity = {};
             //    $scope.display.createUniversity = false;
             //    $scope.display.search = $scope.display.searchUniversity = true;
             //};

             //#endregion 


             //cd.analytics.trackEvent('Library Choose', 'Search', term);
             //function getDepartments() {
             //    sLibrary.items().then(function (response) {
             //        var data = response.success ? response.payload : [];
             //        allDepartments = data.nodes;
             //        $scope.departments = allDepartments;
             //    });
             //}
         }
        ]);
