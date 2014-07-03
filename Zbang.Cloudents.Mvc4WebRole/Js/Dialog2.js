    var methods = {
        init: function (options) {
            var wrapper;
            var settings = $.extend({
                submitCallBack: function () { },
                cancelCallBack: function () { },
                //  hideClassBack: function () { },
            }, options);
            //the entire dialog
            var extraCss = this.attr('class') || 'confirm';
            var dialog = createDiv().addClass("popupWrppr").addClass(extraCss),
                //the title section of the dialog
                titleDiv = createDiv()
                    .append($('<header class="popupHeader"></header').text(this.attr('title')))
                    //appending the close button and attaching a close event//
                    .append(createButton().addClass('closeDialog').click(function () {
                        if (resetButton.length) {
                            resetButton.click();
                        } else {
                            hideForm();

                        }
                    }));



            var bodyDiv = createDiv().html(this.html());
            var form = bodyDiv.find('form').submit(function (e) {
                e.preventDefault();
                var $form = $(this);
                if (!$form.valid || $form.valid()) {
                    if (settings.submitCallBack($form.prop('action'), $form.serializeArray(), $form) !== false) {
                        wrapper.hide();
                        cd.resetForm(form);
                    }
                }

            });

            var resetButton = bodyDiv.find('button[type="reset"],input[type="reset"]').click(function () {
                settings.cancelCallBack();
                hideForm();
            });

            if (!form.length) {
                bodyDiv.find('[data-event="submit"]').click(function (e) {
                    settings.submitCallBack();
                    wrapper.hide();
                });

            }
            //fix ie issue
            if (Modernizr.input.placeholder) {
                bodyDiv.find('textarea').val('');
            }
            //create the modal
            var modal = createDiv().addClass('modal2');


            //appending it all up
            dialog.append(titleDiv, bodyDiv/*, buttons*/);

            // put it in a wrapper
            wrapper = createDiv().append(dialog, modal).hide();
            //appending it to body
            $('body').append(wrapper);
            this.remove();
            return wrapper;

            function hideForm() {
                wrapper.hide();
                var form = wrapper.find('form');
                if (form.length) {
                    cd.resetForm(form);
                }
            }

        },
        show: function () {
            this.show();
            this.find('input[type=text]:first').focus();
        },
        hide: function (elem) {
            this.hide();
            if (this.is('form')) {
                cd.resetForm(this);
            }            
        }    
    };
    function createButton() {
        return $('<button></button>');
    }
    function createDiv() {
        return $('<div></div>');
    }

    $.fn.dialog = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.tooltip');
        }
    };
