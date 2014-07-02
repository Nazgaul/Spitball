define('trustedHtml',['app'], function (app) {
    app.filter('trustedHtml',
    ['$sce',
    function ($sce) {        
        return function (text) {
            return $sce.trustAsHtml(text);
        };
    }
    ]);
});