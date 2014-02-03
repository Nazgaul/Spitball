<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Zbang.Zbox.Domain.Common" %>
<%/*%><script src="~/Scripts/jquery-1.6.2-vsdoc.js" type="text/javascript"></script><%*/%>
<script type="text/javascript">
    $(document).ready(function () {

        $('#btnShareBox').button({
            disabled: function () {
                return parseInt($('#selecteBoxId').val()) > 0
            }
        });

        $('#btnShareBox').click(function (e) {
            if (Zbox.Box.boxes.GetCurrentBox().UserPermission < 4) {
                e.stopPropagation();
                e.preventDefault();
                $('#btnShareBox').button('disable');
                return false;
            }

            return false;

        });

        //$('#btnShareBox').button();
        $('#selectedBoxIdPermission').change(function () {
            if ($(this).val() >= 4) {
                $('#btnShareBox').button('option', 'disabled', false);

            }
            else {
                $('#btnShareBox').button('option', 'disabled', true);

            }
        });
    });
</script>
<div class="bar top-dotted-border txt-11 blue-dark txt-no-dec">
    <img alt="" class="membersIcon" src="/Content/Images/icons/icon-members.png" />
    MEMBERS:<div id="shareBox" class="title-button">
        <div class="my-button">
            <button id="btnShareBox" class="rounded btnBlue txt-shadow txt-13 white btnAddMember">
                Add Member</button>
        </div>
    </div>
</div>
<%--#conversations-container button--%>
