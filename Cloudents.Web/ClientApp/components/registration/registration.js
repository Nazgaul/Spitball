import registerEmail from './steps/registerEmail/registerEmail.vue'
import registerPhone from './steps/registerPhone/registerPhone.vue'
import registerUsername from './steps/registerUsername/registerUsername.vue'
import registerAccount from './steps/registerAccount/registerAccount.vue'
import {mapActions, mapGetters} from 'vuex'

export default {
    components: {registerEmail, registerPhone, registerUsername, registerAccount},
    props: {
        propStep:{
            type: Number,
            default: 0
        }
    },
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
        ...mapGetters(['getRegistrationStep', 'fromPath']),
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
            let url = this.fromPath || {path: '/ask', query: {q: ''}};
            this.$router.push({...url});
        },
        nextStep() {
            if (this.step >= 2) {
                this.$router.push({path: '/ask', query: {q: ''}}); //TODO: change to the market place when we'll build it.
                return;
            }
            this.step++;
        }
    },
    created() {
        this.step = this.propStep || this.getRegistrationStep;
    }
}