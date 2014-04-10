(function ($, cd, dataContext, pubsub, ZboxResources, analytics) {
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
            $('#quizCheckAnswers').click(function () {
                $('#quiz').addClass('checkQuiz');

                $('#quizTQuestion').children('li').each(function () {
                    var $this = $(this);
                    var answer = $this.find('input:checked');
                    if (!answer.length) {
                        $this.addClass('noAnswer userWrong').attr('data-no-answer', $('#quizTQuestion').attr('data-no-answer'));
                        return;
                    }
                    if (answer.parent().hasClass('correct')) {
                        $this.addClass('userCorrect');
                        return;
                    }
                    $this.addClass('userWrong');

                });
                $('#quizTQuestion').find('input').prop('disabled', 'disabled');
            });

        }
    }

})(jQuery, cd, cd.data, cd.pubsub, ZboxResources, cd.analytics);
