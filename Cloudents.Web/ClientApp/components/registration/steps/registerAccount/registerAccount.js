import stepTemplate from '../stepTemplate.vue';
import registrationService from "../../../../services/registrationService";
import disableForm from '../../../mixins/submitDisableMixin'

export default {
    mixins:[disableForm],
    components: {stepTemplate},
    methods: {
        finishRegistration() {
            window.isAuth = true;
            this.$router.push({path: '/ask', query: {q: ''}});
        }
    },
};