window.cd = (function () {
    "use strict";
    var History = window.History;


    $(document).on('click', 'a.navigation', function (e) {
        e.preventDefault();
        History.pushState(null, document.title, $(this).prop('href'));
    });
    function getPage() {
        var url = History.getState().hash;
        if (url === '/' || url.match(/Dashboard/i) !== null) {
            return 'dashboard';
        }
        if (url.match(/library/i) !== null) {
            return 'library';
        }
    }
    var previousPage = '';
    History.Adapter.bind(window, 'statechange', function () {
        
        var currentPage = getPage(),
            pageChanged = currentPage !== previousPage;

        if (pageChanged) {
            $('#main').children('div').hide();
            $('#btnLib, #btnMy').hide();
        }
        if (currentPage === 'library') {
            if (pageChanged) {
                $('#Library,#btnLib').show();
            }
            $(window).trigger('library');
        }
        if (currentPage === 'dashboard') {
            if (pageChanged) {
                $('#My,#btnMy').show();
            }
            $(window).trigger('dashboard');
        }
        previousPage = currentPage;

    });
    $(function () {
        History.Adapter.trigger(window, 'statechange');
    });

    function getParameterByName(name, windowToCheck) {
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS, 'i');
        windowToCheck = windowToCheck || window;
        var results = regex.exec(windowToCheck.location.search);
        if (results === null) {
            return "";
        }
        else {
            return decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    };

    return {
        getParameterByName: getParameterByName
    }
})(window.cd = window.cd || {});

