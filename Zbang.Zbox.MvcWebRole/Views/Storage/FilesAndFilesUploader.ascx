<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%/*%><script src="~/Scripts/jquery-1.6.2-vsdoc.js" type="text/javascript"></script><%*/%>
<script type="text/javascript">
    $(document).ready(function () {
        $('#btnAddLink').button({ disabled: true });
        $('#downloadAllFiles').button({
            disabled: true,
            icons: { primary: 'downloadAllIcon' }
        });
        $('#selectedBoxIdPermission').change(function () {
            if ($(this).val() > 1) {
                $('#btnAddLink').button('option', 'disabled', false);
            }
            else {
                $('#btnAddLink').button('option', 'disabled', true);
            }
        });
        $('#downloadAllFiles>span').hover(function () {
            $(this).css('text-decoration', 'underline');
        },
            function () {
                $(this).css('text-decoration', 'none');
            });
        $('#downloadAllFiles').click(function (e) {           
            $(this).attr('href', $(this).attr('rel') + $('#selecteBoxId').val());
        });
    });
</script>
<div id="files-container">
    <div id="files-inner-container">
        <div id="files-header" class="bwBorder txt-shadow white txt-13">
           <span class="style71 " id="boxItemCount"> <label>0</label>items</span><span class="txt-15 fileCountDLsep">|</span>
               <a target="_blank" id="downloadAllFiles" rel=<%:Url.Content("~/DownloadBox/")  %> href="#" style="background:none;">Download All</a>
        </div>
        <div id="files-center">
            <% Html.RenderPartial("~/Views/Storage/Files.ascx"); %>
        </div>       
    </div>
</div>





