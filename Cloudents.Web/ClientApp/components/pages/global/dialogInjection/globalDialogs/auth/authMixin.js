import registrationService from '../../../../../../services/registrationService2';
import analyticsService from '../../../../../../services/analytics.service.js';
import * as routeNames from '../../../../../../routes/routeNames'
export default {
    props: {
        goTo: {
            type: Function
        }
    },
    data() {
        return {
            googleLoading: false,
            routeNames
        };
    },
    computed: {
        btnLoading() {
            return this.$store.getters.getGlobalLoading
        },
        termsLink() {
            let isFrymo = this.$store.getters.isFrymo
            return isFrymo ? 'https://help.frymo.com/en/article/terms' : 'https://help.spitball.co/en/article/terms-of-service'
        },
        policyLink() {
            let isFrymo = this.$store.getters.isFrymo
            return isFrymo ? 'https://help.frymo.com/en/policies' : 'https://help.spitball.co/en/article/privacy-policy'
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

                    if(self.isFromTutorReuqest) {
                        self.$store.dispatch('updateRequestDialog', true);
                        self.$store.dispatch('updateTutorReqStep', 'tutorRequestSuccess')
                        self.$store.dispatch('toggleProfileFollower', true)
                        return
                    }
                    
                    analyticsService.sb_unitedEvent('Login', 'Start Google')
                    commit('setComponent', '')
                    dispatch('updateLoginStatus', true)

                    let pathToRedirect = ['/','/learn','/register2'];
                    if (pathToRedirect.indexOf(self.$route.path) > -1) {
                        this.$router.push({name: this.routeNames.LoginRedirect})
                        return
                    }
                    // if(self.$route.path === '/' || self.$route.path === '/learn') {
                    // }
                    dispatch('userStatus')
                }).catch(error => {
                    if(error) {
                        self.$emit('showToasterError');
                    }
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