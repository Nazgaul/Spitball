﻿/*!
 * imagesLoaded PACKAGED v3.1.8
 * JavaScript is all like "You images are done yet or what?"
 * MIT License
 */


/*!
 * EventEmitter v4.2.6 - git.io/ee
 * Oliver Caldwell
 * MIT license
 * @preserve
 */

(function () {


	/**
	 * Class for managing events.
	 * Can be extended to provide event functionality in other classes.
	 *
	 * @class EventEmitter Manages event registering and emitting.
	 */
	function EventEmitter() {}

	// Shortcuts to improve speed and size
	var proto = EventEmitter.prototype;
	var exports = this;
	var originalGlobalValue = exports.EventEmitter;

	/**
	 * Finds the index of the listener for the event in it's storage array.
	 *
	 * @param {Function[]} listeners Array of listeners to search through.
	 * @param {Function} listener Method to look for.
	 * @return {Number} Index of the specified listener, -1 if not found
	 * @api private
	 */
	function indexOfListener(listeners, listener) {
		var i = listeners.length;
		while (i--) {
			if (listeners[i].listener === listener) {
				return i;
			}
		}

		return -1;
	}

	/**
	 * Alias a method while keeping the context correct, to allow for overwriting of target method.
	 *
	 * @param {String} name The name of the target method.
	 * @return {Function} The aliased method
	 * @api private
	 */
	function alias(name) {
		return function aliasClosure() {
			return this[name].apply(this, arguments);
		};
	}

	/**
	 * Returns the listener array for the specified event.
	 * Will initialise the event object and listener arrays if required.
	 * Will return an object if you use a regex search. The object contains keys for each matched event. So /ba[rz]/ might return an object containing bar and baz. But only if you have either defined them with defineEvent or added some listeners to them.
	 * Each property in the object response is an array of listener functions.
	 *
	 * @param {String|RegExp} evt Name of the event to return the listeners from.
	 * @return {Function[]|Object} All listener functions for the event.
	 */
	proto.getListeners = function getListeners(evt) {
		var events = this._getEvents();
		var response;
		var key;

		// Return a concatenated array of all matching events if
		// the selector is a regular expression.
		if (typeof evt === 'object') {
			response = {};
			for (key in events) {
				if (events.hasOwnProperty(key) && evt.test(key)) {
					response[key] = events[key];
				}
			}
		}
		else {
			response = events[evt] || (events[evt] = []);
		}

		return response;
	};

	/**
	 * Takes a list of listener objects and flattens it into a list of listener functions.
	 *
	 * @param {Object[]} listeners Raw listener objects.
	 * @return {Function[]} Just the listener functions.
	 */
	proto.flattenListeners = function flattenListeners(listeners) {
		var flatListeners = [];
		var i;

		for (i = 0; i < listeners.length; i += 1) {
			flatListeners.push(listeners[i].listener);
		}

		return flatListeners;
	};

	/**
	 * Fetches the requested listeners via getListeners but will always return the results inside an object. This is mainly for internal use but others may find it useful.
	 *
	 * @param {String|RegExp} evt Name of the event to return the listeners from.
	 * @return {Object} All listener functions for an event in an object.
	 */
	proto.getListenersAsObject = function getListenersAsObject(evt) {
		var listeners = this.getListeners(evt);
		var response;

		if (listeners instanceof Array) {
			response = {};
			response[evt] = listeners;
		}

		return response || listeners;
	};

	/**
	 * Adds a listener function to the specified event.
	 * The listener will not be added if it is a duplicate.
	 * If the listener returns true then it will be removed after it is called.
	 * If you pass a regular expression as the event name then the listener will be added to all events that match it.
	 *
	 * @param {String|RegExp} evt Name of the event to attach the listener to.
	 * @param {Function} listener Method to be called when the event is emitted. If the function returns true then it will be removed after calling.
	 * @return {Object} Current instance of EventEmitter for chaining.
	 */
	proto.addListener = function addListener(evt, listener) {
		var listeners = this.getListenersAsObject(evt);
		var listenerIsWrapped = typeof listener === 'object';
		var key;

		for (key in listeners) {
			if (listeners.hasOwnProperty(key) && indexOfListener(listeners[key], listener) === -1) {
				listeners[key].push(listenerIsWrapped ? listener : {
					listener: listener,
					once: false
				});
			}
		}

		return this;
	};

	/**
	 * Alias of addListener
	 */
	proto.on = alias('addListener');

	/**
	 * Semi-alias of addListener. It will add a listener that will be
	 * automatically removed after it's first execution.
	 *
	 * @param {String|RegExp} evt Name of the event to attach the listener to.
	 * @param {Function} listener Method to be called when the event is emitted. If the function returns true then it will be removed after calling.
	 * @return {Object} Current instance of EventEmitter for chaining.
	 */
	proto.addOnceListener = function addOnceListener(evt, listener) {
		return this.addListener(evt, {
			listener: listener,
			once: true
		});
	};

	/**
	 * Alias of addOnceListener.
	 */
	proto.once = alias('addOnceListener');

	/**
	 * Defines an event name. This is required if you want to use a regex to add a listener to multiple events at once. If you don't do this then how do you expect it to know what event to add to? Should it just add to every possible match for a regex? No. That is scary and bad.
	 * You need to tell it what event names should be matched by a regex.
	 *
	 * @param {String} evt Name of the event to create.
	 * @return {Object} Current instance of EventEmitter for chaining.
	 */
	proto.defineEvent = function defineEvent(evt) {
		this.getListeners(evt);
		return this;
	};

	/**
	 * Uses defineEvent to define multiple events.
	 *
	 * @param {String[]} evts An array of event names to define.
	 * @return {Object} Current instance of EventEmitter for chaining.
	 */
	proto.defineEvents = function defineEvents(evts) {
		for (var i = 0; i < evts.length; i += 1) {
			this.defineEvent(evts[i]);
		}
		return this;
	};

	/**
	 * Removes a listener function from the specified event.
	 * When passed a regular expression as the event name, it will remove the listener from all events that match it.
	 *
	 * @param {String|RegExp} evt Name of the event to remove the listener from.
	 * @param {Function} listener Method to remove from the event.
	 * @return {Object} Current instance of EventEmitter for chaining.
	 */
	proto.removeListener = function removeListener(evt, listener) {
		var listeners = this.getListenersAsObject(evt);
		var index;
		var key;

		for (key in listeners) {
			if (listeners.hasOwnProperty(key)) {
				index = indexOfListener(listeners[key], listener);

				if (index !== -1) {
					listeners[key].splice(index, 1);
				}
			}
		}

		return this;
	};

	/**
	 * Alias of removeListener
	 */
	proto.off = alias('removeListener');

	/**
	 * Adds listeners in bulk using the manipulateListeners method.
	 * If you pass an object as the second argument you can add to multiple events at once. The object should contain key value pairs of events and listeners or listener arrays. You can also pass it an event name and an array of listeners to be added.
	 * You can also pass it a regular expression to add the array of listeners to all events that match it.
	 * Yeah, this function does quite a bit. That's probably a bad thing.
	 *
	 * @param {String|Object|RegExp} evt An event name if you will pass an array of listeners next. An object if you wish to add to multiple events at once.
	 * @param {Function[]} [listeners] An optional array of listener functions to add.
	 * @return {Object} Current instance of EventEmitter for chaining.
	 */
	proto.addListeners = function addListeners(evt, listeners) {
		// Pass through to manipulateListeners
		return this.manipulateListeners(false, evt, listeners);
	};

	/**
	 * Removes listeners in bulk using the manipulateListeners method.
	 * If you pass an object as the second argument you can remove from multiple events at once. The object should contain key value pairs of events and listeners or listener arrays.
	 * You can also pass it an event name and an array of listeners to be removed.
	 * You can also pass it a regular expression to remove the listeners from all events that match it.
	 *
	 * @param {String|Object|RegExp} evt An event name if you will pass an array of listeners next. An object if you wish to remove from multiple events at once.
	 * @param {Function[]} [listeners] An optional array of listener functions to remove.
	 * @return {Object} Current instance of EventEmitter for chaining.
	 */
	proto.removeListeners = function removeListeners(evt, listeners) {
		// Pass through to manipulateListeners
		return this.manipulateListeners(true, evt, listeners);
	};

	/**
	 * Edits listeners in bulk. The addListeners and removeListeners methods both use this to do their job. You should really use those instead, this is a little lower level.
	 * The first argument will determine if the listeners are removed (true) or added (false).
	 * If you pass an object as the second argument you can add/remove from multiple events at once. The object should contain key value pairs of events and listeners or listener arrays.
	 * You can also pass it an event name and an array of listeners to be added/removed.
	 * You can also pass it a regular expression to manipulate the listeners of all events that match it.
	 *
	 * @param {Boolean} remove True if you want to remove listeners, false if you want to add.
	 * @param {String|Object|RegExp} evt An event name if you will pass an array of listeners next. An object if you wish to add/remove from multiple events at once.
	 * @param {Function[]} [listeners] An optional array of listener functions to add/remove.
	 * @return {Object} Current instance of EventEmitter for chaining.
	 */
	proto.manipulateListeners = function manipulateListeners(remove, evt, listeners) {
		var i;
		var value;
		var single = remove ? this.removeListener : this.addListener;
		var multiple = remove ? this.removeListeners : this.addListeners;

		// If evt is an object then pass each of it's properties to this method
		if (typeof evt === 'object' && !(evt instanceof RegExp)) {
			for (i in evt) {
				if (evt.hasOwnProperty(i) && (value = evt[i])) {
					// Pass the single listener straight through to the singular method
					if (typeof value === 'function') {
						single.call(this, i, value);
					}
					else {
						// Otherwise pass back to the multiple function
						multiple.call(this, i, value);
					}
				}
			}
		}
		else {
			// So evt must be a string
			// And listeners must be an array of listeners
			// Loop over it and pass each one to the multiple method
			i = listeners.length;
			while (i--) {
				single.call(this, evt, listeners[i]);
			}
		}

		return this;
	};

	/**
	 * Removes all listeners from a specified event.
	 * If you do not specify an event then all listeners will be removed.
	 * That means every event will be emptied.
	 * You can also pass a regex to remove all events that match it.
	 *
	 * @param {String|RegExp} [evt] Optional name of the event to remove all listeners for. Will remove from every event if not passed.
	 * @return {Object} Current instance of EventEmitter for chaining.
	 */
	proto.removeEvent = function removeEvent(evt) {
		var type = typeof evt;
		var events = this._getEvents();
		var key;

		// Remove different things depending on the state of evt
		if (type === 'string') {
			// Remove all listeners for the specified event
			delete events[evt];
		}
		else if (type === 'object') {
			// Remove all events matching the regex.
			for (key in events) {
				if (events.hasOwnProperty(key) && evt.test(key)) {
					delete events[key];
				}
			}
		}
		else {
			// Remove all listeners in all events
			delete this._events;
		}

		return this;
	};

	/**
	 * Alias of removeEvent.
	 *
	 * Added to mirror the node API.
	 */
	proto.removeAllListeners = alias('removeEvent');

	/**
	 * Emits an event of your choice.
	 * When emitted, every listener attached to that event will be executed.
	 * If you pass the optional argument array then those arguments will be passed to every listener upon execution.
	 * Because it uses `apply`, your array of arguments will be passed as if you wrote them out separately.
	 * So they will not arrive within the array on the other side, they will be separate.
	 * You can also pass a regular expression to emit to all events that match it.
	 *
	 * @param {String|RegExp} evt Name of the event to emit and execute listeners for.
	 * @param {Array} [args] Optional array of arguments to be passed to each listener.
	 * @return {Object} Current instance of EventEmitter for chaining.
	 */
	proto.emitEvent = function emitEvent(evt, args) {
		var listeners = this.getListenersAsObject(evt);
		var listener;
		var i;
		var key;
		var response;

		for (key in listeners) {
			if (listeners.hasOwnProperty(key)) {
				i = listeners[key].length;

				while (i--) {
					// If the listener returns true then it shall be removed from the event
					// The function is executed either with a basic call or an apply if there is an args array
					listener = listeners[key][i];

					if (listener.once === true) {
						this.removeListener(evt, listener.listener);
					}

					response = listener.listener.apply(this, args || []);

					if (response === this._getOnceReturnValue()) {
						this.removeListener(evt, listener.listener);
					}
				}
			}
		}

		return this;
	};

	/**
	 * Alias of emitEvent
	 */
	proto.trigger = alias('emitEvent');

	/**
	 * Subtly different from emitEvent in that it will pass its arguments on to the listeners, as opposed to taking a single array of arguments to pass on.
	 * As with emitEvent, you can pass a regex in place of the event name to emit to all events that match it.
	 *
	 * @param {String|RegExp} evt Name of the event to emit and execute listeners for.
	 * @param {...*} Optional additional arguments to be passed to each listener.
	 * @return {Object} Current instance of EventEmitter for chaining.
	 */
	proto.emit = function emit(evt) {
		var args = Array.prototype.slice.call(arguments, 1);
		return this.emitEvent(evt, args);
	};

	/**
	 * Sets the current value to check against when executing listeners. If a
	 * listeners return value matches the one set here then it will be removed
	 * after execution. This value defaults to true.
	 *
	 * @param {*} value The new value to check for when executing listeners.
	 * @return {Object} Current instance of EventEmitter for chaining.
	 */
	proto.setOnceReturnValue = function setOnceReturnValue(value) {
		this._onceReturnValue = value;
		return this;
	};

	/**
	 * Fetches the current value to check against when executing listeners. If
	 * the listeners return value matches this one then it should be removed
	 * automatically. It will return true by default.
	 *
	 * @return {*|Boolean} The current value to check for or the default, true.
	 * @api private
	 */
	proto._getOnceReturnValue = function _getOnceReturnValue() {
		if (this.hasOwnProperty('_onceReturnValue')) {
			return this._onceReturnValue;
		}
		else {
			return true;
		}
	};

	/**
	 * Fetches the events object and creates one if required.
	 *
	 * @return {Object} The events storage object.
	 * @api private
	 */
	proto._getEvents = function _getEvents() {
		return this._events || (this._events = {});
	};

	/**
	 * Reverts the global {@link EventEmitter} to its previous value and returns a reference to this version.
	 *
	 * @return {Function} Non conflicting EventEmitter class.
	 */
	EventEmitter.noConflict = function noConflict() {
		exports.EventEmitter = originalGlobalValue;
		return EventEmitter;
	};

	// Expose the class either via AMD, CommonJS or the global object
	if (typeof define === 'function' && define.amd) {
		define('eventEmitter/EventEmitter',[],function () {
			return EventEmitter;
		});
	}
	else if (typeof module === 'object' && module.exports){
		module.exports = EventEmitter;
	}
	else {
		this.EventEmitter = EventEmitter;
	}
}.call(this));

/*!
 * eventie v1.0.4
 * event binding helper
 *   eventie.bind( elem, 'click', myFn )
 *   eventie.unbind( elem, 'click', myFn )
 */

/*jshint browser: true, undef: true, unused: true */
/*global define: false */

( function( window ) {



	var docElem = document.documentElement;

	var bind = function() {};

	function getIEEvent( obj ) {
		var event = window.event;
		// add event.target
		event.target = event.target || event.srcElement || obj;
		return event;
	}

	if ( docElem.addEventListener ) {
		bind = function( obj, type, fn ) {
			obj.addEventListener( type, fn, false );
		};
	} else if ( docElem.attachEvent ) {
		bind = function( obj, type, fn ) {
			obj[ type + fn ] = fn.handleEvent ?
				function() {
					var event = getIEEvent( obj );
					fn.handleEvent.call( fn, event );
				} :
				function() {
					var event = getIEEvent( obj );
					fn.call( obj, event );
				};
			obj.attachEvent( "on" + type, obj[ type + fn ] );
		};
	}

	var unbind = function() {};

	if ( docElem.removeEventListener ) {
		unbind = function( obj, type, fn ) {
			obj.removeEventListener( type, fn, false );
		};
	} else if ( docElem.detachEvent ) {
		unbind = function( obj, type, fn ) {
			obj.detachEvent( "on" + type, obj[ type + fn ] );
			try {
				delete obj[ type + fn ];
			} catch ( err ) {
				// can't delete window object properties
				obj[ type + fn ] = undefined;
			}
		};
	}

	var eventie = {
		bind: bind,
		unbind: unbind
	};

// transport
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( 'eventie/eventie',eventie );
	} else {
		// browser global
		window.eventie = eventie;
	}

})( this );

/*!
 * imagesLoaded v3.1.8
 * JavaScript is all like "You images are done yet or what?"
 * MIT License
 */

( function( window, factory ) {
	// universal module definition

	/*global define: false, module: false, require: false */

	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( [
			'eventEmitter/EventEmitter',
			'eventie/eventie'
		], function( EventEmitter, eventie ) {
			return factory( window, EventEmitter, eventie );
		});
	} else if ( typeof exports === 'object' ) {
		// CommonJS
		module.exports = factory(
			window,
			require('wolfy87-eventemitter'),
			require('eventie')
		);
	} else {
		// browser global
		window.imagesLoaded = factory(
			window,
			window.EventEmitter,
			window.eventie
		);
	}

})( window,

// --------------------------  factory -------------------------- //

	function factory( window, EventEmitter, eventie ) {



		var $ = window.jQuery;
		var console = window.console;
		var hasConsole = typeof console !== 'undefined';

// -------------------------- helpers -------------------------- //

// extend objects
		function extend( a, b ) {
			for ( var prop in b ) {
				a[ prop ] = b[ prop ];
			}
			return a;
		}

		var objToString = Object.prototype.toString;
		function isArray( obj ) {
			return objToString.call( obj ) === '[object Array]';
		}

// turn element or nodeList into an array
		function makeArray( obj ) {
			var ary = [];
			if ( isArray( obj ) ) {
				// use object if already an array
				ary = obj;
			} else if ( typeof obj.length === 'number' ) {
				// convert nodeList to array
				for ( var i=0, len = obj.length; i < len; i++ ) {
					ary.push( obj[i] );
				}
			} else {
				// array of single index
				ary.push( obj );
			}
			return ary;
		}

		// -------------------------- imagesLoaded -------------------------- //

		/**
		 * @param {Array, Element, NodeList, String} elem
		 * @param {Object or Function} options - if function, use as callback
		 * @param {Function} onAlways - callback function
		 */
		function ImagesLoaded( elem, options, onAlways ) {
			// coerce ImagesLoaded() without new, to be new ImagesLoaded()
			if ( !( this instanceof ImagesLoaded ) ) {
				return new ImagesLoaded( elem, options );
			}
			// use elem as selector string
			if ( typeof elem === 'string' ) {
				elem = document.querySelectorAll( elem );
			}

			this.elements = makeArray( elem );
			this.options = extend( {}, this.options );

			if ( typeof options === 'function' ) {
				onAlways = options;
			} else {
				extend( this.options, options );
			}

			if ( onAlways ) {
				this.on( 'always', onAlways );
			}

			this.getImages();

			if ( $ ) {
				// add jQuery Deferred object
				this.jqDeferred = new $.Deferred();
			}

			// HACK check async to allow time to bind listeners
			var _this = this;
			setTimeout( function() {
				_this.check();
			});
		}

		ImagesLoaded.prototype = new EventEmitter();

		ImagesLoaded.prototype.options = {};

		ImagesLoaded.prototype.getImages = function() {
			this.images = [];

			// filter & find items if we have an item selector
			for ( var i=0, len = this.elements.length; i < len; i++ ) {
				var elem = this.elements[i];
				// filter siblings
				if ( elem.nodeName === 'IMG' ) {
					this.addImage( elem );
				}
				// find children
				// no non-element nodes, #143
				var nodeType = elem.nodeType;
				if ( !nodeType || !( nodeType === 1 || nodeType === 9 || nodeType === 11 ) ) {
					continue;
				}
				var childElems = elem.querySelectorAll('img');
				// concat childElems to filterFound array
				for ( var j=0, jLen = childElems.length; j < jLen; j++ ) {
					var img = childElems[j];
					this.addImage( img );
				}
			}
		};

		/**
		 * @param {Image} img
		 */
		ImagesLoaded.prototype.addImage = function( img ) {
			var loadingImage = new LoadingImage( img );
			this.images.push( loadingImage );
		};

		ImagesLoaded.prototype.check = function() {
			var _this = this;
			var checkedCount = 0;
			var length = this.images.length;
			this.hasAnyBroken = false;
			// complete if no images
			if ( !length ) {
				this.complete();
				return;
			}

			function onConfirm( image, message ) {
				if ( _this.options.debug && hasConsole ) {
					console.log( 'confirm', image, message );
				}

				_this.progress( image );
				checkedCount++;
				if ( checkedCount === length ) {
					_this.complete();
				}
				return true; // bind once
			}

			for ( var i=0; i < length; i++ ) {
				var loadingImage = this.images[i];
				loadingImage.on( 'confirm', onConfirm );
				loadingImage.check();
			}
		};

		ImagesLoaded.prototype.progress = function( image ) {
			this.hasAnyBroken = this.hasAnyBroken || !image.isLoaded;
			// HACK - Chrome triggers event before object properties have changed. #83
			var _this = this;
			setTimeout( function() {
				_this.emit( 'progress', _this, image );
				if ( _this.jqDeferred && _this.jqDeferred.notify ) {
					_this.jqDeferred.notify( _this, image );
				}
			});
		};

		ImagesLoaded.prototype.complete = function() {
			var eventName = this.hasAnyBroken ? 'fail' : 'done';
			this.isComplete = true;
			var _this = this;
			// HACK - another setTimeout so that confirm happens after progress
			setTimeout( function() {
				_this.emit( eventName, _this );
				_this.emit( 'always', _this );
				if ( _this.jqDeferred ) {
					var jqMethod = _this.hasAnyBroken ? 'reject' : 'resolve';
					_this.jqDeferred[ jqMethod ]( _this );
				}
			});
		};

		// -------------------------- jquery -------------------------- //

		if ( $ ) {
			$.fn.imagesLoaded = function( options, callback ) {
				var instance = new ImagesLoaded( this, options, callback );
				return instance.jqDeferred.promise( $(this) );
			};
		}


		// --------------------------  -------------------------- //

		function LoadingImage( img ) {
			this.img = img;
		}

		LoadingImage.prototype = new EventEmitter();

		LoadingImage.prototype.check = function() {
			// first check cached any previous images that have same src
			var resource = cache[ this.img.src ] || new Resource( this.img.src );
			if ( resource.isConfirmed ) {
				this.confirm( resource.isLoaded, 'cached was confirmed' );
				return;
			}

			// If complete is true and browser supports natural sizes,
			// try to check for image status manually.
			if ( this.img.complete && this.img.naturalWidth !== undefined ) {
				// report based on naturalWidth
				this.confirm( this.img.naturalWidth !== 0, 'naturalWidth' );
				return;
			}

			// If none of the checks above matched, simulate loading on detached element.
			var _this = this;
			resource.on( 'confirm', function( resrc, message ) {
				_this.confirm( resrc.isLoaded, message );
				return true;
			});

			resource.check();
		};

		LoadingImage.prototype.confirm = function( isLoaded, message ) {
			this.isLoaded = isLoaded;
			this.emit( 'confirm', this, message );
		};

		// -------------------------- Resource -------------------------- //

		// Resource checks each src, only once
		// separate class from LoadingImage to prevent memory leaks. See #115

		var cache = {};

		function Resource( src ) {
			this.src = src;
			// add to cache
			cache[ src ] = this;
		}

		Resource.prototype = new EventEmitter();

		Resource.prototype.check = function() {
			// only trigger checking once
			if ( this.isChecked ) {
				return;
			}
			// simulate loading on detached element
			var proxyImage = new Image();
			eventie.bind( proxyImage, 'load', this );
			eventie.bind( proxyImage, 'error', this );
			proxyImage.src = this.src;
			// set flag
			this.isChecked = true;
		};

		// ----- events ----- //

		// trigger specified handler for event type
		Resource.prototype.handleEvent = function( event ) {
			var method = 'on' + event.type;
			if ( this[ method ] ) {
				this[ method ]( event );
			}
		};

		Resource.prototype.onload = function( event ) {
			this.confirm( true, 'onload' );
			this.unbindProxyEvents( event );
		};

		Resource.prototype.onerror = function( event ) {
			this.confirm( false, 'onerror' );
			this.unbindProxyEvents( event );
		};

		// ----- confirm ----- //

		Resource.prototype.confirm = function( isLoaded, message ) {
			this.isConfirmed = true;
			this.isLoaded = isLoaded;
			this.emit( 'confirm', this, message );
		};

		Resource.prototype.unbindProxyEvents = function( event ) {
			eventie.unbind( event.target, 'load', this );
			eventie.unbind( event.target, 'error', this );
		};

		// -----  ----- //

		return ImagesLoaded;

	});

/*!
 * fullPage 2.7.3
 * https://github.com/alvarotrigo/fullPage.js
 * @license MIT licensed
 *
 * Copyright (C) 2015 alvarotrigo.com - A project by Alvaro Trigo
 */
