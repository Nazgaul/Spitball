(function ($, cd, dataContext, pubsub, ZboxResources, analytics, Modernizr) {
    "use strict";

    if (window.scriptLoaded.isLoaded('quiz')) {
        return;
    }
    var eById = document.getElementById.bind(document),
        sideBar = eById('quizSideBar'),
        quizQuestionList = eById('quizQuestionList'),
        quizAddQuestion = eById('quizAddQuestion'),
        saveBtn = eById('saveQuiz');

    pubsub.subscribe('initQuiz', function () {

    });

    function showQuiz() {
        //show the quiz div

    }



    function registerEvents() {
        $(quizQuestionList).on('keyup', '.questionHolder', function (e) {
            var question = this;

            if (e.target.classList.contains('questionAnswer')) {
                enableNextOption(e.target);
                return;
            }


                
            function enableNextOption(answer) {
                var nextAnswer = answer.parentElement.nextElementSibling;

                if (nextAnswer && answer.value.length) {
                    nextAnswer.firstElementChild.disabled = nextAnswer.lastElementChild.disabled = false;
                }
            }


        });

        $(quizQuestionList).on('click', '.quizRemoveQuestion', function () {
            $(this).parent().remove();
        });
        $(quizAddQuestion).click(function () {
            quizQuestionList.insertAdjacentHTML('beforeend', eById('quizQuestionTemplate').innerHTML);
        });
    }
    registerEvents();

})(jQuery, window.cd, window.cd.data, cd.pubsub, window.ZboxResources, window.cd.analytics, Modernizr);