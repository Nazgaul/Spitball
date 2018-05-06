import InvisibleRecaptcha from 'vue-invisible-recaptcha'
import registerEmail from './steps/registerEmail.vue'
import registerPhone from './steps/registerPhone.vue'
import registerUsername from './steps/registerUsername.vue'
import registerAccount from './steps/registerAccount.vue'

export default {
    components: { InvisibleRecaptcha, registerEmail, registerPhone, registerUsername, registerAccount },
    data(){
        return {
            step: this.$route.query.hasOwnProperty("emailVerified") ? 3 : 0
        }
    }
}