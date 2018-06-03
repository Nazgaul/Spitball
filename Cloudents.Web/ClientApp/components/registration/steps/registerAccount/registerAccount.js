import accountNum from "./accountNum.vue";
import stepTemplate from '../stepTemplate.vue';
import registrationService from "../../../../services/registrationService";
import disableForm from '../../../mixins/submitDisableMixin'

export default {
    mixins:[disableForm],
    components: {accountNum, stepTemplate},
    data() {
        return {
            openDialog: false,
            dialogWasViewed: false,
            showSummary: false,
            accountNum: ""
        }
    },
    methods: {
        next() {
            if(!this.submitted) {
                this.submitForm();
                if (!this.dialogWasViewed) {
                    this.openDialog = true;
                }
                else {
                    this.showSummary = true;
                }
            }
        },
        closeDialog() {
            this.openDialog = false;
            this.dialogWasViewed = true;
        },
        finishRegistration() {
            this.$router.push({path: '/note', query: {q: ''}});
        }
    },
    beforeCreate() {
        var self = this;
        registrationService.getAccountNum()
            .then(function (response) {
                self.accountNum = response.data.password;
            })
    }
};