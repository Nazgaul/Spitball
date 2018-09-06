import { mapGetters, mapActions } from 'vuex';
import userBlock from '../helpers/user-block/user-block'
import askQuestionBtn from "../results/helpers/askQuestionBtn/askQuestionBtn.vue";
import notesDummy from "./notesDummy";

export default {
    name: "notificationCenter",
    components: {userBlock, askQuestionBtn},
    data() {
        return {
        notesDummy: notesDummy
        }
    },
    props: {
        isAsk: {
            type: Boolean,
            default: true
        },
    },
    computed: {
        ...mapGetters({notificationItems: 'getNotifications', notificationEmptyState: 'notificationEmptyState'}),

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