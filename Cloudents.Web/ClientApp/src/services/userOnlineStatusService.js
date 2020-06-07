const UserStatus = function(objInit){
    this.id = objInit.id;
    this.online = objInit.online || false;
};

const createUserStatus = function(objInit){
    return new UserStatus(objInit);
};

export default{
    createUserStatus
}