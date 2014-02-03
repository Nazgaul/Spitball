//knockout new binding
ko.bindingHandlers.notvisible = {
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var isCurrentlyNotVisible = (element.style.display === "none");
        if (value && !isCurrentlyNotVisible) {
            element.style.display = "none";
        }
        else if ((!value) && isCurrentlyNotVisible) {
            element.style.display = "";
        }
    }
};
ko.bindingHandlers.stopBinding = {
    init: function () {
        return { controlsDescendantBindings: true };
    }
};
ko.virtualElements.allowedBindings.stopBinding = true;
$.validator.unobtrusive.parseDynamicContent = function (selector) {
    //use the normal unobstrusive.parse method
    $.validator.unobtrusive.parse(selector);

    //get the relevant form
    var form = $(selector).first().closest('form');

    //get the collections of unobstrusive validators, and jquery validators
    //and compare the two
    var unobtrusiveValidation = form.data('unobtrusiveValidation');
    var validator = form.validate();

    $.each(unobtrusiveValidation.options.rules, function (elname, elrules) {
        if (validator.settings.rules[elname] == undefined) {
            var args = {};
            $.extend(args, elrules);
            args.messages = unobtrusiveValidation.options.messages[elname];
            //edit:use quoted strings for the name selector
            $("[name='" + elname + "']").rules("add", args);
        } else {
            $.each(elrules, function (rulename, data) {
                if (validator.settings.rules[elname][rulename] == undefined) {
                    var args = {};
                    args[rulename] = data;
                    args.messages = unobtrusiveValidation.options.messages[elname][rulename];
                    //edit:use quoted strings for the name selector
                    $("[name='" + elname + "']").rules("add", args);
                }
            });
        }
    });
}



// Hooks up a form to jQuery Validation
ko.bindingHandlers.validate = {
    init: function (elem, valueAccessor) {
        $(elem).validate();
    }
};

// Controls whether or not the text in a textbox is selected based on a model property
ko.bindingHandlers.selected = {
    init: function (elem, valueAccessor) {
        $(elem).blur(function () {
            var boundProperty = valueAccessor();
            if (ko.isWriteableObservable(boundProperty)) {
                boundProperty(false);
            }
        });
    },
    update: function (elem, valueAccessor) {
        var shouldBeSelected = ko.utils.unwrapObservable(valueAccessor());
        if (shouldBeSelected) {
            $(elem).select();
        }
    }
};

// Makes a textbox lose focus if you press "enter"
ko.bindingHandlers.blurOnEnter = {
    init: function (elem, valueAccessor) {
        $(elem).keypress(function (evt) {
            if (evt.keyCode === 13 /* enter */) {
                evt.preventDefault();
                $(elem).triggerHandler("change");
                $(elem).blur();
            }
        });
    }
};

// Simulates HTML5-style placeholders on older browsers
ko.bindingHandlers.placeholder = {
    init: function (elem, valueAccessor) {
        var placeholderText = ko.utils.unwrapObservable(valueAccessor()),
            input = $(elem);

        input.attr('placeholder', placeholderText);

        // For older browsers, manually implement placeholder behaviors
        if (!Modernizr.input.placeholder) {
            input.focus(function () {
                if (input.val() === placeholderText) {
                    input.val('');
                    input.removeClass('placeholder');
                }
            }).blur(function () {
                setTimeout(function () {
                    if (input.val() === '' || input.val() === placeholderText) {
                        input.addClass('placeholder');
                        input.val(placeholderText);
                    }
                }, 0);
            }).blur();

            input.parents('form').submit(function () {
                if (input.val() === placeholderText) {
                    input.val('');
                }
            });
        }
    }
};