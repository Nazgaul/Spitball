(function (ko, $) {

    if (window.scriptLoaded.isLoaded('b')) {
        return;
    }

    /* ---- Begin integration of Underscore template engine with Knockout. Could go in a separate file of course. ---- */
    ko.cdTemplateEngine = function () {
        this['allowTemplateRewriting'] = false;
        this['cache'] = {};
        this['string'] = '';
    }
    ko.cdTemplateEngine.prototype = ko.utils.extend(new ko.templateEngine(), {
        renderTemplateSource: function (templateSource, bindingContext, options) {
            console.log(bindingContext);
           // console.log(templateSource);
           // console.log(bindingContext, options);

            var
        templateNodesFunc = templateSource['nodes'],
        templateNodes = templateNodesFunc ? templateSource['nodes']() : null;

            if (templateNodes) {
                var x = ko.utils.makeArray(templateNodes.cloneNode(true).childNodes);
                console.log(x)
                return x;
            } else {
                var templateText = templateSource['text']();
                this.string += templateText;
                if (bindingContext.$index() === 5) {
                    var z = ko.utils.parseHtmlFragment(this.string);
                    this.string = '';
                    return z;
                }
                return ko.utils.parseHtmlFragment('');

            }


            // Precompile and cache the templates for efficiency
            //var precompiled = templateSource['data']('precompiled');
            //if (!precompiled) {
            //    precompiled = _.template("<% with($data) { %> " + templateSource.text() + " <% } %>");
            //    templateSource['data']('precompiled', precompiled);
            //}
            //// Run the template and parse its output into an array of DOM elements
            //var renderedMarkup = precompiled(bindingContext).replace(/\s+/g, " ");
            

        }//,
        //createJavaScriptEvaluatorBlock: function (script) {
        //    //console.log(script);
        //    return "<%= " + script + " %>";
        //}
    });

    //ko.setTemplateEngine(new ko.cdTemplateEngine());
    /* ---- End integration of Underscore template engine with Knockout ---- */


    //knockout new binding
    //ko.bindingHandlers.notvisible = {
    //    update: function (element, valueAccessor) {
    //        var value = ko.utils.unwrapObservable(valueAccessor());
    //        var isCurrentlyNotVisible = (element.style.display === "none");
    //        if (value && !isCurrentlyNotVisible) {
    //            element.style.display = "none";
    //        }
    //        else if ((!value) && isCurrentlyNotVisible) {
    //            element.style.display = "";
    //        }
    //    }
    //};
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
    //ko.bindingHandlers.selected = {
    //    init: function (elem, valueAccessor) {
    //        $(elem).blur(function () {
    //            var boundProperty = valueAccessor();
    //            if (ko.isWriteableObservable(boundProperty)) {
    //                boundProperty(false);
    //            }
    //        });
    //    },
    //    update: function (elem, valueAccessor) {
    //        var shouldBeSelected = ko.utils.unwrapObservable(valueAccessor());
    //        if (shouldBeSelected) {
    //            $(elem).select();
    //        }
    //    }
    //};

    // Makes a textbox lose focus if you press "enter"
    //ko.bindingHandlers.blurOnEnter = {
    //    init: function (elem, valueAccessor) {
    //        $(elem).keypress(function (evt) {
    //            if (evt.keyCode === 13 /* enter */) {
    //                evt.preventDefault();
    //                $(elem).triggerHandler("change");
    //                $(elem).blur();
    //            }
    //        });
    //    }
    //};

    // Simulates HTML5-style placeholders on older browsers
    ko.bindingHandlers.placeholder = {
        init: function (elem, valueAccessor) {
            /// <summary></summary>
            /// <param name="elem" type="Element"></param>
            /// <param name="valueAccessor" type="Object"></param>

            if (!Modernizr.input.placeholder) {
                var placeholderText = elem.getAttribute('placeholder'),
                    input = $(elem);

                //input.attr('placeholder', placeholderText);

                // For older browsers, manually implement placeholder behaviors

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
    //sort
    //ko.observableArray.fn.sortByProperty = function (prop) {
    //    this.sort(function (obj1, obj2) {
    //        if (obj1[prop] == obj2[prop])
    //            return 0;
    //        else if (obj1[prop] < obj2[prop])
    //            return -1;
    //        else
    //            return 1;
    //    });
    //}

    //this is for contenteditable
    //ko.bindingHandlers.textEditbale = {
    //    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
    //        ko.utils.registerEventHandler(element, "blur", function () {
    //            var modelValue = valueAccessor();
    //            var elementValue = element.innerHTML;
    //            if (ko.isWriteableObservable(modelValue)) {
    //                modelValue(elementValue);
    //            }

    //        })
    //    },
    //    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
    //        var value = ko.utils.unwrapObservable(valueAccessor()) || "";
    //        element.innerHTML = value;
    //    }
    //};


    //ko.bindingHandlers.cssToTransition = {
    //    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
    //    },
    //    update: function (element, valueAccessor) {
    //        /// <summary></summary>
    //        /// <param name="element" type="Element"></param>
    //        /// <param name="valueAccessor" type="Object"></param>

    //        var value = ko.utils.unwrapObservable(valueAccessor());
    //        console.log(value);

    //        ko.utils.objectForEach(value, function (className, shouldHaveClass) {
    //            shouldHaveClass = ko.utils.unwrapObservable(shouldHaveClass);

    //            if (shouldHaveClass) {
    //                if ($(element).hasClass(className)) {
    //                    return;
    //                }
    //                $(element).css('display', '').addClass(className);
    //            }
    //            else {
    //                if (!$(element).hasClass(className)) {
    //                    return;
    //                }
    //                $(element).css('display', '').removeClass(className);
    //            }
    //        });

    //    }
    //};
    //ko.bindingHandlers.visiblefade = {
    //    'update': function (element, valueAccessor) {
    //        var value = ko.utils.unwrapObservable(valueAccessor());
    //        var isCurrentlyVisible = !(element.style.display == "none");
    //        if (value && !isCurrentlyVisible)
    //            $(element).fadeIn();
    //            //element.style.display = "";
    //        else if ((!value) && isCurrentlyVisible)
    //            $(element).fadeOut();
    //        //element.style.display = "none";
    //    }
    //};

    //wrapper to an observable that requires accept/cancel
    //ko.protectedObservable = function (initialValue) {
    //    //private variables
    //    var _actualValue = ko.observable(initialValue),
    //        _tempValue = initialValue;

    //    function protectedObservable() {
    //        _actualValue(arguments[0]);

    //    }
    //    //computed observable that we will return
    //    var result = ko.computed({
    //        //always return the actual value
    //        read: function () {
    //            return _actualValue();
    //        },
    //        //stored in a temporary spot until commit
    //        write: function (newValue) {
    //            _tempValue = newValue;
    //        }
    //    });

    //    //if different, commit temp value
    //    result.commit = function () {
    //        if (_tempValue !== _actualValue()) {
    //            _actualValue(_tempValue);
    //        }
    //    };

    //    //force subscribers to take original
    //    result.reset = function () {
    //        _actualValue.valueHasMutated();
    //        _tempValue = _actualValue();   //reset temp value
    //    };
    //    //ko.utils.extend(protectedObservable, ko.protectedObservable['fn']);
    //    return result;
    //};

    ko.bindingHandlers.fadeVisible = {
        init: function (element, valueAccessor) {
            // Initially set the element to be instantly visible/hidden depending on the value
            var value = valueAccessor();
            $(element).toggle(ko.utils.unwrapObservable(value)); // Use "unwrapObservable" so we can handle values that may or may not be observable
        },
        update: function (element, valueAccessor) {
            // Whenever the value subsequently changes, slowly fade the element in or out
            var value = ko.utils.unwrapObservable(valueAccessor());
            if (!value) {
                $(element).finish().hide();
                return;
            }
            $(element).finish().fadeIn();
        }
    };
    ko.bindingHandlers['attr2'] = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            ko.utils.objectForEach(value, function (attrName, attrValue) {
                //attrValue = ko.utils.unwrapObservable(attrValue);

                element[attrName] = attrValue;
            });
            //console.log('init')
            //console.log(element,valueAccessor,allBindings,viewModel,bindingContext)

        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            //   console.log('update')
            //console.log(element, valueAccessor, allBindings, viewModel, bindingContext)
        }
    };

})(ko, jQuery);

(function () {

    if (window.scriptLoaded.isLoaded('classList')) {
        return;
    }
    if (typeof document !== "undefined" && !("classList" in document.documentElement)) {

        (function (view) {

            "use strict";

            if (!('HTMLElement' in view) && !('Element' in view)) return;

            var
                  classListProp = "classList"
                , protoProp = "prototype"
                , elemCtrProto = (view.HTMLElement || view.Element)[protoProp]
                , objCtr = Object
                , strTrim = String[protoProp].trim || function () {
                    return this.replace(/^\s+|\s+$/g, "");
                }
                , arrIndexOf = Array[protoProp].indexOf || function (item) {
                    var
                          i = 0
                        , len = this.length
                    ;
                    for (; i < len; i++) {
                        if (i in this && this[i] === item) {
                            return i;
                        }
                    }
                    return -1;
                }
                // Vendors: please allow content code to instantiate DOMExceptions
                , DOMEx = function (type, message) {
                    this.name = type;
                    this.code = DOMException[type];
                    this.message = message;
                }
                , checkTokenAndGetIndex = function (classList, token) {
                    if (token === "") {
                        throw new DOMEx(
                              "SYNTAX_ERR"
                            , "An invalid or illegal string was specified"
                        );
                    }
                    if (/\s/.test(token)) {
                        throw new DOMEx(
                              "INVALID_CHARACTER_ERR"
                            , "String contains an invalid character"
                        );
                    }
                    return arrIndexOf.call(classList, token);
                }
                , ClassList = function (elem) {
                    var
                          trimmedClasses = strTrim.call(elem.className)
                        , classes = trimmedClasses ? trimmedClasses.split(/\s+/) : []
                        , i = 0
                        , len = classes.length
                    ;
                    for (; i < len; i++) {
                        this.push(classes[i]);
                    }
                    this._updateClassName = function () {
                        elem.className = this.toString();
                    };
                }
                , classListProto = ClassList[protoProp] = []
                , classListGetter = function () {
                    return new ClassList(this);
                }
            ;
            // Most DOMException implementations don't allow calling DOMException's toString()
            // on non-DOMExceptions. Error's toString() is sufficient here.
            DOMEx[protoProp] = Error[protoProp];
            classListProto.item = function (i) {
                return this[i] || null;
            };
            classListProto.contains = function (token) {
                token += "";
                return checkTokenAndGetIndex(this, token) !== -1;
            };
            classListProto.add = function () {
                var
                      tokens = arguments
                    , i = 0
                    , l = tokens.length
                    , token
                    , updated = false
                ;
                do {
                    token = tokens[i] + "";
                    if (checkTokenAndGetIndex(this, token) === -1) {
                        this.push(token);
                        updated = true;
                    }
                }
                while (++i < l);

                if (updated) {
                    this._updateClassName();
                }
            };
            classListProto.remove = function () {
                var
                      tokens = arguments
                    , i = 0
                    , l = tokens.length
                    , token
                    , updated = false
                ;
                do {
                    token = tokens[i] + "";
                    var index = checkTokenAndGetIndex(this, token);
                    if (index !== -1) {
                        this.splice(index, 1);
                        updated = true;
                    }
                }
                while (++i < l);

                if (updated) {
                    this._updateClassName();
                }
            };
            classListProto.toggle = function (token, forse) {
                token += "";

                var
                      result = this.contains(token)
                    , method = result ?
                        forse !== true && "remove"
                    :
                        forse !== false && "add"
                ;

                if (method) {
                    this[method](token);
                }

                return !result;
            };
            classListProto.toString = function () {
                return this.join(" ");
            };

            if (objCtr.defineProperty) {
                var classListPropDesc = {
                    get: classListGetter
                    , enumerable: true
                    , configurable: true
                };
                try {
                    objCtr.defineProperty(elemCtrProto, classListProp, classListPropDesc);
                } catch (ex) { // IE 8 doesn't support enumerable:true
                    if (ex.number === -0x7FF5EC54) {
                        classListPropDesc.enumerable = false;
                        objCtr.defineProperty(elemCtrProto, classListProp, classListPropDesc);
                    }
                }
            } else if (objCtr[protoProp].__defineGetter__) {
                elemCtrProto.__defineGetter__(classListProp, classListGetter);
            }

        }(self));

    }
}());