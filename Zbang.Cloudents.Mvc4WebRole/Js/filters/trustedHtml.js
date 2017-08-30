
app.filter('trustedHtml',
['$sce',
function ($sce) {
    "use strict";
    return function (text) {
        return $sce.trustAsHtml(text);
    };
}
]);
