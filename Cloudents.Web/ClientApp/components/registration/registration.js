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
            toasterTimeout: 5000,
            step: {
                type: Number
            },
            REGISTRATION_STEPS
        }
    },
    // watch: {
    //     step: function (val) {
    //         this.incrementRegistrationStep(val)
    //     },
    // },
    computed: {
        ...mapGetters(['getRegistrationStep', 'fromPath', 'getShowToaster', 'getToasterText']),
        // step() {
        //     if(this.$route.meta.step){
        //         this.updateRegistrationStep(this.$route.meta.step)
        //     }
        //     return this.getRegistrationStep;
        // }
    },
    watch:{
        getShowToaster: function (val) {
            if (val) {
                var self = this;
                setTimeout(function () {
                    self.updateToasterParams({
                        showToaster: false
                    })
                }, this.toasterTimeout)
            }
        }
    },
    methods: {

        ...mapActions(['incrementRegistrationStep', 'updateToasterParams']),

        $_back() {
            let url = this.fromPath || {path: '/ask', query: {q: ''}};
            this.$router.push({...url});
        },
        nextStep() {
            if (REGISTRATION_STEPS.indexOf(this.getRegistrationStep) >= REGISTRATION_STEPS.length) {
                this.$router.push({path: '/ask', query: {q: ''}});
                return;
            }
            // this.step++;
            this.incrementRegistrationStep();
            this.step = this.getRegistrationStep;
        }
    },
    created() {
        console.log('step!!!!')

        if(this.autoIncrementStep){
            this.incrementRegistrationStep();
            console.log('step2222')
        }
        this.step = this.getRegistrationStep;
    }
}