import {mapGetters, mapActions} from 'vuex'
import stepTemplate from '../stepTemplate.vue'

export default {
    components:{stepTemplate},
    data() {
        return {
            userEmail: this.$store.getters.getEmail || '',
            emailSent: false,
            googleSignInParams: {
                client_id: '997823384046-ddhrphigu0hsgkk1dglajaifcg2rggbm.apps.googleusercontent.com',
                scope: [
                    'https://www.google.com/m8/feeds/contacts/default/full',
                    'https://www.googleapis.com/auth/drive.readonly'
                ],
                immediate: false
            }
        }
    },
    computed: {
        ...mapGetters(['getEmail']),
    },
    methods: {
        ...mapActions(['updateEmail']),
        next() {
            this.updateEmail(this.userEmail);
            this.emailSent = true;
        },
        onSignInSuccess (googleUser) {
            // `googleUser` is the GoogleUser object that represents the just-signed-in user.
            // See https://developers.google.com/identity/sign-in/web/reference#users
            const profile = googleUser.getBasicProfile() // etc etc
            debugger;
        },
        onSignInError (error) {
            // `error` contains any error occurred.
            console.log('OH NOES', error)
        }
    }
}