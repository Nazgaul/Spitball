(function () {
    angular.module('app').config(config);

    config.$inject = ['$ocLazyLoadProvider'];

    function config($ocLazyLoadProvider) {
        $ocLazyLoadProvider.config({
            modules: [{
                name: 'quizCreate',
                serie: true,
                files: [
                   {0}
                ]
            }]
        });
    }
})();