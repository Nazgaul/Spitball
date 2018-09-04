import { mapGetters, mapActions } from 'vuex';
import userBlock from '../helpers/user-block/user-block'
import askQuestionBtn from "../results/helpers/askQuestionBtn/askQuestionBtn.vue";
import notificationItems from "./notesDummy";

export default {
    name: "notificationCenter",
    components: {userBlock, askQuestionBtn},
    data() {
        return {
        notesDummy: notificationItems
        }
    },
    props: {
        isAsk: {
            type: Boolean,
            default: true
        },

    },
    computed: {
        ...mapGetters({notificationItems: 'getNotifications'}),

    },
    methods: {
        ...mapActions(['addNotificationItemAction', 'updateNotification', 'archiveNotification']),
        doNotificationAction(item){
            let id = item.id;
            item.isVisited = true;
            this.updateNotification(id)
        },
        sendToArchive(item){
            event.stopPropagation();
            let id = item.id;
            this.archiveNotification(id)
        }
    },
    created() {
        this.notesDummy.forEach((serverItem) => {
            this.addNotificationItemAction(serverItem);
        });
    }
}