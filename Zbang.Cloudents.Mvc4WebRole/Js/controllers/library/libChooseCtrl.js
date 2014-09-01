mLibrary.controller('LibChooseCtrl',
        ['$scope',
            '$timeout',
            '$filter',
           '$modal',
           '$location',
           'debounce',
         'sLibrary',
         'sFacebook',

         function ($scope, $timeout, $filter, $modal, $location, debounce, sLibrary, sFacebook) {

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

                     $timeout(function () {
                         $scope.$emit('viewContentLoaded');
                     });
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
             }, 200);


             $scope.selectUniversity = function (university) {
                 $scope.selectedUni = university;
                 $scope.display.searchUniversity = $scope.display.search = $scope.display.facebook = false;
                 $scope.display.complete = $scope.display.choose = true;
             }

             //#endregion

             //#region facebook


             //#region create department

             $scope.createDepartment = function () {
                 $scope.display.createDep = true;
                 $scope.display.choose = false;
             };

             //#endregion

             //#region choose department
             $scope.searchDepartment = debounce(function () {
                 if (!$scope.params.departmentSearch) {
                     $scope.departments = null;
                     return;
                 }

                 $timeout(function () {
                     var departments = [{
                         id: 1,
                         image: 'http://placehold.it/66x66',
                         name: 'adep1'
                     }, {
                         id: 2,
                         image: 'http://placehold.it/66x66',
                         name: 'bdep2'
                     }, {
                         id: 3,
                         image: 'http://placehold.it/66x66',
                         name: 'cdep3'
                     }, {
                         id: 4,
                         image: 'http://placehold.it/66x66',
                         name: 'ddep4'
                     }, ]

                     $scope.departments = $filter('orderByFilter')(departments, { field: 'name', input: $scope.params.departmentSearch });

                 }, 100);


                 //    sLibrary.searchDepartment({}).then(function (response) {
                 //        var data = response.success ? response.payload : {};
                 //        $scope.departments = data;
                 //        $scope.departments = $filter('orderByFilter')(departments, { field: 'name', input: $scope.params.departmentSearch });
                 //    });                 
             }, 200);

             $scope.selectDepartment = function (department) {
                 $scope.selectedDepartment = department;
                 $scope.params.departmentSearch = department.name;
                 $scope.departments = null;

             };

             $scope.chooseDepartment = function () {
                 $location.path('/dashboard/');
             };

             $scope.backDepartment = function () {
                 $scope.selectedDepartment = null;
                 $scope.departments = $scope.params.departmentSearch= null;                 
                 $scope.display.choose = $scope.display.complete = false;                 
                 $scope.display.searchUniversity = $scope.display.facebook = true;
             };

             $scope.backCreateDepartment = function () {                 
                 $scope.formData.createDepartment = {};
                 $scope.display.createDep = false;
                 $scope.display.choose = true;
             };
             //#endregion

             //#region create university 
             var countries = ['Afghanistan', 'Albania', 'Algeria', 'Andorra', 'Angola', 'Antigua & Deps', 'Argentina', 'Armenia', 'Australia', 'Austria', 'Azerbaijan', 'Bahamas', 'Bahrain', 'Bangladesh', 'Barbados', 'Belarus', 'Belgium', 'Belize', 'Benin', 'Bhutan',
             'Bolivia', 'Bosnia Herzegovina', 'Botswana', 'Brazil', 'Brunei', 'Bulgaria', 'Burkina', 'Burundi', 'Cambodia', 'Cameroon', 'Canada', 'Cape Verde', 'Central African Rep', 'Chad', 'Chile', 'China', 'Colombia', 'Comoros', 'Congo', 'Congo {Democratic Rep}',
             'Costa Rica', 'Croatia', 'Cuba', 'Cyprus', 'Czech Republic', 'Denmark', 'Djibouti', 'Dominica', 'Dominican Republic', 'East Timor', 'Ecuador', 'Egypt', 'El Salvador', 'Equatorial Guinea', 'Eritrea', 'Estonia', 'Ethiopia', 'Fiji', 'Finland', 'France', 'Gabon',
             'Gambia', 'Georgia', 'Germany', 'Ghana', 'Greece', 'Grenada', 'Guatemala', 'Guinea', 'Guinea-Bissau', 'Guyana', 'Haiti', 'Honduras', 'Hungary', 'Iceland', 'India', 'Indonesia', 'Iran', 'Iraq', 'Ireland {Republic}', 'Israel', 'Italy', 'Ivory Coast', 'Jamaica', 'Japan',
             'Jordan', 'Kazakhstan', 'Kenya', 'Kiribati', 'Korea North', 'Korea South', 'Kosovo', 'Kuwait', 'Kyrgyzstan', 'Laos', 'Latvia', 'Lebanon', 'Lesotho', 'Liberia', 'Libya', 'Liechtenstein', 'Lithuania', 'Luxembourg', 'Macedonia', 'Madagascar', 'Malawi', 'Malaysia', 'Maldives',
             'Mali', 'Malta', 'Marshall Islands', 'Mauritania', 'Mauritius', 'Mexico', 'Micronesia', 'Moldova', 'Monaco', 'Mongolia', 'Montenegro', 'Morocco', 'Mozambique', 'Myanmar, {Burma}', 'Namibia', 'Nauru', 'Nepal', 'Netherlands', 'New Zealand', 'Nicaragua', 'Niger', 'Nigeria',
             'Norway', 'Oman', 'Pakistan', 'Palau', 'Panama', 'Papua New Guinea', 'Paraguay', 'Peru', 'Philippines', 'Poland', 'Portugal', 'Qatar', 'Romania', 'Russian Federation', 'Rwanda', 'St Kitts & Nevis', 'St Lucia', 'Saint Vincent & the Grenadines', 'Samoa', 'San Marino',
             'Sao Tome & Principe', 'Saudi Arabia', 'Senegal', 'Serbia', 'Seychelles', 'Sierra Leone', 'Singapore', 'Slovakia', 'Slovenia', 'Solomon Islands', 'Somalia', 'South Africa', 'South Sudan', 'Spain', 'Sri Lanka', 'Sudan', 'Suriname', 'Swaziland', 'Sweden', 'Switzerland',
             'Syria', 'Taiwan', 'Tajikistan', 'Tanzania', 'Thailand', 'Togo', 'Tonga', 'Trinidad & Tobago', 'Tunisia', 'Turkey', 'Turkmenistan', 'Tuvalu', 'Uganda', 'Ukraine', 'United Arab Emirates', 'United Kingdom', 'United States', 'Uruguay', 'Uzbekistan', 'Vanuatu', 'Vatican City',
             'Venezuela', 'Vietnam', 'Yemen', 'Zambia', 'Zimbabwe'];

             $scope.createUniversity = function () {
                 $scope.display.createUniversity = true;
                 $scope.display.search = $scope.display.searchUniversity = $scope.display.facebook
                     = $scope.display.complete = $scope.display.choose = false;

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
