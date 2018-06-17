import registerEmail from './steps/registerEmail/registerEmail.vue'
import registerPhone from './steps/registerPhone/registerPhone.vue'
import registerUsername from './steps/registerUsername/registerUsername.vue'
import registerAccount from './steps/registerAccount/registerAccount.vue'
import {mapActions, mapGetters} from 'vuex'
import {REGISTRATION_STEPS} from "./../../store/constants";

export default {
    components: {registerEmail, registerPhone, registerUsername, registerAccount},
    props: {
        autoIncrementStep:{
            type: Boolean,
        }
    },
    data() {
        return {
            // step: this.$route.meta.step || 0,
            showDialog: false,
            step: {
                type: Number
            },REGISTRATION_STEPS
        }
    },
    // watch: {
    //     step: function (val) {
    //         this.incrementRegistrationStep(val)
    //     },
    // },
    computed: {
        ...mapGetters(['getRegistrationStep', 'fromPath']),
        // step() {
        //     if(this.$route.meta.step){
        //         this.updateRegistrationStep(this.$route.meta.step)
        //     }
        //     return this.getRegistrationStep;
        // }
    },
    methods: {
        ...mapActions(['incrementRegistrationStep']),
        $_back() {
            let url = this.fromPath || {path: '/ask', query: {q: ''}};
            this.$router.push({...url});
        },
        nextStep() {
            if (REGISTRATION_STEPS.indexOf(this.getRegistrationStep) >= REGISTRATION_STEPS.length) {
                this.$router.push({path: '/ask', query: {q: ''}}); //TODO: change to the market place when we'll build it.
                return;
            }
            // this.step++;
            this.incrementRegistrationStep();
            this.step = this.getRegistrationStep;
        }
    },
    created() {
        if(this.autoIncrementStep){
            this.incrementRegistrationStep();
        }
        this.step = this.getRegistrationStep;
    }
}