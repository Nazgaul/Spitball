(function ($) {
    jQuery.fn.extend({
        elastic: function () {

            //	We will create a div clone of the textarea
            //	by copying these attributes from the textarea to the div.
            var mimics = [
				'paddingTop',
				'paddingRight',
				'paddingBottom',
				'paddingLeft',
				'fontSize',
				'lineHeight',
				'fontFamily',
				'width',
				'fontWeight',
				'border-top-width',
				'border-right-width',
				'border-bottom-width',
				'border-left-width',
				'borderTopStyle',
				'borderTopColor',
				'borderRightStyle',
				'borderRightColor',
				'borderBottomStyle',
				'borderBottomColor',
				'borderLeftStyle',
				'borderLeftColor'
            ];

            return this.each(function () {

                // Elastic only works on textareas
                if (this.type !== 'textarea') {
                    return false;
                }

                var $textarea = jQuery(this),

				lineHeight = 0,
				minheight = 0,
				maxheight = 0,
				$twin = createTwin();

                //   goalheight = 0;

                // Opera returns max-height of -1 if not set
                if (maxheight < 0) { maxheight = Number.MAX_VALUE; }

                // Append the twin to the DOM
                // We are going to meassure the height of this, not the textarea.
                $twin.appendTo('body');

                function createTwin() {

                    var $twin = $('.twin');
                    if (!$twin.length) {
                        $twin = jQuery('<div>...</div')
                       .css({
                           'position': 'absolute',
                           'display': 'none',
                           'top': -999999,
                           'word-wrap': 'break-word',
                           'white-space': 'pre-wrap'
                       }).addClass('twin');
                    }
                    lineHeight = parseInt($textarea.css('line-height'), 10) || parseInt($textarea.css('font-size'), '10'),
                    minheight = parseInt($textarea.css('height'), 10) || lineHeight * 3,
                    maxheight = parseInt($textarea.css('max-height'), 10) || Number.MAX_VALUE;
                    return $twin;
                }

                // Copy the essential styles (mimics) from the textarea to the twin
                var i = mimics.length;
                while (i--) {
                    $twin.css(mimics[i].toString(), $textarea.css(mimics[i].toString()));
                }

                // Updates the width of the twin. (solution for textareas with widths in percent)
                function setTwinWidth() {
                    var curatedWidth = Math.floor(parseInt($textarea.width(), 10));
                    if ($twin.width() !== curatedWidth) {
                        $twin.css({ 'width': curatedWidth + 'px' });

                        // Update height of textarea
                        update(true);
                    }
                }

                // Sets a given height and overflow state on the textarea
                function setHeightAndOverflow(height, overflow) {
                    var curratedHeight = Math.floor(parseInt(height, 10));
                    if ($textarea.height() !== curratedHeight) {
                        $textarea.css({ 'height': curratedHeight + 'px', 'overflow-y': overflow });
                    }
                }

                // This function will update the height of the textarea if necessary 
                function update(forced) {

                    // Get curated content from the textarea.
                    var textareaContent = $textarea.val().replace(/&/g, '&amp;').replace(/ {2}/g, '&nbsp;').replace(/<|>/g, '&gt;').replace(/\n/g, '<br />');

                    // Compare curated content with curated twin.
                    var twinContent = $twin.html().replace(/<br>/ig, '<br />');

                    if (forced || textareaContent + '...' !== twinContent) {

                        // Add an extra white space so new rows are added when you are at the end of a row.
                        $twin.html(textareaContent + '...');

                        // Change textarea height if twin plus the height of one line differs more than 3 pixel from textarea height
                        if ($textarea.height() !== 0) {
                            if (Math.abs($twin.height() - $textarea.height()) > 3) {

                                var goalheight = $twin.height() + lineHeight;
                                if (goalheight >= maxheight) {
                                    setHeightAndOverflow(maxheight, 'auto');
                                } else if (goalheight <= minheight) {
                                    setHeightAndOverflow(minheight, 'hidden');
                                } else {
                                    setHeightAndOverflow(goalheight, 'hidden');
                                }

                            }
                        }

                    }

                }

                // Hide scrollbars
                $textarea.css({ 'overflow': 'hidden' });

                // Update textarea size on keyup, change, cut and paste
                $textarea.bind('keyup change cut paste', function () {
                    update();
                });

                // Update width of twin if browser or textarea is resized (solution for textareas with widths in percent)
                $(window).bind('resize', setTwinWidth);
                $textarea.bind('resize', setTwinWidth);
                $textarea.bind('update', update);

                // Compact textarea on blur //removed by Daniel - as not required
                //$textarea.bind('blur', function () {
                //    if ($twin.height() < maxheight) {
                //        if ($twin.height() > minheight) {
                //            $textarea.height($twin.height()+lineHeight);
                //        } else {
                //            $textarea.height(minheight);
                //        }
                //    }
                //});

                // And this line is to catch the browser paste event
                $textarea.bind('input paste', function (e) { setTimeout(update, 250); });
                $textarea.data('elastic', $twin);
                $twin.data('elastic', $textarea);
                // Run update once when elastic is initialized
                update();

            });
        }


    });

})(jQuery);