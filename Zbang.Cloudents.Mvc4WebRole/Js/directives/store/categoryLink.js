app.directive('categoryLink',
    [ '$location',
        function ($location) {
            var search = $location.search()['universityid'];
            return {
                restrict: "A",
                link: function (scope, elem, attrs) {
                    elem.find('[rel="cat"]').each(function () {                      
                        this.href = this.href + '?universityid=' + search;
                    });
                }
            };
        }
    ]);
