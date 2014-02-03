/// <reference path="/Scripts/jquery-1.6.2-vsdoc.js" />

function Boxes(ownedBoxes, subscribedBoxes, invitedBoxes) {
    this.setValue(ownedBoxes);
    this.setValue(subscribedBoxes);
    this.setValue(invitedBoxes);
}
Boxes.prototype.addedBox = {};

Boxes.prototype.addBox = function (boxItem) {
    var boxItem = new BoxItem(boxItem);
    this.addedBox[boxItem.shortUid] = boxItem;
}

Boxes.prototype.setValue = function (value) {
    var self = this;
    $.each(value, function (index, boxItemeach) {
        var boxItem = new BoxItem(boxItemeach);
        self.addedBox[boxItem.shortUid] = boxItem;
    });
}
Boxes.prototype.toString = function () {
    var stringresult = "";
    $.each(this.addedBox, function (index, BoxItemeach) {
        stringresult += BoxItemeach.toString();
    });
    return stringresult;
}
Boxes.prototype.GetBox = function (boxId) {
    return this.addedBox[boxId];
}

Boxes.prototype.GetCurrentBox = function () {
    var boxId = $('#selectedBoxSUID').val();
    return this.GetBox(boxId);
}

function BoxItem(value) {
    this.setValue(value);
}
BoxItem.prototype.setValue = function (value) {
    this.BoxId = value.BoxId;
    this.uId = value.uId;
    this.BoxName = value.BoxName;
    this.BoxOwner = value.BoxOwner;
    this.PrivacySettings = value.PrivacySettingString;
    this.PrivacyPassword = value.PrivacyPassword;
    this.NotificationSettings = value.NotificationSettingsString;
    this.UserPermission = value.UserPermission;
    this.shortUid = value.shortUid;
    //this.BoxOwnerUserName = value.BoxOwnerUserName;    
}
BoxItem.prototype.toString = function () {
    return this.BoxId;
}



Zbox.Box = {
    boxes: null,
    //initial load of boxes
    AddBoxes: function (boxes) {
        var self = this;
        var ownedBoxes = [];
        this.boxes = new Boxes(boxes.ownedBoxes, boxes.subscribedBox, boxes.invitedBoxes);
        var subscribeBoxes = [];
        var invitedBoxes = [];
        self.BoxTemplate = $('#boxTemplate').clone();
        self.SubscribeBoxTemplate = $('#subscribedBoxTemplate').clone();
        self.InvitedBoxTemplate = $('#InvitedBoxTemplate').clone();

        $('#boxTemplate').remove();
        $('#subscribedBoxTemplate').remove();
        $('#InvitedBoxTemplate').remove();

        for (var i = 0; i < boxes.ownedBoxes.length; i++) {
            ownedBoxes.push(Zbox.Box.AddBoxEntry(boxes.ownedBoxes[i], self.BoxTemplate));
        }
        
        $('#divBoxes').append(ownedBoxes.join(''));

        for (var i = 0; i < boxes.subscribedBox.length; i++) {
            subscribeBoxes.push(Zbox.Box.AddBoxEntry(boxes.subscribedBox[i], self.SubscribeBoxTemplate));
        }
        
        $('#divSubscribedBoxes').append(subscribeBoxes.join(''));

        for (var i = 0; i < boxes.invitedBoxes.length; i++) {
            invitedBoxes.push(Zbox.Box.AddBoxEntry(boxes.invitedBoxes[i], self.InvitedBoxTemplate));
        }
        
        $('#divInvitedBoxes').append(invitedBoxes.join(''));
    },
    AddBox: function (box) {
        this.boxes.addBox(box);
        $('#divBoxes').prepend(Zbox.Box.AddBoxEntry(box, this.BoxTemplate));
    },

    BoxTemplate: '',
    SubscribeBoxTemplate: '',
    InvitedBoxTemplate: '',

    ShortBoxName: function (longBoxName, length) {
        if (length == null)
            length = 20;
        var boxName = longBoxName;
        if (boxName.length > length) {
            boxName = boxName.slice(0, length - 3);
            boxName += "...";
        }
        return boxName;
    },

    AddBoxEntry: function (boxDto, template) {
        boxDto.ShortBoxName = Zbox.Box.ShortBoxName(boxDto.BoxName);
        //var boxItem = $(template).clone();

        return Zbox.changeTemplateText(template.clone().html(), boxDto);
    },
    DeleteBox: function (boxId, OnSuccess, OnError, BeforeSend) {
        var deleteBoxRequest = new ZboxAjaxRequest({
            url: '/Box/DeleteBox',
            data: { boxId: boxId },
            beforeSend: function () {
                //        boxName.text('(Deleting...)');
                //$("div#dialog-delete-Box").dialog('close');
                BeforeSend();
            },
            success: function (data) {
                OnSuccess();
                Zbox.toaster('Box deleted');
                Zbox.LoadUserStatus();
            },
            error: function (error) {
                OnError();
            }
        });
        deleteBoxRequest.Post();
    },
    ChangeBoxName: function (boxId, newText, OnSuccess, OnError, OnComplete) {
        var ChangeBoxName = new ZboxAjaxRequest({
            url: "/Box/ChangeBoxName",
            data: { boxId: boxId, newBoxName: newText },
            success: function (data) {
                OnSuccess();
                Zbox.toaster('Box name changed');
            },
            error: function (error) {
                OnError();
                Zbox.toaster('Error in Box name changed');
            },
            complete: function (text) {
                OnComplete();
            }
        });

        ChangeBoxName.Post();
    }
}








