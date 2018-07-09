import stepTemplate from '../stepTemplate.vue';
import disableForm from '../../../mixins/submitDisableMixin'
import {mapActions} from 'vuex'
const initialPointsNum =100

export default {
    mixins:[disableForm],
    components: {stepTemplate},
    data(){return{initialPointsNum}},
    methods: {
        ...mapActions(['incrementRegistrationStep']),
        finishRegistration() {
            window.isAuth = true;
            this.incrementRegistrationStep();
            this.$router.push({path: '/ask', query: {q: ''}});
        }
    },
};