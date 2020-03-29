import registrationService from '../../../../../../services/registrationService2';
import analyticsService from '../../../../../../services/analytics.service.js';

export default {
    data() {
        return {
            googleLoading: false,
        };
    },
    computed: {
        btnLoading() {
            return this.$store.getters.getGlobalLoading
        }
    },
    methods: {
        gmailRegister() {
            this.googleLoading = true;
            let self = this
            registrationService.googleRegistration()
                .then(({data}) => {
                    let { commit, dispatch } = self.$store
                    
                    self.googleLoading = false;

                    if (!data.isSignedIn) {
                        analyticsService.sb_unitedEvent('Registration', 'Start Google')
                        self.component = 'setPhone2'
                        return
                    }
                    
                    analyticsService.sb_unitedEvent('Login', 'Start Google')
                    commit('setRegisterDialog', false)
                    commit('setLoginDialog', false)
                    dispatch('updateLoginStatus', true)
                }).catch(error => {
                    let { response: { data } } = error
                    // if(data.Google) self.errors.gmail = self.$t('loginRegister_error_gmail')

                    self.errors.gmail = data["Google"] ? data["Google"][0] : ''; // TODO:
                    self.googleLoading = false;
                    self.$appInsights.trackException({exception: new Error(error)})
                })
        }
    },
    mounted() {
        let self = this;
        this.$nextTick(function () {
            this.$loadScript("https://apis.google.com/js/client:platform.js")
                .then(()=>{
                    self.$store.dispatch('gapiLoad');
                }).catch(ex => {
                    self.$appInsights.trackException({exception: new Error(ex)});
                })
        });
    }
}