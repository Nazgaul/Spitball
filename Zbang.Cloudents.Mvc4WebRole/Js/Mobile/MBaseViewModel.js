(function (pubsub, cd) {
    if (window.scriptLoaded.isLoaded('mBvm')) {
        return;
    }

    //var searchAvaible = false, timer = 0, currentPage = '', $seachInput = $('#seachInput');
    var currentPage = '';
    //window.scrollTo(0, 1);

    //$('#searchMenu').click(function (e) {
    //    if (!searchAvaible) {
    //        e.preventDefault();
    //        return;
    //    }
    //});
    //var el = document.getElementById('search');
    //el.addEventListener('transitionend', focusOnElement, false);
    //el.addEventListener('webkitTransitionEnd', focusOnElement, false);
    //function focusOnElement() {
    //    $seachInput.focus();
    //}
    //$('#searchClose').click(function () {
    //    $seachInput.val('');
    //    cd.pubsub.publish('close_search_' + currentPage);
    //});

    //$seachInput.keyup(function (e) {
    //    var $this = $(this);
    //    clearTimeout(timer);
    //    timer = setTimeout(function () {
    //        if ($.trim($this.val()) === '') {
    //            return;
    //        }
    //        cd.pubsub.publish('trig_search_' + currentPage, $this.val());
    //    }, 600);
    //});
    //pubsub.subscribe('dash_boxes', function () {
    //    searchAvaible = true;
    //    currentPage = 'boxes';
    //});
    //pubsub.subscribe('dash_search', function (data) {
    //    searchAvaible = true;
    //    $seachInput.val(data.query);
    //    //location.hash = 'search';
    //    currentPage = 'boxes';
    //});
    //pubsub.subscribe('lib_nodes', function (data) {
    //    searchAvaible = true;
    //    currentPage = 'library';

    //});

    //pubsub.subscribe('lib_uni', function () {

    //});
    //pubsub.subscribe('lib_load', function (data) {

    //});
    //pubsub.subscribe('lib_search', function (data) {
    //    searchAvaible = true;
    //    $seachInput.val(data.query);
    //    //location.hash = 'search';
    //    currentPage = 'library';
    //});

    //pubsub.subscribe('box_load', function (data) {
    //    searchAvaible = false;
    //});
    //pubsub.subscribe('item_load', function (data) {
    //});

    var wpr = document.getElementsByClassName('wpr')[0], previousHashState = '';
    cd.pubsub.subscribe('nav_hash', function (hash) {
        wpr.style.overflow = 'visible';
        wpr.style.height = '100%';
        document.body.style.overflow = 'visible';
        $('#lib_content').show();

        switch (hash) {
            case 'settingspanel':
                window.scrollTo(0, 1);
                wpr.style.overflow = 'hidden';
                //this is for iphone
                wpr.style.height = document.documentElement.clientHeight + 'px';
                break;
            case 'invites':
            case 'addbox':
            //case 'adddepartment':
            case 'updates':
            //case 'addacademicbox':
                wpr.style.overflow = 'hidden';
                break;
            case 'settings':
                cd.pubsub.publish('box_settings');
                break;
            case 'addacademicbox':
            case 'adddepartment':
                $('#lib_content').hide();
                break;
            default:
                break;

        }
    });
    cd.loader.registerFacebook();
    $('#logOut').click(function (e) {
        cd.userLogout(e)
        cd.historyManager.remove();
    });

    //#region keyboard show
    var is_keyboard = false,
    is_landscape = false,
    initial_screen_size = window.innerHeight,
    $footer = $('footer');



    cd.orientationDetection();
    //android do not put on - this will collide with footer logic
    //window.addEventListener("resize", function () {
    //    is_keyboard = (window.innerHeight < initial_screen_size);
    //    is_landscape = (screen.height < screen.width);

    //    updateViews();
    //}, false);

    //ios
    //$("input").bind("focus blur", function () {
    //    $(window).scrollTop(1);
    //    is_keyboard = $(window).scrollTop() > 0;
    //    $(window).scrollTop(0);
    //    updateViews();
    //});
    $('input,textarea').focus(function () {
        $footer.hide();
    });
    $('input,textarea').blur(function () {
        $footer.show();
    });
    function updateViews() {
        //if (is_keyboard) {
        //    $footer.hide();
        //}
        //else
        //    $footer.show();
    }
    //#endregion

    //#region footer


    //var el;
    //window.onresize = window.onscroll = function () {

    //    if (el && $(el).offset().top + $(el).height() - (window.pageYOffset + window.innerHeight) > 1) {

    //        el.style.position = 'absolute';
    //        el.style.bottom = 'auto';
    //        el.style.top = (window.pageYOffset + window.innerHeight - $(el).height()) + 'px';;
    //    }
    //    //var lowerleft = [window.pageXOffset, (window.pageYOffset + window.innerHeight)];
    //    //var lowerright = [(lowerleft[0] + window.innerWidth), lowerleft[1]];
    //    //var zoomFactor = window.innerWidth / document.documentElement.clientWidth;

    //    //el.style.width = lowerright[0] - lowerleft[0] + 'px';
    //    //el.style.height = parseInt(window.innerHeight / 10) + 'px';
    //    //el.style.left = lowerleft[0] + 'px';
    //    //el.style.top = lowerleft[1] - el.offsetHeight + 'px';
    //    //el.style.fontSize = parseInt(zoomFactor * 60) + 'px';
    //}

    //window.onload = function () {
    //    el = document.getElementsByTagName('footer')[0];
    //    //	fontAdjustment = 60/document.documentElement.clientWidth;
    //    window.onresize();
    //}

    //#endregion
    pubsub.subscribe('register', function () {
        cd.notification('you need to register');
    });


    //wpr.style.overflow = 'visible';
    //wpr.style.height = '100%';
    //document.body.style.overflow = 'visible';
    //$('#lib_content').show();
    //switch (hash) {
    //    case 'settingspanel':
    //        window.scrollTo(0, 1);
    //        wpr.style.overflow = 'hidden';
    //        //this is for iphone
    //        wpr.style.height = document.documentElement.clientHeight + 'px';
    //        break;
    //    case 'invites':
    //    case 'addbox':
    //        //case 'adddepartment':
    //    case 'updates':
    //        //case 'addacademicbox':
    //        wpr.style.overflow = 'hidden';
    //        break;
    //    case 'settings':
    //        cd.pubsub.publish('box_settings');
    //        break;
    //    case 'addacademicbox':
    //    case 'adddepartment':
    //        $('#lib_content').hide();
    //        break;
    //    default:
    //        break;

    //}

    //#region 'popup' navigation
    $(document).on('click', '[data-modelNav]', function (e) {
        /// <summary>Handle the application navigation</summary>
        /// <param name="e" type="Event"></param>
        $('.selected').removeClass('selected');
        removeOverflow();
        var selectId = $(this).attr('data-modelNav');
        if (selectId)
        {
            $('#' + selectId).addClass('selected');
            window.scrollTo(0, 1);
            wpr.style.overflow = 'hidden';
            //this is for iphone
            wpr.style.height = document.documentElement.clientHeight + 'px';
            $('.courseList').css('margin-top', '0');
        }
    }).on('click', 'a[data-navigation]', function (e) {
        /// <param name="e" type="Event"></param>
        $('.selected').removeClass('selected');
        removeOverflow();
//        e.stopPropagation();
    }).on('submit', function () {
        $('.selected').removeClass('selected');
        removeOverflow();
    });

    function removeOverflow() {
        wpr.style.overflow = 'visible';
        wpr.style.height = '100%';
        document.body.style.overflow = 'visible';
        $('.courseList').css('margin-top', '');
    }
    
    
    //#endregion


})(cd.pubsub, cd);