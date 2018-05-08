import accountNum from "./accountNum.vue";
import stepTemplate from '../stepTemplate.vue'

export default {
    components: {accountNum, stepTemplate},
    data() {
        return {
            openDialog: false,
            dialogWasViewed: false,
            showSummary: false,
        }
    },
    methods: {
        next() {
            if (!this.dialogWasViewed) {
                this.openDialog = true;
            }
            else {
                this.showSummary = true;
            }
        },
        closeDialog() {
            this.openDialog = false;
            this.dialogWasViewed = true
        },
        finishRegistration() {
            this.$router.push({path: '/note', query: {q: ''}});
        }
    }
}