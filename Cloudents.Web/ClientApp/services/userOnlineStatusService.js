import { Status } from './constructors.js'

// REMOVE IT
// const UserStatus = function(objInit){
//     this.id = objInit.id;
//     this.online = objInit.online || false;
// };

const createUserStatus = function(objInit){
    return new Status.UserStatus(objInit);
};

export default{
    createUserStatus
}