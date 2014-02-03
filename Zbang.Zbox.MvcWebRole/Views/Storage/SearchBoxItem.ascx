<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%/*%><script src="~/Scripts/jquery-1.6.2-vsdoc.js" type="text/javascript"></script><%*/%>

<script type="text/javascript">

    $(document).ready(function () {

        $('#searchTextField').watermark('search files', { className: 'style5', useNative: false });
        $('#searchTextField').keypress(function (e) {
            if (e.which == 13) {
                e.preventDefault();
                $('#searchButton').click();
            }
        });

        //        $('div#searchContainer').focusout(function (e) {        
        //            if ($(e.target).parents('div#searchContainer').length == 0) {
        //                $('input#searchTextField').val('');
        //            }
        //        });

        $('#searchButton').click(function (e) {
            e.preventDefault();

            var searchText = $('#searchTextField').val();

            if (typeof (searchText) == 'undefined' || searchText == '' || searchText == 'search') {
                $('#searchErrorMessage').text('Please enter a search query')
                                               .show()
                                               .delay(30000)
                                               .fadeOut('slow');
                return;
            }

            $('#searchErrorMessage').clearQueue().hide();

            var searchRequest = new ZboxAjaxRequest({
                beforeSend: function () {
                    $('#searchIcon').attr('src', '/Content/Images/loading-animation.gif');
                },
                url: "/Storage/SearchBoxItem?searchText=" + searchText,
                success: function (resultsByCategory) {
                    
                    Zbox.ShowSearchResults('Results for "' + searchText + '"', resultsByCategory);
                },
                error: function (error) {
                    $('#searchErrorMessage').text(error)
                                               .show()
                                               .delay(30000)
                                               .fadeOut('slow');
                },
                complete: function () {
                    $('#searchIcon').attr('src', '/Content/Images/search.png');
                    $('#searchTextField').val('');
                }
            });
            searchRequest.Get();
        });

        /*
        while in the search results, box items navigate the main view to the box instead of doing their usual thing
        */
        //        $('div#search-results div.boxItem a').live('click', function (e) {
        //            e.preventDefault();
        //            var boxitem = $(this).closest('div.boxItem');

        //            //TODO change #1 - will be effected 
        //            //TODO find item in box feature 
        //            var itemElementId = boxitem.find('input.item-id').attr('id');

        //            var boxId = boxitem.closest('div.box-results-container').find('input.box-id').val();

        //            // this will cause div#search-results to be emptied, otherwise we would have problem exploting input.item-id html id to find it again in the 
        //            // box, when change #1 is finished this code will be effected.
        //            $('div#dialog-search-results').dialog('close');

        //            $('#boxEntry' + boxId).click();

        //            //TODO this needs to change when UI will be refactored to a higher level event driven (ie a box load done event or something)
        //            setTimeout(function () {
        //                var itemElement = $('#' + itemElementId).closest('div.boxItem');
        //                var count = 0;

        //                var scrollInterval = setInterval(function () {
        //                    if (++count > 25) {
        //                        alert('item not visible after 25 scroll clicks');
        //                        clearInterval(scrollInterval);
        //                        return;
        //                    }

        //                    if (isBoxItemVisibleOnXAxis(itemElement)) {
        //                        clearInterval(scrollInterval);

        //                        //select found item for 10 seconds
        //                        itemElement.toggleClass('selectedBoxItem');
        //                        setTimeout(function () {
        //                            itemElement.toggleClass('selectedBoxItem');
        //                        }, 20000);
        //                    } else {
        //                        $('div.simply-scroll-forward').click();
        //                    }
        //                }, 500);
        //            }, 2000);
        //        });

        $('div.box-results-container a.box-name').live('click', function (e) {
            e.preventDefault();
            var boxId = $(this).closest('div.box-results-container').find('input.box-id').val();
            $('#dialog-search-results').dialog('close');

            $('#boxEntry' + boxId).click();
        });


    });

    function isBoxItemVisibleOnXAxis(itemElement) {

        //need to fix that
        var filesContainerStartX = 0; //parseInt(filesContainerState.offsetLeft);
        var filesContainerEndX = 0; // filesContainerStartX + parseInt(filesContainerState.offsetWidth);

        var itemOffset = itemElement.offset();

        var itemStartX = parseInt(itemOffset.left);
        var itemEndX = itemStartX + parseInt(itemElement.width());

        return (itemStartX > filesContainerStartX && itemStartX < filesContainerEndX) &&
                    (itemEndX > filesContainerStartX && itemEndX < filesContainerEndX);
    }
</script>
<div id="searchErrorMessage" style="display: none;">
</div>
<div id="searchContainer" class="text-box rounded black-border">
    <div class="inner">
        <div>
            <input type="text" id="searchTextField" name="searchText" class="style5" /></div>
        <a href="#" id="searchButton" title="search">
            <img alt="" id="searchIcon" src="/Content/Images/search.png" /></a>
    </div>
</div>
