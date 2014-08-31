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
                     if (!$scope.display.search || $scope.formData.searchInput) {
                         $scope.display.facebook = true;
                     }
                     
                 });

             });
             //#endregion

             //#region search
             var lastQuery;
             $scope.search = debounce(function () {
                 var query = $scope.formData.searchInput;

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
             },200);
             
             //#endregion

             //#region facebook


             

             //cd.analytics.trackEvent('Library Choose', 'Search', term);
         }
        ]);
