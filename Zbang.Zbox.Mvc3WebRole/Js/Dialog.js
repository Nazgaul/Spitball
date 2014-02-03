(function ($) {
    methods = {
        init: function (options) {
            var wrapper,
            settings = $.extend({
                submitCallBack: function () { }
            }, options);
            //the entire dialog
            var dialog = createDiv(),
            //the title section of the dialog
            titleDiv = createDiv()
                //apending the title of the object we called
            //.append($('<span></span').text(this.attr('title')))
                .append($('<header class="popupHeader popupGradient"></header').text(this.attr('title')));
                //appending the close button and attaching a close event
            //.append(createButton().text('X').click(function () {
               // wrapper.hide();
            //}));
            
            

            var bodyDiv = createDiv().html(this.html());
            bodyDiv.find('form').submit(function (e) {
                e.preventDefault();
                var $form = $(this);
                if (!$form.valid || $form.valid()) {
                    if (settings.submitCallBack($form.attr('action'), $form.serializeArray()) !== false) {
                        wrapper.hide();
                    }
                }
                
            });
            bodyDiv.find('button[type="reset"]').click(function () {
                wrapper.hide();
            });
            //create the modal
            var modal = createDiv();

            // create the buttons section
            //var buttons = createDiv()
            // the ok button
            //.append(createButton().text(settings.buttonOk.text).click(function () {
            //    if (settings.buttonOk.click() !== false) {
            //        close();
            //    }
            //}));
            //// the cancel button if needed
            //if (settings.buttonCancel.isExists) {
            //    buttons.append(createButton().text(settings.buttonCancel.text).click(function () {
            //        close();
            //    }));
            //}
            //appending it all up
            dialog.append(titleDiv, bodyDiv/*, buttons*/);

            // put it in a wrapper
            wrapper = createDiv().append(dialog, modal).hide().addClass("popupWrppr confirm txtshd1");
            //appending it to body
            $('body').append(wrapper);
            this.remove();
            return wrapper;


        },
        show: function () {
            this.show();
        },
        hide: function () {
            //this.hide();
            this.hide();
            //this.find('input').each(function () {
            //    this.value = '';
            //});
        },
    };
    function createButton() {
        return $('<button></button>');
    }
    function createDiv() {
        return $('<div></div>');
    }
    function close(elem) {
        elem.hide();
        //wrapper.remove();
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
}(jQuery));