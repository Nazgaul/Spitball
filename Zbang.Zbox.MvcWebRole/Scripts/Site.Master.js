
var centerPanel;            // to make height changes easier

$(document).ready(function () {
    // resizing
    centerPanel = $('#box-comments');   
    $(window).resize(function () {
        //$.simplyScroll.fn.Scrollable();
        $('#divFiles').trigger('resizeElement');
        $('#divFilesToUploadConversation').trigger('resizeElement');
        resizeContext($(document));
    });

    $(".menu-item").click(function () {
        resizeCenterPanel(document); resizeCenterPanel(document);
    });

    // the .dialog() of the ui is causing changes in the page, these lines functions like "redraw"
    //
    $(this).bind("dialogopen", function (event, ui) {
        resizeContext($(window));
    });
    $(this).bind("dialogclose", function (event, ui) {
        resizeContext($(window));
    });

    $('#eastPaneAccordion').accordion({
        active: false,
        disabled: true,
        collapsible: true,
        clearStyle: true,       
    });
    $("#Privacy").find('a').click(function (e) {
        if (!Zbox.BoxItemAction.CheckForPermission(7)) {
            return false;
        }
    });




    $('#eastPaneAccordion').find('h3').removeClass('ui-corner-all');

    setInterval(Zbox.KeepSessionAlive, 1000 * 60 * 5);

    setInterval(Zbox.UpdateScreenTime, 1000 * 60 * 5);
    resizeContext($(document));

});
// end of document load



// resize - TODO: check and clean events that are changing size of elements
function resizeContext(doc) {    
    //$.event.remove(this, "resize", resizeContext);
    var $uiLayoutCenter = $('#ui-layout-center');
    var $uiLayoutEast = $('#ui-layout-east');
    var $uiLayoutWest = $('#ui-layout-west');
    var $uiLayoutSouth =  $('#ui-layout-south');
    $uiLayoutCenter.width($(doc).width() - (10 + 3 + 1 + 1) - ($uiLayoutEast.outerWidth(true) + $uiLayoutWest.outerWidth(true))); //10 ,3 padding 1 border ,1 for extra space cause by mozilla press ctrl+-
    $uiLayoutSouth.width($(doc).width() + 18 - 1 - $uiLayoutEast.outerWidth(true)); //1 border

    $uiLayoutEast.height($(doc).height() - 227);
    $uiLayoutWest.height($(doc).height() - 36 - 4); // height of header: 36 , paading top 4
    $uiLayoutCenter.height($uiLayoutEast.height());
    $('#boxes-layout').height($(doc).height() - 209); //header: 33, search: ~ 95, banner: 93
    // $('#box-comments').height($(w).height() - 337); // 311 is ideal: south panel: 165 + (header + header of center panel): 145
    resizeCenterPanel(doc);

    // temporary hack for ie8:
    if (jQuery.browser.msie && jQuery.browser.version == "8.0") {
        $uiLayoutCenter.width($uiLayoutCenter.width() - 10);
        $uiLayoutSouth.width($uiLayoutSouth.width() - 10);
    }
    //dialog cause an error in here ( and it should).
    //$.event.add(this, "resize", resizeContext);

}
function resizeCenterPanel(doc) {
    $(centerPanel).height($('#ui-layout-east').outerHeight() - $('#conversations-container').height() - 30);
}