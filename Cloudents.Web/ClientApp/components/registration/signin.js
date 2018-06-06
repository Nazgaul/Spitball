import stepTemplate from './steps/stepTemplate.vue';
import VueRecaptcha from 'vue-recaptcha';
import registrationService from '../../services/registrationService';
import SbInput from "../question/helpers/sbInput/sbInput.vue";
import {mapGetters} from 'vuex'
const defaultSubmitRoute={path: '/note', query: {q: ''}};
export default {
    components: {stepTemplate, VueRecaptcha, SbInput},
    data() {
        return {
            userEmail: '',
            password: '',
            keepLogedIn: false,
            submitted:false,
            recaptcha: '',
            errorMessage:'',
        }
    },
    computed:{
        ...mapGetters(['unAuthPath'])
    },
    methods: {
        submit() {
            self = this;
                registrationService.signIn(this.userEmail, this.password, this.recaptcha)
                    .then(function () {
                        let url=self.unAuthPath||defaultSubmitRoute;
                        self.$router.push({...url});
                    }, function (reason) {
                        debugger;
                        self.errorMessage = reason.response.data;
                    });
        },
        onVerify(response) {
            this.recaptcha = response;
        },
        onExpired() {
            this.recaptcha = "";
        }
    }
}