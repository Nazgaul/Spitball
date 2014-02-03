function permission(userType) {
    var self = this;
    var usertype = userType; ;

    self.ConnectedToBox = function () {
        return usertype > 1;
    };
    self.IsInvited = function (type) {
        if (type === undefined) {
            type = usertype;
        }
        return type === 1;
    };
    self.IsSubscribe = function (type) {
        if (type === undefined) {
            type = usertype;
        }
        return type === 2;
    };
    self.IsOwner = function (type) {
        if (type === undefined) {
            type = usertype;
        }
        return type === 3;
    };
    self.IsNotConnected = function () {
        return !this.ConnectedToBox();
    };
}


permission.prototype.IsDeleteAllow = function (owner) {
    return this.ConnectedToBox() && (this.IsOwner() || $.trim(owner) === $.trim($('#userName').data('id')));
};
permission.prototype.IsInviteAllow = function () {
    return this.ConnectedToBox();
};
permission.prototype.AccessBoxSettings = function () {
    return this.ConnectedToBox();
};
permission.prototype.GetConnectionToBox = function () {
    if (this.IsNotConnected()) {
        return this.UserToBox.Unsubscribe;
    }
    if (this.IsOwner()) {
        return this.UserToBox.Owner;
    }
    if (this.IsSubscribe()) {
        return this.UserToBox.Subscribe;
    }
};
permission.prototype.UserToBox = {
    Unsubscribe: 1,
    Subscribe: 2,
    Owner: 3
};


