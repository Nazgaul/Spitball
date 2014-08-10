app.directive('contenteditable', function () {
    return {
        scope: {
            editCallback: '&editCallback'
        },
        link: function (scope, elem, attrs, ctrl) {
            var text;

            elem.on('focus', function () {
                text = elem.text();
                var range = document.createRange();
                var sel = window.getSelection();
                range.setStart(elem[0], 1);
                range.collapse(true);
                sel.removeAllRanges();
                sel.addRange(range);
            });
            elem.on('blur', save);

            elem.on('keydown', function (e) {

                var keyCode = e.keyCode
                if (keyCode === 13) {
                    e.preventDefault();
                    save();
                    elem[0].contentEditable = false;

                    return;
                }
                if (keyCode === 27) {
                    e.preventDefault();
                    scope.$apply(function () {
                        elem.text(text);
                    });
                    elem.blur();
                    elem[0].contentEditable = false;
                    return;
                }
            });


            function save(e) {
                elem[0].contentEditable = false;

                var editedText = elem.text();
                if (text === editedText) {
                    return;
                }

                if (!editedText.length) {
                    scope.$apply(function () {
                        elem.text(text);
                    });
                    return;
                }
                text = elem.text();
                scope.editCallback({ t: text });

            }
        }
    };
}).
directive('contenteditableTrigger', function () {
    return {
        link: function (scope, elem, attrs) {
            elem.on('click', function () {
                var target = $('#' + attrs.contenteditableTrigger)[0];
                target.contentEditable = true;
                target.focus();
            });

        }
    };
});