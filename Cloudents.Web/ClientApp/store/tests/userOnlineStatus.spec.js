import userOnlineStatus from '../userOnlineStatus';
import userOnlineStatusService from './../../services/userOnlineStatusService';

describe('STORE_userOnlineStatus_GETTER_getUserStatus', ()=>{
    it('should return user status', ()=>{
        const userStatusObj = userOnlineStatusService.createUserStatus({id: 12345, online: true});
        const userStatus = {[userStatusObj.id]: userStatusObj.online};
        const result = {
            12345: true
        };
        const state = {userStatus};
        expect(userOnlineStatus.getters.getUserStatus(state)).toEqual(result);
    });
});