(function(global, factory) {
	'use strict';
	if (typeof define === 'function' && define.amd) {
		define(['jquery'], function($) {
			return factory($, global, global.document, global.Math);
		});
	} else if (typeof exports !== 'undefined') {
		module.exports = factory(require('jquery'), global, global.document, global.Math);
	} else {
		factory(jQuery, global, global.document, global.Math);
	}
})(typeof window !== 'undefined' ? window : this, function($, window, document, Math, undefined) {
	'use strict';

	// keeping central set of classnames and selectors
	var WRAPPER =               'fullpage-wrapper';
	var WRAPPER_SEL =           '.' + WRAPPER;

	// slimscroll
	var SCROLLABLE =            'fp-scrollable';
	var SCROLLABLE_SEL =        '.' + SCROLLABLE;
	var SLIMSCROLL_BAR_SEL =    '.slimScrollBar';
	var SLIMSCROLL_RAIL_SEL =   '.slimScrollRail';

	// util
	var RESPONSIVE =            'fp-responsive';
	var NO_TRANSITION =         'fp-notransition';
	var DESTROYED =             'fp-destroyed';
	var ENABLED =               'fp-enabled';
	var VIEWING_PREFIX =        'fp-viewing';
	var ACTIVE =                'active';
	var ACTIVE_SEL =            '.' + ACTIVE;

	// section
	var SECTION_DEFAULT_SEL =   '.section';
	var SECTION =               'fp-section';
	var SECTION_SEL =           '.' + SECTION;
	var SECTION_ACTIVE_SEL =    SECTION_SEL + ACTIVE_SEL;
	var SECTION_FIRST_SEL =     SECTION_SEL + ':first';
	var SECTION_LAST_SEL =      SECTION_SEL + ':last';
	var TABLE_CELL =            'fp-tableCell';
	var TABLE_CELL_SEL =        '.' + TABLE_CELL;
	var AUTO_HEIGHT =       'fp-auto-height';
	var AUTO_HEIGHT_SEL =   '.fp-auto-height';

	// section nav
	var SECTION_NAV =           'fp-nav';
	var SECTION_NAV_SEL =       '#' + SECTION_NAV;
	var SECTION_NAV_TOOLTIP =   'fp-tooltip';
	var SHOW_ACTIVE_TOOLTIP =   'fp-show-active';

	// slide
	var SLIDE_DEFAULT_SEL =     '.slide';
	var SLIDE =                 'fp-slide';
	var SLIDE_SEL =             '.' + SLIDE;
	var SLIDE_ACTIVE_SEL =      SLIDE_SEL + ACTIVE_SEL;
	var SLIDES_WRAPPER =        'fp-slides';
	var SLIDES_WRAPPER_SEL =    '.' + SLIDES_WRAPPER;
	var SLIDES_CONTAINER =      'fp-slidesContainer';
	var SLIDES_CONTAINER_SEL =  '.' + SLIDES_CONTAINER;
	var TABLE =                 'fp-table';

	// slide nav
	var SLIDES_NAV =            'fp-slidesNav';
	var SLIDES_NAV_SEL =        '.' + SLIDES_NAV;
	var SLIDES_NAV_LINK_SEL =   SLIDES_NAV_SEL + ' a';
	var SLIDES_ARROW =          'fp-controlArrow';
	var SLIDES_ARROW_SEL =      '.' + SLIDES_ARROW;
	var SLIDES_PREV =           'fp-prev';
	var SLIDES_PREV_SEL =       '.' + SLIDES_PREV;
	var SLIDES_ARROW_PREV =     SLIDES_ARROW + ' ' + SLIDES_PREV;
	var SLIDES_ARROW_PREV_SEL = SLIDES_ARROW_SEL + SLIDES_PREV_SEL;
	var SLIDES_NEXT =           'fp-next';
	var SLIDES_NEXT_SEL =       '.' + SLIDES_NEXT;
	var SLIDES_ARROW_NEXT =     SLIDES_ARROW + ' ' + SLIDES_NEXT;
	var SLIDES_ARROW_NEXT_SEL = SLIDES_ARROW_SEL + SLIDES_NEXT_SEL;

	var $window = $(window);
	var $document = $(document);

	$.fn.fullpage = function(options) {

		// common jQuery objects
		var $htmlBody = $('html, body');
		var $body = $('body');

		var FP = $.fn.fullpage;
		// Create some defaults, extending them with any options that were provided
		options = $.extend({
			//navigation
			menu: false,
			anchors:[],
			lockAnchors: false,
			navigation: false,
			navigationPosition: 'right',
			navigationTooltips: [],
			showActiveTooltip: false,
			slidesNavigation: false,
			slidesNavPosition: 'bottom',
			scrollBar: false,

			//scrolling
			css3: true,
			scrollingSpeed: 700,
			autoScrolling: true,
			fitToSection: true,
			fitToSectionDelay: 1000,
			easing: 'easeInOutCubic',
			easingcss3: 'ease',
			loopBottom: false,
			loopTop: false,
			loopHorizontal: true,
			continuousVertical: false,
			normalScrollElements: null,
			scrollOverflow: false,
			touchSensitivity: 5,
			normalScrollElementTouchThreshold: 5,

			//Accessibility
			keyboardScrolling: true,
			animateAnchor: true,
			recordHistory: true,

			//design
			controlArrows: true,
			controlArrowColor: '#fff',
			verticalCentered: true,
			resize: false,
			sectionsColor : [],
			paddingTop: 0,
			paddingBottom: 0,
			fixedElements: null,
			responsive: 0, //backwards compabitility with responsiveWiddth
			responsiveWidth: 0,
			responsiveHeight: 0,

			//Custom selectors
			sectionSelector: SECTION_DEFAULT_SEL,
			slideSelector: SLIDE_DEFAULT_SEL,


			//events
			afterLoad: null,
			onLeave: null,
			afterRender: null,
			afterResize: null,
			afterReBuild: null,
			afterSlideLoad: null,
			onSlideLeave: null
		}, options);

		displayWarnings();

		//easeInOutCubic animation included in the plugin
		$.extend($.easing,{ easeInOutCubic: function (x, t, b, c, d) {if ((t/=d/2) < 1) return c/2*t*t*t + b;return c/2*((t-=2)*t*t + 2) + b;}});

		//TO BE REMOVED in future versions. Maintained temporaly for backwards compatibility.
		$.extend($.easing,{ easeInQuart: function (x, t, b, c, d) { return c*(t/=d)*t*t*t + b; }});

		/**
		* Sets the autoScroll option.
		* It changes the scroll bar visibility and the history of the site as a result.
		*/
		FP.setAutoScrolling = function(value, type){
			setVariableState('autoScrolling', value, type);

			var element = $(SECTION_ACTIVE_SEL);

			if(options.autoScrolling && !options.scrollBar){
				$htmlBody.css({
					'overflow' : 'hidden',
					'height' : '100%'
				});

				FP.setRecordHistory(originals.recordHistory, 'internal');

				//for IE touch devices
				container.css({
					'-ms-touch-action': 'none',
					'touch-action': 'none'
				});

				if(element.length){
					//moving the container up
					silentScroll(element.position().top);
				}

			}else{
				$htmlBody.css({
					'overflow' : 'visible',
					'height' : 'initial'
				});

				FP.setRecordHistory(false, 'internal');

				//for IE touch devices
				container.css({
					'-ms-touch-action': '',
					'touch-action': ''
				});

				silentScroll(0);

				//scrolling the page to the section with no animation
				if (element.length) {
					$htmlBody.scrollTop(element.position().top);
				}
			}
		};

		/**
		* Defines wheter to record the history for each hash change in the URL.
		*/
		FP.setRecordHistory = function(value, type){
			setVariableState('recordHistory', value, type);
		};

		/**
		* Defines the scrolling speed
		*/
		FP.setScrollingSpeed = function(value, type){
			setVariableState('scrollingSpeed', value, type);
		};

		/**
		* Sets fitToSection
		*/
		FP.setFitToSection = function(value, type){
			setVariableState('fitToSection', value, type);
		};

		/**
		* Sets lockAnchors
		*/
		FP.setLockAnchors = function(value){
			options.lockAnchors = value;
		};

		/**
		* Adds or remove the possiblity of scrolling through sections by using the mouse wheel or the trackpad.
		*/
		FP.setMouseWheelScrolling = function (value){
			if(value){
				addMouseWheelHandler();
			}else{
				removeMouseWheelHandler();
			}
		};

		/**
		* Adds or remove the possiblity of scrolling through sections by using the mouse wheel/trackpad or touch gestures.
		* Optionally a second parameter can be used to specify the direction for which the action will be applied.
		*
		* @param directions string containing the direction or directions separated by comma.
		*/
		FP.setAllowScrolling = function (value, directions){
			if(typeof directions !== 'undefined'){
				directions = directions.replace(/ /g,'').split(',');

				$.each(directions, function (index, direction){
					setIsScrollAllowed(value, direction, 'm');
				});
			}
			else if(value){

				FP.setMouseWheelScrolling(true);
				addTouchHandler();
			}else{
				FP.setMouseWheelScrolling(false);
				removeTouchHandler();
			}
		};

		/**
		* Adds or remove the possiblity of scrolling through sections by using the keyboard arrow keys
		*/
		FP.setKeyboardScrolling = function (value, directions){
			if(typeof directions !== 'undefined'){
				directions = directions.replace(/ /g,'').split(',');

				$.each(directions, function (index, direction){
					setIsScrollAllowed(value, direction, 'k');
				});
			}else{
				options.keyboardScrolling = value;
			}
		};

		/**
		* Moves the page up one section.
		*/
		FP.moveSectionUp = function(){
			var prev = $(SECTION_ACTIVE_SEL).prev(SECTION_SEL);

			//looping to the bottom if there's no more sections above
			if (!prev.length && (options.loopTop || options.continuousVertical)) {
				prev = $(SECTION_SEL).last();
			}

			if (prev.length) {
				scrollPage(prev, null, true);
			}
		};

		/**
		* Moves the page down one section.
		*/
		FP.moveSectionDown = function (){
			var next = $(SECTION_ACTIVE_SEL).next(SECTION_SEL);

			//looping to the top if there's no more sections below
			if(!next.length &&
				(options.loopBottom || options.continuousVertical)){
				next = $(SECTION_SEL).first();
			}

			if(next.length){
				scrollPage(next, null, false);
			}
		};

		/**
		* Moves the page to the given section and slide with no animation.
		* Anchors or index positions can be used as params.
		*/
		FP.silentMoveTo = function(sectionAnchor, slideAnchor){
			requestAnimFrame(function(){
				FP.setScrollingSpeed (0, 'internal');
			});

			FP.moveTo(sectionAnchor, slideAnchor)

			requestAnimFrame(function(){
				FP.setScrollingSpeed (originals.scrollingSpeed, 'internal');
			});
		};

		/**
		* Moves the page to the given section and slide.
		* Anchors or index positions can be used as params.
		*/
		FP.moveTo = function (sectionAnchor, slideAnchor){
			var destiny = getSectionByAnchor(sectionAnchor);

			if (typeof slideAnchor !== 'undefined'){
				scrollPageAndSlide(sectionAnchor, slideAnchor);
			}else if(destiny.length > 0){
				scrollPage(destiny);
			}
		};

		/**
		* Slides right the slider of the active section.
		*/
		FP.moveSlideRight = function(){
			moveSlide('next');
		};

		/**
		* Slides left the slider of the active section.
		*/
		FP.moveSlideLeft = function(){
			moveSlide('prev');
		};

		/**
		 * When resizing is finished, we adjust the slides sizes and positions
		 */
		FP.reBuild = function(resizing){
			if(container.hasClass(DESTROYED)){ return; }  //nothing to do if the plugin was destroyed

			requestAnimFrame(function(){
				isResizing = true;
			});

			var windowsWidth = $window.width();
			windowsHeight = $window.height();  //updating global var

			//text resizing
			if (options.resize) {
				resizeMe(windowsHeight, windowsWidth);
			}

			$(SECTION_SEL).each(function(){
				var slidesWrap = $(this).find(SLIDES_WRAPPER_SEL);
				var slides = $(this).find(SLIDE_SEL);

				//adjusting the height of the table-cell for IE and Firefox
				if(options.verticalCentered){
					$(this).find(TABLE_CELL_SEL).css('height', getTableHeight($(this)) + 'px');
				}

				$(this).css('height', windowsHeight + 'px');

				//resizing the scrolling divs
				if(options.scrollOverflow){
					if(slides.length){
						slides.each(function(){
							createSlimScrolling($(this));
						});
					}else{
						createSlimScrolling($(this));
					}
				}

				//adjusting the position fo the FULL WIDTH slides...
				if (slides.length > 1) {
					landscapeScroll(slidesWrap, slidesWrap.find(SLIDE_ACTIVE_SEL));
				}
			});

			var activeSection = $(SECTION_ACTIVE_SEL);
			var sectionIndex = activeSection.index(SECTION_SEL);

			//isn't it the first section?
			if(sectionIndex){
				//adjusting the position for the current section
				FP.silentMoveTo(sectionIndex + 1);
			}

			requestAnimFrame(function(){
				isResizing = false;
			});
			$.isFunction( options.afterResize ) && resizing && options.afterResize.call(container);
			$.isFunction( options.afterReBuild ) && !resizing && options.afterReBuild.call(container);
		};

		/**
		* Turns fullPage.js to normal scrolling mode when the viewport `width` or `height`
		* are smaller than the set limit values.
		*/
		FP.setResponsive = function (active){
			var isResponsive = container.hasClass(RESPONSIVE);

			if(active){
				if(!isResponsive){
					FP.setAutoScrolling(false, 'internal');
					FP.setFitToSection(false, 'internal');
					$(SECTION_NAV_SEL).hide();
					container.addClass(RESPONSIVE);
				}
			}
			else if(isResponsive){
				FP.setAutoScrolling(originals.autoScrolling, 'internal');
				FP.setFitToSection(originals.autoScrolling, 'internal');
				$(SECTION_NAV_SEL).show();
				container.removeClass(RESPONSIVE);
			}
		}

		//flag to avoid very fast sliding for landscape sliders
		var slideMoving = false;

		var isTouchDevice = navigator.userAgent.match(/(iPhone|iPod|iPad|Android|playbook|silk|BlackBerry|BB10|Windows Phone|Tizen|Bada|webOS|IEMobile|Opera Mini)/);
		var isTouch = (('ontouchstart' in window) || (navigator.msMaxTouchPoints > 0) || (navigator.maxTouchPoints));
		var container = $(this);
		var windowsHeight = $window.height();
		var isResizing = false;
		var isWindowFocused = true;
		var lastScrolledDestiny;
		var lastScrolledSlide;
		var canScroll = true;
		var scrollings = [];
		var nav;
		var controlPressed;
		var isScrollAllowed = {};
		isScrollAllowed.m = {  'up':true, 'down':true, 'left':true, 'right':true };
		isScrollAllowed.k = $.extend(true,{}, isScrollAllowed.m);
		var originals = $.extend(true, {}, options); //deep copy

		//timeouts
		var resizeId;
		var afterSectionLoadsId;
		var afterSlideLoadsId;
		var scrollId;
		var scrollId2;
		var keydownId;

		if($(this).length){
			init();
		}

		function init(){
			//if css3 is not supported, it will use jQuery animations
			if(options.css3){
				options.css3 = support3d();
			}

			//no anchors option? Checking for them in the DOM attributes
			if(!options.anchors.length){
				options.anchors = $('[data-anchor]').map(function(){
					return $(this).data('anchor').toString();
				}).get();
			}

			prepareDom();
			FP.setAllowScrolling(true);

			//due to https://github.com/alvarotrigo/fullPage.js/issues/1502
			windowsHeight = $window.height();

			FP.setAutoScrolling(options.autoScrolling, 'internal');

			//the starting point is a slide?
			var activeSlide = $(SECTION_ACTIVE_SEL).find(SLIDE_ACTIVE_SEL);

			//the active section isn't the first one? Is not the first slide of the first section? Then we load that section/slide by default.
			if( activeSlide.length &&  ($(SECTION_ACTIVE_SEL).index(SECTION_SEL) !== 0 || ($(SECTION_ACTIVE_SEL).index(SECTION_SEL) === 0 && activeSlide.index() !== 0))){
				silentLandscapeScroll(activeSlide);
			}

			responsive();

			//setting the class for the body element
			setBodyClass();

			$window.on('load', function() {
				scrollToAnchor();
			});
		}

		/**
		* Works over the DOM structure to set it up for the current fullpage optionss.
		*/
		function prepareDom(){
			container.css({
				'height': '100%',
				'position': 'relative'
			});

			//adding a class to recognize the container internally in the code
			container.addClass(WRAPPER);
			$('html').addClass(ENABLED);

			container.removeClass(DESTROYED); //in case it was destroyed before initilizing it again

			addInternalSelectors();

			//styling the sections / slides / menu
			$(SECTION_SEL).each(function(index){
				var section = $(this);
				var slides = section.find(SLIDE_SEL);
				var numSlides = slides.length;

				styleSection(section, index);
				styleMenu(section, index);

				// if there's any slide
				if (numSlides > 0) {
					styleSlides(section, slides, numSlides);
				}else{
					if(options.verticalCentered){
						addTableClass(section);
					}
				}
			});

			//fixed elements need to be moved out of the plugin container due to problems with CSS3.
			if(options.fixedElements && options.css3){
				$(options.fixedElements).appendTo($body);
			}

			//vertical centered of the navigation + active bullet
			if(options.navigation){
				addVerticalNavigation();
			}

			if(options.scrollOverflow){
				if(document.readyState === 'complete'){
					createSlimScrollingHandler();
				}
				//after DOM and images are loaded
				$window.on('load', createSlimScrollingHandler);
			}else{
				afterRenderActions();
			}
		}

		/**
		* Styles the horizontal slides for a section.
		*/
		function styleSlides(section, slides, numSlides){
			var sliderWidth = numSlides * 100;
			var slideWidth = 100 / numSlides;

			slides.wrapAll('<div class="' + SLIDES_CONTAINER + '" />');
			slides.parent().wrap('<div class="' + SLIDES_WRAPPER + '" />');

			section.find(SLIDES_CONTAINER_SEL).css('width', sliderWidth + '%');

			if(numSlides > 1){
				if(options.controlArrows){
					createSlideArrows(section);
				}

				if(options.slidesNavigation){
					addSlidesNavigation(section, numSlides);
				}
			}

			slides.each(function(index) {
				$(this).css('width', slideWidth + '%');

				if(options.verticalCentered){
					addTableClass($(this));
				}
			});

			var startingSlide = section.find(SLIDE_ACTIVE_SEL);

			//if the slide won't be an starting point, the default will be the first one
			//the active section isn't the first one? Is not the first slide of the first section? Then we load that section/slide by default.
			if( startingSlide.length &&  ($(SECTION_ACTIVE_SEL).index(SECTION_SEL) !== 0 || ($(SECTION_ACTIVE_SEL).index(SECTION_SEL) === 0 && startingSlide.index() !== 0))){
				silentLandscapeScroll(startingSlide);
			}else{
				slides.eq(0).addClass(ACTIVE);
			}
		}

		/**
		* Styling vertical sections
		*/
		function styleSection(section, index){
			//if no active section is defined, the 1st one will be the default one
			if(!index && $(SECTION_ACTIVE_SEL).length === 0) {
				section.addClass(ACTIVE);
			}

			section.css('height', windowsHeight + 'px');

			if(options.paddingTop){
				section.css('padding-top', options.paddingTop);
			}

			if(options.paddingBottom){
				section.css('padding-bottom', options.paddingBottom);
			}

			if (typeof options.sectionsColor[index] !==  'undefined') {
				section.css('background-color', options.sectionsColor[index]);
			}

			if (typeof options.anchors[index] !== 'undefined') {
				section.attr('data-anchor', options.anchors[index]);
			}
		}

		/**
		* Sets the data-anchor attributes to the menu elements and activates the current one.
		*/
		function styleMenu(section, index){
			if (typeof options.anchors[index] !== 'undefined') {
				//activating the menu / nav element on load
				if(section.hasClass(ACTIVE)){
					activateMenuAndNav(options.anchors[index], index);
				}
			}

			//moving the menu outside the main container if it is inside (avoid problems with fixed positions when using CSS3 tranforms)
			if(options.menu && options.css3 && $(options.menu).closest(WRAPPER_SEL).length){
				$(options.menu).appendTo($body);
			}
		}

		/**
		* Adds internal classes to be able to provide customizable selectors
		* keeping the link with the style sheet.
		*/
		function addInternalSelectors(){
			//adding internal class names to void problem with common ones
			$(options.sectionSelector).each(function(){
				$(this).addClass(SECTION);
			});
			$(options.slideSelector).each(function(){
				$(this).addClass(SLIDE);
			});
		}

		/**
		* Creates the control arrows for the given section
		*/
		function createSlideArrows(section){
			section.find(SLIDES_WRAPPER_SEL).after('<div class="' + SLIDES_ARROW_PREV + '"></div><div class="' + SLIDES_ARROW_NEXT + '"></div>');

			if(options.controlArrowColor!='#fff'){
				section.find(SLIDES_ARROW_NEXT_SEL).css('border-color', 'transparent transparent transparent '+options.controlArrowColor);
				section.find(SLIDES_ARROW_PREV_SEL).css('border-color', 'transparent '+ options.controlArrowColor + ' transparent transparent');
			}

			if(!options.loopHorizontal){
				section.find(SLIDES_ARROW_PREV_SEL).hide();
			}
		}

		/**
		* Creates a vertical navigation bar.
		*/
		function addVerticalNavigation(){
			$body.append('<div id="' + SECTION_NAV + '"><ul></ul></div>');
			var nav = $(SECTION_NAV_SEL);

			nav.addClass(function() {
				return options.showActiveTooltip ? SHOW_ACTIVE_TOOLTIP + ' ' + options.navigationPosition : options.navigationPosition;
			});

			for (var i = 0; i < $(SECTION_SEL).length; i++) {
				var link = '';
				if (options.anchors.length) {
					link = options.anchors[i];
				}

				var li = '<li><a href="#' + link + '"><span></span></a>';

				// Only add tooltip if needed (defined by user)
				var tooltip = options.navigationTooltips[i];

				if (typeof tooltip !== 'undefined' && tooltip !== '') {
					li += '<div class="' + SECTION_NAV_TOOLTIP + ' ' + options.navigationPosition + '">' + tooltip + '</div>';
				}

				li += '</li>';

				nav.find('ul').append(li);
			}

			//centering it vertically
			$(SECTION_NAV_SEL).css('margin-top', '-' + ($(SECTION_NAV_SEL).height()/2) + 'px');

			//activating the current active section
			$(SECTION_NAV_SEL).find('li').eq($(SECTION_ACTIVE_SEL).index(SECTION_SEL)).find('a').addClass(ACTIVE);
		}

		/**
		* Creates the slim scroll scrollbar for the sections and slides inside them.
		*/
		function createSlimScrollingHandler(){
			$(SECTION_SEL).each(function(){
				var slides = $(this).find(SLIDE_SEL);

				if(slides.length){
					slides.each(function(){
						createSlimScrolling($(this));
					});
				}else{
					createSlimScrolling($(this));
				}

			});
			afterRenderActions();
		}

		/**
		* Actions and callbacks to fire afterRender
		*/
		function afterRenderActions(){
			var section = $(SECTION_ACTIVE_SEL);

			solveBugSlimScroll(section);
			lazyLoad(section);
			playMedia(section);

			$.isFunction( options.afterLoad ) && options.afterLoad.call(section, section.data('anchor'), (section.index(SECTION_SEL) + 1));
			$.isFunction( options.afterRender ) && options.afterRender.call(container);
		}


		/**
		* Solves a bug with slimScroll vendor library #1037, #553
		*/
		function solveBugSlimScroll(section){
			var slides = section.find('SLIDES_WRAPPER');
			var scrollableWrap = section.find(SCROLLABLE_SEL);

			if(slides.length){
				scrollableWrap = slides.find(SLIDE_ACTIVE_SEL);
			}

			scrollableWrap.mouseover();
		}


		var isScrolling = false;

		//when scrolling...
		$window.on('scroll', scrollHandler);

		function scrollHandler(){
			var currentSection;

			if(!options.autoScrolling || options.scrollBar){
				var currentScroll = $window.scrollTop();
				var visibleSectionIndex = 0;
				var initial = Math.abs(currentScroll - document.querySelectorAll(SECTION_SEL)[0].offsetTop);

				//taking the section which is showing more content in the viewport
				var sections =  document.querySelectorAll(SECTION_SEL);
				for (var i = 0; i < sections.length; ++i) {
					var section = sections[i];

					var current = Math.abs(currentScroll - section.offsetTop);

					if(current < initial){
						visibleSectionIndex = i;
						initial = current;
					}
				}

				//geting the last one, the current one on the screen
				currentSection = $(sections).eq(visibleSectionIndex);

				//setting the visible section as active when manually scrolling
				//executing only once the first time we reach the section
				if(!currentSection.hasClass(ACTIVE) && !currentSection.hasClass(AUTO_HEIGHT)){
					isScrolling = true;
					var leavingSection = $(SECTION_ACTIVE_SEL);
					var leavingSectionIndex = leavingSection.index(SECTION_SEL) + 1;
					var yMovement = getYmovement(currentSection);
					var anchorLink  = currentSection.data('anchor');
					var sectionIndex = currentSection.index(SECTION_SEL) + 1;
					var activeSlide = currentSection.find(SLIDE_ACTIVE_SEL);

					if(activeSlide.length){
						var slideAnchorLink = activeSlide.data('anchor');
						var slideIndex = activeSlide.index();
					}

					if(canScroll){
						currentSection.addClass(ACTIVE).siblings().removeClass(ACTIVE);

						$.isFunction( options.onLeave ) && options.onLeave.call( leavingSection, leavingSectionIndex, sectionIndex, yMovement);

						$.isFunction( options.afterLoad ) && options.afterLoad.call( currentSection, anchorLink, sectionIndex);
						lazyLoad(currentSection);

						activateMenuAndNav(anchorLink, sectionIndex - 1);

						if(options.anchors.length){
							//needed to enter in hashChange event when using the menu with anchor links
							lastScrolledDestiny = anchorLink;

							setState(slideIndex, slideAnchorLink, anchorLink, sectionIndex);
						}
					}

					//small timeout in order to avoid entering in hashChange event when scrolling is not finished yet
					clearTimeout(scrollId);
					scrollId = setTimeout(function(){
						isScrolling = false;
					}, 100);
				}

				if(options.fitToSection){
					//for the auto adjust of the viewport to fit a whole section
					clearTimeout(scrollId2);

					scrollId2 = setTimeout(function(){
						//checking fitToSection again in case it was set to false before the timeout delay
						if(canScroll && options.fitToSection){
							//allows to scroll to an active section and
							//if the section is already active, we prevent firing callbacks
							if($(SECTION_ACTIVE_SEL).is(currentSection)){
								requestAnimFrame(function(){
									isResizing = true;
								});
							}
							scrollPage(currentSection);

							isResizing = false;
						}
					}, options.fitToSectionDelay);
				}
			}
		}


		/**
		* Determines whether the active section or slide is scrollable through and scrolling bar
		*/
		function isScrollable(activeSection){
			//if there are landscape slides, we check if the scrolling bar is in the current one or not
			if(activeSection.find(SLIDES_WRAPPER_SEL).length){
				return activeSection.find(SLIDE_ACTIVE_SEL).find(SCROLLABLE_SEL);
			}

			return activeSection.find(SCROLLABLE_SEL);
		}

		/**
		* Determines the way of scrolling up or down:
		* by 'automatically' scrolling a section or by using the default and normal scrolling.
		*/
		function scrolling(type, scrollable){
			if (!isScrollAllowed.m[type]){
				return;
			}
			var check, scrollSection;

			if(type == 'down'){
				check = 'bottom';
				scrollSection = FP.moveSectionDown;
			}else{
				check = 'top';
				scrollSection = FP.moveSectionUp;
			}

			if(scrollable.length > 0 ){
				//is the scrollbar at the start/end of the scroll?
				if(isScrolled(check, scrollable)){
					scrollSection();
				}else{
					return true;
				}
			}else{
				// moved up/down
				scrollSection();
			}
		}


		var touchStartY = 0;
		var touchStartX = 0;
		var touchEndY = 0;
		var touchEndX = 0;

		/* Detecting touch events

		* As we are changing the top property of the page on scrolling, we can not use the traditional way to detect it.
		* This way, the touchstart and the touch moves shows an small difference between them which is the
		* used one to determine the direction.
		*/
		function touchMoveHandler(event){
			var e = event.originalEvent;

			// additional: if one of the normalScrollElements isn't within options.normalScrollElementTouchThreshold hops up the DOM chain
			if (!checkParentForNormalScrollElement(event.target) && isReallyTouch(e) ) {

				if(options.autoScrolling){
					//preventing the easing on iOS devices
					event.preventDefault();
				}

				var activeSection = $(SECTION_ACTIVE_SEL);
				var scrollable = isScrollable(activeSection);

				if (canScroll && !slideMoving) { //if theres any #
					var touchEvents = getEventsPage(e);

					touchEndY = touchEvents.y;
					touchEndX = touchEvents.x;

					//if movement in the X axys is greater than in the Y and the currect section has slides...
					if (activeSection.find(SLIDES_WRAPPER_SEL).length && Math.abs(touchStartX - touchEndX) > (Math.abs(touchStartY - touchEndY))) {

						//is the movement greater than the minimum resistance to scroll?
						if (Math.abs(touchStartX - touchEndX) > ($window.width() / 100 * options.touchSensitivity)) {
							if (touchStartX > touchEndX) {
								if(isScrollAllowed.m.right){
									FP.moveSlideRight(); //next
								}
							} else {
								if(isScrollAllowed.m.left){
									FP.moveSlideLeft(); //prev
								}
							}
						}
					}

					//vertical scrolling (only when autoScrolling is enabled)
					else if(options.autoScrolling){

						//is the movement greater than the minimum resistance to scroll?
						if (Math.abs(touchStartY - touchEndY) > ($window.height() / 100 * options.touchSensitivity)) {
							if (touchStartY > touchEndY) {
								scrolling('down', scrollable);
							} else if (touchEndY > touchStartY) {
								scrolling('up', scrollable);
							}
						}
					}
				}
			}

		}

		/**
		 * recursive function to loop up the parent nodes to check if one of them exists in options.normalScrollElements
		 * Currently works well for iOS - Android might need some testing
		 * @param  {Element} el  target element / jquery selector (in subsequent nodes)
		 * @param  {int}     hop current hop compared to options.normalScrollElementTouchThreshold
		 * @return {boolean} true if there is a match to options.normalScrollElements
		 */
		function checkParentForNormalScrollElement (el, hop) {
			hop = hop || 0;
			var parent = $(el).parent();

			if (hop < options.normalScrollElementTouchThreshold &&
				parent.is(options.normalScrollElements) ) {
				return true;
			} else if (hop == options.normalScrollElementTouchThreshold) {
				return false;
			} else {
				return checkParentForNormalScrollElement(parent, ++hop);
			}
		}

		/**
		* As IE >= 10 fires both touch and mouse events when using a mouse in a touchscreen
		* this way we make sure that is really a touch event what IE is detecting.
		*/
		function isReallyTouch(e){
			//if is not IE   ||  IE is detecting `touch` or `pen`
			return typeof e.pointerType === 'undefined' || e.pointerType != 'mouse';
		}

		/**
		* Handler for the touch start event.
		*/
		function touchStartHandler(event){
			var e = event.originalEvent;

			//stopping the auto scroll to adjust to a section
			if(options.fitToSection){
				$htmlBody.stop();
			}

			if(isReallyTouch(e)){
				var touchEvents = getEventsPage(e);
				touchStartY = touchEvents.y;
				touchStartX = touchEvents.x;
			}
		}

		/**
		* Gets the average of the last `number` elements of the given array.
		*/
		function getAverage(elements, number){
			var sum = 0;

			//taking `number` elements from the end to make the average, if there are not enought, 1
			var lastElements = elements.slice(Math.max(elements.length - number, 1));

			for(var i = 0; i < lastElements.length; i++){
				sum = sum + lastElements[i];
			}

			return Math.ceil(sum/number);
		}

		/**
		 * Detecting mousewheel scrolling
		 *
		 * http://blogs.sitepointstatic.com/examples/tech/mouse-wheel/index.html
		 * http://www.sitepoint.com/html5-javascript-mouse-wheel/
		 */
		var prevTime = new Date().getTime();

		function MouseWheelHandler(e) {
			var curTime = new Date().getTime();

			//autoscrolling and not zooming?
			if(options.autoScrolling && !controlPressed){
				// cross-browser wheel delta
				e = e || window.event;
				var value = e.wheelDelta || -e.deltaY || -e.detail;
				var delta = Math.max(-1, Math.min(1, value));
				var horizontalDetection = typeof e.wheelDeltaX !== 'undefined' || typeof e.deltaX !== 'undefined';
				var isScrollingVertically = (Math.abs(e.wheelDeltaX) < Math.abs(e.wheelDelta)) || (Math.abs(e.deltaX ) < Math.abs(e.deltaY) || !horizontalDetection);

				//Limiting the array to 150 (lets not waste memory!)
				if(scrollings.length > 149){
					scrollings.shift();
				}

				//keeping record of the previous scrollings
				scrollings.push(Math.abs(value));

				//preventing to scroll the site on mouse wheel when scrollbar is present
				if(options.scrollBar){
					e.preventDefault ? e.preventDefault() : e.returnValue = false;
				}

				var activeSection = $(SECTION_ACTIVE_SEL);
				var scrollable = isScrollable(activeSection);

				//time difference between the last scroll and the current one
				var timeDiff = curTime-prevTime;
				prevTime = curTime;

				//haven't they scrolled in a while?
				//(enough to be consider a different scrolling action to scroll another section)
				if(timeDiff > 200){
					//emptying the array, we dont care about old scrollings for our averages
					scrollings = [];
				}

				if(canScroll){
					var averageEnd = getAverage(scrollings, 10);
					var averageMiddle = getAverage(scrollings, 70);
					var isAccelerating = averageEnd >= averageMiddle;

					//to avoid double swipes...
					if(isAccelerating && isScrollingVertically){
						//scrolling down?
						if (delta < 0) {
							scrolling('down', scrollable);

							//scrolling up?
						}else {
							scrolling('up', scrollable);
						}
					}
				}

				return false;
			}

			if(options.fitToSection){
				//stopping the auto scroll to adjust to a section
				$htmlBody.stop();
			}
		}

		/**
		* Slides a slider to the given direction.
		*/
		function moveSlide(direction){
			var activeSection = $(SECTION_ACTIVE_SEL);
			var slides = activeSection.find(SLIDES_WRAPPER_SEL);
			var numSlides = slides.find(SLIDE_SEL).length;

			// more than one slide needed and nothing should be sliding
			if (!slides.length || slideMoving || numSlides < 2) {
				return;
			}

			var currentSlide = slides.find(SLIDE_ACTIVE_SEL);
			var destiny = null;

			if(direction === 'prev'){
				destiny = currentSlide.prev(SLIDE_SEL);
			}else{
				destiny = currentSlide.next(SLIDE_SEL);
			}

			//isn't there a next slide in the secuence?
			if(!destiny.length){
				//respect loopHorizontal settin
				if (!options.loopHorizontal) return;

				if(direction === 'prev'){
					destiny = currentSlide.siblings(':last');
				}else{
					destiny = currentSlide.siblings(':first');
				}
			}

			slideMoving = true;

			landscapeScroll(slides, destiny);
		}

		/**
		* Maintains the active slides in the viewport
		* (Because he `scroll` animation might get lost with some actions, such as when using continuousVertical)
		*/
		function keepSlidesPosition(){
			$(SLIDE_ACTIVE_SEL).each(function(){
				silentLandscapeScroll($(this), 'internal');
			});
		}

		//IE < 10 pollify for requestAnimationFrame
		window.requestAnimFrame = function(){
			return window.requestAnimationFrame || function(callback){ callback() }
		}();

		/**
		* Scrolls the site to the given element and scrolls to the slide if a callback is given.
		*/
		function scrollPage(element, callback, isMovementUp){
			//requestAnimFrame is used in order to prevent a Chrome 44 bug (http://stackoverflow.com/a/31961816/1081396)
			requestAnimFrame(function(){
				var dest = element.position();
				if(typeof dest === 'undefined'){ return; } //there's no element to scroll, leaving the function

				//auto height? Scrolling only a bit, the next element's height. Otherwise the whole viewport.
				var dtop = element.hasClass(AUTO_HEIGHT) ? (dest.top - windowsHeight + element.height()) : dest.top;

				//local variables
				var v = {
					element: element,
					callback: callback,
					isMovementUp: isMovementUp,
					dest: dest,
					dtop: dtop,
					yMovement: getYmovement(element),
					anchorLink: element.data('anchor'),
					sectionIndex: element.index(SECTION_SEL),
					activeSlide: element.find(SLIDE_ACTIVE_SEL),
					activeSection: $(SECTION_ACTIVE_SEL),
					leavingSection: $(SECTION_ACTIVE_SEL).index(SECTION_SEL) + 1,

					//caching the value of isResizing at the momment the function is called
					//because it will be checked later inside a setTimeout and the value might change
					localIsResizing: isResizing
				};
				console.log(v);

				//quiting when destination scroll is the same as the current one
				if((v.activeSection.is(element) && !isResizing) || (options.scrollBar && $window.scrollTop() === v.dtop && !element.hasClass(AUTO_HEIGHT) )){ return; }

				if(v.activeSlide.length){
					var slideAnchorLink = v.activeSlide.data('anchor');
					var slideIndex = v.activeSlide.index();
				}

				// If continuousVertical && we need to wrap around
				if (options.autoScrolling && options.continuousVertical && typeof (v.isMovementUp) !== "undefined" &&
				((!v.isMovementUp && v.yMovement == 'up') || // Intending to scroll down but about to go up or
					(v.isMovementUp && v.yMovement == 'down'))) { // intending to scroll up but about to go down

					v = createInfiniteSections(v);
				}

				//callback (onLeave) if the site is not just resizing and readjusting the slides
				if($.isFunction(options.onLeave) && !v.localIsResizing){
					if(options.onLeave.call(v.activeSection, v.leavingSection, (v.sectionIndex + 1), v.yMovement) === false){
						return;
					}else{
						stopMedia(v.activeSection);
					}
				}

				element.addClass(ACTIVE).siblings().removeClass(ACTIVE);
				lazyLoad(element);

				//preventing from activating the MouseWheelHandler event
				//more than once if the page is scrolling
				canScroll = false;

				setState(slideIndex, slideAnchorLink, v.anchorLink, v.sectionIndex);

				performMovement(v);

				//flag to avoid callingn `scrollPage()` twice in case of using anchor links
				lastScrolledDestiny = v.anchorLink;

				//avoid firing it twice (as it does also on scroll)
				activateMenuAndNav(v.anchorLink, v.sectionIndex);
				console.log("bo");
			});
		}

		/**
		* Performs the movement (by CSS3 or by jQuery)
		*/
		function performMovement(v){
			// using CSS3 translate functionality
			if (options.css3 && options.autoScrolling && !options.scrollBar) {

				var translate3d = 'translate3d(0px, -' + v.dtop + 'px, 0px)';
				transformContainer(translate3d, true);

				//even when the scrollingSpeed is 0 there's a little delay, which might cause the
				//scrollingSpeed to change in case of using silentMoveTo();
				if(options.scrollingSpeed){
					afterSectionLoadsId = setTimeout(function () {
						afterSectionLoads(v);
					}, options.scrollingSpeed);
				}else{
					afterSectionLoads(v);
				}
			}

			// using jQuery animate
			else{
				var scrollSettings = getScrollSettings(v);

				$(scrollSettings.element).animate(
					scrollSettings.options,
					options.scrollingSpeed, options.easing).promise().done(function () { //only one single callback in case of animating  `html, body`
					afterSectionLoads(v);
				});
			}
		}

		/**
		* Gets the scrolling settings depending on the plugin autoScrolling option
		*/
		function getScrollSettings(v){
			var scroll = {};

			if(options.autoScrolling && !options.scrollBar){
				scroll.options = { 'top': -v.dtop};
				scroll.element = WRAPPER_SEL;
			}else{
				scroll.options = { 'scrollTop': v.dtop};
				scroll.element = 'html, body';
			}

			return scroll;
		}

		/**
		* Adds sections before or after the current one to create the infinite effect.
		*/
		function createInfiniteSections(v){
			// Scrolling down
			if (!v.isMovementUp) {
				// Move all previous sections to after the active section
				$(SECTION_ACTIVE_SEL).after(v.activeSection.prevAll(SECTION_SEL).get().reverse());
			}
			else { // Scrolling up
				// Move all next sections to before the active section
				$(SECTION_ACTIVE_SEL).before(v.activeSection.nextAll(SECTION_SEL));
			}

			// Maintain the displayed position (now that we changed the element order)
			silentScroll($(SECTION_ACTIVE_SEL).position().top);

			// Maintain the active slides visible in the viewport
			keepSlidesPosition();

			// save for later the elements that still need to be reordered
			v.wrapAroundElements = v.activeSection;

			// Recalculate animation variables
			v.dest = v.element.position();
			v.dtop = v.dest.top;
			v.yMovement = getYmovement(v.element);

			return v;
		}

		/**
		* Fix section order after continuousVertical changes have been animated
		*/
		function continuousVerticalFixSectionOrder (v) {
			// If continuousVertical is in effect (and autoScrolling would also be in effect then),
			// finish moving the elements around so the direct navigation will function more simply
			if (!v.wrapAroundElements || !v.wrapAroundElements.length) {
				return;
			}

			if (v.isMovementUp) {
				$(SECTION_FIRST_SEL).before(v.wrapAroundElements);
			}
			else {
				$(SECTION_LAST_SEL).after(v.wrapAroundElements);
			}

			silentScroll($(SECTION_ACTIVE_SEL).position().top);

			// Maintain the active slides visible in the viewport
			keepSlidesPosition();
		}


		/**
		* Actions to do once the section is loaded.
		*/
		function afterSectionLoads (v){
			continuousVerticalFixSectionOrder(v);

			v.element.find('.fp-scrollable').mouseover();

			FP.setFitToSection(!v.element.hasClass(AUTO_HEIGHT));

			//callback (afterLoad) if the site is not just resizing and readjusting the slides
			$.isFunction(options.afterLoad) && !v.localIsResizing && options.afterLoad.call(v.element, v.anchorLink, (v.sectionIndex + 1));

			playMedia(v.element)

			canScroll = true;

			$.isFunction(v.callback) && v.callback.call(this);
		}

		/**
		* Lazy loads image, video and audio elements.
		*/
		function lazyLoad(destiny){
			//Lazy loading images, videos and audios
			var slide = destiny.find(SLIDE_ACTIVE_SEL);
			if( slide.length ) {
				destiny = $(slide);
			}

			destiny.find('img[data-src], source[data-src], audio[data-src]').each(function(){
				$(this).attr('src', $(this).data('src'));
				$(this).removeAttr('data-src');

				if($(this).is('source')){
					$(this).closest('video').get(0).load();
				}
			});
		}

		/**
		* Plays video and audio elements.
		*/
		function playMedia(destiny){
			//playing HTML5 media elements
			destiny.find('video, audio').each(function(){
				var element = $(this).get(0);

				if( element.hasAttribute('autoplay') && typeof element.play === 'function' ) {
					element.play();
				}
			});
		}

		/**
		* Stops video and audio elements.
		*/
		function stopMedia(destiny){
			//stopping HTML5 media elements
			destiny.find('video, audio').each(function(){
				var element = $(this).get(0);

				if( !element.hasAttribute('data-ignore') && typeof element.pause === 'function' ) {
					element.pause();
				}
			});
		}

		/**
		* Scrolls to the anchor in the URL when loading the site
		*/
		function scrollToAnchor(){
			//getting the anchor link in the URL and deleting the `#`
			var value =  window.location.hash.replace('#', '').split('/');
			var section = value[0];
			var slide = value[1];

			if(section){  //if theres any #
				if(options.animateAnchor){
					scrollPageAndSlide(section, slide);
				}else{
					FP.silentMoveTo(section, slide);
				}
			}
		}

		//detecting any change on the URL to scroll to the given anchor link
		//(a way to detect back history button as we play with the hashes on the URL)
		$window.on('hashchange', hashChangeHandler);

		function hashChangeHandler(){
			if(!isScrolling && !options.lockAnchors){
				var value =  window.location.hash.replace('#', '').split('/');
				var section = value[0];
				var slide = value[1];

				//when moving to a slide in the first section for the first time (first time to add an anchor to the URL)
				var isFirstSlideMove =  (typeof lastScrolledDestiny === 'undefined');
				var isFirstScrollMove = (typeof lastScrolledDestiny === 'undefined' && typeof slide === 'undefined' && !slideMoving);


				if(section.length){

					/*in order to call scrollpage() only once for each destination at a time
					It is called twice for each scroll otherwise, as in case of using anchorlinks `hashChange`
					event is fired on every scroll too.*/
					if ((section && section !== lastScrolledDestiny) && !isFirstSlideMove || isFirstScrollMove || (!slideMoving && lastScrolledSlide != slide ))  {
						scrollPageAndSlide(section, slide);
					}
				}
			}
		}

		/**
		 * Sliding with arrow keys, both, vertical and horizontal
		 */
		$document.keydown(keydownHandler);

		//to prevent scrolling while zooming
		$document.keyup(function(e){
			if(isWindowFocused){ //the keyup gets fired on new tab ctrl + t in Firefox
				controlPressed = e.ctrlKey;
			}
		});

		//when opening a new tab (ctrl + t), `control` won't be pressed when comming back.
		$(window).blur(function() {
			isWindowFocused = false;
			controlPressed = false;
		});

		var keydownId;
		function keydownHandler(e) {

			clearTimeout(keydownId);


			var activeElement = $(':focus');

			if(!activeElement.is('textarea') && !activeElement.is('input') && !activeElement.is('select') &&
				options.keyboardScrolling && options.autoScrolling){
				var keyCode = e.which;

				//preventing the scroll with arrow keys & spacebar & Page Up & Down keys
				var keyControls = [40, 38, 32, 33, 34];
				if($.inArray(keyCode, keyControls) > -1){
					e.preventDefault();
				}

				controlPressed = e.ctrlKey;

				keydownId = setTimeout(function(){
					onkeydown(e);
				},150);
			}
		}

		/**
		* Keydown event
		*/
		function onkeydown(e){
			var shiftPressed = e.shiftKey;

			switch (e.which) {
				//up
			case 38:
			case 33:
				if(isScrollAllowed.k.up){
					FP.moveSectionUp();
				}
				break;

			//down
			case 32: //spacebar
				if(shiftPressed && isScrollAllowed.k.up){
					FP.moveSectionUp();
					break;
				}
			case 40:
			case 34:
				if(isScrollAllowed.k.down){
					FP.moveSectionDown();
				}
				break;

			//Home
			case 36:
				if(isScrollAllowed.k.up){
					FP.moveTo(1);
				}
				break;

			//End
			case 35:
				if(isScrollAllowed.k.down){
					FP.moveTo( $(SECTION_SEL).length );
				}
				break;

			//left
			case 37:
				if(isScrollAllowed.k.left){
					FP.moveSlideLeft();
				}
				break;

			//right
			case 39:
				if(isScrollAllowed.k.right){
					FP.moveSlideRight();
				}
				break;

			default:
				return; // exit this handler for other keys
			}
		}

		//binding the mousemove when the mouse's middle button is released
		container.mousedown(function(e){
			//middle button
			if (e.which == 2){
				oldPageY = e.pageY;
				container.on('mousemove', mouseMoveHandler);
			}
		});

		//unbinding the mousemove when the mouse's middle button is released
		container.mouseup(function(e){
			//middle button
			if (e.which == 2){
				container.off('mousemove');
			}
		});

		/**
		* Detecting the direction of the mouse movement.
		* Used only for the middle button of the mouse.
		*/
		var oldPageY = 0;
		function mouseMoveHandler(e){
			// moving up
			if(canScroll){
				if (e.pageY < oldPageY){
					FP.moveSectionUp();

					// moving downw
				}else if(e.pageY > oldPageY){
					FP.moveSectionDown();
				}
			}
			oldPageY = e.pageY;
		}

		/**
		* Scrolls to the section when clicking the navigation bullet
		*/
		$document.on('click touchstart', SECTION_NAV_SEL + ' a', function(e){
			e.preventDefault();
			var index = $(this).parent().index();
			scrollPage($(SECTION_SEL).eq(index));
		});

		/**
		* Scrolls the slider to the given slide destination for the given section
		*/
		$document.on('click touchstart', SLIDES_NAV_LINK_SEL, function(e){
			e.preventDefault();
			var slides = $(this).closest(SECTION_SEL).find(SLIDES_WRAPPER_SEL);
			var destiny = slides.find(SLIDE_SEL).eq($(this).closest('li').index());

			landscapeScroll(slides, destiny);
		});

		/**
		* Applying normalScroll elements.
		* Ignoring the scrolls over the specified selectors.
		*/
		if(options.normalScrollElements){
			$document.on('mouseenter', options.normalScrollElements, function () {
				FP.setMouseWheelScrolling(false);
			});

			$document.on('mouseleave', options.normalScrollElements, function(){
				FP.setMouseWheelScrolling(true);
			});
		}

		/**
		 * Scrolling horizontally when clicking on the slider controls.
		 */
		$(SECTION_SEL).on('click touchstart', SLIDES_ARROW_SEL, function() {
			if ($(this).hasClass(SLIDES_PREV)) {
				if(isScrollAllowed.m.left){
					FP.moveSlideLeft();
				}
			} else {
				if(isScrollAllowed.m.right){
					FP.moveSlideRight();
				}
			}
		});

		/**
		* Scrolls horizontal sliders.
		*/
		function landscapeScroll(slides, destiny){
			var destinyPos = destiny.position();
			var slideIndex = destiny.index();
			var section = slides.closest(SECTION_SEL);
			var sectionIndex = section.index(SECTION_SEL);
			var anchorLink = section.data('anchor');
			var slidesNav = section.find(SLIDES_NAV_SEL);
			var slideAnchor = getAnchor(destiny);

			//caching the value of isResizing at the momment the function is called
			//because it will be checked later inside a setTimeout and the value might change
			var localIsResizing = isResizing;

			if(options.onSlideLeave){
				var prevSlide = section.find(SLIDE_ACTIVE_SEL);
				var prevSlideIndex = prevSlide.index();
				var xMovement = getXmovement(prevSlideIndex, slideIndex);

				//if the site is not just resizing and readjusting the slides
				if(!localIsResizing && xMovement!=='none'){
					if($.isFunction( options.onSlideLeave )){
						if(options.onSlideLeave.call( prevSlide, anchorLink, (sectionIndex + 1), prevSlideIndex, xMovement, slideIndex ) === false){
							slideMoving = false;
							return;
						}
					}
				}
			}

			destiny.addClass(ACTIVE).siblings().removeClass(ACTIVE);
			if(!localIsResizing){
				lazyLoad(destiny);
			}

			if(!options.loopHorizontal && options.controlArrows){
				//hidding it for the fist slide, showing for the rest
				section.find(SLIDES_ARROW_PREV_SEL).toggle(slideIndex!==0);

				//hidding it for the last slide, showing for the rest
				section.find(SLIDES_ARROW_NEXT_SEL).toggle(!destiny.is(':last-child'));
			}

			//only changing the URL if the slides are in the current section (not for resize re-adjusting)
			if(section.hasClass(ACTIVE)){
				setState(slideIndex, slideAnchor, anchorLink, sectionIndex);
			}

			var afterSlideLoads = function(){
				//if the site is not just resizing and readjusting the slides
				if(!localIsResizing){
					$.isFunction( options.afterSlideLoad ) && options.afterSlideLoad.call( destiny, anchorLink, (sectionIndex + 1), slideAnchor, slideIndex);
				}
				//letting them slide again
				slideMoving = false;
			};

			if(options.css3){
				var translate3d = 'translate3d(-' + Math.round(destinyPos.left) + 'px, 0px, 0px)';

				addAnimation(slides.find(SLIDES_CONTAINER_SEL), options.scrollingSpeed>0).css(getTransforms(translate3d));

				afterSlideLoadsId = setTimeout(function(){
					afterSlideLoads();
				}, options.scrollingSpeed, options.easing);
			}else{
				slides.animate({
					scrollLeft : Math.round(destinyPos.left)
				}, options.scrollingSpeed, options.easing, function() {

					afterSlideLoads();
				});
			}

			slidesNav.find(ACTIVE_SEL).removeClass(ACTIVE);
			slidesNav.find('li').eq(slideIndex).find('a').addClass(ACTIVE);
		}

		//when resizing the site, we adjust the heights of the sections, slimScroll...
		$window.resize(resizeHandler);

		var previousHeight = windowsHeight;
		function resizeHandler(){
			//checking if it needs to get responsive
			responsive();

			// rebuild immediately on touch devices
			if (isTouchDevice) {
				var activeElement = $(document.activeElement);

				//if the keyboard is NOT visible
				if (!activeElement.is('textarea') && !activeElement.is('input') && !activeElement.is('select')) {
					var currentHeight = $window.height();

					//making sure the change in the viewport size is enough to force a rebuild. (20 % of the window to avoid problems when hidding scroll bars)
					if( Math.abs(currentHeight - previousHeight) > (20 * Math.max(previousHeight, currentHeight) / 100) ){
						FP.reBuild(true);
						previousHeight = currentHeight;
					}
				}
			}else{
				//in order to call the functions only when the resize is finished
				//http://stackoverflow.com/questions/4298612/jquery-how-to-call-resize-event-only-once-its-finished-resizing
				clearTimeout(resizeId);

				resizeId = setTimeout(function(){
					FP.reBuild(true);
				}, 350);
			}
		}

		/**
		* Checks if the site needs to get responsive and disables autoScrolling if so.
		* A class `fp-responsive` is added to the plugin's container in case the user wants to use it for his own responsive CSS.
		*/
		function responsive(){
			var widthLimit = options.responsive || options.responsiveWidth; //backwards compatiblity
			var heightLimit = options.responsiveHeight;

			if(widthLimit){
				FP.setResponsive($window.width() < widthLimit);
			}

			if(heightLimit){
				var isResponsive = container.hasClass(RESPONSIVE);

				//if its not already in responsive mode because of the `width` limit
				if(!isResponsive){
					FP.setResponsive($window.height() < heightLimit);
				}
			}
		}

		/**
		* Adds transition animations for the given element
		*/
		function addAnimation(element){
			var transition = 'all ' + options.scrollingSpeed + 'ms ' + options.easingcss3;

			element.removeClass(NO_TRANSITION);
			return element.css({
				'-webkit-transition': transition,
				'transition': transition
			});
		}

		/**
		* Remove transition animations for the given element
		*/
		function removeAnimation(element){
			return element.addClass(NO_TRANSITION);
		}

		/**
		 * Resizing of the font size depending on the window size as well as some of the images on the site.
		 */
		function resizeMe(displayHeight, displayWidth) {
			//Standard dimensions, for which the body font size is correct
			var preferredHeight = 825;
			var preferredWidth = 900;

			if (displayHeight < preferredHeight || displayWidth < preferredWidth) {
				var heightPercentage = (displayHeight * 100) / preferredHeight;
				var widthPercentage = (displayWidth * 100) / preferredWidth;
				var percentage = Math.min(heightPercentage, widthPercentage);
				var newFontSize = percentage.toFixed(2);

				$body.css('font-size', newFontSize + '%');
			} else {
				$body.css('font-size', '100%');
			}
		}

		/**
		 * Activating the website navigation dots according to the given slide name.
		 */
		function activateNavDots(name, sectionIndex){
			if(options.navigation){
				$(SECTION_NAV_SEL).find(ACTIVE_SEL).removeClass(ACTIVE);
				if(name){
					$(SECTION_NAV_SEL).find('a[href="#' + name + '"]').addClass(ACTIVE);
				}else{
					$(SECTION_NAV_SEL).find('li').eq(sectionIndex).find('a').addClass(ACTIVE);
				}
			}
		}

		/**
		 * Activating the website main menu elements according to the given slide name.
		 */
		function activateMenuElement(name){
			if(options.menu){
				$(options.menu).find(ACTIVE_SEL).removeClass(ACTIVE);
				$(options.menu).find('[data-menuanchor="'+name+'"]').addClass(ACTIVE);
			}
		}

		function activateMenuAndNav(anchor, index){
			activateMenuElement(anchor);
			activateNavDots(anchor, index);
		}

		/**
		* Return a boolean depending on whether the scrollable element is at the end or at the start of the scrolling
		* depending on the given type.
		*/
		function isScrolled(type, scrollable){
			if(type === 'top'){
				return !scrollable.scrollTop();
			}else if(type === 'bottom'){
				return scrollable.scrollTop() + 1 + scrollable.innerHeight() >= scrollable[0].scrollHeight;
			}
		}

		/**
		* Retuns `up` or `down` depending on the scrolling movement to reach its destination
		* from the current section.
		*/
		function getYmovement(destiny){
			var fromIndex = $(SECTION_ACTIVE_SEL).index(SECTION_SEL);
			var toIndex = destiny.index(SECTION_SEL);
			if( fromIndex == toIndex){
				return 'none';
			}
			if(fromIndex > toIndex){
				return 'up';
			}
			return 'down';
		}

		/**
		* Retuns `right` or `left` depending on the scrolling movement to reach its destination
		* from the current slide.
		*/
		function getXmovement(fromIndex, toIndex){
			if( fromIndex == toIndex){
				return 'none';
			}
			if(fromIndex > toIndex){
				return 'left';
			}
			return 'right';
		}


		function createSlimScrolling(element){
			//needed to make `scrollHeight` work under Opera 12
			element.css('overflow', 'hidden');

			//in case element is a slide
			var section = element.closest(SECTION_SEL);
			var scrollable = element.find(SCROLLABLE_SEL);
			var contentHeight;

			//if there was scroll, the contentHeight will be the one in the scrollable section
			if(scrollable.length){
				contentHeight = scrollable.get(0).scrollHeight;
			}else{
				contentHeight = element.get(0).scrollHeight;
				if(options.verticalCentered){
					contentHeight = element.find(TABLE_CELL_SEL).get(0).scrollHeight;
				}
			}

			var scrollHeight = windowsHeight - parseInt(section.css('padding-bottom')) - parseInt(section.css('padding-top'));

			//needs scroll?
			if ( contentHeight > scrollHeight) {
				//was there already an scroll ? Updating it
				if(scrollable.length){
					scrollable.css('height', scrollHeight + 'px').parent().css('height', scrollHeight + 'px');
				}
				//creating the scrolling
				else{
					if(options.verticalCentered){
						element.find(TABLE_CELL_SEL).wrapInner('<div class="' + SCROLLABLE + '" />');
					}else{
						element.wrapInner('<div class="' + SCROLLABLE + '" />');
					}

					element.find(SCROLLABLE_SEL).slimScroll({
						allowPageScroll: true,
						height: scrollHeight + 'px',
						size: '10px',
						alwaysVisible: true
					});
				}
			}

			//removing the scrolling when it is not necessary anymore
			else{
				removeSlimScroll(element);
			}

			//undo
			element.css('overflow', '');
		}

		function removeSlimScroll(element){
			element.find(SCROLLABLE_SEL).children().first().unwrap().unwrap();
			element.find(SLIMSCROLL_BAR_SEL).remove();
			element.find(SLIMSCROLL_RAIL_SEL).remove();
		}

		function addTableClass(element){
			element.addClass(TABLE).wrapInner('<div class="' + TABLE_CELL + '" style="height:' + getTableHeight(element) + 'px;" />');
		}

		function getTableHeight(element){
			var sectionHeight = windowsHeight;

			if(options.paddingTop || options.paddingBottom){
				var section = element;
				if(!section.hasClass(SECTION)){
					section = element.closest(SECTION_SEL);
				}

				var paddings = parseInt(section.css('padding-top')) + parseInt(section.css('padding-bottom'));
				sectionHeight = (windowsHeight - paddings);
			}

			return sectionHeight;
		}

		/**
		* Adds a css3 transform property to the container class with or without animation depending on the animated param.
		*/
		function transformContainer(translate3d, animated){
			if(animated){
				addAnimation(container);
			}else{
				removeAnimation(container);
			}

			container.css(getTransforms(translate3d));

			//syncronously removing the class after the animation has been applied.
			setTimeout(function(){
				container.removeClass(NO_TRANSITION);
			},10);
		}

		/**
		* Gets a section by its anchor / index
		*/
		function getSectionByAnchor(sectionAnchor){
			//section
			var section = $(SECTION_SEL + '[data-anchor="'+sectionAnchor+'"]');
			if(!section.length){
				section = $(SECTION_SEL).eq( (sectionAnchor -1) );
			}

			return section;
		}

		/**
		* Gets a slide inside a given section by its anchor / index
		*/
		function getSlideByAnchor(slideAnchor, section){
			var slides = section.find(SLIDES_WRAPPER_SEL);
			var slide =  slides.find(SLIDE_SEL + '[data-anchor="'+slideAnchor+'"]');

			if(!slide.length){
				slide = slides.find(SLIDE_SEL).eq(slideAnchor);
			}

			return slide;
		}

		/**
		* Scrolls to the given section and slide anchors
		*/
		function scrollPageAndSlide(destiny, slide){
			var section = getSectionByAnchor(destiny);

			//default slide
			if (typeof slide === 'undefined') {
				slide = 0;
			}

			//we need to scroll to the section and then to the slide
			if (destiny !== lastScrolledDestiny && !section.hasClass(ACTIVE)){
				scrollPage(section, function(){
					scrollSlider(section, slide);
				});
			}
			//if we were already in the section
			else{
				scrollSlider(section, slide);
			}
		}

		/**
		* Scrolls the slider to the given slide destination for the given section
		*/
		function scrollSlider(section, slideAnchor){
			if(typeof slideAnchor !== 'undefined'){
				var slides = section.find(SLIDES_WRAPPER_SEL);
				var destiny =  getSlideByAnchor(slideAnchor, section);

				if(destiny.length){
					landscapeScroll(slides, destiny);
				}
			}
		}

		/**
		* Creates a landscape navigation bar with dots for horizontal sliders.
		*/
		function addSlidesNavigation(section, numSlides){
			section.append('<div class="' + SLIDES_NAV + '"><ul></ul></div>');
			var nav = section.find(SLIDES_NAV_SEL);

			//top or bottom
			nav.addClass(options.slidesNavPosition);

			for(var i=0; i< numSlides; i++){
				nav.find('ul').append('<li><a href="#"><span></span></a></li>');
			}

			//centering it
			nav.css('margin-left', '-' + (nav.width()/2) + 'px');

			nav.find('li').first().find('a').addClass(ACTIVE);
		}


		/**
		* Sets the state of the website depending on the active section/slide.
		* It changes the URL hash when needed and updates the body class.
		*/
		function setState(slideIndex, slideAnchor, anchorLink, sectionIndex){
			var sectionHash = '';

			if(options.anchors.length && !options.lockAnchors){

				//isn't it the first slide?
				if(slideIndex){
					if(typeof anchorLink !== 'undefined'){
						sectionHash = anchorLink;
					}

					//slide without anchor link? We take the index instead.
					if(typeof slideAnchor === 'undefined'){
						slideAnchor = slideIndex;
					}

					lastScrolledSlide = slideAnchor;
					setUrlHash(sectionHash + '/' + slideAnchor);

					//first slide won't have slide anchor, just the section one
				}else if(typeof slideIndex !== 'undefined'){
					lastScrolledSlide = slideAnchor;
					setUrlHash(anchorLink);
				}

				//section without slides
				else{
					setUrlHash(anchorLink);
				}
			}

			setBodyClass();
		}

		/**
		* Sets the URL hash.
		*/
		function setUrlHash(url){
			if(options.recordHistory){
				location.hash = url;
			}else{
				//Mobile Chrome doesn't work the normal way, so... lets use HTML5 for phones :)
				if(isTouchDevice || isTouch){
					history.replaceState(undefined, undefined, '#' + url);
				}else{
					var baseUrl = window.location.href.split('#')[0];
					window.location.replace( baseUrl + '#' + url );
				}
			}
		}

		/**
		* Gets the anchor for the given slide / section. Its index will be used if there's none.
		*/
		function getAnchor(element){
			var anchor = element.data('anchor');
			var index = element.index();

			//Slide without anchor link? We take the index instead.
			if(typeof anchor === 'undefined'){
				anchor = index;
			}

			return anchor;
		}

		/**
		* Sets a class for the body of the page depending on the active section / slide
		*/
		function setBodyClass(){
			var section = $(SECTION_ACTIVE_SEL);
			var slide = section.find(SLIDE_ACTIVE_SEL);

			var sectionAnchor = getAnchor(section);
			var slideAnchor = getAnchor(slide);

			var sectionIndex = section.index(SECTION_SEL);

			var text = String(sectionAnchor);

			if(slide.length){
				text = text + '-' + slideAnchor;
			}

			//changing slash for dash to make it a valid CSS style
			text = text.replace('/', '-').replace('#','');

			//removing previous anchor classes
			var classRe = new RegExp('\\b\\s?' + VIEWING_PREFIX + '-[^\\s]+\\b', "g");
			$body[0].className = $body[0].className.replace(classRe, '');

			//adding the current anchor
			$body.addClass(VIEWING_PREFIX + '-' + text);
		}

		/**
		* Checks for translate3d support
		* @return boolean
		* http://stackoverflow.com/questions/5661671/detecting-transform-translate3d-support
		*/
		function support3d() {
			var el = document.createElement('p'),
				has3d,
				transforms = {
					'webkitTransform':'-webkit-transform',
					'OTransform':'-o-transform',
					'msTransform':'-ms-transform',
					'MozTransform':'-moz-transform',
					'transform':'transform'
				};

			// Add it to the body to get the computed style.
			document.body.insertBefore(el, null);

			for (var t in transforms) {
				if (el.style[t] !== undefined) {
					el.style[t] = 'translate3d(1px,1px,1px)';
					has3d = window.getComputedStyle(el).getPropertyValue(transforms[t]);
				}
			}

			document.body.removeChild(el);

			return (has3d !== undefined && has3d.length > 0 && has3d !== 'none');
		}

		/**
		* Removes the auto scrolling action fired by the mouse wheel and trackpad.
		* After this function is called, the mousewheel and trackpad movements won't scroll through sections.
		*/
		function removeMouseWheelHandler(){
			if (document.addEventListener) {
				document.removeEventListener('mousewheel', MouseWheelHandler, false); //IE9, Chrome, Safari, Oper
				document.removeEventListener('wheel', MouseWheelHandler, false); //Firefox
				document.removeEventListener('MozMousePixelScroll', MouseWheelHandler, false); //old Firefox
			} else {
				document.detachEvent('onmousewheel', MouseWheelHandler); //IE 6/7/8
			}
		}

		/**
		* Adds the auto scrolling action for the mouse wheel and trackpad.
		* After this function is called, the mousewheel and trackpad movements will scroll through sections
		* https://developer.mozilla.org/en-US/docs/Web/Events/wheel
		*/
		function addMouseWheelHandler(){
			var prefix = '';
			var _addEventListener;

			if (window.addEventListener){
				_addEventListener = "addEventListener";
			}else{
				_addEventListener = "attachEvent";
				prefix = 'on';
			}

			// detect available wheel event
			var support = 'onwheel' in document.createElement('div') ? 'wheel' : // Modern browsers support "wheel"
				document.onmousewheel !== undefined ? 'mousewheel' : // Webkit and IE support at least "mousewheel"
				'DOMMouseScroll'; // let's assume that remaining browsers are older Firefox


			if(support == 'DOMMouseScroll'){
				document[ _addEventListener ](prefix + 'MozMousePixelScroll', MouseWheelHandler, false);
			}

			//handle MozMousePixelScroll in older Firefox
			else{
				document[ _addEventListener ](prefix + support, MouseWheelHandler, false);
			}
		}

		/**
		* Adds the possibility to auto scroll through sections on touch devices.
		*/
		function addTouchHandler(){
			if(isTouchDevice || isTouch){
				//Microsoft pointers
				var MSPointer = getMSPointer();

				$(WRAPPER_SEL).off('touchstart ' +  MSPointer.down).on('touchstart ' + MSPointer.down, touchStartHandler);
				$(WRAPPER_SEL).off('touchmove ' + MSPointer.move).on('touchmove ' + MSPointer.move, touchMoveHandler);
			}
		}

		/**
		* Removes the auto scrolling for touch devices.
		*/
		function removeTouchHandler(){
			if(isTouchDevice || isTouch){
				//Microsoft pointers
				var MSPointer = getMSPointer();

				$(WRAPPER_SEL).off('touchstart ' + MSPointer.down);
				$(WRAPPER_SEL).off('touchmove ' + MSPointer.move);
			}
		}

		/*
		* Returns and object with Microsoft pointers (for IE<11 and for IE >= 11)
		* http://msdn.microsoft.com/en-us/library/ie/dn304886(v=vs.85).aspx
		*/
		function getMSPointer(){
			var pointer;

			//IE >= 11 & rest of browsers
			if(window.PointerEvent){
				pointer = { down: 'pointerdown', move: 'pointermove'};
			}

			//IE < 11
			else{
				pointer = { down: 'MSPointerDown', move: 'MSPointerMove'};
			}

			return pointer;
		}

		/**
		* Gets the pageX and pageY properties depending on the browser.
		* https://github.com/alvarotrigo/fullPage.js/issues/194#issuecomment-34069854
		*/
		function getEventsPage(e){
			var events = [];

			events.y = (typeof e.pageY !== 'undefined' && (e.pageY || e.pageX) ? e.pageY : e.touches[0].pageY);
			events.x = (typeof e.pageX !== 'undefined' && (e.pageY || e.pageX) ? e.pageX : e.touches[0].pageX);

			//in touch devices with scrollBar:true, e.pageY is detected, but we have to deal with touch events. #1008
			if(isTouch && isReallyTouch(e) && options.scrollBar){
				events.y = e.touches[0].pageY;
				events.x = e.touches[0].pageX;
			}

			return events;
		}

		/**
		* Slides silently (with no animation) the active slider to the given slide.
		*/
		function silentLandscapeScroll(activeSlide, noCallbacks){
			FP.setScrollingSpeed (0, 'internal');

			if(typeof noCallbacks !== 'undefined'){
				//preventing firing callbacks afterSlideLoad etc.
				isResizing = true;
			}

			landscapeScroll(activeSlide.closest(SLIDES_WRAPPER_SEL), activeSlide);

			if(typeof noCallbacks !== 'undefined'){
				isResizing = false;
			}

			FP.setScrollingSpeed(originals.scrollingSpeed, 'internal');
		}

		/**
		* Scrolls silently (with no animation) the page to the given Y position.
		*/
		function silentScroll(top){
			if(options.scrollBar){
				container.scrollTop(top);
			}
			else if (options.css3) {
				var translate3d = 'translate3d(0px, -' + top + 'px, 0px)';
				transformContainer(translate3d, false);
			}
			else {
				container.css('top', -top);
			}
		}

		/**
		* Returns the cross-browser transform string.
		*/
		function getTransforms(translate3d){
			return {
				'-webkit-transform': translate3d,
				'-moz-transform': translate3d,
				'-ms-transform':translate3d,
				'transform': translate3d
			};
		}

		/**
		* Allowing or disallowing the mouse/swipe scroll in a given direction. (not for keyboard)
		* @type  m (mouse) or k (keyboard)
		*/
		function setIsScrollAllowed(value, direction, type){
			switch (direction){
			case 'up': isScrollAllowed[type].up = value; break;
			case 'down': isScrollAllowed[type].down = value; break;
			case 'left': isScrollAllowed[type].left = value; break;
			case 'right': isScrollAllowed[type].right = value; break;
			case 'all':
				if(type == 'm'){
					FP.setAllowScrolling(value);
				}else{
					FP.setKeyboardScrolling(value);
				}
			}
		}

		/*
		* Destroys fullpage.js plugin events and optinally its html markup and styles
		*/
		FP.destroy = function(all){
			FP.setAutoScrolling(false, 'internal');
			FP.setAllowScrolling(false);
			FP.setKeyboardScrolling(false);
			container.addClass(DESTROYED);

			clearTimeout(afterSlideLoadsId);
			clearTimeout(afterSectionLoadsId);
			clearTimeout(resizeId);
			clearTimeout(scrollId);
			clearTimeout(scrollId2);

			$window
				.off('scroll', scrollHandler)
				.off('hashchange', hashChangeHandler)
				.off('resize', resizeHandler);

			$document
				.off('click', SECTION_NAV_SEL + ' a')
				.off('mouseenter', SECTION_NAV_SEL + ' li')
				.off('mouseleave', SECTION_NAV_SEL + ' li')
				.off('click', SLIDES_NAV_LINK_SEL)
				.off('mouseover', options.normalScrollElements)
				.off('mouseout', options.normalScrollElements);

			$(SECTION_SEL)
				.off('click', SLIDES_ARROW_SEL);

			clearTimeout(afterSlideLoadsId);
			clearTimeout(afterSectionLoadsId);

			//lets make a mess!
			if(all){
				destroyStructure();
			}
		};

		/*
		* Removes inline styles added by fullpage.js
		*/
		function destroyStructure(){
			//reseting the `top` or `translate` properties to 0
			silentScroll(0);

			$(SECTION_NAV_SEL + ', ' + SLIDES_NAV_SEL +  ', ' + SLIDES_ARROW_SEL).remove();

			//removing inline styles
			$(SECTION_SEL).css( {
				'height': '',
				'background-color' : '',
				'padding': ''
			});

			$(SLIDE_SEL).css( {
				'width': ''
			});

			container.css({
				'height': '',
				'position': '',
				'-ms-touch-action': '',
				'touch-action': ''
			});

			$htmlBody.css({
				'overflow': '',
				'height': ''
			});

			// remove .fp-enabled class
			$('html').removeClass(ENABLED);

			// remove all of the .fp-viewing- classes
			$.each($body.get(0).className.split(/\s+/), function (index, className) {
				if (className.indexOf(VIEWING_PREFIX) === 0) {
					$body.removeClass(className);
				}
			});

			//removing added classes
			$(SECTION_SEL + ', ' + SLIDE_SEL).each(function(){
				removeSlimScroll($(this));
				$(this).removeClass(TABLE + ' ' + ACTIVE);
			});

			removeAnimation(container);

			//Unwrapping content
			container.find(TABLE_CELL_SEL + ', ' + SLIDES_CONTAINER_SEL + ', ' + SLIDES_WRAPPER_SEL).each(function(){
				//unwrap not being use in case there's no child element inside and its just text
				$(this).replaceWith(this.childNodes);
			});

			//scrolling the page to the top with no animation
			$htmlBody.scrollTop(0);
		}

		/*
		* Sets the state for a variable with multiple states (original, and temporal)
		* Some variables such as `autoScrolling` or `recordHistory` might change automatically its state when using `responsive` or `autoScrolling:false`.
		* This function is used to keep track of both states, the original and the temporal one.
		* If type is not 'internal', then we assume the user is globally changing the variable.
		*/
		function setVariableState(variable, value, type){
			options[variable] = value;
			if(type !== 'internal'){
				originals[variable] = value;
			}
		}

		/**
		* Displays warnings
		*/
		function displayWarnings(){
			// Disable mutually exclusive settings
			if (options.continuousVertical &&
				(options.loopTop || options.loopBottom)) {
				options.continuousVertical = false;
				showError('warn', 'Option `loopTop/loopBottom` is mutually exclusive with `continuousVertical`; `continuousVertical` disabled');
			}

			if(options.scrollBar && options.scrollOverflow){
				showError('warn', 'Option `scrollBar` is mutually exclusive with `scrollOverflow`. Sections with scrollOverflow might not work well in Firefox');
			}

			if(options.continuousVertical && options.scrollBar){
				options.continuousVertical = false;
				showError('warn', 'Option `scrollBar` is mutually exclusive with `continuousVertical`; `continuousVertical` disabled');
			}

			//anchors can not have the same value as any element ID or NAME
			$.each(options.anchors, function(index, name){
				if($('#' + name).length || $('[name="'+name+'"]').length ){
					showError('error', 'data-anchor tags can not have the same value as any `id` element on the site (or `name` element for IE).');
				}
			});
		}

		/**
		* Shows a message in the console of the given type.
		*/
		function showError(type, text){
			console && console[type] && console[type]('fullPage: ' + text);
		}
	};
});
/*! VelocityJS.org (1.2.2). (C) 2014 Julian Shapiro. MIT @license: en.wikipedia.org/wiki/MIT_License */

