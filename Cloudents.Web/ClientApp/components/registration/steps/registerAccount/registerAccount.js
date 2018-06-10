import stepTemplate from '../stepTemplate.vue';
import registrationService from "../../../../services/registrationService";
import disableForm from '../../../mixins/submitDisableMixin'
import {mapActions, mapGetters} from 'vuex'


export default {
    mixins:[disableForm],
    components: {stepTemplate},
    methods: {
        ...mapActions(['incrementRegistrationStep']),
        finishRegistration() {
            window.isAuth = true;
            this.incrementRegistrationStep();
            this.$router.push({path: '/ask', query: {q: ''}});
        }
    },
};