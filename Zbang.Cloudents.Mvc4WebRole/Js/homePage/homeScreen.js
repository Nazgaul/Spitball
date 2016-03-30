function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return "";
}

$('.language button').click(function () {
    document.cookie = "l3 =" + $(this).data('cookie') + "; path=/;domain=spitball.co";
    location.reload();
});

window.addEventListener("load", function load() {
    window.removeEventListener("load", load, false); //remove listener, no longer needed
    var val = getCookie('l3').toLowerCase();
    var text = $('.language').find('button[data-cookie="' + val + '"]').html();
    $('nav li.language span:first').text(text);

    (function (i, s, o, g, r, a, m) {
        i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
            (i[r].q = i[r].q || []).push(arguments)
        }, i[r].l = 1 * new Date(); a = s.createElement(o),
        m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
    })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

    ga('create', 'UA-9850006-3', 'auto');
    ga('send', 'pageview');


    $('.mobileLink').click(function (e) {
        ga('send', 'event', 'Mobile Link', e.target.id);
    });

}, false);


//jQuery is required to run this code
//

//Object.defineProperty(HTMLMediaElement.prototype, 'playing', {
//    get: function () {
//        return !!(this.currentTime > 0 && !this.paused && !this.ended && this.readyState > 2);
//    }
//})

//(function () {
//    var min_w = 300; // minimum video width allowed
//    var vid_w_orig; // original video dimensions
//    var vid_h_orig, containerHeight, videoElement;


//    jQuery(function () { // runs after DOM has loaded
//        videoElement = jQuery('video');
//        vid_w_orig = parseInt(videoElement.attr('width'));
//        vid_h_orig = parseInt(videoElement.attr('height'));
//        containerHeight = $('#spitballFeatures').height();
//        jQuery(window).resize(function () { resizeToCover(); });
//        jQuery(window).trigger('resize');

//        //videoElement.bind('play', function() {
//        //    console.log('here');
//        //});
//    });

//    function resizeToCover() {

//        // set the video viewport to the window size
//        //jQuery('#video-viewport').width(jQuery(window).width());
//        //jQuery('#video-viewport').height(jQuery(window).height());

//        // use largest scale factor of horizontal/vertical
//        var scale_h = jQuery(window).width() / vid_w_orig;
//        var scale_v = jQuery(window).height() / vid_h_orig;
//        var scale = scale_h > scale_v ? scale_h : scale_v;

//        // don't allow scaled width < minimum video width
//        if (scale * vid_w_orig < min_w) {
//            scale = min_w / vid_w_orig;
//        };

//        // now scale the video
//        videoElement.width(scale * vid_w_orig);
//        videoElement.height(scale * vid_h_orig);

//        //console.log(videoElement[0].canPlayType());
//        // and center it by scrolling the video viewport
//        jQuery('#video-viewport').scrollLeft((videoElement.width() - jQuery(window).width()) / 2);
//        jQuery('#video-viewport').scrollTop((videoElement.height() - jQuery(window).height()) / 2);

//        var extra = 0;
//        if (parseInt($('#content').css('marginTop').replace(/[^-\d\.]/g, ''), 10) === 0) {
//            extra = 100;
//        }
//        $('#spitballFeatures').height(Math.max(containerHeight, jQuery(window).height() - extra));
//        //  jQuery('#video-viewport').after('.container').height(jQuery('#video-viewport').height());
//    };
//})();
