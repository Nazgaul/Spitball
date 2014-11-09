mLibrary.controller('LibChooseCtrl',
        ['$scope',
            '$timeout',
            '$filter',
           'sModal',
           '$location',
           'debounce',
         'sLibrary',
         'sFacebook',
         'sUserDetails',
         '$analytics',
         function ($scope, $timeout, $filter, sModal, $location, debounce, sLibrary, sFacebook, sUserDetails, $analytics) {
             "use strict";
             $scope.formData = {};
             $scope.display = {
                 searchUniversity: true
             };

             sFacebook.getToken().then(function (token) {
                 if (!token) {
                     return;
                 }
                 sLibrary.facebookFriends({ authToken: token }).then(function (data) {
                     $scope.FBUniversities = data;
                     if (!data.length) {
                         $analytics.eventTrack('no facebook', {
                             category: 'Select university'
                         });
                     }

                     if (!$scope.display.search || !$scope.formData.searchInput.length) {
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
                     $scope.universities = null;
                    $scope.display.facebook = true;
                     lastQuery = null;
                     return;
                 }

                 sLibrary.searchUniversities({ term: query }).then(function (data) {
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

                 sLibrary.searchUniversities({ term: query }).then(function (data) {
                     $scope.createUniversities = data;
                 });
             };

             $scope.selectUniversity = function (university, isFacebook) {
                 $scope.selectedUni = university;
                 sLibrary.updateUniversity({ UniversityId: university.id }).then(function (data) {
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


             $scope.createUniversity = function () {
                 $scope.display.createUniversity = true;
                 $scope.display.search = $scope.display.searchUniversity = $scope.display.facebook
                      = false;

             };

             $scope.createUniversitySubmit = function (isValid) {
                 if (!isValid) {
                     return;
                 }
                 sLibrary.createUniversity($scope.formData.createUniversity).then(function () {
                     window.open('/dashboard/', '_self');


                     //TODO analytics

                 });
             };          
         }
        ]);
