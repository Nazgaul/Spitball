function permission(userType) {
    var self = this;
    var usertype = ko.observable(userType);

    self.ConnectedToBox = function () {
        return usertype() > 1;
    };
    self.IsInvited = function (type) {
        type = type || usertype();
        return type === 1;
    };
    self.IsSubscribe = function (type) {
        type = type || usertype();
        return type === 2;
    };
    self.IsOwner = function (type) {
        type = type || usertype();
        return type === 3;
    };
    self.IsNotConnected = function () {
        return !this.ConnectedToBox();
    };
}


permission.prototype.IsDeleteAllow = function (owner) {
    return this.ConnectedToBox() && (this.IsOwner() || $.trim(owner) === $.trim($('#userName').data('id')));
};
permission.prototype.UserToBox = {
    Unsubscribe: 1,
    Subscribe: 2,
    Owner: 3
};


