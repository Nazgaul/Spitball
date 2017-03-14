var Login = function () {
    var handleLogin = function () {
        $('#login_alert_text').hide();
        $('.forms .login').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            rules: {
                email: {
                    required: true,
                    email: true
                },
                password: {
                    required: true,
                    minlength: 6
                },
            },
            highlight: function (element) { // hightlight error inputs
                $(element)
                    .closest('.form-group').addClass('has-error'); // set error class to the control group
            },
            success: function (label) {
                label.closest('.form-group').removeClass('has-error');
                label.remove();
            },

            errorPlacement: function (error, element) {
                error.insertAfter(element);
            },

            submitHandler: function (form) {
                signin(form);
            }
        });

        $('.forms .login input').keypress(function (e) {
            if (e.which === 13) {
                if ($('.forms .login').validate().form()) {
                    $('.forms .login').submit();
                }
                return false;
            }
        });
    };

    function disableState(elem) {
        $(elem).attr('disabled', 'disabed').addClass('disabled');
    }
    function signin(form) {
        var submitBtn = $(form).find('.btn-primary');
        disableState(submitBtn);
        var values = $(form).serialize();
        
        $.post('/home/login', values).done(function (data) {
            if (!data.success) {
                var text;
                try {
                    text = data.payload.error;
                } catch (e) {
                    text = 'Unspecified error';
                }
                $('#login_alert_text span').text(text);
                $('#login_alert_text').show();
                submitBtn.removeClass('disabled').removeAttr('disabled');
                return;
            }

            var returnUrl = data.payload;
            if (returnUrl) {
                window.location.href = decodeURIComponent(returnUrl);
                return;
            }
            window.location.href = data.payload;

        });
    }
    function getUrlVars() {
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    }
    return {
        init: function () { handleLogin(); }
    };
}();
Login.init();