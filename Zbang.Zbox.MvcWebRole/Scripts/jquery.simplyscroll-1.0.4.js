/*
* simplyScroll 1.0.4 - a scroll-tastic jQuery plugin
*
* http://logicbox.net/jquery/simplyscroll
* http://logicbox.net/blog/simplyscroll-jquery-plugin
* http://plugins.jquery.com/project/simplyScroll
*
* Copyright (c) 2009 Will Kelly - http://logicbox.net
*
* Dual licensed under the MIT (MIT-LICENSE.txt)
* and GPL (GPL-LICENSE.txt) licenses.
*
* Last revised: 03/07/2009 21:13
*
*/

(function ($) {

    $.fn.simplyScroll = function (o) {
        return this.each(function () {
            new $.simplyScroll(this, o);
        });
    };




    var defaults = {
        className: 'simply-scroll',
        frameRate: 24, //No of movements per second
        speed: 1, //No of pixels per frame
        horizontal: true,
        autoMode: 'off', //disables buttons 'loop','bounce'
        pauseOnHover: true,
        startOnLoad: false, //use this if having rendering problems (safari 3 + Mac OSX?)
        localJsonSource: '', //format [{"src":"images/pic.jpg","title":"title","link":"http://"},{etc..}]
        flickrFeed: '',
        jsonImgWidth: 240,
        jsonImgHeight: 180,
        height: 150
    };

    $.simplyScroll = function (el, o) {
        var self = this;
        this.o = $.extend({}, defaults, o || {});
        this.auto = this.o.autoMode !== "off" ? true : false;

        //called on ul/ol/div etc
        this.$list = $(el);

        //generate extra markup
        this.$list.addClass('simply-scroll-list')
		.wrap('<div class="simply-scroll-clip"></div>')
		.parent().wrap('<div class="' + this.o.className + ' simply-scroll-container"></div>');

        if (!this.o.auto) { //button placeholders
            this.$list.parent().parent()
		.prepend('<div class="simply-scroll-forward"></div>')
		.prepend('<div class="simply-scroll-back"></div>');
        }

        //load image data
        if (this.o.flickrFeed) {
            $.getJSON(this.o.flickrFeed + "&format=json&jsoncallback=?",
			function (data) {
			    json = [];
			    $.each(data.items, function (i, item) {
			        json.push({
			            "src": item.media.m,
			            "title": item.title,
			            "link": item.link
			        });
			    });
			    self.renderData(json);
			}
		);
        } else if (this.o.localJsonSource) {
            $.getJSON(this.o.localJsonSource,
			function (json) {
			    self.renderData(json);
			}
		);
        } else {

            if (!this.o.startOnLoad) {
                this.init();
            } else {
                //wait for load before completing setup
                $(window).load(function () { self.init(); });
            }

        }

    };

    $.simplyScroll.fn = $.simplyScroll.prototype = {};

    $.simplyScroll.fn.extend = $.simplyScroll.extend = $.extend;

    $.simplyScroll.fn.extend({
        pos: 0,
        posMaxfunc: function (posMax) {
            $.simplyScroll.fn.pos = posMax;
        },
        clipMaximum: 0,
        clipMaxfunc: function (clipMax) {
            $.simplyScroll.fn.clipMaximum = clipMax;
        },
        init: function () {
            //shortcuts
            this.$items = this.$list.children();
            this.$clip = this.$list.parent();
            this.$container = this.$clip.parent();

            if (!this.o.horizontal) {
                this.itemMax = this.$items.eq(0).outerHeight(true);
                this.clipMax = this.$clip.height();
                this.dimension = 'height';
                this.moveBackClass = 'simply-scroll-btn-up';
                this.moveForwardClass = 'simply-scroll-btn-down';
            } else {
                this.itemMax = this.$items.eq(0).outerWidth(true);
                this.clipMax = this.$clip.width();
                this.dimension = 'width';
                this.moveBackClass = 'simply-scroll-btn-left';
                this.moveForwardClass = 'simply-scroll-btn-right';
                this.$clip.css('height', this.o.height + 'px');
            }

            this.posMin = 0;

            /* 
            IMPORTANT: The script assumes multiple elements within a list are the same width or height 
            to work out how many extra elements to generate to simulate the loop. 
		
            If you want this script to work with unequal sized elements don't modify the next line 
            to do this:
		
            this.$items.each(function() {
            self.posMax += !this.o.horizontal ? $(this).outerHeight(true) : $(this).outerWidth(true);
            });
		
            as it will cause white-space and a jump to appear when elements have wildly different dimensions
            See: http://logicbox.net/jquery/simplyscroll/test_unequalelements.html
		
            Instead simply add an extra wrapper element around your list and init simplyScroll on that, 
            essentially scrolling just one element. Less efficient but it does the job!
		
            */
            this.posMax = this.$items.length * this.itemMax;

            this.$list.css(this.dimension, this.posMax + 'px');

            if (this.o.autoMode == 'loop') {
                var addItems = Math.ceil(this.clipMax / this.itemMax);
                this.$items.slice(0, addItems).clone(true).appendTo(this.$list);
                this.posMax += (this.clipMax - this.o.speed);
                this.$list.css(this.dimension, this.posMax + (this.itemMax * addItems) + 'px');

            }

            this.interval = null;
            this.intervalDelay = Math.floor(1000 / this.o.frameRate);

            //ensure that speed is divisible by item width
            while (this.itemMax % this.o.speed !== 0) {
                this.o.speed--;
                if (this.o.speed === 0) {
                    this.o.speed = 1; break;
                }
            }

            var self = this;
            this.trigger = null;

            self.posMaxfunc(this.posMax);
            self.clipMaxfunc(this.clipMax);



            this.funcMoveBack = function () { self.trigger = this; self.moveBack(); };
            this.funcMoveForward = function () { self.trigger = this; self.moveForward(); };
            this.funcMoveForwardOnce = function () { self.trigger = this; self.moveForwardOnce(); };
            this.funcMoveBackOnce = function () { self.trigger = this; self.moveBackOnce(); };
            this.funcMoveStop = function () { self.moveStop(); };
            this.funcMoveResume = function () { self.moveResume(); };
            this.funcResize = function () {
                self.trigger = this;
                self.resizeContainer();
            };
            this.funcElementChanged = function () { self.trigger = this; self.ElementChanged(); };

            if (this.auto) {
                if (this.o.pauseOnHover) {
                    this.$clip.hover(this.funcMoveStop, this.funcMoveResume);
                }
                this.moveForward();
            } else {
                this.$btnBack = $('.simply-scroll-back', this.$container)
				.addClass('simply-scroll-btn' + ' ' + this.moveBackClass + ' ' + 'disabled')
				.click(this.funcMoveBackOnce);
                this.$btnForward = $('.simply-scroll-forward', this.$container)
				.addClass('simply-scroll-btn' + ' ' + this.moveForwardClass + ' ' + 'disabled')
				.click(this.funcMoveForwardOnce);
            }
            this.$container.bind('resizeElement', this.funcResize);
            this.$container.bind('elementChanged', this.funcElementChanged);
        },
        resizeContainer: function () {

            var self = this;
            this.clipMax = this.$clip.width();
            if (this.posMax > this.clipMax) {

                this.$btnForward.removeClass('disabled');
                this.clipMax = this.$container.width();
            }
            else {

                this.$btnForward.addClass('disabled');
            }
        },
        ElementChanged: function () {
            var self = this;
            //this.$list = $(el);
            self.$items = self.$list.children();
            self.itemMax = self.$items.eq(0).outerWidth(true);
            self.posMax = self.$items.length * self.itemMax;
            self.dimension = 'width';
            self.$list.css(self.dimension, self.posMax + 'px');

            //this.pos = this.posMax;
            //self.pos = this.posMax;
            self.resizeContainer();
        },
        moveForward: function () {

            var self = this;
            this.movement = 'forward';
            if (this.trigger !== null) {
                this.$btnBack.removeClass('disabled');
            }
            self.interval = setInterval(function () {
                if (!self.o.horizontal && self.$clip[0].scrollTop < (self.posMax - self.clipMax)) {
                    self.$clip[0].scrollTop += self.o.speed;
                } else if (self.o.horizontal && self.$clip[0].scrollLeft < (self.posMax - self.clipMax)) {
                    self.$clip[0].scrollLeft += self.o.speed;
                } else if (self.o.autoMode == 'loop') {
                    self.resetPos();
                } else {
                    self.moveStop(self.movement);
                }
            }, self.intervalDelay);
        },
        moveForwardOnce: function () {
            var self = this;
            this.movement = 'forward';
            if (!this.$btnForward.hasClass('disabled')) {
                if (this.trigger !== null) {
                    this.$btnBack.removeClass('disabled');
                }
                if (self.o.horizontal && self.$clip[0].scrollLeft < (self.posMax - self.clipMax)) {
                    self.$clip[0].scrollLeft += self.clipMaximum;
                }

                if (self.$clip[0].scrollLeft >= (self.posMax - self.clipMax)) {
                    self.moveStop(self.movement);
                }
            }
        },

        moveBackOnce: function () {
            var self = this;
            this.movement = 'back';
            if (!this.$btnBack.hasClass('disabled')) {
                if (this.trigger !== null) {
                    this.$btnForward.removeClass('disabled');
                }

                if (self.o.horizontal && self.$clip[0].scrollLeft > 0) {
                    self.$clip[0].scrollLeft -= self.clipMax;
                }
                if (self.o.horizontal && self.$clip[0].scrollLeft <= 0) {
                    self.moveStop(self.movement);
                }
            }

        },
        moveBack: function () {
            var self = this;
            this.movement = 'back';
            if (this.trigger !== null) {
                this.$btnForward.removeClass('disabled');
            }
            self.interval = setInterval(function () {
                if (!self.o.horizontal && self.$clip[0].scrollTop > 0) {
                    self.$clip[0].scrollTop -= self.o.speed;
                } else if (self.o.horizontal && self.$clip[0].scrollLeft > 0) {
                    self.$clip[0].scrollLeft -= self.o.speed;
                } else if (self.o.autoMode == 'loop') {
                    self.resetPos();
                } else {
                    self.moveStop(self.movement);
                }
            }, self.intervalDelay);
        },
        moveStop: function (moveDir) {
            clearInterval(this.interval);
            if (this.trigger !== null) {
                if (typeof moveDir != "undefined") {
                    $(this.trigger).addClass('disabled');
                }
                this.trigger = null;
            }
            if (this.auto) {
                if (this.o.autoMode == 'bounce') {
                    moveDir == 'forward' ? this.moveBack() : this.moveForward();
                }
            }
        },
        moveResume: function () {
            this.movement == 'forward' ? this.moveForward() : this.moveBack();
        },
        resetPos: function () {
            if (!this.o.horizontal) {
                this.$clip[0].scrollTop = 0;
            } else {
                this.$clip[0].scrollLeft = 0;
            }
        },
        renderData: function (json) {
            if (json.length > 0) { //render json data
                var self = this;
                $.each(json, function (i, item) {
                    $("<img/>").attr({
                        src: item.src,
                        title: item.title,
                        alt: item.title,
                        width: self.o.jsonImgWidth,
                        height: self.o.jsonImgHeight
                    }).appendTo(self.$list);
                });
                this.init();
            }
        }
    });

})(jQuery);