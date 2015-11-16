(function () {
    angular.module('app').directive('firstLetter',
        function () {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    if (attrs.firstLetter) {
                        element.text(attrs.firstLetter[0].toUpperCase());
                    }
                }
            };
        }
    );
})();


(function () {
    angular.module('app').directive('nextQuestion',
        function () {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    //console.log(scope);
                    //console.log(attrs);

                    //element.focus(function() {
                    //    console.log('here');
                    //})
                    var id;
                    //console.log(id);
                    element[0].addEventListener('focus', function (e) {
                        var elem = $(e.target).parents('.portlet'),
                        tempId = elem.attr('id');
                        //console.log(e.target, attrs.nextQuestion, id);
                        if (!id) {
                            id = tempId;
                            return;
                        }
                        if (id !== tempId) {

                            var questionElem = $(document.getElementById(id));
                            element.find('.error').removeClass('error');
                            var questionElement = questionElem.find('[contenteditable="true"]');
                            if (!questionElement.text()) {
                                questionElement.parents('.ta-root').addClass('error');
                                questionElement.focus();
                                return;
                            }

                            var elements = questionElem.find('.ng-invalid');
                            if (elements.length) {
                                elements.parents('.answer').addClass('error');
                                elements[0].focus();
                                return;
                            }
                            scope.$broadcast('question-ok',id);
                            console.log('here');
                            id = tempId;
                        }
                    }, true);

                }
            };
        }
    );
})();

