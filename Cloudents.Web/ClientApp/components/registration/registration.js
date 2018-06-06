import registerEmail from './steps/registerEmail/registerEmail.vue'
import registerPhone from './steps/registerPhone/registerPhone.vue'
import registerUsername from './steps/registerUsername/registerUsername.vue'
import registerAccount from './steps/registerAccount/registerAccount.vue'
import {mapActions, mapGetters} from 'vuex'

export default {
    components: {registerEmail, registerPhone, registerUsername, registerAccount},
    data() {
        return {
            // step: this.$route.meta.step || 0,
            showDialog: false,
            step: 0
        }
    },
    watch: {
        step: function (val) {
            this.updateRegistrationStep(val)
        },
    },
    computed: {
        ...mapGetters(['getRegistrationStep']),
        // step() {
        //     if(this.$route.meta.step){
        //         this.updateRegistrationStep(this.$route.meta.step)
        //     }
        //     return this.getRegistrationStep;
        // }
    },
    methods: {
        ...mapActions(['updateRegistrationStep']),
        $_back() {
            this.$router.go(-1);
        },
        nextStep() {
            if (this.step === 3) {
                this.$router.this.$router.push({path: '/note', query: {q: ''}}); //TODO: change to the market place when we'll build it.
                return;
            }
            this.step++;
        }
    },
    created() {
        this.step = 0;//this.$route.meta.step || this.getRegistrationStep; TODO: revert after design changes are implemented
    }
}