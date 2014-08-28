mLibrary.controller('LibChooseCtrl',
        ['$scope',
            '$timeout',
           '$modal',
           'debounce',
         'sLibrary',
         'sFacebook',

         function ($scope, $timeout, $modal, debounce, sLibrary, sFacebook) {

             $scope.formData = {};
             $scope.display = {};

             $timeout(function () {
                 $scope.$emit('viewContentLoaded');
             });

             sFacebook.getToken().then(function (token) {
                 sLibrary.facebookFriends({ authToken: token }).then(function (response) {
                     var data = response.success ? response.payload : {};
                     $scope.FBUniversities = data;
                     $scope.display.facebook = true;
                 });

             });
             //#endregion

             //#region search
             $scope.search = debounce(function () {
                 var term = $scope.formData.searchInput;

                 if (term == '') {
                     $scope.display.search = false;
                     $scope.display.facebook = true;
                 }

                 sLibrary.searchUniversities({ term: term }).then(function (response) {
                     var data = response.success ? response.payload : {};
                     $scope.display.search = true;
                     $scope.display.facebook = false;
                     $scope.universities = data;
                 });             
             },150);
             
             //#endregion

             //#region facebook


             

             //cd.analytics.trackEvent('Library Choose', 'Search', term);
         }
        ]);
