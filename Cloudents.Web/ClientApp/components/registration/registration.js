//import InvisibleRecaptcha from 'vue-invisible-recaptcha'
import registerEmail from './steps/registerEmail/registerEmail.vue'
import registerPhone from './steps/registerPhone/registerPhone.vue'
import registerUsername from './steps/registerUsername/registerUsername.vue'
import registerAccount from './steps/registerAccount/registerAccount.vue'

export default {
    components: { /*InvisibleRecaptcha, */registerEmail, registerPhone, registerUsername, registerAccount },
    data(){
        return {
            step: this.$route.meta.step || 2
        }
    },
    methods: {
        $_back() {
            this.$router.go(-1);
        },
        nextStep(){
            if(this.step === 3){
                this.$router.this.$router.push({path: '/note', query: {q: ''}}); //TODO: change to the market place when we'll build it.
                return;
            }
            this.step++;
        }
    },
}