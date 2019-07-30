import NotificationService from '../services/notificationService';
import { NOTIFICATION } from "./mutation-types";


//BLITZ WTF
const emptyState = {
    action: 'empty',
    headline: 'Sure there will be soon',
    title: 'There is no new messages for now',
    timeago: "2018-09-04T00:58:34.2096593Z",
    type: 'empty'
};
const state = {
    notifications: [],
    emptyNotificationState: [NotificationService.createNotificationObj(emptyState)],
};
const mutations = {
    [NOTIFICATION.UPDATE_NOTIFICATION](state, notificationCreated) {
        state.notifications.push(notificationCreated);
    },
    [NOTIFICATION.UPDATE_NOTIFICATION_PROPS](state, id) {
        state.notifications.find(function (obj) {
            if (obj.id === id) {
                obj.isVisited = true;
            }
        });
    },
    [NOTIFICATION.ARCHIVE_NOTIFICATION](state, id) {
            // get index of object with id:37
            let removeIndex =  state.notifications.map(function (item) {
                return item.id;
            }).indexOf(id);
            // remove object
        state.notifications.splice(removeIndex, 1);
    }
};

const getters = {
    getNotifications: (state) => {
        if(state.notifications.length > 0){
            return state.notifications
        }else{
            return state.emptyNotificationState
        }
    }
};
const actions = {
    addNotificationItemAction({commit, getters}, notification) {
        let notificationCreated = NotificationService.createNotificationObj(notification);
        commit(NOTIFICATION.UPDATE_NOTIFICATION, notificationCreated);
    },
    updateNotification({commit, getters}, id) {
        commit(NOTIFICATION.UPDATE_NOTIFICATION_PROPS, id);
    },
    archiveNotification({commit, getters}, id) {
        commit(NOTIFICATION.ARCHIVE_NOTIFICATION, id);
    }
};
export default {
    actions, state, mutations, getters
}