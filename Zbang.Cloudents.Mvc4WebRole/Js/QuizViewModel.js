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

        function answerPerQuestion(data) {
            this.QuestionId = data.q;
            this.AnswerId = data.a;
        }
        function registerEvents() {
            $('#quizCheckAnswers').click(function () {
                $('#quiz').addClass('checkQuiz');

                var answerSheet = [];
                $('#quizTQuestion').children('li').each(function () {

                    var $this = $(this);
                    var answer = $this.find('input:checked');
                    answerSheet.push(new answerPerQuestion({ q: $this.attr('data-id'), a: answer.attr('data-id') }));
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
                var totalQuestion = $('#quizTQuestion').children('li').length,
                    rightAnswers = $('#quizTQuestion').children('li.userCorrect').length

                $('#quizTQuestion').find('input').prop('disabled', 'disabled');
                $('#quizUserScore').attr('data-user-score', Math.round(
                   rightAnswers / totalQuestion * 100));
                $('#quizUserRight').attr('data-results', rightAnswers);
                $('#quizUserWrong').attr('data-results', totalQuestion - rightAnswers);
                var startTime = new Date();
                startTime.setMinutes(-10);

                dataContext.quizSaveQuest({
                    data: {
                        StartTime: startTime,
                        EndTime: new Date(),
                        QuizId: cd.getParameterFromUrl(4),
                        Answers: answerSheet
                    },
                    error: function () { }

                });

            });
            $('#quizRetake').click(function () {
                $('#quiz').removeClass('checkQuiz');
                $('#quizTQuestion').find('input').removeAttr('disabled', 'disabled');
                
                $('#quizTQuestion').children('li').removeClass('noAnswer userWrong userCorrect');
            });

            $('#quizTimer').stopwatch();
        }


    }

})(jQuery, cd, cd.data, cd.pubsub, ZboxResources, cd.analytics);