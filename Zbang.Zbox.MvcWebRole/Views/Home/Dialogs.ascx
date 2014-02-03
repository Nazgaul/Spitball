<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Zbang.Zbox.Infrastructure.Enums" %>

<div id="dialog-confirm">
    <div id="dialog-content">
    </div>
</div>
<div id="dialog-search-results">
    <div id="search-results">
    </div>
</div>
<div id="dialog-create-Box" style="display:none;">
    <div id="dialog-CreateBoxcontent">
        <div>
            <span class="txt-13 blue-5">Box name:</span><br/ />
            <input type="text" id="boxName" class="Gradiant2 rounded border-blue2" /></div>
        <div>
            <div>               
            </div>
            <div id="divPrivacy">
                <span class="txt-13 blue-5 float-left">Privacy:</span>
                <select id="Privacy" class="style8 txt-13">
                    <option value="<%: (int)BoxPrivacySettings.InvitationOnly %>">Members only</option>
                    <option value="<%: (int)BoxPrivacySettings.NotShared %>">Just Me</option>
                    <option value="<%: (int)BoxPrivacySettings.Public %>">Public</option>                    
                    <option value="<%: (int)BoxPrivacySettings.PasswordProtected %>">Password Protected</option>
                </select>
                <div id="divPassword" style="display: none;">
                <input style="width: 255px;" class="password Gradiant2 rounded border-blue2" id="password2" type="password" />
                <input style="width: 255px; display: none;" class="password Gradiant2 rounded border-blue2" id="Text1" type="text" />
                <span class="sepLine" style="color: #999999">&nbsp</span>
                <input style="border: 0;" type="checkbox" id="myCheck2" />
                <label class="style66 ">Show password</label>
                    
            </div>          
                <div class="Notification">
                    <input type="checkbox" id="myCheck" checked="checked" value="<%: (int)NotificationSettings.On %>" /><span>Send email notifications for this box</span>
                </div>
                <div>
                    <span id="spanCreateBoxMessage" class="baloon-feedback"></span>
                </div>
            </div>
        </div>
    </div>
</div>
<div id="dialog-delete-Box" style="display:none;">
    <div id="dialon-DeleteBoxContent">
    <div class="style81">You are about to delete the box "<span id="boxName"></span>" and remove all its content.</div>
    <div class="style81">Are you sure you want to delete this box</div>
    </div>
</div>

<div id="dialog-share-Box" style="display:none;">
        <div>
            <div id="divTo">                        
                <span class="style58AAA  txt-13 blue-5">To:&nbsp</span>
                <div class="divToWrap Gradiant2 rounded border-blue2">
                <div id="recipients-display">
                    <input placeholder="name@example.com" id="To" name="To" type="email" style="background:none;"  />
                    </div>
                </div>
                <div class="clear"></div>
            </div>
            <div style="padding-bottom: 5px;">
                <span class="style58AAA txt-13 blue-5">Include a personal note: </span><span class="style59">(optional)</span></div>
            <div style="width: 100%;">
                <textarea id="textAreaPersonalNote" spellcheck="true" class="Gradiant2 rounded border-blue2 gray"></textarea></div>
            <div id="divPrivacy1">
            <span id="ErrorMail"></span>
                <span class="style22AA   txt-13 blue-5 float-left">Privacy:</span>
                <select id="privacySelect" class="style8AAA  txt-13">
                    <option value="<%:BoxPrivacySettings.NotShared %>">Just Me</option>
                    <option value="<%:BoxPrivacySettings.Public %>">Public</option>
                    <option value="<%:BoxPrivacySettings.InvitationOnly %>">Members only</option>
                    <option value="<%:BoxPrivacySettings.PasswordProtected %>">Password protected</option>
                </select>
                </div>
            <div id="ShareBoxdivPassword" style="display: none;">
                <input style="width:245px;" class="password Gradiant2 rounded border-blue2" id="password" type="password" />
                <input style="width: 245px; display: none;" class="password Gradiant2 rounded border-blue2" id="password1" type="text" />
                <span class="sepLine" style="color: #999999">&nbsp</span>
                <input style="border: 0;" type="checkbox" id="checkPassword" />
                <label class="style66">Show password</label>
            </div>
            <div class="clear"></div>
        </div>
    </div>


