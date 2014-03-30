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

        $(saveQuiz).click(function () {

            parseQuiz();

            function parseQuiz() {
                var quizName = quizSideBar.querySelector('.quizName'),
                    questions = quizQuestionList.querySelectorAll('.questionHolder'),                    
                    errorElements = [];

                if (quizName.length > 0) {
                    errorElements.push(quizName);
                }

                for (var i = 0, l = questions.length; i < l; i++) {
                    checkValidQuestion(questions[i]);
                }


                function checkValidQuestion(question) {                    
                    var isQuestionText,
                        answers = question.querySelectorAll('.quizAnswer'),
                        answersArr = [];
                    
                    //check question text 
                    isQuestionText = question.querySelector('.questionText').value.length > 0;         

                    //get question answers
                    for (var i = 0, l = answers.length; i < l; i++) {
                        var answer = answers[i].querySelector('.questionAnswer');
                        if (answer.value.length > 0) {
                            answersArr.push(answer);
                        }
                    }

                    //check if one answer is selected
                    var checked = false;
                    for (var i = 0, l = answersArr.length; i < l && !checked; i++) {
                        if (answersArr[i].nextElementSibling.checked) {
                            checked = true;
                        }
                    }
                    
                    if (answers)
                }
            }

        });
    }
    registerEvents();

})(jQuery, window.cd, window.cd.data, cd.pubsub, window.ZboxResources, window.cd.analytics, Modernizr);