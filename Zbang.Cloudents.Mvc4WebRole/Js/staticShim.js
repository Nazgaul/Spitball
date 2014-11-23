(function () {
    "use strict";
    Node.prototype.hasClass = function (className) {
        if (this.classList) {
            return this.classList.contains(className);
        } else {
            return (-1 < this.className.indexOf(className));
        }
    };

    Node.prototype.addClass = function (obj) {
        switch (Object.prototype.toString.call(obj)) {
            case '[object String]':
                addClass.apply(this, [obj]);
                break;
            case '[object Array]':
                addClass.apply(this, [obj.join(' ')]);
                break;
            default:
                throw new Error('Invalid argument');
                break;
        }

        return this;

        function addClass(className) {
            if (this.classList) {
                if (className.indexOf(' ') > -1) {
                    var array = className.split(' ');
                    for (var i = 0, l = array.length; i < l; i++) {
                        this.addClass(array[i]);
                    }
                } else {
                    this.classList.add(className);
                }
            } else if (!this.hasClass(className)) {
                var classes = this.className.split(" ");
                classes.push(className);
                this.className = classes.join(" ");
            }
        }
    };

    Node.prototype.removeClass = function (obj) {
        switch (Object.prototype.toString.call(obj)) {
            case '[object String]':
                removeClass.apply(this, [obj]);
                break;
            case '[object Array]':
                removeClass.apply(this, [obj.join(' ')]);
                break;
            default:
                throw new Error('Invalid argument');
                break;
        }

        return this;

        function removeClass(className) {
            if (this.classList) {
                if (className.indexOf(' ') > -1) {
                    var array = className.split(' ');
                    for (var i = 0, l = array.length; i < l; i++) {
                        this.removeClass(array[i]);
                    }
                } else {
                    this.classList.remove(className);
                }
            } else {

                if (className.indexOf(' ') > -1) {
                    var array = className.split(' ');
                    for (var i = 0, l = array.length; i < l; i++) {
                        this.removeClass(array[i]);
                    }
                } else {
                    this.className = this.className.replace(className, '');
                }                
            }
        }   

    };

    Node.prototype.toggleClass = function (className) {
        if (this.classList) {
            this.classList.toggle(className);
        } else {
            if (!this.hasClass(className)) {
                this.addClass(className);
            }
            else {
                this.removeClass(className);
            }
        }
        return this;
    }
})();

(function () {

    "use strict";

    //shim for String's trim function..
    function trim(string) {
        return string.trim ? string.trim() : string.replace(/^\s+|\s+$/g, "");
    }

    //returns whether the given element has the given class name..
    function hasClassName(element, className) {
        //refactoring of Prototype's function..
        var elClassName = element.className;
        if (!elClassName)
            return false;
        var regex = new RegExp("(^|\\s)" + className + "(\\s|$)");
        return regex.test(element.className);
    }

    function removeClassName(element, className) {
        //refactoring of Prototype's function..
        var elClassName = element.className;
        if (!elClassName)
            return;
        element.className = elClassName.replace(
            new RegExp("(^|\\s+)" + className + "(\\s+|$)"), ' ');
    }

    function addClassName(element, className) {
        var elClassName = element.className;
        if (elClassName)
            element.className += " " + className;
        else
            element.className = className;
    }

    //strings to make event attachment x-browser.. 
    var addEvent = document.addEventListener;    
    //the class which is added when the placeholder is being used..
    var placeHolderClassName = 'usingPlaceHolder';

    //allows the given textField to use it's placeholder attribute
    //as if it's functionality is supported natively..
    window.placeHolder = function (textField) {

        //don't do anything if you get it for free..
        if ('placeholder' in document.createElement('input'))
            return;

        //don't do anything if the place holder attribute is not
        //defined or is blank..
        var placeHolder = textField.getAttribute('placeholder');
        if (!placeHolder)
            return;

        //if it's just the empty string do nothing..
        placeHolder = trim(placeHolder);
        if (placeHolder === '')
            return;

        //called on blur - sets the value to the place holder if it's empty..
        var onBlur = function () {
            if (textField.value !== '') //a space is a valid input..
                return;
            textField.value = placeHolder;
            addClassName(textField, placeHolderClassName);
        };

        //the blur event..
        textField[addEvent]('onblur', onBlur, false);

        //the focus event - removes the place holder if required..
        textField[addEvent]('onfocus', function () {
            if (hasClassName(textField, placeHolderClassName)) {
                removeClassName(textField, placeHolderClassName);
                textField.value = "";
            }
        }, false);

        //the submit event on the form to which it's associated - if the
        //placeholder is attached set the value to be empty..
        var form = textField.form;
        if (form) {
            form[addEvent]('onsubmit', function () {
                if (hasClassName(textField, placeHolderClassName))
                    textField.value = '';
            }, false);
        }

        onBlur(); //call the onBlur to set it initially..
    };

}());