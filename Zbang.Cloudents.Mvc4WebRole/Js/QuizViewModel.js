(function ($, cd, dataContext,pubsub, ZboxResources, analytics) {
    "use strict";

    if (window.scriptLoaded.isLoaded('qvm')) {
        return;
    }

    var eById = document.getElementById.bind(document);

    cd.loadModel('quiz', 'QuizContext', QuizViewModel);

    function QuizViewModel() {

        cd.pubsub.subscribe('quiz', function (data) {
            
            initQuiz();
            registerEvents();
            pubsub.publish('quiz_load');
        });

        function initQuiz() {

        }

        function registerEvents() {
            
        }
    }

})(jQuery,cd, cd.data, cd.pubsub, ZboxResources,cd.analytics);
