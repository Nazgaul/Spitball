
(function () {
    angular.module('app').directive('showForm', showForm);

    function showForm() {
        return {
            restrict: "A",
            link: function (scope, element) {
                
                element.click(function () {
                    $('body').find('.add-reply').hide();
                    scope.$parent.f.openForm = false;
                    scope.$parent.f.add.newText = '';
                    element.parents('.comment').find('.add-reply').show().find('textarea').focus();
                    scope.$apply();
                });
            }
        };
    }
})()