/*************************
   Velocity jQuery Shim
*************************/

/*! VelocityJS.org jQuery Shim (1.0.1). (C) 2014 The jQuery Foundation. MIT @license: en.wikipedia.org/wiki/MIT_License. */

/* This file contains the jQuery functions that Velocity relies on, thereby removing Velocity's dependency on a full copy of jQuery, and allowing it to work in any environment. */
/* These shimmed functions are only used if jQuery isn't present. If both this shim and jQuery are loaded, Velocity defaults to jQuery proper. */
/* Browser support: Using this shim instead of jQuery proper removes support for IE8. */

;(function (window) {
	/***************
	     Setup
	***************/

	/* If jQuery is already loaded, there's no point in loading this shim. */
	if (window.jQuery) {
		return;
	}

	/* jQuery base. */
	var $ = function (selector, context) {
		return new $.fn.init(selector, context);
	};

	/********************
	   Private Methods
	********************/

	/* jQuery */
	$.isWindow = function (obj) {
		/* jshint eqeqeq: false */
		return obj != null && obj == obj.window;
	};

	/* jQuery */
	$.type = function (obj) {
		if (obj == null) {
			return obj + "";
		}

		return typeof obj === "object" || typeof obj === "function" ?
			class2type[toString.call(obj)] || "object" :
			typeof obj;
	};

	/* jQuery */
	$.isArray = Array.isArray || function (obj) {
		return $.type(obj) === "array";
	};

	/* jQuery */
	function isArraylike (obj) {
		var length = obj.length,
			type = $.type(obj);

		if (type === "function" || $.isWindow(obj)) {
			return false;
		}

		if (obj.nodeType === 1 && length) {
			return true;
		}

		return type === "array" || length === 0 || typeof length === "number" && length > 0 && (length - 1) in obj;
	}

	/***************
	   $ Methods
	***************/

	/* jQuery: Support removed for IE<9. */
	$.isPlainObject = function (obj) {
		var key;

		if (!obj || $.type(obj) !== "object" || obj.nodeType || $.isWindow(obj)) {
			return false;
		}

		try {
			if (obj.constructor &&
				!hasOwn.call(obj, "constructor") &&
				!hasOwn.call(obj.constructor.prototype, "isPrototypeOf")) {
				return false;
			}
		} catch (e) {
			return false;
		}

		for (key in obj) {}

		return key === undefined || hasOwn.call(obj, key);
	};

	/* jQuery */
	$.each = function(obj, callback, args) {
		var value,
			i = 0,
			length = obj.length,
			isArray = isArraylike(obj);

		if (args) {
			if (isArray) {
				for (; i < length; i++) {
					value = callback.apply(obj[i], args);

					if (value === false) {
						break;
					}
				}
			} else {
				for (i in obj) {
					value = callback.apply(obj[i], args);

					if (value === false) {
						break;
					}
				}
			}

		} else {
			if (isArray) {
				for (; i < length; i++) {
					value = callback.call(obj[i], i, obj[i]);

					if (value === false) {
						break;
					}
				}
			} else {
				for (i in obj) {
					value = callback.call(obj[i], i, obj[i]);

					if (value === false) {
						break;
					}
				}
			}
		}

		return obj;
	};

	/* Custom */
	$.data = function (node, key, value) {
		/* $.getData() */
		if (value === undefined) {
			var id = node[$.expando],
				store = id && cache[id];

			if (key === undefined) {
				return store;
			} else if (store) {
				if (key in store) {
					return store[key];
				}
			}
			/* $.setData() */
		} else if (key !== undefined) {
			var id = node[$.expando] || (node[$.expando] = ++$.uuid);

			cache[id] = cache[id] || {};
			cache[id][key] = value;

			return value;
		}
	};

	/* Custom */
	$.removeData = function (node, keys) {
		var id = node[$.expando],
			store = id && cache[id];

		if (store) {
			$.each(keys, function(_, key) {
				delete store[key];
			});
		}
	};

	/* jQuery */
	$.extend = function () {
		var src, copyIsArray, copy, name, options, clone,
			target = arguments[0] || {},
			i = 1,
			length = arguments.length,
			deep = false;

		if (typeof target === "boolean") {
			deep = target;

			target = arguments[i] || {};
			i++;
		}

		if (typeof target !== "object" && $.type(target) !== "function") {
			target = {};
		}

		if (i === length) {
			target = this;
			i--;
		}

		for (; i < length; i++) {
			if ((options = arguments[i]) != null) {
				for (name in options) {
					src = target[name];
					copy = options[name];

					if (target === copy) {
						continue;
					}

					if (deep && copy && ($.isPlainObject(copy) || (copyIsArray = $.isArray(copy)))) {
						if (copyIsArray) {
							copyIsArray = false;
							clone = src && $.isArray(src) ? src : [];

						} else {
							clone = src && $.isPlainObject(src) ? src : {};
						}

						target[name] = $.extend(deep, clone, copy);

					} else if (copy !== undefined) {
						target[name] = copy;
					}
				}
			}
		}

		return target;
	};

	/* jQuery 1.4.3 */
	$.queue = function (elem, type, data) {
		function $makeArray (arr, results) {
			var ret = results || [];

			if (arr != null) {
				if (isArraylike(Object(arr))) {
					/* $.merge */
					(function(first, second) {
						var len = +second.length,
							j = 0,
							i = first.length;

						while (j < len) {
							first[i++] = second[j++];
						}

						if (len !== len) {
							while (second[j] !== undefined) {
								first[i++] = second[j++];
							}
						}

						first.length = i;

						return first;
					})(ret, typeof arr === "string" ? [arr] : arr);
				} else {
					[].push.call(ret, arr);
				}
			}

			return ret;
		}

		if (!elem) {
			return;
		}

		type = (type || "fx") + "queue";

		var q = $.data(elem, type);

		if (!data) {
			return q || [];
		}

		if (!q || $.isArray(data)) {
			q = $.data(elem, type, $makeArray(data));
		} else {
			q.push(data);
		}

		return q;
	};

	/* jQuery 1.4.3 */
	$.dequeue = function (elems, type) {
		/* Custom: Embed element iteration. */
		$.each(elems.nodeType ? [ elems ] : elems, function(i, elem) {
			type = type || "fx";

			var queue = $.queue(elem, type),
				fn = queue.shift();

			if (fn === "inprogress") {
				fn = queue.shift();
			}

			if (fn) {
				if (type === "fx") {
					queue.unshift("inprogress");
				}

				fn.call(elem, function() {
					$.dequeue(elem, type);
				});
			}
		});
	};

	/******************
	   $.fn Methods
	******************/

	/* jQuery */
	$.fn = $.prototype = {
		init: function (selector) {
			/* Just return the element wrapped inside an array; don't proceed with the actual jQuery node wrapping process. */
			if (selector.nodeType) {
				this[0] = selector;

				return this;
			} else {
				throw new Error("Not a DOM node.");
			}
		},

		offset: function () {
			/* jQuery altered code: Dropped disconnected DOM node checking. */
			var box = this[0].getBoundingClientRect ? this[0].getBoundingClientRect() : { top: 0, left: 0 };

			return {
				top: box.top + (window.pageYOffset || document.scrollTop  || 0)  - (document.clientTop  || 0),
				left: box.left + (window.pageXOffset || document.scrollLeft  || 0) - (document.clientLeft || 0)
			};
		},

		position: function () {
			/* jQuery */
			function offsetParent() {
				var offsetParent = this.offsetParent || document;

				while (offsetParent && (!offsetParent.nodeType.toLowerCase === "html" && offsetParent.style.position === "static")) {
					offsetParent = offsetParent.offsetParent;
				}

				return offsetParent || document;
			}

			/* Zepto */
			var elem = this[0],
				offsetParent = offsetParent.apply(elem),
				offset = this.offset(),
				parentOffset = /^(?:body|html)$/i.test(offsetParent.nodeName) ? { top: 0, left: 0 } : $(offsetParent).offset()

			offset.top -= parseFloat(elem.style.marginTop) || 0;
			offset.left -= parseFloat(elem.style.marginLeft) || 0;

			if (offsetParent.style) {
				parentOffset.top += parseFloat(offsetParent.style.borderTopWidth) || 0
				parentOffset.left += parseFloat(offsetParent.style.borderLeftWidth) || 0
			}

			return {
				top: offset.top - parentOffset.top,
				left: offset.left - parentOffset.left
			};
		}
	};

	/**********************
	   Private Variables
	**********************/

	/* For $.data() */
	var cache = {};
	$.expando = "velocity" + (new Date().getTime());
	$.uuid = 0;

	/* For $.queue() */
	var class2type = {},
		hasOwn = class2type.hasOwnProperty,
		toString = class2type.toString;

	var types = "Boolean Number String Function Array Date RegExp Object Error".split(" ");
	for (var i = 0; i < types.length; i++) {
		class2type["[object " + types[i] + "]"] = types[i].toLowerCase();
	}

	/* Makes $(node) possible, without having to call init. */
	$.fn.init.prototype = $.fn;

	/* Globalize Velocity onto the window, and assign its Utilities property. */
	window.Velocity = { Utilities: $ };
})(window);

