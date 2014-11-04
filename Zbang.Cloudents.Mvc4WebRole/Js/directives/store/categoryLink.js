
app.directive('categoryLink',
    [ '$location',
        function ($location) {
            "use strict";
            var search = $location.search()['universityid'] || $location.search()['universityId'];
            return {
                restrict: "A",
                link: function (scope, elem, attrs) {
                    if (search) {
                        elem.find('[rel="cat"]').each(function() {
                            this.href = this.href + '?universityid=' + search;
                        });
                    }
                }
            };
        }
    ]);
