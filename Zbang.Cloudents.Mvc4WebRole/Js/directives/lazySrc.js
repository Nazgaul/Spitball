angular.module('lazySrc', [])
    .directive('lazySrcContainer', function ($window, $document) {
        "use strict";
        return {
            restirct: 'A',
            controller: function ($scope) {
                    var images = [];

                    var renderTimer = null;
                    var renderDelay = 100;

                    var doc = $document;
                    var documentHeight = doc.height();
                    var documentTimer = null;
                    var documentDelay = 2000;

                    var isWatchingWindow = false;

                    // ---
                    //#region PUBLIC METHODS.
                    // ---

                    this.addImage = function (image) {
                        images.push(image);

                        if (!renderTimer) {

                            startRenderTimer();

                        }

                        if (!isWatchingWindow) {
                            startWatchingWindow();
                        }
                    }

                    this.removeImage = function (image) {

                        var index = images.indexOf(image);
                        images.splice(index, 1);

                        if (!images.length) {

                            clearRenderTimer();
                            stopWatchingWindow();
                        }
                    }
                    //#endregion PUBLIC METHODS.

                    // ---
                    //#region PRIVATE METHODS.
                    // ---


                    function checkDocumentHeight() {

                        if (renderTimer) {
                            return;
                        }

                        var currentDocumentHeight = doc.height();

                        if (currentDocumentHeight === documentHeight) {
                            return;
                        }

                        documentHeight = currentDocumentHeight;

                        startRenderTimer();
                    }


                    function checkImages() {
                        var visible = [],
                            hidden = [],
                            elementHeight = element.height(),
                            scrollTop = element.scrollTop();
                        var topFoldOffset = scrollTop;
                        var bottomFoldOffset = (topFoldOffset + elementHeight);

                        for (var i = 0 ; i < images.length ; i++) {

                            var image = images[i];

                            if (image.isVisible(topFoldOffset, bottomFoldOffset)) {

                                visible.push(image);

                            } else {

                                hidden.push(image);

                            }

                        }

                        for (var i = 0 ; i < visible.length ; i++) {

                            visible[i].render();
                        }
                        images = hidden;
                        clearRenderTimer();
                        if (!images.length) {
                            stopWatchingWindow();
                        }
                    }

                    function clearRenderTimer() {
                        clearTimeout(renderTimer);
                        renderTimer = null;
                    }

                    function startRenderTimer() {
                        renderTimer = setTimeout(checkImages, renderDelay);
                    }

                    function startWatchingWindow() {
                        isWatchingWindow = true;

                        element.on("scroll.lazySrc", windowChanged);

                        win.on("resize.lazySrc", windowChanged);
                        win.on("scroll.lazySrc", windowChanged);

                        documentTimer = setInterval(checkDocumentHeight, documentDelay);

                    }


                    function stopWatchingWindow() {

                        isWatchingWindow = false;

                        element.off("scroll.lazySrc", windowChanged);

                        // Stop watching for window changes.
                        win.off("resize.lazySrc");
                        win.off("scroll.lazySrc");

                        // Stop watching for document changes.
                        clearInterval(documentTimer);

                    }


                    // I start the render time if the window changes.
                    function windowChanged() {

                        if (!renderTimer) {

                            startRenderTimer();

                        }

                    }


                    //#endregion 
                
            },
            link: function (scope, element, attrs) {
            }
        }
    })
    .directive(
        "lazySrc",
        function () {

            function LazyImage(element) {

                var source = null;
                var isRendered = false;
                var height = null;


                // ---
                // PUBLIC METHODS.
                // ---


                function isVisible(topFoldOffset, bottomFoldOffset) {

                    if (!element.is(":visible")) {

                        return (false);

                    }
                    if (height === null) {

                        height = element.height();

                    }
                    var top = element.offset().top;
                    var bottom = (top + height);

                    return (
                        (
                            (top <= bottomFoldOffset) &&
                            (top >= topFoldOffset)
                            )
                        ||
                        (
                            (bottom <= bottomFoldOffset) &&
                            (bottom >= topFoldOffset)
                            )
                        ||
                        (
                            (top <= topFoldOffset) &&
                            (bottom >= bottomFoldOffset)
                            )
                        );

                }

                function render() {

                    isRendered = true;

                    renderSource();

                }


                function setSource(newSource) {

                    source = newSource;

                    if (isRendered) {

                        renderSource();

                    }

                }


                // ---
                // PRIVATE METHODS.
                // ---

                function renderSource() {

                    element[0].src = source;

                }

                return ({
                    isVisible: isVisible,
                    render: render,
                    setSource: setSource
                });

            }


            // ------------------------------------------ //
            // ------------------------------------------ //


            return {
                restrict: "A",
                require: '^lazySrcContainer',
                link: function ($scope, element, attributes, containerCtrl) {

                    var lazyImage = new LazyImage(element);

                    containerCtrl.addImage(lazyImage);

                    attributes.$observe(
                        "lazySrc",
                        function (newSource) {

                            lazyImage.setSource(newSource);

                        }
                    );

                    $scope.$on(
                        "$destroy",
                        function () {

                            containerCtrl.removeImage(lazyImage);

                        }
                    );

                }
            };
        });