/******************
    Velocity.js
******************/

;(function (factory) {
	/* CommonJS module. */
	if (typeof module === "object" && typeof module.exports === "object") {
		module.exports = factory();
		/* AMD module. */
	} else if (typeof define === "function" && define.amd) {
		define(factory);
		/* Browser globals. */
	} else {
		factory();
	}
}(function() {
	return function (global, window, document, undefined) {

		/***************
		    Summary
		***************/

		/*
		- CSS: CSS stack that works independently from the rest of Velocity.
		- animate(): Core animation method that iterates over the targeted elements and queues the incoming call onto each element individually.
		  - Pre-Queueing: Prepare the element for animation by instantiating its data cache and processing the call's options.
		  - Queueing: The logic that runs once the call has reached its point of execution in the element's $.queue() stack.
		              Most logic is placed here to avoid risking it becoming stale (if the element's properties have changed).
		  - Pushing: Consolidation of the tween data followed by its push onto the global in-progress calls container.
		- tick(): The single requestAnimationFrame loop responsible for tweening all in-progress calls.
		- completeCall(): Handles the cleanup process for each Velocity call.
		*/

		/*********************
		   Helper Functions
		*********************/

		/* IE detection. Gist: https://gist.github.com/julianshapiro/9098609 */
		var IE = (function() {
			if (document.documentMode) {
				return document.documentMode;
			} else {
				for (var i = 7; i > 4; i--) {
					var div = document.createElement("div");

					div.innerHTML = "<!--[if IE " + i + "]><span></span><![endif]-->";

					if (div.getElementsByTagName("span").length) {
						div = null;

						return i;
					}
				}
			}

			return undefined;
		})();

		/* rAF shim. Gist: https://gist.github.com/julianshapiro/9497513 */
		var rAFShim = (function() {
			var timeLast = 0;

			return window.webkitRequestAnimationFrame || window.mozRequestAnimationFrame || function(callback) {
				var timeCurrent = (new Date()).getTime(),
					timeDelta;

				/* Dynamically set delay on a per-tick basis to match 60fps. */
				/* Technique by Erik Moller. MIT license: https://gist.github.com/paulirish/1579671 */
				timeDelta = Math.max(0, 16 - (timeCurrent - timeLast));
				timeLast = timeCurrent + timeDelta;

				return setTimeout(function() { callback(timeCurrent + timeDelta); }, timeDelta);
			};
		})();

		/* Array compacting. Copyright Lo-Dash. MIT License: https://github.com/lodash/lodash/blob/master/LICENSE.txt */
		function compactSparseArray (array) {
			var index = -1,
				length = array ? array.length : 0,
				result = [];

			while (++index < length) {
				var value = array[index];

				if (value) {
					result.push(value);
				}
			}

			return result;
		}

		function sanitizeElements (elements) {
			/* Unwrap jQuery/Zepto objects. */
			if (Type.isWrapped(elements)) {
				elements = [].slice.call(elements);
				/* Wrap a single element in an array so that $.each() can iterate with the element instead of its node's children. */
			} else if (Type.isNode(elements)) {
				elements = [ elements ];
			}

			return elements;
		}

		var Type = {
			isString: function (variable) {
				return (typeof variable === "string");
			},
			isArray: Array.isArray || function (variable) {
				return Object.prototype.toString.call(variable) === "[object Array]";
			},
			isFunction: function (variable) {
				return Object.prototype.toString.call(variable) === "[object Function]";
			},
			isNode: function (variable) {
				return variable && variable.nodeType;
			},
			/* Copyright Martin Bohm. MIT License: https://gist.github.com/Tomalak/818a78a226a0738eaade */
			isNodeList: function (variable) {
				return typeof variable === "object" &&
					/^\[object (HTMLCollection|NodeList|Object)\]$/.test(Object.prototype.toString.call(variable)) &&
					variable.length !== undefined &&
					(variable.length === 0 || (typeof variable[0] === "object" && variable[0].nodeType > 0));
			},
			/* Determine if variable is a wrapped jQuery or Zepto element. */
			isWrapped: function (variable) {
				return variable && (variable.jquery || (window.Zepto && window.Zepto.zepto.isZ(variable)));
			},
			isSVG: function (variable) {
				return window.SVGElement && (variable instanceof window.SVGElement);
			},
			isEmptyObject: function (variable) {
				for (var name in variable) {
					return false;
				}

				return true;
			}
		};

		/*****************
		   Dependencies
		*****************/

		var $,
			isJQuery = false;

		if (global.fn && global.fn.jquery) {
			$ = global;
			isJQuery = true;
		} else {
			$ = window.Velocity.Utilities;
		}

		if (IE <= 8 && !isJQuery) {
			throw new Error("Velocity: IE8 and below require jQuery to be loaded before Velocity.");
		} else if (IE <= 7) {
			/* Revert to jQuery's $.animate(), and lose Velocity's extra features. */
			jQuery.fn.velocity = jQuery.fn.animate;

			/* Now that $.fn.velocity is aliased, abort this Velocity declaration. */
			return;
		}

		/*****************
		    Constants
		*****************/

		var DURATION_DEFAULT = 400,
			EASING_DEFAULT = "swing";

		/*************
		    State
		*************/

		var Velocity = {
			/* Container for page-wide Velocity state data. */
			State: {
				/* Detect mobile devices to determine if mobileHA should be turned on. */
				isMobile: /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent),
				/* The mobileHA option's behavior changes on older Android devices (Gingerbread, versions 2.3.3-2.3.7). */
				isAndroid: /Android/i.test(navigator.userAgent),
				isGingerbread: /Android 2\.3\.[3-7]/i.test(navigator.userAgent),
				isChrome: window.chrome,
				isFirefox: /Firefox/i.test(navigator.userAgent),
				/* Create a cached element for re-use when checking for CSS property prefixes. */
				prefixElement: document.createElement("div"),
				/* Cache every prefix match to avoid repeating lookups. */
				prefixMatches: {},
				/* Cache the anchor used for animating window scrolling. */
				scrollAnchor: null,
				/* Cache the browser-specific property names associated with the scroll anchor. */
				scrollPropertyLeft: null,
				scrollPropertyTop: null,
				/* Keep track of whether our RAF tick is running. */
				isTicking: false,
				/* Container for every in-progress call to Velocity. */
				calls: []
			},
			/* Velocity's custom CSS stack. Made global for unit testing. */
			CSS: { /* Defined below. */ },
			/* A shim of the jQuery utility functions used by Velocity -- provided by Velocity's optional jQuery shim. */
			Utilities: $,
			/* Container for the user's custom animation redirects that are referenced by name in place of the properties map argument. */
			Redirects: { /* Manually registered by the user. */ },
			Easings: { /* Defined below. */ },
			/* Attempt to use ES6 Promises by default. Users can override this with a third-party promises library. */
			Promise: window.Promise,
			/* Velocity option defaults, which can be overriden by the user. */
			defaults: {
				queue: "",
				duration: DURATION_DEFAULT,
				easing: EASING_DEFAULT,
				begin: undefined,
				complete: undefined,
				progress: undefined,
				display: undefined,
				visibility: undefined,
				loop: false,
				delay: false,
				mobileHA: true,
				/* Advanced: Set to false to prevent property values from being cached between consecutive Velocity-initiated chain calls. */
				_cacheValues: true
			},
			/* A design goal of Velocity is to cache data wherever possible in order to avoid DOM requerying. Accordingly, each element has a data cache. */
			init: function (element) {
				$.data(element, "velocity", {
					/* Store whether this is an SVG element, since its properties are retrieved and updated differently than standard HTML elements. */
					isSVG: Type.isSVG(element),
					/* Keep track of whether the element is currently being animated by Velocity.
					   This is used to ensure that property values are not transferred between non-consecutive (stale) calls. */
					isAnimating: false,
					/* A reference to the element's live computedStyle object. Learn more here: https://developer.mozilla.org/en/docs/Web/API/window.getComputedStyle */
					computedStyle: null,
					/* Tween data is cached for each animation on the element so that data can be passed across calls --
					   in particular, end values are used as subsequent start values in consecutive Velocity calls. */
					tweensContainer: null,
					/* The full root property values of each CSS hook being animated on this element are cached so that:
					   1) Concurrently-animating hooks sharing the same root can have their root values' merged into one while tweening.
					   2) Post-hook-injection root values can be transferred over to consecutively chained Velocity calls as starting root values. */
					rootPropertyValueCache: {},
					/* A cache for transform updates, which must be manually flushed via CSS.flushTransformCache(). */
					transformCache: {}
				});
			},
			/* A parallel to jQuery's $.css(), used for getting/setting Velocity's hooked CSS properties. */
			hook: null, /* Defined below. */
			/* Velocity-wide animation time remapping for testing purposes. */
			mock: false,
			version: { major: 1, minor: 2, patch: 2 },
			/* Set to 1 or 2 (most verbose) to output debug info to console. */
			debug: false
		};

		/* Retrieve the appropriate scroll anchor and property name for the browser: https://developer.mozilla.org/en-US/docs/Web/API/Window.scrollY */
		if (window.pageYOffset !== undefined) {
			Velocity.State.scrollAnchor = window;
			Velocity.State.scrollPropertyLeft = "pageXOffset";
			Velocity.State.scrollPropertyTop = "pageYOffset";
		} else {
			Velocity.State.scrollAnchor = document.documentElement || document.body.parentNode || document.body;
			Velocity.State.scrollPropertyLeft = "scrollLeft";
			Velocity.State.scrollPropertyTop = "scrollTop";
		}

		/* Shorthand alias for jQuery's $.data() utility. */
		function Data (element) {
			/* Hardcode a reference to the plugin name. */
			var response = $.data(element, "velocity");

			/* jQuery <=1.4.2 returns null instead of undefined when no match is found. We normalize this behavior. */
			return response === null ? undefined : response;
		};

		/**************
		    Easing
		**************/

		/* Step easing generator. */
		function generateStep (steps) {
			return function (p) {
				return Math.round(p * steps) * (1 / steps);
			};
		}

		/* Bezier curve function generator. Copyright Gaetan Renaudeau. MIT License: http://en.wikipedia.org/wiki/MIT_License */
		function generateBezier (mX1, mY1, mX2, mY2) {
			var NEWTON_ITERATIONS = 4,
				NEWTON_MIN_SLOPE = 0.001,
				SUBDIVISION_PRECISION = 0.0000001,
				SUBDIVISION_MAX_ITERATIONS = 10,
				kSplineTableSize = 11,
				kSampleStepSize = 1.0 / (kSplineTableSize - 1.0),
				float32ArraySupported = "Float32Array" in window;

			/* Must contain four arguments. */
			if (arguments.length !== 4) {
				return false;
			}

			/* Arguments must be numbers. */
			for (var i = 0; i < 4; ++i) {
				if (typeof arguments[i] !== "number" || isNaN(arguments[i]) || !isFinite(arguments[i])) {
					return false;
				}
			}

			/* X values must be in the [0, 1] range. */
			mX1 = Math.min(mX1, 1);
			mX2 = Math.min(mX2, 1);
			mX1 = Math.max(mX1, 0);
			mX2 = Math.max(mX2, 0);

			var mSampleValues = float32ArraySupported ? new Float32Array(kSplineTableSize) : new Array(kSplineTableSize);

			function A (aA1, aA2) { return 1.0 - 3.0 * aA2 + 3.0 * aA1; }
			function B (aA1, aA2) { return 3.0 * aA2 - 6.0 * aA1; }
			function C (aA1)      { return 3.0 * aA1; }

			function calcBezier (aT, aA1, aA2) {
				return ((A(aA1, aA2)*aT + B(aA1, aA2))*aT + C(aA1))*aT;
			}

			function getSlope (aT, aA1, aA2) {
				return 3.0 * A(aA1, aA2)*aT*aT + 2.0 * B(aA1, aA2) * aT + C(aA1);
			}

			function newtonRaphsonIterate (aX, aGuessT) {
				for (var i = 0; i < NEWTON_ITERATIONS; ++i) {
					var currentSlope = getSlope(aGuessT, mX1, mX2);

					if (currentSlope === 0.0) return aGuessT;

					var currentX = calcBezier(aGuessT, mX1, mX2) - aX;
					aGuessT -= currentX / currentSlope;
				}

				return aGuessT;
			}

			function calcSampleValues () {
				for (var i = 0; i < kSplineTableSize; ++i) {
					mSampleValues[i] = calcBezier(i * kSampleStepSize, mX1, mX2);
				}
			}

			function binarySubdivide (aX, aA, aB) {
				var currentX, currentT, i = 0;

				do {
					currentT = aA + (aB - aA) / 2.0;
					currentX = calcBezier(currentT, mX1, mX2) - aX;
					if (currentX > 0.0) {
						aB = currentT;
					} else {
						aA = currentT;
					}
				} while (Math.abs(currentX) > SUBDIVISION_PRECISION && ++i < SUBDIVISION_MAX_ITERATIONS);

				return currentT;
			}

			function getTForX (aX) {
				var intervalStart = 0.0,
					currentSample = 1,
					lastSample = kSplineTableSize - 1;

				for (; currentSample != lastSample && mSampleValues[currentSample] <= aX; ++currentSample) {
					intervalStart += kSampleStepSize;
				}

				--currentSample;

				var dist = (aX - mSampleValues[currentSample]) / (mSampleValues[currentSample+1] - mSampleValues[currentSample]),
					guessForT = intervalStart + dist * kSampleStepSize,
					initialSlope = getSlope(guessForT, mX1, mX2);

				if (initialSlope >= NEWTON_MIN_SLOPE) {
					return newtonRaphsonIterate(aX, guessForT);
				} else if (initialSlope == 0.0) {
					return guessForT;
				} else {
					return binarySubdivide(aX, intervalStart, intervalStart + kSampleStepSize);
				}
			}

			var _precomputed = false;

			function precompute() {
				_precomputed = true;
				if (mX1 != mY1 || mX2 != mY2) calcSampleValues();
			}

			var f = function (aX) {
				if (!_precomputed) precompute();
				if (mX1 === mY1 && mX2 === mY2) return aX;
				if (aX === 0) return 0;
				if (aX === 1) return 1;

				return calcBezier(getTForX(aX), mY1, mY2);
			};

			f.getControlPoints = function() { return [{ x: mX1, y: mY1 }, { x: mX2, y: mY2 }]; };

			var str = "generateBezier(" + [mX1, mY1, mX2, mY2] + ")";
			f.toString = function () { return str; };

			return f;
		}

		/* Runge-Kutta spring physics function generator. Adapted from Framer.js, copyright Koen Bok. MIT License: http://en.wikipedia.org/wiki/MIT_License */
		/* Given a tension, friction, and duration, a simulation at 60FPS will first run without a defined duration in order to calculate the full path. A second pass
		   then adjusts the time delta -- using the relation between actual time and duration -- to calculate the path for the duration-constrained animation. */
		var generateSpringRK4 = (function () {
			function springAccelerationForState (state) {
				return (-state.tension * state.x) - (state.friction * state.v);
			}

			function springEvaluateStateWithDerivative (initialState, dt, derivative) {
				var state = {
					x: initialState.x + derivative.dx * dt,
					v: initialState.v + derivative.dv * dt,
					tension: initialState.tension,
					friction: initialState.friction
				};

				return { dx: state.v, dv: springAccelerationForState(state) };
			}

			function springIntegrateState (state, dt) {
				var a = {
					    dx: state.v,
					    dv: springAccelerationForState(state)
				    },
					b = springEvaluateStateWithDerivative(state, dt * 0.5, a),
					c = springEvaluateStateWithDerivative(state, dt * 0.5, b),
					d = springEvaluateStateWithDerivative(state, dt, c),
					dxdt = 1.0 / 6.0 * (a.dx + 2.0 * (b.dx + c.dx) + d.dx),
					dvdt = 1.0 / 6.0 * (a.dv + 2.0 * (b.dv + c.dv) + d.dv);

				state.x = state.x + dxdt * dt;
				state.v = state.v + dvdt * dt;

				return state;
			}

			return function springRK4Factory (tension, friction, duration) {

				var initState = {
					    x: -1,
					    v: 0,
					    tension: null,
					    friction: null
				    },
					path = [0],
					time_lapsed = 0,
					tolerance = 1 / 10000,
					DT = 16 / 1000,
					have_duration, dt, last_state;

				tension = parseFloat(tension) || 500;
				friction = parseFloat(friction) || 20;
				duration = duration || null;

				initState.tension = tension;
				initState.friction = friction;

				have_duration = duration !== null;

				/* Calculate the actual time it takes for this animation to complete with the provided conditions. */
				if (have_duration) {
					/* Run the simulation without a duration. */
					time_lapsed = springRK4Factory(tension, friction);
					/* Compute the adjusted time delta. */
					dt = time_lapsed / duration * DT;
				} else {
					dt = DT;
				}

				while (true) {
					/* Next/step function .*/
					last_state = springIntegrateState(last_state || initState, dt);
					/* Store the position. */
					path.push(1 + last_state.x);
					time_lapsed += 16;
					/* If the change threshold is reached, break. */
					if (!(Math.abs(last_state.x) > tolerance && Math.abs(last_state.v) > tolerance)) {
						break;
					}
				}

				/* If duration is not defined, return the actual time required for completing this animation. Otherwise, return a closure that holds the
				   computed path and returns a snapshot of the position according to a given percentComplete. */
				return !have_duration ? time_lapsed : function(percentComplete) { return path[ (percentComplete * (path.length - 1)) | 0 ]; };
			};
		}());

		/* jQuery easings. */
		Velocity.Easings = {
			linear: function(p) { return p; },
			swing: function(p) { return 0.5 - Math.cos( p * Math.PI ) / 2 },
			/* Bonus "spring" easing, which is a less exaggerated version of easeInOutElastic. */
			spring: function(p) { return 1 - (Math.cos(p * 4.5 * Math.PI) * Math.exp(-p * 6)); }
		};

		/* CSS3 and Robert Penner easings. */
		$.each(
			[
				[ "ease", [ 0.25, 0.1, 0.25, 1.0 ] ],
				[ "ease-in", [ 0.42, 0.0, 1.00, 1.0 ] ],
				[ "ease-out", [ 0.00, 0.0, 0.58, 1.0 ] ],
				[ "ease-in-out", [ 0.42, 0.0, 0.58, 1.0 ] ],
				[ "easeInSine", [ 0.47, 0, 0.745, 0.715 ] ],
				[ "easeOutSine", [ 0.39, 0.575, 0.565, 1 ] ],
				[ "easeInOutSine", [ 0.445, 0.05, 0.55, 0.95 ] ],
				[ "easeInQuad", [ 0.55, 0.085, 0.68, 0.53 ] ],
				[ "easeOutQuad", [ 0.25, 0.46, 0.45, 0.94 ] ],
				[ "easeInOutQuad", [ 0.455, 0.03, 0.515, 0.955 ] ],
				[ "easeInCubic", [ 0.55, 0.055, 0.675, 0.19 ] ],
				[ "easeOutCubic", [ 0.215, 0.61, 0.355, 1 ] ],
				[ "easeInOutCubic", [ 0.645, 0.045, 0.355, 1 ] ],
				[ "easeInQuart", [ 0.895, 0.03, 0.685, 0.22 ] ],
				[ "easeOutQuart", [ 0.165, 0.84, 0.44, 1 ] ],
				[ "easeInOutQuart", [ 0.77, 0, 0.175, 1 ] ],
				[ "easeInQuint", [ 0.755, 0.05, 0.855, 0.06 ] ],
				[ "easeOutQuint", [ 0.23, 1, 0.32, 1 ] ],
				[ "easeInOutQuint", [ 0.86, 0, 0.07, 1 ] ],
				[ "easeInExpo", [ 0.95, 0.05, 0.795, 0.035 ] ],
				[ "easeOutExpo", [ 0.19, 1, 0.22, 1 ] ],
				[ "easeInOutExpo", [ 1, 0, 0, 1 ] ],
				[ "easeInCirc", [ 0.6, 0.04, 0.98, 0.335 ] ],
				[ "easeOutCirc", [ 0.075, 0.82, 0.165, 1 ] ],
				[ "easeInOutCirc", [ 0.785, 0.135, 0.15, 0.86 ] ]
			], function(i, easingArray) {
				Velocity.Easings[easingArray[0]] = generateBezier.apply(null, easingArray[1]);
			});

		/* Determine the appropriate easing type given an easing input. */
		function getEasing(value, duration) {
			var easing = value;

			/* The easing option can either be a string that references a pre-registered easing,
			   or it can be a two-/four-item array of integers to be converted into a bezier/spring function. */
			if (Type.isString(value)) {
				/* Ensure that the easing has been assigned to jQuery's Velocity.Easings object. */
				if (!Velocity.Easings[value]) {
					easing = false;
				}
			} else if (Type.isArray(value) && value.length === 1) {
				easing = generateStep.apply(null, value);
			} else if (Type.isArray(value) && value.length === 2) {
				/* springRK4 must be passed the animation's duration. */
				/* Note: If the springRK4 array contains non-numbers, generateSpringRK4() returns an easing
				   function generated with default tension and friction values. */
				easing = generateSpringRK4.apply(null, value.concat([ duration ]));
			} else if (Type.isArray(value) && value.length === 4) {
				/* Note: If the bezier array contains non-numbers, generateBezier() returns false. */
				easing = generateBezier.apply(null, value);
			} else {
				easing = false;
			}

			/* Revert to the Velocity-wide default easing type, or fall back to "swing" (which is also jQuery's default)
			   if the Velocity-wide default has been incorrectly modified. */
			if (easing === false) {
				if (Velocity.Easings[Velocity.defaults.easing]) {
					easing = Velocity.defaults.easing;
				} else {
					easing = EASING_DEFAULT;
				}
			}

			return easing;
		}

		/*****************
		    CSS Stack
		*****************/

		/* The CSS object is a highly condensed and performant CSS stack that fully replaces jQuery's.
		   It handles the validation, getting, and setting of both standard CSS properties and CSS property hooks. */
		/* Note: A "CSS" shorthand is aliased so that our code is easier to read. */
		var CSS = Velocity.CSS = {

			/*************
			    RegEx
			*************/

			RegEx: {
				isHex: /^#([A-f\d]{3}){1,2}$/i,
				/* Unwrap a property value's surrounding text, e.g. "rgba(4, 3, 2, 1)" ==> "4, 3, 2, 1" and "rect(4px 3px 2px 1px)" ==> "4px 3px 2px 1px". */
				valueUnwrap: /^[A-z]+\((.*)\)$/i,
				wrappedValueAlreadyExtracted: /[0-9.]+ [0-9.]+ [0-9.]+( [0-9.]+)?/,
				/* Split a multi-value property into an array of subvalues, e.g. "rgba(4, 3, 2, 1) 4px 3px 2px 1px" ==> [ "rgba(4, 3, 2, 1)", "4px", "3px", "2px", "1px" ]. */
				valueSplit: /([A-z]+\(.+\))|(([A-z0-9#-.]+?)(?=\s|$))/ig
			},

			/************
			    Lists
			************/

			Lists: {
				colors: [ "fill", "stroke", "stopColor", "color", "backgroundColor", "borderColor", "borderTopColor", "borderRightColor", "borderBottomColor", "borderLeftColor", "outlineColor" ],
				transformsBase: [ "translateX", "translateY", "scale", "scaleX", "scaleY", "skewX", "skewY", "rotateZ" ],
				transforms3D: [ "transformPerspective", "translateZ", "scaleZ", "rotateX", "rotateY" ]
			},

			/************
			    Hooks
			************/

			/* Hooks allow a subproperty (e.g. "boxShadowBlur") of a compound-value CSS property
			   (e.g. "boxShadow: X Y Blur Spread Color") to be animated as if it were a discrete property. */
			/* Note: Beyond enabling fine-grained property animation, hooking is necessary since Velocity only
			   tweens properties with single numeric values; unlike CSS transitions, Velocity does not interpolate compound-values. */
			Hooks: {
				/********************
				    Registration
				********************/

				/* Templates are a concise way of indicating which subproperties must be individually registered for each compound-value CSS property. */
				/* Each template consists of the compound-value's base name, its constituent subproperty names, and those subproperties' default values. */
				templates: {
					"textShadow": [ "Color X Y Blur", "black 0px 0px 0px" ],
					"boxShadow": [ "Color X Y Blur Spread", "black 0px 0px 0px 0px" ],
					"clip": [ "Top Right Bottom Left", "0px 0px 0px 0px" ],
					"backgroundPosition": [ "X Y", "0% 0%" ],
					"transformOrigin": [ "X Y Z", "50% 50% 0px" ],
					"perspectiveOrigin": [ "X Y", "50% 50%" ]
				},

				/* A "registered" hook is one that has been converted from its template form into a live,
				   tweenable property. It contains data to associate it with its root property. */
				registered: {
					/* Note: A registered hook looks like this ==> textShadowBlur: [ "textShadow", 3 ],
					   which consists of the subproperty's name, the associated root property's name,
					   and the subproperty's position in the root's value. */
				},
				/* Convert the templates into individual hooks then append them to the registered object above. */
				register: function () {
					/* Color hooks registration: Colors are defaulted to white -- as opposed to black -- since colors that are
					   currently set to "transparent" default to their respective template below when color-animated,
					   and white is typically a closer match to transparent than black is. An exception is made for text ("color"),
					   which is almost always set closer to black than white. */
					for (var i = 0; i < CSS.Lists.colors.length; i++) {
						var rgbComponents = (CSS.Lists.colors[i] === "color") ? "0 0 0 1" : "255 255 255 1";
						CSS.Hooks.templates[CSS.Lists.colors[i]] = [ "Red Green Blue Alpha", rgbComponents ];
					}

					var rootProperty,
						hookTemplate,
						hookNames;

					/* In IE, color values inside compound-value properties are positioned at the end the value instead of at the beginning.
					   Thus, we re-arrange the templates accordingly. */
					if (IE) {
						for (rootProperty in CSS.Hooks.templates) {
							hookTemplate = CSS.Hooks.templates[rootProperty];
							hookNames = hookTemplate[0].split(" ");

							var defaultValues = hookTemplate[1].match(CSS.RegEx.valueSplit);

							if (hookNames[0] === "Color") {
								/* Reposition both the hook's name and its default value to the end of their respective strings. */
								hookNames.push(hookNames.shift());
								defaultValues.push(defaultValues.shift());

								/* Replace the existing template for the hook's root property. */
								CSS.Hooks.templates[rootProperty] = [ hookNames.join(" "), defaultValues.join(" ") ];
							}
						}
					}

					/* Hook registration. */
					for (rootProperty in CSS.Hooks.templates) {
						hookTemplate = CSS.Hooks.templates[rootProperty];
						hookNames = hookTemplate[0].split(" ");

						for (var i in hookNames) {
							var fullHookName = rootProperty + hookNames[i],
								hookPosition = i;

							/* For each hook, register its full name (e.g. textShadowBlur) with its root property (e.g. textShadow)
							   and the hook's position in its template's default value string. */
							CSS.Hooks.registered[fullHookName] = [ rootProperty, hookPosition ];
						}
					}
				},

				/*****************************
				   Injection and Extraction
				*****************************/

				/* Look up the root property associated with the hook (e.g. return "textShadow" for "textShadowBlur"). */
				/* Since a hook cannot be set directly (the browser won't recognize it), style updating for hooks is routed through the hook's root property. */
				getRoot: function (property) {
					var hookData = CSS.Hooks.registered[property];

					if (hookData) {
						return hookData[0];
					} else {
						/* If there was no hook match, return the property name untouched. */
						return property;
					}
				},
				/* Convert any rootPropertyValue, null or otherwise, into a space-delimited list of hook values so that
				   the targeted hook can be injected or extracted at its standard position. */
				cleanRootPropertyValue: function(rootProperty, rootPropertyValue) {
					/* If the rootPropertyValue is wrapped with "rgb()", "clip()", etc., remove the wrapping to normalize the value before manipulation. */
					if (CSS.RegEx.valueUnwrap.test(rootPropertyValue)) {
						rootPropertyValue = rootPropertyValue.match(CSS.RegEx.valueUnwrap)[1];
					}

					/* If rootPropertyValue is a CSS null-value (from which there's inherently no hook value to extract),
					   default to the root's default value as defined in CSS.Hooks.templates. */
					/* Note: CSS null-values include "none", "auto", and "transparent". They must be converted into their
					   zero-values (e.g. textShadow: "none" ==> textShadow: "0px 0px 0px black") for hook manipulation to proceed. */
					if (CSS.Values.isCSSNullValue(rootPropertyValue)) {
						rootPropertyValue = CSS.Hooks.templates[rootProperty][1];
					}

					return rootPropertyValue;
				},
				/* Extracted the hook's value from its root property's value. This is used to get the starting value of an animating hook. */
				extractValue: function (fullHookName, rootPropertyValue) {
					var hookData = CSS.Hooks.registered[fullHookName];

					if (hookData) {
						var hookRoot = hookData[0],
							hookPosition = hookData[1];

						rootPropertyValue = CSS.Hooks.cleanRootPropertyValue(hookRoot, rootPropertyValue);

						/* Split rootPropertyValue into its constituent hook values then grab the desired hook at its standard position. */
						return rootPropertyValue.toString().match(CSS.RegEx.valueSplit)[hookPosition];
					} else {
						/* If the provided fullHookName isn't a registered hook, return the rootPropertyValue that was passed in. */
						return rootPropertyValue;
					}
				},
				/* Inject the hook's value into its root property's value. This is used to piece back together the root property
				   once Velocity has updated one of its individually hooked values through tweening. */
				injectValue: function (fullHookName, hookValue, rootPropertyValue) {
					var hookData = CSS.Hooks.registered[fullHookName];

					if (hookData) {
						var hookRoot = hookData[0],
							hookPosition = hookData[1],
							rootPropertyValueParts,
							rootPropertyValueUpdated;

						rootPropertyValue = CSS.Hooks.cleanRootPropertyValue(hookRoot, rootPropertyValue);

						/* Split rootPropertyValue into its individual hook values, replace the targeted value with hookValue,
						   then reconstruct the rootPropertyValue string. */
						rootPropertyValueParts = rootPropertyValue.toString().match(CSS.RegEx.valueSplit);
						rootPropertyValueParts[hookPosition] = hookValue;
						rootPropertyValueUpdated = rootPropertyValueParts.join(" ");

						return rootPropertyValueUpdated;
					} else {
						/* If the provided fullHookName isn't a registered hook, return the rootPropertyValue that was passed in. */
						return rootPropertyValue;
					}
				}
			},

			/*******************
			   Normalizations
			*******************/

			/* Normalizations standardize CSS property manipulation by pollyfilling browser-specific implementations (e.g. opacity)
			   and reformatting special properties (e.g. clip, rgba) to look like standard ones. */
			Normalizations: {
				/* Normalizations are passed a normalization target (either the property's name, its extracted value, or its injected value),
				   the targeted element (which may need to be queried), and the targeted property value. */
				registered: {
					clip: function (type, element, propertyValue) {
						switch (type) {
						case "name":
							return "clip";
						/* Clip needs to be unwrapped and stripped of its commas during extraction. */
						case "extract":
							var extracted;

							/* If Velocity also extracted this value, skip extraction. */
							if (CSS.RegEx.wrappedValueAlreadyExtracted.test(propertyValue)) {
								extracted = propertyValue;
							} else {
								/* Remove the "rect()" wrapper. */
								extracted = propertyValue.toString().match(CSS.RegEx.valueUnwrap);

								/* Strip off commas. */
								extracted = extracted ? extracted[1].replace(/,(\s+)?/g, " ") : propertyValue;
							}

							return extracted;
						/* Clip needs to be re-wrapped during injection. */
						case "inject":
							return "rect(" + propertyValue + ")";
						}
					},

					blur: function(type, element, propertyValue) {
						switch (type) {
						case "name":
							return Velocity.State.isFirefox ? "filter" : "-webkit-filter";
						case "extract":
							var extracted = parseFloat(propertyValue);

							/* If extracted is NaN, meaning the value isn't already extracted. */
							if (!(extracted || extracted === 0)) {
								var blurComponent = propertyValue.toString().match(/blur\(([0-9]+[A-z]+)\)/i);

								/* If the filter string had a blur component, return just the blur value and unit type. */
								if (blurComponent) {
									extracted = blurComponent[1];
									/* If the component doesn't exist, default blur to 0. */
								} else {
									extracted = 0;
								}
							}

							return extracted;
						/* Blur needs to be re-wrapped during injection. */
						case "inject":
							/* For the blur effect to be fully de-applied, it needs to be set to "none" instead of 0. */
							if (!parseFloat(propertyValue)) {
								return "none";
							} else {
								return "blur(" + propertyValue + ")";
							}
						}
					},

					/* <=IE8 do not support the standard opacity property. They use filter:alpha(opacity=INT) instead. */
					opacity: function (type, element, propertyValue) {
						if (IE <= 8) {
							switch (type) {
							case "name":
								return "filter";
							case "extract":
								/* <=IE8 return a "filter" value of "alpha(opacity=\d{1,3})".
								   Extract the value and convert it to a decimal value to match the standard CSS opacity property's formatting. */
								var extracted = propertyValue.toString().match(/alpha\(opacity=(.*)\)/i);

								if (extracted) {
									/* Convert to decimal value. */
									propertyValue = extracted[1] / 100;
								} else {
									/* When extracting opacity, default to 1 since a null value means opacity hasn't been set. */
									propertyValue = 1;
								}

								return propertyValue;
							case "inject":
								/* Opacified elements are required to have their zoom property set to a non-zero value. */
								element.style.zoom = 1;

								/* Setting the filter property on elements with certain font property combinations can result in a
								   highly unappealing ultra-bolding effect. There's no way to remedy this throughout a tween, but dropping the
								   value altogether (when opacity hits 1) at leasts ensures that the glitch is gone post-tweening. */
								if (parseFloat(propertyValue) >= 1) {
									return "";
								} else {
									/* As per the filter property's spec, convert the decimal value to a whole number and wrap the value. */
									return "alpha(opacity=" + parseInt(parseFloat(propertyValue) * 100, 10) + ")";
								}
							}
							/* With all other browsers, normalization is not required; return the same values that were passed in. */
						} else {
							switch (type) {
							case "name":
								return "opacity";
							case "extract":
								return propertyValue;
							case "inject":
								return propertyValue;
							}
						}
					}
				},

				/*****************************
				    Batched Registrations
				*****************************/

				/* Note: Batched normalizations extend the CSS.Normalizations.registered object. */
				register: function () {

					/*****************
					    Transforms
					*****************/

					/* Transforms are the subproperties contained by the CSS "transform" property. Transforms must undergo normalization
					   so that they can be referenced in a properties map by their individual names. */
					/* Note: When transforms are "set", they are actually assigned to a per-element transformCache. When all transform
					   setting is complete complete, CSS.flushTransformCache() must be manually called to flush the values to the DOM.
					   Transform setting is batched in this way to improve performance: the transform style only needs to be updated
					   once when multiple transform subproperties are being animated simultaneously. */
					/* Note: IE9 and Android Gingerbread have support for 2D -- but not 3D -- transforms. Since animating unsupported
					   transform properties results in the browser ignoring the *entire* transform string, we prevent these 3D values
					   from being normalized for these browsers so that tweening skips these properties altogether
					   (since it will ignore them as being unsupported by the browser.) */
					if (!(IE <= 9) && !Velocity.State.isGingerbread) {
						/* Note: Since the standalone CSS "perspective" property and the CSS transform "perspective" subproperty
						share the same name, the latter is given a unique token within Velocity: "transformPerspective". */
						CSS.Lists.transformsBase = CSS.Lists.transformsBase.concat(CSS.Lists.transforms3D);
					}

					for (var i = 0; i < CSS.Lists.transformsBase.length; i++) {
						/* Wrap the dynamically generated normalization function in a new scope so that transformName's value is
						paired with its respective function. (Otherwise, all functions would take the final for loop's transformName.) */
						(function() {
							var transformName = CSS.Lists.transformsBase[i];

							CSS.Normalizations.registered[transformName] = function (type, element, propertyValue) {
								switch (type) {
									/* The normalized property name is the parent "transform" property -- the property that is actually set in CSS. */
								case "name":
									return "transform";
								/* Transform values are cached onto a per-element transformCache object. */
								case "extract":
									/* If this transform has yet to be assigned a value, return its null value. */
									if (Data(element) === undefined || Data(element).transformCache[transformName] === undefined) {
										/* Scale CSS.Lists.transformsBase default to 1 whereas all other transform properties default to 0. */
										return /^scale/i.test(transformName) ? 1 : 0;
										/* When transform values are set, they are wrapped in parentheses as per the CSS spec.
										   Thus, when extracting their values (for tween calculations), we strip off the parentheses. */
									} else {
										return Data(element).transformCache[transformName].replace(/[()]/g, "");
									}
								case "inject":
									var invalid = false;

									/* If an individual transform property contains an unsupported unit type, the browser ignores the *entire* transform property.
									   Thus, protect users from themselves by skipping setting for transform values supplied with invalid unit types. */
									/* Switch on the base transform type; ignore the axis by removing the last letter from the transform's name. */
									switch (transformName.substr(0, transformName.length - 1)) {
										/* Whitelist unit types for each transform. */
									case "translate":
										invalid = !/(%|px|em|rem|vw|vh|\d)$/i.test(propertyValue);
										break;
									/* Since an axis-free "scale" property is supported as well, a little hack is used here to detect it by chopping off its last letter. */
									case "scal":
									case "scale":
										/* Chrome on Android has a bug in which scaled elements blur if their initial scale
										   value is below 1 (which can happen with forcefeeding). Thus, we detect a yet-unset scale property
										   and ensure that its first value is always 1. More info: http://stackoverflow.com/questions/10417890/css3-animations-with-transform-causes-blurred-elements-on-webkit/10417962#10417962 */
										if (Velocity.State.isAndroid && Data(element).transformCache[transformName] === undefined && propertyValue < 1) {
											propertyValue = 1;
										}

										invalid = !/(\d)$/i.test(propertyValue);
										break;
									case "skew":
										invalid = !/(deg|\d)$/i.test(propertyValue);
										break;
									case "rotate":
										invalid = !/(deg|\d)$/i.test(propertyValue);
										break;
									}

									if (!invalid) {
										/* As per the CSS spec, wrap the value in parentheses. */
										Data(element).transformCache[transformName] = "(" + propertyValue + ")";
									}

									/* Although the value is set on the transformCache object, return the newly-updated value for the calling code to process as normal. */
									return Data(element).transformCache[transformName];
								}
							};
						})();
					}

					/*************
					    Colors
					*************/

					/* Since Velocity only animates a single numeric value per property, color animation is achieved by hooking the individual RGBA components of CSS color properties.
					   Accordingly, color values must be normalized (e.g. "#ff0000", "red", and "rgb(255, 0, 0)" ==> "255 0 0 1") so that their components can be injected/extracted by CSS.Hooks logic. */
					for (var i = 0; i < CSS.Lists.colors.length; i++) {
						/* Wrap the dynamically generated normalization function in a new scope so that colorName's value is paired with its respective function.
						   (Otherwise, all functions would take the final for loop's colorName.) */
						(function () {
							var colorName = CSS.Lists.colors[i];

							/* Note: In IE<=8, which support rgb but not rgba, color properties are reverted to rgb by stripping off the alpha component. */
							CSS.Normalizations.registered[colorName] = function(type, element, propertyValue) {
								switch (type) {
								case "name":
									return colorName;
								/* Convert all color values into the rgb format. (Old IE can return hex values and color names instead of rgb/rgba.) */
								case "extract":
									var extracted;

									/* If the color is already in its hookable form (e.g. "255 255 255 1") due to having been previously extracted, skip extraction. */
									if (CSS.RegEx.wrappedValueAlreadyExtracted.test(propertyValue)) {
										extracted = propertyValue;
									} else {
										var converted,
											colorNames = {
												black: "rgb(0, 0, 0)",
												blue: "rgb(0, 0, 255)",
												gray: "rgb(128, 128, 128)",
												green: "rgb(0, 128, 0)",
												red: "rgb(255, 0, 0)",
												white: "rgb(255, 255, 255)"
											};

										/* Convert color names to rgb. */
										if (/^[A-z]+$/i.test(propertyValue)) {
											if (colorNames[propertyValue] !== undefined) {
												converted = colorNames[propertyValue]
											} else {
												/* If an unmatched color name is provided, default to black. */
												converted = colorNames.black;
											}
											/* Convert hex values to rgb. */
										} else if (CSS.RegEx.isHex.test(propertyValue)) {
											converted = "rgb(" + CSS.Values.hexToRgb(propertyValue).join(" ") + ")";
											/* If the provided color doesn't match any of the accepted color formats, default to black. */
										} else if (!(/^rgba?\(/i.test(propertyValue))) {
											converted = colorNames.black;
										}

										/* Remove the surrounding "rgb/rgba()" string then replace commas with spaces and strip
										   repeated spaces (in case the value included spaces to begin with). */
										extracted = (converted || propertyValue).toString().match(CSS.RegEx.valueUnwrap)[1].replace(/,(\s+)?/g, " ");
									}

									/* So long as this isn't <=IE8, add a fourth (alpha) component if it's missing and default it to 1 (visible). */
									if (!(IE <= 8) && extracted.split(" ").length === 3) {
										extracted += " 1";
									}

									return extracted;
								case "inject":
									/* If this is IE<=8 and an alpha component exists, strip it off. */
									if (IE <= 8) {
										if (propertyValue.split(" ").length === 4) {
											propertyValue = propertyValue.split(/\s+/).slice(0, 3).join(" ");
										}
										/* Otherwise, add a fourth (alpha) component if it's missing and default it to 1 (visible). */
									} else if (propertyValue.split(" ").length === 3) {
										propertyValue += " 1";
									}

									/* Re-insert the browser-appropriate wrapper("rgb/rgba()"), insert commas, and strip off decimal units
									   on all values but the fourth (R, G, and B only accept whole numbers). */
									return (IE <= 8 ? "rgb" : "rgba") + "(" + propertyValue.replace(/\s+/g, ",").replace(/\.(\d)+(?=,)/g, "") + ")";
								}
							};
						})();
					}
				}
			},

			/************************
			   CSS Property Names
			************************/

			Names: {
				/* Camelcase a property name into its JavaScript notation (e.g. "background-color" ==> "backgroundColor").
				   Camelcasing is used to normalize property names between and across calls. */
				camelCase: function (property) {
					return property.replace(/-(\w)/g, function (match, subMatch) {
						return subMatch.toUpperCase();
					});
				},

				/* For SVG elements, some properties (namely, dimensional ones) are GET/SET via the element's HTML attributes (instead of via CSS styles). */
				SVGAttribute: function (property) {
					var SVGAttributes = "width|height|x|y|cx|cy|r|rx|ry|x1|x2|y1|y2";

					/* Certain browsers require an SVG transform to be applied as an attribute. (Otherwise, application via CSS is preferable due to 3D support.) */
					if (IE || (Velocity.State.isAndroid && !Velocity.State.isChrome)) {
						SVGAttributes += "|transform";
					}

					return new RegExp("^(" + SVGAttributes + ")$", "i").test(property);
				},

				/* Determine whether a property should be set with a vendor prefix. */
				/* If a prefixed version of the property exists, return it. Otherwise, return the original property name.
				   If the property is not at all supported by the browser, return a false flag. */
				prefixCheck: function (property) {
					/* If this property has already been checked, return the cached value. */
					if (Velocity.State.prefixMatches[property]) {
						return [ Velocity.State.prefixMatches[property], true ];
					} else {
						var vendors = [ "", "Webkit", "Moz", "ms", "O" ];

						for (var i = 0, vendorsLength = vendors.length; i < vendorsLength; i++) {
							var propertyPrefixed;

							if (i === 0) {
								propertyPrefixed = property;
							} else {
								/* Capitalize the first letter of the property to conform to JavaScript vendor prefix notation (e.g. webkitFilter). */
								propertyPrefixed = vendors[i] + property.replace(/^\w/, function(match) { return match.toUpperCase(); });
							}

							/* Check if the browser supports this property as prefixed. */
							if (Type.isString(Velocity.State.prefixElement.style[propertyPrefixed])) {
								/* Cache the match. */
								Velocity.State.prefixMatches[property] = propertyPrefixed;

								return [ propertyPrefixed, true ];
							}
						}

						/* If the browser doesn't support this property in any form, include a false flag so that the caller can decide how to proceed. */
						return [ property, false ];
					}
				}
			},

			/************************
			   CSS Property Values
			************************/

			Values: {
				/* Hex to RGB conversion. Copyright Tim Down: http://stackoverflow.com/questions/5623838/rgb-to-hex-and-hex-to-rgb */
				hexToRgb: function (hex) {
					var shortformRegex = /^#?([a-f\d])([a-f\d])([a-f\d])$/i,
						longformRegex = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i,
						rgbParts;

					hex = hex.replace(shortformRegex, function (m, r, g, b) {
						return r + r + g + g + b + b;
					});

					rgbParts = longformRegex.exec(hex);

					return rgbParts ? [ parseInt(rgbParts[1], 16), parseInt(rgbParts[2], 16), parseInt(rgbParts[3], 16) ] : [ 0, 0, 0 ];
				},

				isCSSNullValue: function (value) {
					/* The browser defaults CSS values that have not been set to either 0 or one of several possible null-value strings.
					   Thus, we check for both falsiness and these special strings. */
					/* Null-value checking is performed to default the special strings to 0 (for the sake of tweening) or their hook
					   templates as defined as CSS.Hooks (for the sake of hook injection/extraction). */
					/* Note: Chrome returns "rgba(0, 0, 0, 0)" for an undefined color whereas IE returns "transparent". */
					return (value == 0 || /^(none|auto|transparent|(rgba\(0, ?0, ?0, ?0\)))$/i.test(value));
				},

				/* Retrieve a property's default unit type. Used for assigning a unit type when one is not supplied by the user. */
				getUnitType: function (property) {
					if (/^(rotate|skew)/i.test(property)) {
						return "deg";
					} else if (/(^(scale|scaleX|scaleY|scaleZ|alpha|flexGrow|flexHeight|zIndex|fontWeight)$)|((opacity|red|green|blue|alpha)$)/i.test(property)) {
						/* The above properties are unitless. */
						return "";
					} else {
						/* Default to px for all other properties. */
						return "px";
					}
				},

				/* HTML elements default to an associated display type when they're not set to display:none. */
				/* Note: This function is used for correctly setting the non-"none" display value in certain Velocity redirects, such as fadeIn/Out. */
				getDisplayType: function (element) {
					var tagName = element && element.tagName.toString().toLowerCase();

					if (/^(b|big|i|small|tt|abbr|acronym|cite|code|dfn|em|kbd|strong|samp|var|a|bdo|br|img|map|object|q|script|span|sub|sup|button|input|label|select|textarea)$/i.test(tagName)) {
						return "inline";
					} else if (/^(li)$/i.test(tagName)) {
						return "list-item";
					} else if (/^(tr)$/i.test(tagName)) {
						return "table-row";
					} else if (/^(table)$/i.test(tagName)) {
						return "table";
					} else if (/^(tbody)$/i.test(tagName)) {
						return "table-row-group";
						/* Default to "block" when no match is found. */
					} else {
						return "block";
					}
				},

				/* The class add/remove functions are used to temporarily apply a "velocity-animating" class to elements while they're animating. */
				addClass: function (element, className) {
					if (element.classList) {
						element.classList.add(className);
					} else {
						element.className += (element.className.length ? " " : "") + className;
					}
				},

				removeClass: function (element, className) {
					if (element.classList) {
						element.classList.remove(className);
					} else {
						element.className = element.className.toString().replace(new RegExp("(^|\\s)" + className.split(" ").join("|") + "(\\s|$)", "gi"), " ");
					}
				}
			},

			/****************************
			   Style Getting & Setting
			****************************/

			/* The singular getPropertyValue, which routes the logic for all normalizations, hooks, and standard CSS properties. */
			getPropertyValue: function (element, property, rootPropertyValue, forceStyleLookup) {
				/* Get an element's computed property value. */
				/* Note: Retrieving the value of a CSS property cannot simply be performed by checking an element's
				   style attribute (which only reflects user-defined values). Instead, the browser must be queried for a property's
				   *computed* value. You can read more about getComputedStyle here: https://developer.mozilla.org/en/docs/Web/API/window.getComputedStyle */
				function computePropertyValue (element, property) {
					/* When box-sizing isn't set to border-box, height and width style values are incorrectly computed when an
					   element's scrollbars are visible (which expands the element's dimensions). Thus, we defer to the more accurate
					   offsetHeight/Width property, which includes the total dimensions for interior, border, padding, and scrollbar.
					   We subtract border and padding to get the sum of interior + scrollbar. */
					var computedValue = 0;

					/* IE<=8 doesn't support window.getComputedStyle, thus we defer to jQuery, which has an extensive array
					   of hacks to accurately retrieve IE8 property values. Re-implementing that logic here is not worth bloating the
					   codebase for a dying browser. The performance repercussions of using jQuery here are minimal since
					   Velocity is optimized to rarely (and sometimes never) query the DOM. Further, the $.css() codepath isn't that slow. */
					if (IE <= 8) {
						computedValue = $.css(element, property); /* GET */
						/* All other browsers support getComputedStyle. The returned live object reference is cached onto its
						   associated element so that it does not need to be refetched upon every GET. */
					} else {
						/* Browsers do not return height and width values for elements that are set to display:"none". Thus, we temporarily
						   toggle display to the element type's default value. */
						var toggleDisplay = false;

						if (/^(width|height)$/.test(property) && CSS.getPropertyValue(element, "display") === 0) {
							toggleDisplay = true;
							CSS.setPropertyValue(element, "display", CSS.Values.getDisplayType(element));
						}

						function revertDisplay () {
							if (toggleDisplay) {
								CSS.setPropertyValue(element, "display", "none");
							}
						}

						if (!forceStyleLookup) {
							if (property === "height" && CSS.getPropertyValue(element, "boxSizing").toString().toLowerCase() !== "border-box") {
								var contentBoxHeight = element.offsetHeight - (parseFloat(CSS.getPropertyValue(element, "borderTopWidth")) || 0) - (parseFloat(CSS.getPropertyValue(element, "borderBottomWidth")) || 0) - (parseFloat(CSS.getPropertyValue(element, "paddingTop")) || 0) - (parseFloat(CSS.getPropertyValue(element, "paddingBottom")) || 0);
								revertDisplay();

								return contentBoxHeight;
							} else if (property === "width" && CSS.getPropertyValue(element, "boxSizing").toString().toLowerCase() !== "border-box") {
								var contentBoxWidth = element.offsetWidth - (parseFloat(CSS.getPropertyValue(element, "borderLeftWidth")) || 0) - (parseFloat(CSS.getPropertyValue(element, "borderRightWidth")) || 0) - (parseFloat(CSS.getPropertyValue(element, "paddingLeft")) || 0) - (parseFloat(CSS.getPropertyValue(element, "paddingRight")) || 0);
								revertDisplay();

								return contentBoxWidth;
							}
						}

						var computedStyle;

						/* For elements that Velocity hasn't been called on directly (e.g. when Velocity queries the DOM on behalf
						   of a parent of an element its animating), perform a direct getComputedStyle lookup since the object isn't cached. */
						if (Data(element) === undefined) {
							computedStyle = window.getComputedStyle(element, null); /* GET */
							/* If the computedStyle object has yet to be cached, do so now. */
						} else if (!Data(element).computedStyle) {
							computedStyle = Data(element).computedStyle = window.getComputedStyle(element, null); /* GET */
							/* If computedStyle is cached, use it. */
						} else {
							computedStyle = Data(element).computedStyle;
						}

						/* IE and Firefox do not return a value for the generic borderColor -- they only return individual values for each border side's color.
						   Also, in all browsers, when border colors aren't all the same, a compound value is returned that Velocity isn't setup to parse.
						   So, as a polyfill for querying individual border side colors, we just return the top border's color and animate all borders from that value. */
						if (property === "borderColor") {
							property = "borderTopColor";
						}

						/* IE9 has a bug in which the "filter" property must be accessed from computedStyle using the getPropertyValue method
						   instead of a direct property lookup. The getPropertyValue method is slower than a direct lookup, which is why we avoid it by default. */
						if (IE === 9 && property === "filter") {
							computedValue = computedStyle.getPropertyValue(property); /* GET */
						} else {
							computedValue = computedStyle[property];
						}

						/* Fall back to the property's style value (if defined) when computedValue returns nothing,
						   which can happen when the element hasn't been painted. */
						if (computedValue === "" || computedValue === null) {
							computedValue = element.style[property];
						}

						revertDisplay();
					}

					/* For top, right, bottom, and left (TRBL) values that are set to "auto" on elements of "fixed" or "absolute" position,
					   defer to jQuery for converting "auto" to a numeric value. (For elements with a "static" or "relative" position, "auto" has the same
					   effect as being set to 0, so no conversion is necessary.) */
					/* An example of why numeric conversion is necessary: When an element with "position:absolute" has an untouched "left"
					   property, which reverts to "auto", left's value is 0 relative to its parent element, but is often non-zero relative
					   to its *containing* (not parent) element, which is the nearest "position:relative" ancestor or the viewport (and always the viewport in the case of "position:fixed"). */
					if (computedValue === "auto" && /^(top|right|bottom|left)$/i.test(property)) {
						var position = computePropertyValue(element, "position"); /* GET */

						/* For absolute positioning, jQuery's $.position() only returns values for top and left;
						   right and bottom will have their "auto" value reverted to 0. */
						/* Note: A jQuery object must be created here since jQuery doesn't have a low-level alias for $.position().
						   Not a big deal since we're currently in a GET batch anyway. */
						if (position === "fixed" || (position === "absolute" && /top|left/i.test(property))) {
							/* Note: jQuery strips the pixel unit from its returned values; we re-add it here to conform with computePropertyValue's behavior. */
							computedValue = $(element).position()[property] + "px"; /* GET */
						}
					}

					return computedValue;
				}

				var propertyValue;

				/* If this is a hooked property (e.g. "clipLeft" instead of the root property of "clip"),
				   extract the hook's value from a normalized rootPropertyValue using CSS.Hooks.extractValue(). */
				if (CSS.Hooks.registered[property]) {
					var hook = property,
						hookRoot = CSS.Hooks.getRoot(hook);

					/* If a cached rootPropertyValue wasn't passed in (which Velocity always attempts to do in order to avoid requerying the DOM),
					   query the DOM for the root property's value. */
					if (rootPropertyValue === undefined) {
						/* Since the browser is now being directly queried, use the official post-prefixing property name for this lookup. */
						rootPropertyValue = CSS.getPropertyValue(element, CSS.Names.prefixCheck(hookRoot)[0]); /* GET */
					}

					/* If this root has a normalization registered, peform the associated normalization extraction. */
					if (CSS.Normalizations.registered[hookRoot]) {
						rootPropertyValue = CSS.Normalizations.registered[hookRoot]("extract", element, rootPropertyValue);
					}

					/* Extract the hook's value. */
					propertyValue = CSS.Hooks.extractValue(hook, rootPropertyValue);

					/* If this is a normalized property (e.g. "opacity" becomes "filter" in <=IE8) or "translateX" becomes "transform"),
					   normalize the property's name and value, and handle the special case of transforms. */
					/* Note: Normalizing a property is mutually exclusive from hooking a property since hook-extracted values are strictly
					   numerical and therefore do not require normalization extraction. */
				} else if (CSS.Normalizations.registered[property]) {
					var normalizedPropertyName,
						normalizedPropertyValue;

					normalizedPropertyName = CSS.Normalizations.registered[property]("name", element);

					/* Transform values are calculated via normalization extraction (see below), which checks against the element's transformCache.
					   At no point do transform GETs ever actually query the DOM; initial stylesheet values are never processed.
					   This is because parsing 3D transform matrices is not always accurate and would bloat our codebase;
					   thus, normalization extraction defaults initial transform values to their zero-values (e.g. 1 for scaleX and 0 for translateX). */
					if (normalizedPropertyName !== "transform") {
						normalizedPropertyValue = computePropertyValue(element, CSS.Names.prefixCheck(normalizedPropertyName)[0]); /* GET */

						/* If the value is a CSS null-value and this property has a hook template, use that zero-value template so that hooks can be extracted from it. */
						if (CSS.Values.isCSSNullValue(normalizedPropertyValue) && CSS.Hooks.templates[property]) {
							normalizedPropertyValue = CSS.Hooks.templates[property][1];
						}
					}

					propertyValue = CSS.Normalizations.registered[property]("extract", element, normalizedPropertyValue);
				}

				/* If a (numeric) value wasn't produced via hook extraction or normalization, query the DOM. */
				if (!/^[\d-]/.test(propertyValue)) {
					/* For SVG elements, dimensional properties (which SVGAttribute() detects) are tweened via
					   their HTML attribute values instead of their CSS style values. */
					if (Data(element) && Data(element).isSVG && CSS.Names.SVGAttribute(property)) {
						/* Since the height/width attribute values must be set manually, they don't reflect computed values.
						   Thus, we use use getBBox() to ensure we always get values for elements with undefined height/width attributes. */
						if (/^(height|width)$/i.test(property)) {
							/* Firefox throws an error if .getBBox() is called on an SVG that isn't attached to the DOM. */
							try {
								propertyValue = element.getBBox()[property];
							} catch (error) {
								propertyValue = 0;
							}
							/* Otherwise, access the attribute value directly. */
						} else {
							propertyValue = element.getAttribute(property);
						}
					} else {
						propertyValue = computePropertyValue(element, CSS.Names.prefixCheck(property)[0]); /* GET */
					}
				}

				/* Since property lookups are for animation purposes (which entails computing the numeric delta between start and end values),
				   convert CSS null-values to an integer of value 0. */
				if (CSS.Values.isCSSNullValue(propertyValue)) {
					propertyValue = 0;
				}

				if (Velocity.debug >= 2) console.log("Get " + property + ": " + propertyValue);

				return propertyValue;
			},

			/* The singular setPropertyValue, which routes the logic for all normalizations, hooks, and standard CSS properties. */
			setPropertyValue: function(element, property, propertyValue, rootPropertyValue, scrollData) {
				var propertyName = property;

				/* In order to be subjected to call options and element queueing, scroll animation is routed through Velocity as if it were a standard CSS property. */
				if (property === "scroll") {
					/* If a container option is present, scroll the container instead of the browser window. */
					if (scrollData.container) {
						scrollData.container["scroll" + scrollData.direction] = propertyValue;
						/* Otherwise, Velocity defaults to scrolling the browser window. */
					} else {
						if (scrollData.direction === "Left") {
							window.scrollTo(propertyValue, scrollData.alternateValue);
						} else {
							window.scrollTo(scrollData.alternateValue, propertyValue);
						}
					}
				} else {
					/* Transforms (translateX, rotateZ, etc.) are applied to a per-element transformCache object, which is manually flushed via flushTransformCache().
					   Thus, for now, we merely cache transforms being SET. */
					if (CSS.Normalizations.registered[property] && CSS.Normalizations.registered[property]("name", element) === "transform") {
						/* Perform a normalization injection. */
						/* Note: The normalization logic handles the transformCache updating. */
						CSS.Normalizations.registered[property]("inject", element, propertyValue);

						propertyName = "transform";
						propertyValue = Data(element).transformCache[property];
					} else {
						/* Inject hooks. */
						if (CSS.Hooks.registered[property]) {
							var hookName = property,
								hookRoot = CSS.Hooks.getRoot(property);

							/* If a cached rootPropertyValue was not provided, query the DOM for the hookRoot's current value. */
							rootPropertyValue = rootPropertyValue || CSS.getPropertyValue(element, hookRoot); /* GET */

							propertyValue = CSS.Hooks.injectValue(hookName, propertyValue, rootPropertyValue);
							property = hookRoot;
						}

						/* Normalize names and values. */
						if (CSS.Normalizations.registered[property]) {
							propertyValue = CSS.Normalizations.registered[property]("inject", element, propertyValue);
							property = CSS.Normalizations.registered[property]("name", element);
						}

						/* Assign the appropriate vendor prefix before performing an official style update. */
						propertyName = CSS.Names.prefixCheck(property)[0];

						/* A try/catch is used for IE<=8, which throws an error when "invalid" CSS values are set, e.g. a negative width.
						   Try/catch is avoided for other browsers since it incurs a performance overhead. */
						if (IE <= 8) {
							try {
								element.style[propertyName] = propertyValue;
							} catch (error) { if (Velocity.debug) console.log("Browser does not support [" + propertyValue + "] for [" + propertyName + "]"); }
							/* SVG elements have their dimensional properties (width, height, x, y, cx, etc.) applied directly as attributes instead of as styles. */
							/* Note: IE8 does not support SVG elements, so it's okay that we skip it for SVG animation. */
						} else if (Data(element) && Data(element).isSVG && CSS.Names.SVGAttribute(property)) {
							/* Note: For SVG attributes, vendor-prefixed property names are never used. */
							/* Note: Not all CSS properties can be animated via attributes, but the browser won't throw an error for unsupported properties. */
							element.setAttribute(property, propertyValue);
						} else {
							element.style[propertyName] = propertyValue;
						}

						if (Velocity.debug >= 2) console.log("Set " + property + " (" + propertyName + "): " + propertyValue);
					}
				}

				/* Return the normalized property name and value in case the caller wants to know how these values were modified before being applied to the DOM. */
				return [ propertyName, propertyValue ];
			},

			/* To increase performance by batching transform updates into a single SET, transforms are not directly applied to an element until flushTransformCache() is called. */
			/* Note: Velocity applies transform properties in the same order that they are chronogically introduced to the element's CSS styles. */
			flushTransformCache: function(element) {
				var transformString = "";

				/* Certain browsers require that SVG transforms be applied as an attribute. However, the SVG transform attribute takes a modified version of CSS's transform string
				   (units are dropped and, except for skewX/Y, subproperties are merged into their master property -- e.g. scaleX and scaleY are merged into scale(X Y). */
				if ((IE || (Velocity.State.isAndroid && !Velocity.State.isChrome)) && Data(element).isSVG) {
					/* Since transform values are stored in their parentheses-wrapped form, we use a helper function to strip out their numeric values.
					   Further, SVG transform properties only take unitless (representing pixels) values, so it's okay that parseFloat() strips the unit suffixed to the float value. */
					function getTransformFloat (transformProperty) {
						return parseFloat(CSS.getPropertyValue(element, transformProperty));
					}

					/* Create an object to organize all the transforms that we'll apply to the SVG element. To keep the logic simple,
					   we process *all* transform properties -- even those that may not be explicitly applied (since they default to their zero-values anyway). */
					var SVGTransforms = {
						translate: [ getTransformFloat("translateX"), getTransformFloat("translateY") ],
						skewX: [ getTransformFloat("skewX") ], skewY: [ getTransformFloat("skewY") ],
						/* If the scale property is set (non-1), use that value for the scaleX and scaleY values
						   (this behavior mimics the result of animating all these properties at once on HTML elements). */
						scale: getTransformFloat("scale") !== 1 ? [ getTransformFloat("scale"), getTransformFloat("scale") ] : [ getTransformFloat("scaleX"), getTransformFloat("scaleY") ],
						/* Note: SVG's rotate transform takes three values: rotation degrees followed by the X and Y values
						   defining the rotation's origin point. We ignore the origin values (default them to 0). */
						rotate: [ getTransformFloat("rotateZ"), 0, 0 ]
					};

					/* Iterate through the transform properties in the user-defined property map order.
					   (This mimics the behavior of non-SVG transform animation.) */
					$.each(Data(element).transformCache, function(transformName) {
						/* Except for with skewX/Y, revert the axis-specific transform subproperties to their axis-free master
						   properties so that they match up with SVG's accepted transform properties. */
						if (/^translate/i.test(transformName)) {
							transformName = "translate";
						} else if (/^scale/i.test(transformName)) {
							transformName = "scale";
						} else if (/^rotate/i.test(transformName)) {
							transformName = "rotate";
						}

						/* Check that we haven't yet deleted the property from the SVGTransforms container. */
						if (SVGTransforms[transformName]) {
							/* Append the transform property in the SVG-supported transform format. As per the spec, surround the space-delimited values in parentheses. */
							transformString += transformName + "(" + SVGTransforms[transformName].join(" ") + ")" + " ";

							/* After processing an SVG transform property, delete it from the SVGTransforms container so we don't
							   re-insert the same master property if we encounter another one of its axis-specific properties. */
							delete SVGTransforms[transformName];
						}
					});
				} else {
					var transformValue,
						perspective;

					/* Transform properties are stored as members of the transformCache object. Concatenate all the members into a string. */
					$.each(Data(element).transformCache, function(transformName) {
						transformValue = Data(element).transformCache[transformName];

						/* Transform's perspective subproperty must be set first in order to take effect. Store it temporarily. */
						if (transformName === "transformPerspective") {
							perspective = transformValue;
							return true;
						}

						/* IE9 only supports one rotation type, rotateZ, which it refers to as "rotate". */
						if (IE === 9 && transformName === "rotateZ") {
							transformName = "rotate";
						}

						transformString += transformName + transformValue + " ";
					});

					/* If present, set the perspective subproperty first. */
					if (perspective) {
						transformString = "perspective" + perspective + " " + transformString;
					}
				}

				CSS.setPropertyValue(element, "transform", transformString);
			}
		};

		/* Register hooks and normalizations. */
		CSS.Hooks.register();
		CSS.Normalizations.register();

		/* Allow hook setting in the same fashion as jQuery's $.css(). */
		Velocity.hook = function (elements, arg2, arg3) {
			var value = undefined;

			elements = sanitizeElements(elements);

			$.each(elements, function(i, element) {
				/* Initialize Velocity's per-element data cache if this element hasn't previously been animated. */
				if (Data(element) === undefined) {
					Velocity.init(element);
				}

				/* Get property value. If an element set was passed in, only return the value for the first element. */
				if (arg3 === undefined) {
					if (value === undefined) {
						value = Velocity.CSS.getPropertyValue(element, arg2);
					}
					/* Set property value. */
				} else {
					/* sPV returns an array of the normalized propertyName/propertyValue pair used to update the DOM. */
					var adjustedSet = Velocity.CSS.setPropertyValue(element, arg2, arg3);

					/* Transform properties don't automatically set. They have to be flushed to the DOM. */
					if (adjustedSet[0] === "transform") {
						Velocity.CSS.flushTransformCache(element);
					}

					value = adjustedSet;
				}
			});

			return value;
		};

		/*****************
		    Animation
		*****************/

		var animate = function() {

			/******************
			    Call Chain
			******************/

			/* Logic for determining what to return to the call stack when exiting out of Velocity. */
			function getChain () {
				/* If we are using the utility function, attempt to return this call's promise. If no promise library was detected,
				   default to null instead of returning the targeted elements so that utility function's return value is standardized. */
				if (isUtility) {
					return promiseData.promise || null;
					/* Otherwise, if we're using $.fn, return the jQuery-/Zepto-wrapped element set. */
				} else {
					return elementsWrapped;
				}
			}

			/*************************
			   Arguments Assignment
			*************************/

			/* To allow for expressive CoffeeScript code, Velocity supports an alternative syntax in which "elements" (or "e"), "properties" (or "p"), and "options" (or "o")
			   objects are defined on a container object that's passed in as Velocity's sole argument. */
			/* Note: Some browsers automatically populate arguments with a "properties" object. We detect it by checking for its default "names" property. */
			var syntacticSugar = (arguments[0] && (arguments[0].p || (($.isPlainObject(arguments[0].properties) && !arguments[0].properties.names) || Type.isString(arguments[0].properties)))),
				/* Whether Velocity was called via the utility function (as opposed to on a jQuery/Zepto object). */
				isUtility,
				/* When Velocity is called via the utility function ($.Velocity()/Velocity()), elements are explicitly
				   passed in as the first parameter. Thus, argument positioning varies. We normalize them here. */
				elementsWrapped,
				argumentIndex;

			var elements,
				propertiesMap,
				options;

			/* Detect jQuery/Zepto elements being animated via the $.fn method. */
			if (Type.isWrapped(this)) {
				isUtility = false;

				argumentIndex = 0;
				elements = this;
				elementsWrapped = this;
				/* Otherwise, raw elements are being animated via the utility function. */
			} else {
				isUtility = true;

				argumentIndex = 1;
				elements = syntacticSugar ? (arguments[0].elements || arguments[0].e) : arguments[0];
			}

			elements = sanitizeElements(elements);

			if (!elements) {
				return;
			}

			if (syntacticSugar) {
				propertiesMap = arguments[0].properties || arguments[0].p;
				options = arguments[0].options || arguments[0].o;
			} else {
				propertiesMap = arguments[argumentIndex];
				options = arguments[argumentIndex + 1];
			}

			/* The length of the element set (in the form of a nodeList or an array of elements) is defaulted to 1 in case a
			   single raw DOM element is passed in (which doesn't contain a length property). */
			var elementsLength = elements.length,
				elementsIndex = 0;

			/***************************
			    Argument Overloading
			***************************/

			/* Support is included for jQuery's argument overloading: $.animate(propertyMap [, duration] [, easing] [, complete]).
			   Overloading is detected by checking for the absence of an object being passed into options. */
			/* Note: The stop and finish actions do not accept animation options, and are therefore excluded from this check. */
			if (!/^(stop|finish)$/i.test(propertiesMap) && !$.isPlainObject(options)) {
				/* The utility function shifts all arguments one position to the right, so we adjust for that offset. */
				var startingArgumentPosition = argumentIndex + 1;

				options = {};

				/* Iterate through all options arguments */
				for (var i = startingArgumentPosition; i < arguments.length; i++) {
					/* Treat a number as a duration. Parse it out. */
					/* Note: The following RegEx will return true if passed an array with a number as its first item.
					   Thus, arrays are skipped from this check. */
					if (!Type.isArray(arguments[i]) && (/^(fast|normal|slow)$/i.test(arguments[i]) || /^\d/.test(arguments[i]))) {
						options.duration = arguments[i];
						/* Treat strings and arrays as easings. */
					} else if (Type.isString(arguments[i]) || Type.isArray(arguments[i])) {
						options.easing = arguments[i];
						/* Treat a function as a complete callback. */
					} else if (Type.isFunction(arguments[i])) {
						options.complete = arguments[i];
					}
				}
			}

			/***************
			    Promises
			***************/

			var promiseData = {
				promise: null,
				resolver: null,
				rejecter: null
			};

			/* If this call was made via the utility function (which is the default method of invocation when jQuery/Zepto are not being used), and if
			   promise support was detected, create a promise object for this call and store references to its resolver and rejecter methods. The resolve
			   method is used when a call completes naturally or is prematurely stopped by the user. In both cases, completeCall() handles the associated
			   call cleanup and promise resolving logic. The reject method is used when an invalid set of arguments is passed into a Velocity call. */
			/* Note: Velocity employs a call-based queueing architecture, which means that stopping an animating element actually stops the full call that
			   triggered it -- not that one element exclusively. Similarly, there is one promise per call, and all elements targeted by a Velocity call are
			   grouped together for the purposes of resolving and rejecting a promise. */
			if (isUtility && Velocity.Promise) {
				promiseData.promise = new Velocity.Promise(function (resolve, reject) {
					promiseData.resolver = resolve;
					promiseData.rejecter = reject;
				});
			}

			/*********************
			   Action Detection
			*********************/

			/* Velocity's behavior is categorized into "actions": Elements can either be specially scrolled into view,
			   or they can be started, stopped, or reversed. If a literal or referenced properties map is passed in as Velocity's
			   first argument, the associated action is "start". Alternatively, "scroll", "reverse", or "stop" can be passed in instead of a properties map. */
			var action;

			switch (propertiesMap) {
			case "scroll":
				action = "scroll";
				break;

			case "reverse":
				action = "reverse";
				break;

			case "finish":
			case "stop":
				/*******************
				    Action: Stop
				*******************/

				/* Clear the currently-active delay on each targeted element. */
				$.each(elements, function(i, element) {
					if (Data(element) && Data(element).delayTimer) {
						/* Stop the timer from triggering its cached next() function. */
						clearTimeout(Data(element).delayTimer.setTimeout);

						/* Manually call the next() function so that the subsequent queue items can progress. */
						if (Data(element).delayTimer.next) {
							Data(element).delayTimer.next();
						}

						delete Data(element).delayTimer;
					}
				});

				var callsToStop = [];

				/* When the stop action is triggered, the elements' currently active call is immediately stopped. The active call might have
				   been applied to multiple elements, in which case all of the call's elements will be stopped. When an element
				   is stopped, the next item in its animation queue is immediately triggered. */
				/* An additional argument may be passed in to clear an element's remaining queued calls. Either true (which defaults to the "fx" queue)
				   or a custom queue string can be passed in. */
				/* Note: The stop command runs prior to Velocity's Queueing phase since its behavior is intended to take effect *immediately*,
				   regardless of the element's current queue state. */

				/* Iterate through every active call. */
				$.each(Velocity.State.calls, function(i, activeCall) {
					/* Inactive calls are set to false by the logic inside completeCall(). Skip them. */
					if (activeCall) {
						/* Iterate through the active call's targeted elements. */
						$.each(activeCall[1], function(k, activeElement) {
							/* If true was passed in as a secondary argument, clear absolutely all calls on this element. Otherwise, only
							   clear calls associated with the relevant queue. */
							/* Call stopping logic works as follows:
							   - options === true --> stop current default queue calls (and queue:false calls), including remaining queued ones.
							   - options === undefined --> stop current queue:"" call and all queue:false calls.
							   - options === false --> stop only queue:false calls.
							   - options === "custom" --> stop current queue:"custom" call, including remaining queued ones (there is no functionality to only clear the currently-running queue:"custom" call). */
							var queueName = (options === undefined) ? "" : options;

							if (queueName !== true && (activeCall[2].queue !== queueName) && !(options === undefined && activeCall[2].queue === false)) {
								return true;
							}

							/* Iterate through the calls targeted by the stop command. */
							$.each(elements, function(l, element) {
								/* Check that this call was applied to the target element. */
								if (element === activeElement) {
									/* Optionally clear the remaining queued calls. */
									if (options === true || Type.isString(options)) {
										/* Iterate through the items in the element's queue. */
										$.each($.queue(element, Type.isString(options) ? options : ""), function(_, item) {
											/* The queue array can contain an "inprogress" string, which we skip. */
											if (Type.isFunction(item)) {
												/* Pass the item's callback a flag indicating that we want to abort from the queue call.
												   (Specifically, the queue will resolve the call's associated promise then abort.)  */
												item(null, true);
											}
										});

										/* Clearing the $.queue() array is achieved by resetting it to []. */
										$.queue(element, Type.isString(options) ? options : "", []);
									}

									if (propertiesMap === "stop") {
										/* Since "reverse" uses cached start values (the previous call's endValues), these values must be
										   changed to reflect the final value that the elements were actually tweened to. */
										/* Note: If only queue:false animations are currently running on an element, it won't have a tweensContainer
										   object. Also, queue:false animations can't be reversed. */
										if (Data(element) && Data(element).tweensContainer && queueName !== false) {
											$.each(Data(element).tweensContainer, function(m, activeTween) {
												activeTween.endValue = activeTween.currentValue;
											});
										}

										callsToStop.push(i);
									} else if (propertiesMap === "finish") {
										/* To get active tweens to finish immediately, we forcefully shorten their durations to 1ms so that
										they finish upon the next rAf tick then proceed with normal call completion logic. */
										activeCall[2].duration = 1;
									}
								}
							});
						});
					}
				});

				/* Prematurely call completeCall() on each matched active call. Pass an additional flag for "stop" to indicate
				   that the complete callback and display:none setting should be skipped since we're completing prematurely. */
				if (propertiesMap === "stop") {
					$.each(callsToStop, function(i, j) {
						completeCall(j, true);
					});

					if (promiseData.promise) {
						/* Immediately resolve the promise associated with this stop call since stop runs synchronously. */
						promiseData.resolver(elements);
					}
				}

				/* Since we're stopping, and not proceeding with queueing, exit out of Velocity. */
				return getChain();

			default:
				/* Treat a non-empty plain object as a literal properties map. */
				if ($.isPlainObject(propertiesMap) && !Type.isEmptyObject(propertiesMap)) {
					action = "start";

					/****************
					    Redirects
					****************/

					/* Check if a string matches a registered redirect (see Redirects above). */
				} else if (Type.isString(propertiesMap) && Velocity.Redirects[propertiesMap]) {
					var opts = $.extend({}, options),
						durationOriginal = opts.duration,
						delayOriginal = opts.delay || 0;

					/* If the backwards option was passed in, reverse the element set so that elements animate from the last to the first. */
					if (opts.backwards === true) {
						elements = $.extend(true, [], elements).reverse();
					}

					/* Individually trigger the redirect for each element in the set to prevent users from having to handle iteration logic in their redirect. */
					$.each(elements, function(elementIndex, element) {
						/* If the stagger option was passed in, successively delay each element by the stagger value (in ms). Retain the original delay value. */
						if (parseFloat(opts.stagger)) {
							opts.delay = delayOriginal + (parseFloat(opts.stagger) * elementIndex);
						} else if (Type.isFunction(opts.stagger)) {
							opts.delay = delayOriginal + opts.stagger.call(element, elementIndex, elementsLength);
						}

						/* If the drag option was passed in, successively increase/decrease (depending on the presense of opts.backwards)
						   the duration of each element's animation, using floors to prevent producing very short durations. */
						if (opts.drag) {
							/* Default the duration of UI pack effects (callouts and transitions) to 1000ms instead of the usual default duration of 400ms. */
							opts.duration = parseFloat(durationOriginal) || (/^(callout|transition)/.test(propertiesMap) ? 1000 : DURATION_DEFAULT);

							/* For each element, take the greater duration of: A) animation completion percentage relative to the original duration,
							   B) 75% of the original duration, or C) a 200ms fallback (in case duration is already set to a low value).
							   The end result is a baseline of 75% of the redirect's duration that increases/decreases as the end of the element set is approached. */
							opts.duration = Math.max(opts.duration * (opts.backwards ? 1 - elementIndex/elementsLength : (elementIndex + 1) / elementsLength), opts.duration * 0.75, 200);
						}

						/* Pass in the call's opts object so that the redirect can optionally extend it. It defaults to an empty object instead of null to
						   reduce the opts checking logic required inside the redirect. */
						Velocity.Redirects[propertiesMap].call(element, element, opts || {}, elementIndex, elementsLength, elements, promiseData.promise ? promiseData : undefined);
					});

					/* Since the animation logic resides within the redirect's own code, abort the remainder of this call.
					   (The performance overhead up to this point is virtually non-existant.) */
					/* Note: The jQuery call chain is kept intact by returning the complete element set. */
					return getChain();
				} else {
					var abortError = "Velocity: First argument (" + propertiesMap + ") was not a property map, a known action, or a registered redirect. Aborting.";

					if (promiseData.promise) {
						promiseData.rejecter(new Error(abortError));
					} else {
						console.log(abortError);
					}

					return getChain();
				}
			}

			/**************************
			    Call-Wide Variables
			**************************/

			/* A container for CSS unit conversion ratios (e.g. %, rem, and em ==> px) that is used to cache ratios across all elements
			   being animated in a single Velocity call. Calculating unit ratios necessitates DOM querying and updating, and is therefore
			   avoided (via caching) wherever possible. This container is call-wide instead of page-wide to avoid the risk of using stale
			   conversion metrics across Velocity animations that are not immediately consecutively chained. */
			var callUnitConversionData = {
				lastParent: null,
				lastPosition: null,
				lastFontSize: null,
				lastPercentToPxWidth: null,
				lastPercentToPxHeight: null,
				lastEmToPx: null,
				remToPx: null,
				vwToPx: null,
				vhToPx: null
			};

			/* A container for all the ensuing tween data and metadata associated with this call. This container gets pushed to the page-wide
			   Velocity.State.calls array that is processed during animation ticking. */
			var call = [];

			/************************
			   Element Processing
			************************/

			/* Element processing consists of three parts -- data processing that cannot go stale and data processing that *can* go stale (i.e. third-party style modifications):
			   1) Pre-Queueing: Element-wide variables, including the element's data storage, are instantiated. Call options are prepared. If triggered, the Stop action is executed.
			   2) Queueing: The logic that runs once this call has reached its point of execution in the element's $.queue() stack. Most logic is placed here to avoid risking it becoming stale.
			   3) Pushing: Consolidation of the tween data followed by its push onto the global in-progress calls container.
			*/

			function processElement () {

				/*************************
				   Part I: Pre-Queueing
				*************************/

				/***************************
				   Element-Wide Variables
				***************************/

				var element = this,
					/* The runtime opts object is the extension of the current call's options and Velocity's page-wide option defaults. */
					opts = $.extend({}, Velocity.defaults, options),
					/* A container for the processed data associated with each property in the propertyMap.
					   (Each property in the map produces its own "tween".) */
					tweensContainer = {},
					elementUnitConversionData;

				/******************
				   Element Init
				******************/

				if (Data(element) === undefined) {
					Velocity.init(element);
				}

				/******************
				   Option: Delay
				******************/

				/* Since queue:false doesn't respect the item's existing queue, we avoid injecting its delay here (it's set later on). */
				/* Note: Velocity rolls its own delay function since jQuery doesn't have a utility alias for $.fn.delay()
				   (and thus requires jQuery element creation, which we avoid since its overhead includes DOM querying). */
				if (parseFloat(opts.delay) && opts.queue !== false) {
					$.queue(element, opts.queue, function(next) {
						/* This is a flag used to indicate to the upcoming completeCall() function that this queue entry was initiated by Velocity. See completeCall() for further details. */
						Velocity.velocityQueueEntryFlag = true;

						/* The ensuing queue item (which is assigned to the "next" argument that $.queue() automatically passes in) will be triggered after a setTimeout delay.
						   The setTimeout is stored so that it can be subjected to clearTimeout() if this animation is prematurely stopped via Velocity's "stop" command. */
						Data(element).delayTimer = {
							setTimeout: setTimeout(next, parseFloat(opts.delay)),
							next: next
						};
					});
				}

				/*********************
				   Option: Duration
				*********************/

				/* Support for jQuery's named durations. */
				switch (opts.duration.toString().toLowerCase()) {
				case "fast":
					opts.duration = 200;
					break;

				case "normal":
					opts.duration = DURATION_DEFAULT;
					break;

				case "slow":
					opts.duration = 600;
					break;

				default:
					/* Remove the potential "ms" suffix and default to 1 if the user is attempting to set a duration of 0 (in order to produce an immediate style change). */
					opts.duration = parseFloat(opts.duration) || 1;
				}

				/************************
				   Global Option: Mock
				************************/

				if (Velocity.mock !== false) {
					/* In mock mode, all animations are forced to 1ms so that they occur immediately upon the next rAF tick.
					   Alternatively, a multiplier can be passed in to time remap all delays and durations. */
					if (Velocity.mock === true) {
						opts.duration = opts.delay = 1;
					} else {
						opts.duration *= parseFloat(Velocity.mock) || 1;
						opts.delay *= parseFloat(Velocity.mock) || 1;
					}
				}

				/*******************
				   Option: Easing
				*******************/

				opts.easing = getEasing(opts.easing, opts.duration);

				/**********************
				   Option: Callbacks
				**********************/

				/* Callbacks must functions. Otherwise, default to null. */
				if (opts.begin && !Type.isFunction(opts.begin)) {
					opts.begin = null;
				}

				if (opts.progress && !Type.isFunction(opts.progress)) {
					opts.progress = null;
				}

				if (opts.complete && !Type.isFunction(opts.complete)) {
					opts.complete = null;
				}

				/*********************************
				   Option: Display & Visibility
				*********************************/

				/* Refer to Velocity's documentation (VelocityJS.org/#displayAndVisibility) for a description of the display and visibility options' behavior. */
				/* Note: We strictly check for undefined instead of falsiness because display accepts an empty string value. */
				if (opts.display !== undefined && opts.display !== null) {
					opts.display = opts.display.toString().toLowerCase();

					/* Users can pass in a special "auto" value to instruct Velocity to set the element to its default display value. */
					if (opts.display === "auto") {
						opts.display = Velocity.CSS.Values.getDisplayType(element);
					}
				}

				if (opts.visibility !== undefined && opts.visibility !== null) {
					opts.visibility = opts.visibility.toString().toLowerCase();
				}

				/**********************
				   Option: mobileHA
				**********************/

				/* When set to true, and if this is a mobile device, mobileHA automatically enables hardware acceleration (via a null transform hack)
				   on animating elements. HA is removed from the element at the completion of its animation. */
				/* Note: Android Gingerbread doesn't support HA. If a null transform hack (mobileHA) is in fact set, it will prevent other tranform subproperties from taking effect. */
				/* Note: You can read more about the use of mobileHA in Velocity's documentation: VelocityJS.org/#mobileHA. */
				opts.mobileHA = (opts.mobileHA && Velocity.State.isMobile && !Velocity.State.isGingerbread);

				/***********************
				   Part II: Queueing
				***********************/

				/* When a set of elements is targeted by a Velocity call, the set is broken up and each element has the current Velocity call individually queued onto it.
				   In this way, each element's existing queue is respected; some elements may already be animating and accordingly should not have this current Velocity call triggered immediately. */
				/* In each queue, tween data is processed for each animating property then pushed onto the call-wide calls array. When the last element in the set has had its tweens processed,
				   the call array is pushed to Velocity.State.calls for live processing by the requestAnimationFrame tick. */
				function buildQueue (next) {

					/*******************
					   Option: Begin
					*******************/

					/* The begin callback is fired once per call -- not once per elemenet -- and is passed the full raw DOM element set as both its context and its first argument. */
					if (opts.begin && elementsIndex === 0) {
						/* We throw callbacks in a setTimeout so that thrown errors don't halt the execution of Velocity itself. */
						try {
							opts.begin.call(elements, elements);
						} catch (error) {
							setTimeout(function() { throw error; }, 1);
						}
					}

					/*****************************************
					   Tween Data Construction (for Scroll)
					*****************************************/

					/* Note: In order to be subjected to chaining and animation options, scroll's tweening is routed through Velocity as if it were a standard CSS property animation. */
					if (action === "scroll") {
						/* The scroll action uniquely takes an optional "offset" option -- specified in pixels -- that offsets the targeted scroll position. */
						var scrollDirection = (/^x$/i.test(opts.axis) ? "Left" : "Top"),
							scrollOffset = parseFloat(opts.offset) || 0,
							scrollPositionCurrent,
							scrollPositionCurrentAlternate,
							scrollPositionEnd;

						/* Scroll also uniquely takes an optional "container" option, which indicates the parent element that should be scrolled --
						   as opposed to the browser window itself. This is useful for scrolling toward an element that's inside an overflowing parent element. */
						if (opts.container) {
							/* Ensure that either a jQuery object or a raw DOM element was passed in. */
							if (Type.isWrapped(opts.container) || Type.isNode(opts.container)) {
								/* Extract the raw DOM element from the jQuery wrapper. */
								opts.container = opts.container[0] || opts.container;
								/* Note: Unlike other properties in Velocity, the browser's scroll position is never cached since it so frequently changes
								   (due to the user's natural interaction with the page). */
								scrollPositionCurrent = opts.container["scroll" + scrollDirection]; /* GET */

								/* $.position() values are relative to the container's currently viewable area (without taking into account the container's true dimensions
								   -- say, for example, if the container was not overflowing). Thus, the scroll end value is the sum of the child element's position *and*
								   the scroll container's current scroll position. */
								scrollPositionEnd = (scrollPositionCurrent + $(element).position()[scrollDirection.toLowerCase()]) + scrollOffset; /* GET */
								/* If a value other than a jQuery object or a raw DOM element was passed in, default to null so that this option is ignored. */
							} else {
								opts.container = null;
							}
						} else {
							/* If the window itself is being scrolled -- not a containing element -- perform a live scroll position lookup using
							   the appropriate cached property names (which differ based on browser type). */
							scrollPositionCurrent = Velocity.State.scrollAnchor[Velocity.State["scrollProperty" + scrollDirection]]; /* GET */
							/* When scrolling the browser window, cache the alternate axis's current value since window.scrollTo() doesn't let us change only one value at a time. */
							scrollPositionCurrentAlternate = Velocity.State.scrollAnchor[Velocity.State["scrollProperty" + (scrollDirection === "Left" ? "Top" : "Left")]]; /* GET */

							/* Unlike $.position(), $.offset() values are relative to the browser window's true dimensions -- not merely its currently viewable area --
							   and therefore end values do not need to be compounded onto current values. */
							scrollPositionEnd = $(element).offset()[scrollDirection.toLowerCase()] + scrollOffset; /* GET */
						}

						/* Since there's only one format that scroll's associated tweensContainer can take, we create it manually. */
						tweensContainer = {
							scroll: {
								rootPropertyValue: false,
								startValue: scrollPositionCurrent,
								currentValue: scrollPositionCurrent,
								endValue: scrollPositionEnd,
								unitType: "",
								easing: opts.easing,
								scrollData: {
									container: opts.container,
									direction: scrollDirection,
									alternateValue: scrollPositionCurrentAlternate
								}
							},
							element: element
						};

						if (Velocity.debug) console.log("tweensContainer (scroll): ", tweensContainer.scroll, element);

						/******************************************
						   Tween Data Construction (for Reverse)
						******************************************/

						/* Reverse acts like a "start" action in that a property map is animated toward. The only difference is
						   that the property map used for reverse is the inverse of the map used in the previous call. Thus, we manipulate
						   the previous call to construct our new map: use the previous map's end values as our new map's start values. Copy over all other data. */
						/* Note: Reverse can be directly called via the "reverse" parameter, or it can be indirectly triggered via the loop option. (Loops are composed of multiple reverses.) */
						/* Note: Reverse calls do not need to be consecutively chained onto a currently-animating element in order to operate on cached values;
						   there is no harm to reverse being called on a potentially stale data cache since reverse's behavior is simply defined
						   as reverting to the element's values as they were prior to the previous *Velocity* call. */
					} else if (action === "reverse") {
						/* Abort if there is no prior animation data to reverse to. */
						if (!Data(element).tweensContainer) {
							/* Dequeue the element so that this queue entry releases itself immediately, allowing subsequent queue entries to run. */
							$.dequeue(element, opts.queue);

							return;
						} else {
							/*********************
							   Options Parsing
							*********************/

							/* If the element was hidden via the display option in the previous call,
							   revert display to "auto" prior to reversal so that the element is visible again. */
							if (Data(element).opts.display === "none") {
								Data(element).opts.display = "auto";
							}

							if (Data(element).opts.visibility === "hidden") {
								Data(element).opts.visibility = "visible";
							}

							/* If the loop option was set in the previous call, disable it so that "reverse" calls aren't recursively generated.
							   Further, remove the previous call's callback options; typically, users do not want these to be refired. */
							Data(element).opts.loop = false;
							Data(element).opts.begin = null;
							Data(element).opts.complete = null;

							/* Since we're extending an opts object that has already been extended with the defaults options object,
							   we remove non-explicitly-defined properties that are auto-assigned values. */
							if (!options.easing) {
								delete opts.easing;
							}

							if (!options.duration) {
								delete opts.duration;
							}

							/* The opts object used for reversal is an extension of the options object optionally passed into this
							   reverse call plus the options used in the previous Velocity call. */
							opts = $.extend({}, Data(element).opts, opts);

							/*************************************
							   Tweens Container Reconstruction
							*************************************/

							/* Create a deepy copy (indicated via the true flag) of the previous call's tweensContainer. */
							var lastTweensContainer = $.extend(true, {}, Data(element).tweensContainer);

							/* Manipulate the previous tweensContainer by replacing its end values and currentValues with its start values. */
							for (var lastTween in lastTweensContainer) {
								/* In addition to tween data, tweensContainers contain an element property that we ignore here. */
								if (lastTween !== "element") {
									var lastStartValue = lastTweensContainer[lastTween].startValue;

									lastTweensContainer[lastTween].startValue = lastTweensContainer[lastTween].currentValue = lastTweensContainer[lastTween].endValue;
									lastTweensContainer[lastTween].endValue = lastStartValue;

									/* Easing is the only option that embeds into the individual tween data (since it can be defined on a per-property basis).
									   Accordingly, every property's easing value must be updated when an options object is passed in with a reverse call.
									   The side effect of this extensibility is that all per-property easing values are forcefully reset to the new value. */
									if (!Type.isEmptyObject(options)) {
										lastTweensContainer[lastTween].easing = opts.easing;
									}

									if (Velocity.debug) console.log("reverse tweensContainer (" + lastTween + "): " + JSON.stringify(lastTweensContainer[lastTween]), element);
								}
							}

							tweensContainer = lastTweensContainer;
						}

						/*****************************************
						   Tween Data Construction (for Start)
						*****************************************/

					} else if (action === "start") {

						/*************************
						    Value Transferring
						*************************/

						/* If this queue entry follows a previous Velocity-initiated queue entry *and* if this entry was created
						   while the element was in the process of being animated by Velocity, then this current call is safe to use
						   the end values from the prior call as its start values. Velocity attempts to perform this value transfer
						   process whenever possible in order to avoid requerying the DOM. */
						/* If values aren't transferred from a prior call and start values were not forcefed by the user (more on this below),
						   then the DOM is queried for the element's current values as a last resort. */
						/* Note: Conversely, animation reversal (and looping) *always* perform inter-call value transfers; they never requery the DOM. */
						var lastTweensContainer;

						/* The per-element isAnimating flag is used to indicate whether it's safe (i.e. the data isn't stale)
						   to transfer over end values to use as start values. If it's set to true and there is a previous
						   Velocity call to pull values from, do so. */
						if (Data(element).tweensContainer && Data(element).isAnimating === true) {
							lastTweensContainer = Data(element).tweensContainer;
						}

						/***************************
						   Tween Data Calculation
						***************************/

						/* This function parses property data and defaults endValue, easing, and startValue as appropriate. */
						/* Property map values can either take the form of 1) a single value representing the end value,
						   or 2) an array in the form of [ endValue, [, easing] [, startValue] ].
						   The optional third parameter is a forcefed startValue to be used instead of querying the DOM for
						   the element's current value. Read Velocity's docmentation to learn more about forcefeeding: VelocityJS.org/#forcefeeding */
						function parsePropertyValue (valueData, skipResolvingEasing) {
							var endValue = undefined,
								easing = undefined,
								startValue = undefined;

							/* Handle the array format, which can be structured as one of three potential overloads:
							   A) [ endValue, easing, startValue ], B) [ endValue, easing ], or C) [ endValue, startValue ] */
							if (Type.isArray(valueData)) {
								/* endValue is always the first item in the array. Don't bother validating endValue's value now
								   since the ensuing property cycling logic does that. */
								endValue = valueData[0];

								/* Two-item array format: If the second item is a number, function, or hex string, treat it as a
								   start value since easings can only be non-hex strings or arrays. */
								if ((!Type.isArray(valueData[1]) && /^[\d-]/.test(valueData[1])) || Type.isFunction(valueData[1]) || CSS.RegEx.isHex.test(valueData[1])) {
									startValue = valueData[1];
									/* Two or three-item array: If the second item is a non-hex string or an array, treat it as an easing. */
								} else if ((Type.isString(valueData[1]) && !CSS.RegEx.isHex.test(valueData[1])) || Type.isArray(valueData[1])) {
									easing = skipResolvingEasing ? valueData[1] : getEasing(valueData[1], opts.duration);

									/* Don't bother validating startValue's value now since the ensuing property cycling logic inherently does that. */
									if (valueData[2] !== undefined) {
										startValue = valueData[2];
									}
								}
								/* Handle the single-value format. */
							} else {
								endValue = valueData;
							}

							/* Default to the call's easing if a per-property easing type was not defined. */
							if (!skipResolvingEasing) {
								easing = easing || opts.easing;
							}

							/* If functions were passed in as values, pass the function the current element as its context,
							   plus the element's index and the element set's size as arguments. Then, assign the returned value. */
							if (Type.isFunction(endValue)) {
								endValue = endValue.call(element, elementsIndex, elementsLength);
							}

							if (Type.isFunction(startValue)) {
								startValue = startValue.call(element, elementsIndex, elementsLength);
							}

							/* Allow startValue to be left as undefined to indicate to the ensuing code that its value was not forcefed. */
							return [ endValue || 0, easing, startValue ];
						}

						/* Cycle through each property in the map, looking for shorthand color properties (e.g. "color" as opposed to "colorRed"). Inject the corresponding
						   colorRed, colorGreen, and colorBlue RGB component tweens into the propertiesMap (which Velocity understands) and remove the shorthand property. */
						$.each(propertiesMap, function(property, value) {
							/* Find shorthand color properties that have been passed a hex string. */
							if (RegExp("^" + CSS.Lists.colors.join("$|^") + "$").test(property)) {
								/* Parse the value data for each shorthand. */
								var valueData = parsePropertyValue(value, true),
									endValue = valueData[0],
									easing = valueData[1],
									startValue = valueData[2];

								if (CSS.RegEx.isHex.test(endValue)) {
									/* Convert the hex strings into their RGB component arrays. */
									var colorComponents = [ "Red", "Green", "Blue" ],
										endValueRGB = CSS.Values.hexToRgb(endValue),
										startValueRGB = startValue ? CSS.Values.hexToRgb(startValue) : undefined;

									/* Inject the RGB component tweens into propertiesMap. */
									for (var i = 0; i < colorComponents.length; i++) {
										var dataArray = [ endValueRGB[i] ];

										if (easing) {
											dataArray.push(easing);
										}

										if (startValueRGB !== undefined) {
											dataArray.push(startValueRGB[i]);
										}

										propertiesMap[property + colorComponents[i]] = dataArray;
									}

									/* Remove the intermediary shorthand property entry now that we've processed it. */
									delete propertiesMap[property];
								}
							}
						});

						/* Create a tween out of each property, and append its associated data to tweensContainer. */
						for (var property in propertiesMap) {

							/**************************
							   Start Value Sourcing
							**************************/

							/* Parse out endValue, easing, and startValue from the property's data. */
							var valueData = parsePropertyValue(propertiesMap[property]),
								endValue = valueData[0],
								easing = valueData[1],
								startValue = valueData[2];

							/* Now that the original property name's format has been used for the parsePropertyValue() lookup above,
							   we force the property to its camelCase styling to normalize it for manipulation. */
							property = CSS.Names.camelCase(property);

							/* In case this property is a hook, there are circumstances where we will intend to work on the hook's root property and not the hooked subproperty. */
							var rootProperty = CSS.Hooks.getRoot(property),
								rootPropertyValue = false;

							/* Other than for the dummy tween property, properties that are not supported by the browser (and do not have an associated normalization) will
							   inherently produce no style changes when set, so they are skipped in order to decrease animation tick overhead.
							   Property support is determined via prefixCheck(), which returns a false flag when no supported is detected. */
							/* Note: Since SVG elements have some of their properties directly applied as HTML attributes,
							   there is no way to check for their explicit browser support, and so we skip skip this check for them. */
							if (!Data(element).isSVG && rootProperty !== "tween" && CSS.Names.prefixCheck(rootProperty)[1] === false && CSS.Normalizations.registered[rootProperty] === undefined) {
								if (Velocity.debug) console.log("Skipping [" + rootProperty + "] due to a lack of browser support.");

								continue;
							}

							/* If the display option is being set to a non-"none" (e.g. "block") and opacity (filter on IE<=8) is being
							   animated to an endValue of non-zero, the user's intention is to fade in from invisible, thus we forcefeed opacity
							   a startValue of 0 if its startValue hasn't already been sourced by value transferring or prior forcefeeding. */
							if (((opts.display !== undefined && opts.display !== null && opts.display !== "none") || (opts.visibility !== undefined && opts.visibility !== "hidden")) && /opacity|filter/.test(property) && !startValue && endValue !== 0) {
								startValue = 0;
							}

							/* If values have been transferred from the previous Velocity call, extract the endValue and rootPropertyValue
							   for all of the current call's properties that were *also* animated in the previous call. */
							/* Note: Value transferring can optionally be disabled by the user via the _cacheValues option. */
							if (opts._cacheValues && lastTweensContainer && lastTweensContainer[property]) {
								if (startValue === undefined) {
									startValue = lastTweensContainer[property].endValue + lastTweensContainer[property].unitType;
								}

								/* The previous call's rootPropertyValue is extracted from the element's data cache since that's the
								   instance of rootPropertyValue that gets freshly updated by the tweening process, whereas the rootPropertyValue
								   attached to the incoming lastTweensContainer is equal to the root property's value prior to any tweening. */
								rootPropertyValue = Data(element).rootPropertyValueCache[rootProperty];
								/* If values were not transferred from a previous Velocity call, query the DOM as needed. */
							} else {
								/* Handle hooked properties. */
								if (CSS.Hooks.registered[property]) {
									if (startValue === undefined) {
										rootPropertyValue = CSS.getPropertyValue(element, rootProperty); /* GET */
										/* Note: The following getPropertyValue() call does not actually trigger a DOM query;
										   getPropertyValue() will extract the hook from rootPropertyValue. */
										startValue = CSS.getPropertyValue(element, property, rootPropertyValue);
										/* If startValue is already defined via forcefeeding, do not query the DOM for the root property's value;
										   just grab rootProperty's zero-value template from CSS.Hooks. This overwrites the element's actual
										   root property value (if one is set), but this is acceptable since the primary reason users forcefeed is
										   to avoid DOM queries, and thus we likewise avoid querying the DOM for the root property's value. */
									} else {
										/* Grab this hook's zero-value template, e.g. "0px 0px 0px black". */
										rootPropertyValue = CSS.Hooks.templates[rootProperty][1];
									}
									/* Handle non-hooked properties that haven't already been defined via forcefeeding. */
								} else if (startValue === undefined) {
									startValue = CSS.getPropertyValue(element, property); /* GET */
								}
							}

							/**************************
							   Value Data Extraction
							**************************/

							var separatedValue,
								endValueUnitType,
								startValueUnitType,
								operator = false;

							/* Separates a property value into its numeric value and its unit type. */
							function separateValue (property, value) {
								var unitType,
									numericValue;

								numericValue = (value || "0")
									.toString()
									.toLowerCase()
									/* Match the unit type at the end of the value. */
									.replace(/[%A-z]+$/, function(match) {
										/* Grab the unit type. */
										unitType = match;

										/* Strip the unit type off of value. */
										return "";
									});

								/* If no unit type was supplied, assign one that is appropriate for this property (e.g. "deg" for rotateZ or "px" for width). */
								if (!unitType) {
									unitType = CSS.Values.getUnitType(property);
								}

								return [ numericValue, unitType ];
							}

							/* Separate startValue. */
							separatedValue = separateValue(property, startValue);
							startValue = separatedValue[0];
							startValueUnitType = separatedValue[1];

							/* Separate endValue, and extract a value operator (e.g. "+=", "-=") if one exists. */
							separatedValue = separateValue(property, endValue);
							endValue = separatedValue[0].replace(/^([+-\/*])=/, function(match, subMatch) {
								operator = subMatch;

								/* Strip the operator off of the value. */
								return "";
							});
							endValueUnitType = separatedValue[1];

							/* Parse float values from endValue and startValue. Default to 0 if NaN is returned. */
							startValue = parseFloat(startValue) || 0;
							endValue = parseFloat(endValue) || 0;

							/***************************************
							   Property-Specific Value Conversion
							***************************************/

							/* Custom support for properties that don't actually accept the % unit type, but where pollyfilling is trivial and relatively foolproof. */
							if (endValueUnitType === "%") {
								/* A %-value fontSize/lineHeight is relative to the parent's fontSize (as opposed to the parent's dimensions),
								   which is identical to the em unit's behavior, so we piggyback off of that. */
								if (/^(fontSize|lineHeight)$/.test(property)) {
									/* Convert % into an em decimal value. */
									endValue = endValue / 100;
									endValueUnitType = "em";
									/* For scaleX and scaleY, convert the value into its decimal format and strip off the unit type. */
								} else if (/^scale/.test(property)) {
									endValue = endValue / 100;
									endValueUnitType = "";
									/* For RGB components, take the defined percentage of 255 and strip off the unit type. */
								} else if (/(Red|Green|Blue)$/i.test(property)) {
									endValue = (endValue / 100) * 255;
									endValueUnitType = "";
								}
							}

							/***************************
							   Unit Ratio Calculation
							***************************/

							/* When queried, the browser returns (most) CSS property values in pixels. Therefore, if an endValue with a unit type of
							   %, em, or rem is animated toward, startValue must be converted from pixels into the same unit type as endValue in order
							   for value manipulation logic (increment/decrement) to proceed. Further, if the startValue was forcefed or transferred
							   from a previous call, startValue may also not be in pixels. Unit conversion logic therefore consists of two steps:
							   1) Calculating the ratio of %/em/rem/vh/vw relative to pixels
							   2) Converting startValue into the same unit of measurement as endValue based on these ratios. */
							/* Unit conversion ratios are calculated by inserting a sibling node next to the target node, copying over its position property,
							   setting values with the target unit type then comparing the returned pixel value. */
							/* Note: Even if only one of these unit types is being animated, all unit ratios are calculated at once since the overhead
							   of batching the SETs and GETs together upfront outweights the potential overhead
							   of layout thrashing caused by re-querying for uncalculated ratios for subsequently-processed properties. */
							/* Todo: Shift this logic into the calls' first tick instance so that it's synced with RAF. */
							function calculateUnitRatios () {

								/************************
								    Same Ratio Checks
								************************/

								/* The properties below are used to determine whether the element differs sufficiently from this call's
								   previously iterated element to also differ in its unit conversion ratios. If the properties match up with those
								   of the prior element, the prior element's conversion ratios are used. Like most optimizations in Velocity,
								   this is done to minimize DOM querying. */
								var sameRatioIndicators = {
									    myParent: element.parentNode || document.body, /* GET */
									    position: CSS.getPropertyValue(element, "position"), /* GET */
									    fontSize: CSS.getPropertyValue(element, "fontSize") /* GET */
								    },
									/* Determine if the same % ratio can be used. % is based on the element's position value and its parent's width and height dimensions. */
									samePercentRatio = ((sameRatioIndicators.position === callUnitConversionData.lastPosition) && (sameRatioIndicators.myParent === callUnitConversionData.lastParent)),
									/* Determine if the same em ratio can be used. em is relative to the element's fontSize. */
									sameEmRatio = (sameRatioIndicators.fontSize === callUnitConversionData.lastFontSize);

								/* Store these ratio indicators call-wide for the next element to compare against. */
								callUnitConversionData.lastParent = sameRatioIndicators.myParent;
								callUnitConversionData.lastPosition = sameRatioIndicators.position;
								callUnitConversionData.lastFontSize = sameRatioIndicators.fontSize;

								/***************************
								   Element-Specific Units
								***************************/

								/* Note: IE8 rounds to the nearest pixel when returning CSS values, thus we perform conversions using a measurement
								   of 100 (instead of 1) to give our ratios a precision of at least 2 decimal values. */
								var measurement = 100,
									unitRatios = {};

								if (!sameEmRatio || !samePercentRatio) {
									var dummy = Data(element).isSVG ? document.createElementNS("http://www.w3.org/2000/svg", "rect") : document.createElement("div");

									Velocity.init(dummy);
									sameRatioIndicators.myParent.appendChild(dummy);

									/* To accurately and consistently calculate conversion ratios, the element's cascaded overflow and box-sizing are stripped.
									   Similarly, since width/height can be artificially constrained by their min-/max- equivalents, these are controlled for as well. */
									/* Note: Overflow must be also be controlled for per-axis since the overflow property overwrites its per-axis values. */
									$.each([ "overflow", "overflowX", "overflowY" ], function(i, property) {
										Velocity.CSS.setPropertyValue(dummy, property, "hidden");
									});
									Velocity.CSS.setPropertyValue(dummy, "position", sameRatioIndicators.position);
									Velocity.CSS.setPropertyValue(dummy, "fontSize", sameRatioIndicators.fontSize);
									Velocity.CSS.setPropertyValue(dummy, "boxSizing", "content-box");

									/* width and height act as our proxy properties for measuring the horizontal and vertical % ratios. */
									$.each([ "minWidth", "maxWidth", "width", "minHeight", "maxHeight", "height" ], function(i, property) {
										Velocity.CSS.setPropertyValue(dummy, property, measurement + "%");
									});
									/* paddingLeft arbitrarily acts as our proxy property for the em ratio. */
									Velocity.CSS.setPropertyValue(dummy, "paddingLeft", measurement + "em");

									/* Divide the returned value by the measurement to get the ratio between 1% and 1px. Default to 1 since working with 0 can produce Infinite. */
									unitRatios.percentToPxWidth = callUnitConversionData.lastPercentToPxWidth = (parseFloat(CSS.getPropertyValue(dummy, "width", null, true)) || 1) / measurement; /* GET */
									unitRatios.percentToPxHeight = callUnitConversionData.lastPercentToPxHeight = (parseFloat(CSS.getPropertyValue(dummy, "height", null, true)) || 1) / measurement; /* GET */
									unitRatios.emToPx = callUnitConversionData.lastEmToPx = (parseFloat(CSS.getPropertyValue(dummy, "paddingLeft")) || 1) / measurement; /* GET */

									sameRatioIndicators.myParent.removeChild(dummy);
								} else {
									unitRatios.emToPx = callUnitConversionData.lastEmToPx;
									unitRatios.percentToPxWidth = callUnitConversionData.lastPercentToPxWidth;
									unitRatios.percentToPxHeight = callUnitConversionData.lastPercentToPxHeight;
								}

								/***************************
								   Element-Agnostic Units
								***************************/

								/* Whereas % and em ratios are determined on a per-element basis, the rem unit only needs to be checked
								   once per call since it's exclusively dependant upon document.body's fontSize. If this is the first time
								   that calculateUnitRatios() is being run during this call, remToPx will still be set to its default value of null,
								   so we calculate it now. */
								if (callUnitConversionData.remToPx === null) {
									/* Default to browsers' default fontSize of 16px in the case of 0. */
									callUnitConversionData.remToPx = parseFloat(CSS.getPropertyValue(document.body, "fontSize")) || 16; /* GET */
								}

								/* Similarly, viewport units are %-relative to the window's inner dimensions. */
								if (callUnitConversionData.vwToPx === null) {
									callUnitConversionData.vwToPx = parseFloat(window.innerWidth) / 100; /* GET */
									callUnitConversionData.vhToPx = parseFloat(window.innerHeight) / 100; /* GET */
								}

								unitRatios.remToPx = callUnitConversionData.remToPx;
								unitRatios.vwToPx = callUnitConversionData.vwToPx;
								unitRatios.vhToPx = callUnitConversionData.vhToPx;

								if (Velocity.debug >= 1) console.log("Unit ratios: " + JSON.stringify(unitRatios), element);

								return unitRatios;
							}

							/********************
							   Unit Conversion
							********************/

							/* The * and / operators, which are not passed in with an associated unit, inherently use startValue's unit. Skip value and unit conversion. */
							if (/[\/*]/.test(operator)) {
								endValueUnitType = startValueUnitType;
								/* If startValue and endValue differ in unit type, convert startValue into the same unit type as endValue so that if endValueUnitType
								   is a relative unit (%, em, rem), the values set during tweening will continue to be accurately relative even if the metrics they depend
								   on are dynamically changing during the course of the animation. Conversely, if we always normalized into px and used px for setting values, the px ratio
								   would become stale if the original unit being animated toward was relative and the underlying metrics change during the animation. */
								/* Since 0 is 0 in any unit type, no conversion is necessary when startValue is 0 -- we just start at 0 with endValueUnitType. */
							} else if ((startValueUnitType !== endValueUnitType) && startValue !== 0) {
								/* Unit conversion is also skipped when endValue is 0, but *startValueUnitType* must be used for tween values to remain accurate. */
								/* Note: Skipping unit conversion here means that if endValueUnitType was originally a relative unit, the animation won't relatively
								   match the underlying metrics if they change, but this is acceptable since we're animating toward invisibility instead of toward visibility,
								   which remains past the point of the animation's completion. */
								if (endValue === 0) {
									endValueUnitType = startValueUnitType;
								} else {
									/* By this point, we cannot avoid unit conversion (it's undesirable since it causes layout thrashing).
									   If we haven't already, we trigger calculateUnitRatios(), which runs once per element per call. */
									elementUnitConversionData = elementUnitConversionData || calculateUnitRatios();

									/* The following RegEx matches CSS properties that have their % values measured relative to the x-axis. */
									/* Note: W3C spec mandates that all of margin and padding's properties (even top and bottom) are %-relative to the *width* of the parent element. */
									var axis = (/margin|padding|left|right|width|text|word|letter/i.test(property) || /X$/.test(property) || property === "x") ? "x" : "y";

									/* In order to avoid generating n^2 bespoke conversion functions, unit conversion is a two-step process:
									   1) Convert startValue into pixels. 2) Convert this new pixel value into endValue's unit type. */
									switch (startValueUnitType) {
									case "%":
										/* Note: translateX and translateY are the only properties that are %-relative to an element's own dimensions -- not its parent's dimensions.
										   Velocity does not include a special conversion process to account for this behavior. Therefore, animating translateX/Y from a % value
										   to a non-% value will produce an incorrect start value. Fortunately, this sort of cross-unit conversion is rarely done by users in practice. */
										startValue *= (axis === "x" ? elementUnitConversionData.percentToPxWidth : elementUnitConversionData.percentToPxHeight);
										break;

									case "px":
										/* px acts as our midpoint in the unit conversion process; do nothing. */
										break;

									default:
										startValue *= elementUnitConversionData[startValueUnitType + "ToPx"];
									}

									/* Invert the px ratios to convert into to the target unit. */
									switch (endValueUnitType) {
									case "%":
										startValue *= 1 / (axis === "x" ? elementUnitConversionData.percentToPxWidth : elementUnitConversionData.percentToPxHeight);
										break;

									case "px":
										/* startValue is already in px, do nothing; we're done. */
										break;

									default:
										startValue *= 1 / elementUnitConversionData[endValueUnitType + "ToPx"];
									}
								}
							}

							/*********************
							   Relative Values
							*********************/

							/* Operator logic must be performed last since it requires unit-normalized start and end values. */
							/* Note: Relative *percent values* do not behave how most people think; while one would expect "+=50%"
							   to increase the property 1.5x its current value, it in fact increases the percent units in absolute terms:
							   50 points is added on top of the current % value. */
							switch (operator) {
							case "+":
								endValue = startValue + endValue;
								break;

							case "-":
								endValue = startValue - endValue;
								break;

							case "*":
								endValue = startValue * endValue;
								break;

							case "/":
								endValue = startValue / endValue;
								break;
							}

							/**************************
							   tweensContainer Push
							**************************/

							/* Construct the per-property tween object, and push it to the element's tweensContainer. */
							tweensContainer[property] = {
								rootPropertyValue: rootPropertyValue,
								startValue: startValue,
								currentValue: startValue,
								endValue: endValue,
								unitType: endValueUnitType,
								easing: easing
							};

							if (Velocity.debug) console.log("tweensContainer (" + property + "): " + JSON.stringify(tweensContainer[property]), element);
						}

						/* Along with its property data, store a reference to the element itself onto tweensContainer. */
						tweensContainer.element = element;
					}

					/*****************
					    Call Push
					*****************/

					/* Note: tweensContainer can be empty if all of the properties in this call's property map were skipped due to not
					   being supported by the browser. The element property is used for checking that the tweensContainer has been appended to. */
					if (tweensContainer.element) {
						/* Apply the "velocity-animating" indicator class. */
						CSS.Values.addClass(element, "velocity-animating");

						/* The call array houses the tweensContainers for each element being animated in the current call. */
						call.push(tweensContainer);

						/* Store the tweensContainer and options if we're working on the default effects queue, so that they can be used by the reverse command. */
						if (opts.queue === "") {
							Data(element).tweensContainer = tweensContainer;
							Data(element).opts = opts;
						}

						/* Switch on the element's animating flag. */
						Data(element).isAnimating = true;

						/* Once the final element in this call's element set has been processed, push the call array onto
						   Velocity.State.calls for the animation tick to immediately begin processing. */
						if (elementsIndex === elementsLength - 1) {
							/* Add the current call plus its associated metadata (the element set and the call's options) onto the global call container.
							   Anything on this call container is subjected to tick() processing. */
							Velocity.State.calls.push([ call, elements, opts, null, promiseData.resolver ]);

							/* If the animation tick isn't running, start it. (Velocity shuts it off when there are no active calls to process.) */
							if (Velocity.State.isTicking === false) {
								Velocity.State.isTicking = true;

								/* Start the tick loop. */
								tick();
							}
						} else {
							elementsIndex++;
						}
					}
				}

				/* When the queue option is set to false, the call skips the element's queue and fires immediately. */
				if (opts.queue === false) {
					/* Since this buildQueue call doesn't respect the element's existing queue (which is where a delay option would have been appended),
					   we manually inject the delay property here with an explicit setTimeout. */
					if (opts.delay) {
						setTimeout(buildQueue, opts.delay);
					} else {
						buildQueue();
					}
					/* Otherwise, the call undergoes element queueing as normal. */
					/* Note: To interoperate with jQuery, Velocity uses jQuery's own $.queue() stack for queuing logic. */
				} else {
					$.queue(element, opts.queue, function(next, clearQueue) {
						/* If the clearQueue flag was passed in by the stop command, resolve this call's promise. (Promises can only be resolved once,
						   so it's fine if this is repeatedly triggered for each element in the associated call.) */
						if (clearQueue === true) {
							if (promiseData.promise) {
								promiseData.resolver(elements);
							}

							/* Do not continue with animation queueing. */
							return true;
						}

						/* This flag indicates to the upcoming completeCall() function that this queue entry was initiated by Velocity.
						   See completeCall() for further details. */
						Velocity.velocityQueueEntryFlag = true;

						buildQueue(next);
					});
				}

				/*********************
				    Auto-Dequeuing
				*********************/

				/* As per jQuery's $.queue() behavior, to fire the first non-custom-queue entry on an element, the element
				   must be dequeued if its queue stack consists *solely* of the current call. (This can be determined by checking
				   for the "inprogress" item that jQuery prepends to active queue stack arrays.) Regardless, whenever the element's
				   queue is further appended with additional items -- including $.delay()'s or even $.animate() calls, the queue's
				   first entry is automatically fired. This behavior contrasts that of custom queues, which never auto-fire. */
				/* Note: When an element set is being subjected to a non-parallel Velocity call, the animation will not begin until
				   each one of the elements in the set has reached the end of its individually pre-existing queue chain. */
				/* Note: Unfortunately, most people don't fully grasp jQuery's powerful, yet quirky, $.queue() function.
				   Lean more here: http://stackoverflow.com/questions/1058158/can-somebody-explain-jquery-queue-to-me */
				if ((opts.queue === "" || opts.queue === "fx") && $.queue(element)[0] !== "inprogress") {
					$.dequeue(element);
				}
			}

			/**************************
			   Element Set Iteration
			**************************/

			/* If the "nodeType" property exists on the elements variable, we're animating a single element.
			   Place it in an array so that $.each() can iterate over it. */
			$.each(elements, function(i, element) {
				/* Ensure each element in a set has a nodeType (is a real element) to avoid throwing errors. */
				if (Type.isNode(element)) {
					processElement.call(element);
				}
			});

			/******************
			   Option: Loop
			******************/

			/* The loop option accepts an integer indicating how many times the element should loop between the values in the
			   current call's properties map and the element's property values prior to this call. */
			/* Note: The loop option's logic is performed here -- after element processing -- because the current call needs
			   to undergo its queue insertion prior to the loop option generating its series of constituent "reverse" calls,
			   which chain after the current call. Two reverse calls (two "alternations") constitute one loop. */
			var opts = $.extend({}, Velocity.defaults, options),
				reverseCallsCount;

			opts.loop = parseInt(opts.loop);
			reverseCallsCount = (opts.loop * 2) - 1;

			if (opts.loop) {
				/* Double the loop count to convert it into its appropriate number of "reverse" calls.
				   Subtract 1 from the resulting value since the current call is included in the total alternation count. */
				for (var x = 0; x < reverseCallsCount; x++) {
					/* Since the logic for the reverse action occurs inside Queueing and therefore this call's options object
					   isn't parsed until then as well, the current call's delay option must be explicitly passed into the reverse
					   call so that the delay logic that occurs inside *Pre-Queueing* can process it. */
					var reverseOptions = {
						delay: opts.delay,
						progress: opts.progress
					};

					/* If a complete callback was passed into this call, transfer it to the loop redirect's final "reverse" call
					   so that it's triggered when the entire redirect is complete (and not when the very first animation is complete). */
					if (x === reverseCallsCount - 1) {
						reverseOptions.display = opts.display;
						reverseOptions.visibility = opts.visibility;
						reverseOptions.complete = opts.complete;
					}

					animate(elements, "reverse", reverseOptions);
				}
			}

			/***************
			    Chaining
			***************/

			/* Return the elements back to the call chain, with wrapped elements taking precedence in case Velocity was called via the $.fn. extension. */
			return getChain();
		};

		/* Turn Velocity into the animation function, extended with the pre-existing Velocity object. */
		Velocity = $.extend(animate, Velocity);
		/* For legacy support, also expose the literal animate method. */
		Velocity.animate = animate;

		/**************
		    Timing
		**************/

		/* Ticker function. */
		var ticker = window.requestAnimationFrame || rAFShim;

		/* Inactive browser tabs pause rAF, which results in all active animations immediately sprinting to their completion states when the tab refocuses.
		   To get around this, we dynamically switch rAF to setTimeout (which the browser *doesn't* pause) when the tab loses focus. We skip this for mobile
		   devices to avoid wasting battery power on inactive tabs. */
		/* Note: Tab focus detection doesn't work on older versions of IE, but that's okay since they don't support rAF to begin with. */
		if (!Velocity.State.isMobile && document.hidden !== undefined) {
			document.addEventListener("visibilitychange", function() {
				/* Reassign the rAF function (which the global tick() function uses) based on the tab's focus state. */
				if (document.hidden) {
					ticker = function(callback) {
						/* The tick function needs a truthy first argument in order to pass its internal timestamp check. */
						return setTimeout(function() { callback(true) }, 16);
					};

					/* The rAF loop has been paused by the browser, so we manually restart the tick. */
					tick();
				} else {
					ticker = window.requestAnimationFrame || rAFShim;
				}
			});
		}

		/************
		    Tick
		************/

		/* Note: All calls to Velocity are pushed to the Velocity.State.calls array, which is fully iterated through upon each tick. */
		function tick (timestamp) {
			/* An empty timestamp argument indicates that this is the first tick occurence since ticking was turned on.
			   We leverage this metadata to fully ignore the first tick pass since RAF's initial pass is fired whenever
			   the browser's next tick sync time occurs, which results in the first elements subjected to Velocity
			   calls being animated out of sync with any elements animated immediately thereafter. In short, we ignore
			   the first RAF tick pass so that elements being immediately consecutively animated -- instead of simultaneously animated
			   by the same Velocity call -- are properly batched into the same initial RAF tick and consequently remain in sync thereafter. */
			if (timestamp) {
				/* We ignore RAF's high resolution timestamp since it can be significantly offset when the browser is
				   under high stress; we opt for choppiness over allowing the browser to drop huge chunks of frames. */
				var timeCurrent = (new Date).getTime();

				/********************
				   Call Iteration
				********************/

				var callsLength = Velocity.State.calls.length;

				/* To speed up iterating over this array, it is compacted (falsey items -- calls that have completed -- are removed)
				   when its length has ballooned to a point that can impact tick performance. This only becomes necessary when animation
				   has been continuous with many elements over a long period of time; whenever all active calls are completed, completeCall() clears Velocity.State.calls. */
				if (callsLength > 10000) {
					Velocity.State.calls = compactSparseArray(Velocity.State.calls);
				}

				/* Iterate through each active call. */
				for (var i = 0; i < callsLength; i++) {
					/* When a Velocity call is completed, its Velocity.State.calls entry is set to false. Continue on to the next call. */
					if (!Velocity.State.calls[i]) {
						continue;
					}

					/************************
					   Call-Wide Variables
					************************/

					var callContainer = Velocity.State.calls[i],
						call = callContainer[0],
						opts = callContainer[2],
						timeStart = callContainer[3],
						firstTick = !!timeStart,
						tweenDummyValue = null;

					/* If timeStart is undefined, then this is the first time that this call has been processed by tick().
					   We assign timeStart now so that its value is as close to the real animation start time as possible.
					   (Conversely, had timeStart been defined when this call was added to Velocity.State.calls, the delay
					   between that time and now would cause the first few frames of the tween to be skipped since
					   percentComplete is calculated relative to timeStart.) */
					/* Further, subtract 16ms (the approximate resolution of RAF) from the current time value so that the
					   first tick iteration isn't wasted by animating at 0% tween completion, which would produce the
					   same style value as the element's current value. */
					if (!timeStart) {
						timeStart = Velocity.State.calls[i][3] = timeCurrent - 16;
					}

					/* The tween's completion percentage is relative to the tween's start time, not the tween's start value
					   (which would result in unpredictable tween durations since JavaScript's timers are not particularly accurate).
					   Accordingly, we ensure that percentComplete does not exceed 1. */
					var percentComplete = Math.min((timeCurrent - timeStart) / opts.duration, 1);

					/**********************
					   Element Iteration
					**********************/

					/* For every call, iterate through each of the elements in its set. */
					for (var j = 0, callLength = call.length; j < callLength; j++) {
						var tweensContainer = call[j],
							element = tweensContainer.element;

						/* Check to see if this element has been deleted midway through the animation by checking for the
						   continued existence of its data cache. If it's gone, skip animating this element. */
						if (!Data(element)) {
							continue;
						}

						var transformPropertyExists = false;

						/**********************************
						   Display & Visibility Toggling
						**********************************/

						/* If the display option is set to non-"none", set it upfront so that the element can become visible before tweening begins.
						   (Otherwise, display's "none" value is set in completeCall() once the animation has completed.) */
						if (opts.display !== undefined && opts.display !== null && opts.display !== "none") {
							if (opts.display === "flex") {
								var flexValues = [ "-webkit-box", "-moz-box", "-ms-flexbox", "-webkit-flex" ];

								$.each(flexValues, function(i, flexValue) {
									CSS.setPropertyValue(element, "display", flexValue);
								});
							}

							CSS.setPropertyValue(element, "display", opts.display);
						}

						/* Same goes with the visibility option, but its "none" equivalent is "hidden". */
						if (opts.visibility !== undefined && opts.visibility !== "hidden") {
							CSS.setPropertyValue(element, "visibility", opts.visibility);
						}

						/************************
						   Property Iteration
						************************/

						/* For every element, iterate through each property. */
						for (var property in tweensContainer) {
							/* Note: In addition to property tween data, tweensContainer contains a reference to its associated element. */
							if (property !== "element") {
								var tween = tweensContainer[property],
									currentValue,
									/* Easing can either be a pre-genereated function or a string that references a pre-registered easing
									   on the Velocity.Easings object. In either case, return the appropriate easing *function*. */
									easing = Type.isString(tween.easing) ? Velocity.Easings[tween.easing] : tween.easing;

								/******************************
								   Current Value Calculation
								******************************/

								/* If this is the last tick pass (if we've reached 100% completion for this tween),
								   ensure that currentValue is explicitly set to its target endValue so that it's not subjected to any rounding. */
								if (percentComplete === 1) {
									currentValue = tween.endValue;
									/* Otherwise, calculate currentValue based on the current delta from startValue. */
								} else {
									var tweenDelta = tween.endValue - tween.startValue;
									currentValue = tween.startValue + (tweenDelta * easing(percentComplete, opts, tweenDelta));

									/* If no value change is occurring, don't proceed with DOM updating. */
									if (!firstTick && (currentValue === tween.currentValue)) {
										continue;
									}
								}

								tween.currentValue = currentValue;

								/* If we're tweening a fake 'tween' property in order to log transition values, update the one-per-call variable so that
								   it can be passed into the progress callback. */
								if (property === "tween") {
									tweenDummyValue = currentValue;
								} else {
									/******************
									   Hooks: Part I
									******************/

									/* For hooked properties, the newly-updated rootPropertyValueCache is cached onto the element so that it can be used
									   for subsequent hooks in this call that are associated with the same root property. If we didn't cache the updated
									   rootPropertyValue, each subsequent update to the root property in this tick pass would reset the previous hook's
									   updates to rootPropertyValue prior to injection. A nice performance byproduct of rootPropertyValue caching is that
									   subsequently chained animations using the same hookRoot but a different hook can use this cached rootPropertyValue. */
									if (CSS.Hooks.registered[property]) {
										var hookRoot = CSS.Hooks.getRoot(property),
											rootPropertyValueCache = Data(element).rootPropertyValueCache[hookRoot];

										if (rootPropertyValueCache) {
											tween.rootPropertyValue = rootPropertyValueCache;
										}
									}

									/*****************
									    DOM Update
									*****************/

									/* setPropertyValue() returns an array of the property name and property value post any normalization that may have been performed. */
									/* Note: To solve an IE<=8 positioning bug, the unit type is dropped when setting a property value of 0. */
									var adjustedSetData = CSS.setPropertyValue(element, /* SET */
										property,
										tween.currentValue + (parseFloat(currentValue) === 0 ? "" : tween.unitType),
										tween.rootPropertyValue,
										tween.scrollData);

									/*******************
									   Hooks: Part II
									*******************/

									/* Now that we have the hook's updated rootPropertyValue (the post-processed value provided by adjustedSetData), cache it onto the element. */
									if (CSS.Hooks.registered[property]) {
										/* Since adjustedSetData contains normalized data ready for DOM updating, the rootPropertyValue needs to be re-extracted from its normalized form. ?? */
										if (CSS.Normalizations.registered[hookRoot]) {
											Data(element).rootPropertyValueCache[hookRoot] = CSS.Normalizations.registered[hookRoot]("extract", null, adjustedSetData[1]);
										} else {
											Data(element).rootPropertyValueCache[hookRoot] = adjustedSetData[1];
										}
									}

									/***************
									   Transforms
									***************/

									/* Flag whether a transform property is being animated so that flushTransformCache() can be triggered once this tick pass is complete. */
									if (adjustedSetData[0] === "transform") {
										transformPropertyExists = true;
									}

								}
							}
						}

						/****************
						    mobileHA
						****************/

						/* If mobileHA is enabled, set the translate3d transform to null to force hardware acceleration.
						   It's safe to override this property since Velocity doesn't actually support its animation (hooks are used in its place). */
						if (opts.mobileHA) {
							/* Don't set the null transform hack if we've already done so. */
							if (Data(element).transformCache.translate3d === undefined) {
								/* All entries on the transformCache object are later concatenated into a single transform string via flushTransformCache(). */
								Data(element).transformCache.translate3d = "(0px, 0px, 0px)";

								transformPropertyExists = true;
							}
						}

						if (transformPropertyExists) {
							CSS.flushTransformCache(element);
						}
					}

					/* The non-"none" display value is only applied to an element once -- when its associated call is first ticked through.
					   Accordingly, it's set to false so that it isn't re-processed by this call in the next tick. */
					if (opts.display !== undefined && opts.display !== "none") {
						Velocity.State.calls[i][2].display = false;
					}
					if (opts.visibility !== undefined && opts.visibility !== "hidden") {
						Velocity.State.calls[i][2].visibility = false;
					}

					/* Pass the elements and the timing data (percentComplete, msRemaining, timeStart, tweenDummyValue) into the progress callback. */
					if (opts.progress) {
						opts.progress.call(callContainer[1],
							callContainer[1],
							percentComplete,
							Math.max(0, (timeStart + opts.duration) - timeCurrent),
							timeStart,
							tweenDummyValue);
					}

					/* If this call has finished tweening, pass its index to completeCall() to handle call cleanup. */
					if (percentComplete === 1) {
						completeCall(i);
					}
				}
			}

			/* Note: completeCall() sets the isTicking flag to false when the last call on Velocity.State.calls has completed. */
			if (Velocity.State.isTicking) {
				ticker(tick);
			}
		}

		/**********************
		    Call Completion
		**********************/

		/* Note: Unlike tick(), which processes all active calls at once, call completion is handled on a per-call basis. */
		function completeCall (callIndex, isStopped) {
			/* Ensure the call exists. */
			if (!Velocity.State.calls[callIndex]) {
				return false;
			}

			/* Pull the metadata from the call. */
			var call = Velocity.State.calls[callIndex][0],
				elements = Velocity.State.calls[callIndex][1],
				opts = Velocity.State.calls[callIndex][2],
				resolver = Velocity.State.calls[callIndex][4];

			var remainingCallsExist = false;

			/*************************
			   Element Finalization
			*************************/

			for (var i = 0, callLength = call.length; i < callLength; i++) {
				var element = call[i].element;

				/* If the user set display to "none" (intending to hide the element), set it now that the animation has completed. */
				/* Note: display:none isn't set when calls are manually stopped (via Velocity("stop"). */
				/* Note: Display gets ignored with "reverse" calls and infinite loops, since this behavior would be undesirable. */
				if (!isStopped && !opts.loop) {
					if (opts.display === "none") {
						CSS.setPropertyValue(element, "display", opts.display);
					}

					if (opts.visibility === "hidden") {
						CSS.setPropertyValue(element, "visibility", opts.visibility);
					}
				}

				/* If the element's queue is empty (if only the "inprogress" item is left at position 0) or if its queue is about to run
				   a non-Velocity-initiated entry, turn off the isAnimating flag. A non-Velocity-initiatied queue entry's logic might alter
				   an element's CSS values and thereby cause Velocity's cached value data to go stale. To detect if a queue entry was initiated by Velocity,
				   we check for the existence of our special Velocity.queueEntryFlag declaration, which minifiers won't rename since the flag
				   is assigned to jQuery's global $ object and thus exists out of Velocity's own scope. */
				if (opts.loop !== true && ($.queue(element)[1] === undefined || !/\.velocityQueueEntryFlag/i.test($.queue(element)[1]))) {
					/* The element may have been deleted. Ensure that its data cache still exists before acting on it. */
					if (Data(element)) {
						Data(element).isAnimating = false;
						/* Clear the element's rootPropertyValueCache, which will become stale. */
						Data(element).rootPropertyValueCache = {};

						var transformHAPropertyExists = false;
						/* If any 3D transform subproperty is at its default value (regardless of unit type), remove it. */
						$.each(CSS.Lists.transforms3D, function(i, transformName) {
							var defaultValue = /^scale/.test(transformName) ? 1 : 0,
								currentValue = Data(element).transformCache[transformName];

							if (Data(element).transformCache[transformName] !== undefined && new RegExp("^\\(" + defaultValue + "[^.]").test(currentValue)) {
								transformHAPropertyExists = true;

								delete Data(element).transformCache[transformName];
							}
						});

						/* Mobile devices have hardware acceleration removed at the end of the animation in order to avoid hogging the GPU's memory. */
						if (opts.mobileHA) {
							transformHAPropertyExists = true;
							delete Data(element).transformCache.translate3d;
						}

						/* Flush the subproperty removals to the DOM. */
						if (transformHAPropertyExists) {
							CSS.flushTransformCache(element);
						}

						/* Remove the "velocity-animating" indicator class. */
						CSS.Values.removeClass(element, "velocity-animating");
					}
				}

				/*********************
				   Option: Complete
				*********************/

				/* Complete is fired once per call (not once per element) and is passed the full raw DOM element set as both its context and its first argument. */
				/* Note: Callbacks aren't fired when calls are manually stopped (via Velocity("stop"). */
				if (!isStopped && opts.complete && !opts.loop && (i === callLength - 1)) {
					/* We throw callbacks in a setTimeout so that thrown errors don't halt the execution of Velocity itself. */
					try {
						opts.complete.call(elements, elements);
					} catch (error) {
						setTimeout(function() { throw error; }, 1);
					}
				}

				/**********************
				   Promise Resolving
				**********************/

				/* Note: Infinite loops don't return promises. */
				if (resolver && opts.loop !== true) {
					resolver(elements);
				}

				/****************************
				   Option: Loop (Infinite)
				****************************/

				if (Data(element) && opts.loop === true && !isStopped) {
					/* If a rotateX/Y/Z property is being animated to 360 deg with loop:true, swap tween start/end values to enable
					   continuous iterative rotation looping. (Otherise, the element would just rotate back and forth.) */
					$.each(Data(element).tweensContainer, function(propertyName, tweenContainer) {
						if (/^rotate/.test(propertyName) && parseFloat(tweenContainer.endValue) === 360) {
							tweenContainer.endValue = 0;
							tweenContainer.startValue = 360;
						}

						if (/^backgroundPosition/.test(propertyName) && parseFloat(tweenContainer.endValue) === 100 && tweenContainer.unitType === "%") {
							tweenContainer.endValue = 0;
							tweenContainer.startValue = 100;
						}
					});

					Velocity(element, "reverse", { loop: true, delay: opts.delay });
				}

				/***************
				   Dequeueing
				***************/

				/* Fire the next call in the queue so long as this call's queue wasn't set to false (to trigger a parallel animation),
				   which would have already caused the next call to fire. Note: Even if the end of the animation queue has been reached,
				   $.dequeue() must still be called in order to completely clear jQuery's animation queue. */
				if (opts.queue !== false) {
					$.dequeue(element, opts.queue);
				}
			}

			/************************
			   Calls Array Cleanup
			************************/

			/* Since this call is complete, set it to false so that the rAF tick skips it. This array is later compacted via compactSparseArray().
			  (For performance reasons, the call is set to false instead of being deleted from the array: http://www.html5rocks.com/en/tutorials/speed/v8/) */
			Velocity.State.calls[callIndex] = false;

			/* Iterate through the calls array to determine if this was the final in-progress animation.
			   If so, set a flag to end ticking and clear the calls array. */
			for (var j = 0, callsLength = Velocity.State.calls.length; j < callsLength; j++) {
				if (Velocity.State.calls[j] !== false) {
					remainingCallsExist = true;

					break;
				}
			}

			if (remainingCallsExist === false) {
				/* tick() will detect this flag upon its next iteration and subsequently turn itself off. */
				Velocity.State.isTicking = false;

				/* Clear the calls array so that its length is reset. */
				delete Velocity.State.calls;
				Velocity.State.calls = [];
			}
		}

		/******************
		    Frameworks
		******************/

		/* Both jQuery and Zepto allow their $.fn object to be extended to allow wrapped elements to be subjected to plugin calls.
		   If either framework is loaded, register a "velocity" extension pointing to Velocity's core animate() method.  Velocity
		   also registers itself onto a global container (window.jQuery || window.Zepto || window) so that certain features are
		   accessible beyond just a per-element scope. This master object contains an .animate() method, which is later assigned to $.fn
		   (if jQuery or Zepto are present). Accordingly, Velocity can both act on wrapped DOM elements and stand alone for targeting raw DOM elements. */
		global.Velocity = Velocity;

		if (global !== window) {
			/* Assign the element function to Velocity's core animate() method. */
			global.fn.velocity = animate;
			/* Assign the object function's defaults to Velocity's global defaults object. */
			global.fn.velocity.defaults = Velocity.defaults;
		}

		/***********************
		   Packaged Redirects
		***********************/

		/* slideUp, slideDown */
		$.each([ "Down", "Up" ], function(i, direction) {
			Velocity.Redirects["slide" + direction] = function (element, options, elementsIndex, elementsSize, elements, promiseData) {
				var opts = $.extend({}, options),
					begin = opts.begin,
					complete = opts.complete,
					computedValues = { height: "", marginTop: "", marginBottom: "", paddingTop: "", paddingBottom: "" },
					inlineValues = {};

				if (opts.display === undefined) {
					/* Show the element before slideDown begins and hide the element after slideUp completes. */
					/* Note: Inline elements cannot have dimensions animated, so they're reverted to inline-block. */
					opts.display = (direction === "Down" ? (Velocity.CSS.Values.getDisplayType(element) === "inline" ? "inline-block" : "block") : "none");
				}

				opts.begin = function() {
					/* If the user passed in a begin callback, fire it now. */
					begin && begin.call(elements, elements);

					/* Cache the elements' original vertical dimensional property values so that we can animate back to them. */
					for (var property in computedValues) {
						inlineValues[property] = element.style[property];

						/* For slideDown, use forcefeeding to animate all vertical properties from 0. For slideUp,
						   use forcefeeding to start from computed values and animate down to 0. */
						var propertyValue = Velocity.CSS.getPropertyValue(element, property);
						computedValues[property] = (direction === "Down") ? [ propertyValue, 0 ] : [ 0, propertyValue ];
					}

					/* Force vertical overflow content to clip so that sliding works as expected. */
					inlineValues.overflow = element.style.overflow;
					element.style.overflow = "hidden";
				}

				opts.complete = function() {
					/* Reset element to its pre-slide inline values once its slide animation is complete. */
					for (var property in inlineValues) {
						element.style[property] = inlineValues[property];
					}

					/* If the user passed in a complete callback, fire it now. */
					complete && complete.call(elements, elements);
					promiseData && promiseData.resolver(elements);
				};

				Velocity(element, computedValues, opts);
			};
		});

		/* fadeIn, fadeOut */
		$.each([ "In", "Out" ], function(i, direction) {
			Velocity.Redirects["fade" + direction] = function (element, options, elementsIndex, elementsSize, elements, promiseData) {
				var opts = $.extend({}, options),
					propertiesMap = { opacity: (direction === "In") ? 1 : 0 },
					originalComplete = opts.complete;

				/* Since redirects are triggered individually for each element in the animated set, avoid repeatedly triggering
				   callbacks by firing them only when the final element has been reached. */
				if (elementsIndex !== elementsSize - 1) {
					opts.complete = opts.begin = null;
				} else {
					opts.complete = function() {
						if (originalComplete) {
							originalComplete.call(elements, elements);
						}

						promiseData && promiseData.resolver(elements);
					}
				}

				/* If a display was passed in, use it. Otherwise, default to "none" for fadeOut or the element-specific default for fadeIn. */
				/* Note: We allow users to pass in "null" to skip display setting altogether. */
				if (opts.display === undefined) {
					opts.display = (direction === "In" ? "auto" : "none");
				}

				Velocity(this, propertiesMap, opts);
			};
		});

		return Velocity;
	}((window.jQuery || window.Zepto || window), window, document);
}));