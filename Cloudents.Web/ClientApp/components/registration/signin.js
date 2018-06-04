import stepTemplate from './steps/stepTemplate.vue';
import VueRecaptcha from 'vue-recaptcha';
import registrationService from '../../services/registrationService';

export default {
    components: {stepTemplate, VueRecaptcha},
    data() {
        return {
            userEmail: '',
            password: '',
            keepLogedIn: false,
            submitted:false,
            recaptcha: '',
        }
    },
    computed:{
        disableSubmit: function () {
            return this.submitted || !(this.userEmail && this.password && this.recaptcha);
        }
    },
    methods: {
        submit() {
            self = this;
            if(!this.submitted) {
                this.submitted = true;
                registrationService.signIn(this.userEmail, this.password, this.recaptcha)
                    .then(function () {
                        self.$router.push({path: '/note', query: {q: ''}});
                    }, function (reason) {
                        debugger;
                        console.error(reason);
                    });
            }
        },
        onVerify(response) {
            this.recaptcha = response;
        },
        onExpired() {
            this.recaptcha = "";
        }
    },
}