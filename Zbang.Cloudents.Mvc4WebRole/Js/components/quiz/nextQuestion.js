'use strict';
//TODO: unused
(function () {
    angular.module('app').directive('nextQuestion',
        function () {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    var id;

                    element[0].addEventListener('focus', function (e) {
                        var elem = $(e.target).parents('.portlet'),
                        tempId = elem.attr('id');
                        if (!id) {
                            id = tempId;
                            return;
                        }
                        if (id !== tempId) {

                            var q = scope.q.questions.filter(function (obj) {
                                return obj.$$hashKey == id;
                            });

                            element.find('.error').removeClass('error');
                            var questionElem = $(document.getElementById(id));
                            if (!q[0].text) {
                                questionElem.find('.ta-root').addClass('error').find('[contenteditable="true"]').focus();
                                return;
                            }
                           



                            var elements = questionElem.find('.ng-invalid');
                            if (elements.length) {
                                elements.parents('.answer').addClass('error');
                                elements[0].focus();
                                return;
                            }
                            if (!q[0].correctAnswer) {
                                questionElem.find('.correct-answer-error').addClass('error');
                                questionElem.find('input[type=radio]:first').focus();
                                return;
                            }
                            scope.$broadcast('question-ok', id);
                            id = tempId;
                        }
                    }, true);

                }
            };
        }
    );
})();