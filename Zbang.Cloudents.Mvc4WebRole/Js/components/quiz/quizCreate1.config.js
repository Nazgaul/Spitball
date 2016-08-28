(function () {
    angular.module('app').config(config);

    config.$inject = ['$ocLazyLoadProvider'];

    function config($ocLazyLoadProvider) {
        $ocLazyLoadProvider.config({
            modules: [{
                name: 'quizCreate',
                serie: true,
                files: [
                   '/bower_components/textAngular/dist/textAngular-rangy.min.js','/bower_components/textAngular/dist/textAngular.js','/scripts/textAngularSetup.js','/js/components/quiz/quizCreate.module.js','/js/components/quiz/quizCreate2.controller.js'
                ]
            }]
        });
    }
})();