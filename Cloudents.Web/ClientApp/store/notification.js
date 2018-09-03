import notificationService from '../services/notificationService';
import { NOTIFICATION } from "./mutation-types";
const state={
    correctAnswer:"",
    deletedAnswer:false
};
const mutations= {
    [NOTIFICATION.UPDATE_NOTIFICATION](state, data) {
        state.notification = data;
    },
};
const getters = {
    getNotifications:(state) => state.notification,
};
const actions = {
    addNotificationItemAction({ commit, getters }, notification){
        let notificationCreated = notificationService.createNotificationObj(notification);
        commit(NOTIFICATION.UPDATE_NOTIFICATION, notificationCreated);
    },
};
export default {
    actions, state, mutations, getters
}