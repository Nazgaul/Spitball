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
                var classes = this.className.split(" ");
                classes.splice(classes.indexOf(className), 1);
                this.className = classes.join(" ");
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