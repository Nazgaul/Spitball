import registrationService from '../../../../../../services/registrationService2';
import analyticsService from '../../../../../../services/analytics.service.js';
import * as routeNames from '../../../../../../routes/routeNames'
export default {
    props: {
        goTo: {
            type: Function
        },
        teacher: {
            type: Boolean,
            default: false
        }
    },
    data() {
        return {
            googleLoading: false,
            routeNames,
            localCode: '',
            phoneNumber: '',
            errors: {
                gmail: '',
                phone: '',
                code: '',
                email: '',
                password: '',
            }
        };
    },
    computed: {
        isVerifyPhone() {
            return this.component === 'verifyPhone'
        },
        btnLoading() {
            return this.$store.getters.getGlobalLoading
        },
        isFromTutorReuqest() {
            return this.$store.getters.getIsFromTutorStep
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
        login(){
            let childComp = this.$refs.childComponent
            let loginObj = {
                email: childComp.email,
                password: childComp.password
            }

            let self = this
            registrationService.emailLogin(loginObj)
                .then(({data}) => {
                    global.country = data.country; // should we need this? @idan
                    analyticsService.sb_unitedEvent('Login', 'Start');

                    if(self.presetRouting()) return
                    
                    self.$store.dispatch('userStatus')
                }).catch(error => {      
                    let { response: { data } } = error

                    self.errors.password = data["Password"] ? error.response.data["Password"][0] : ''
                    self.$appInsights.trackException({exception: new Error(error)})
                })
        },
        gmailRegister() {
            this.googleLoading = true;
            let self = this
            registrationService.googleRegistration()
                .then(({data}) => {
                    self.googleLoading = false;
                    if (!data.isSignedIn) {
                        analyticsService.sb_unitedEvent('Registration', 'Start Google')
                        if(data.param?.phoneNumber) {
                            self.component = 'verifyPhone'
                            return
                        }
                        self.component = 'setPhone2'
                        return
                    }
                    analyticsService.sb_unitedEvent('Login', 'Start Google')

                    if(self.presetRouting()) return
                    window.location.reload()
                    // self.$store.dispatch('userStatus')
                }).catch(error => {
                    if(error) {
                        self.$emit('showToasterError');
                    }
                    self.googleLoading = false;
                    self.$appInsights.trackException({exception: new Error(error)})
                })
        },
        verifyPhone(){
            let childComp = this.$refs.childComponent

			let self = this
			registrationService.smsCodeVerification({number: childComp.smsCode})
				.then(userId => {
                    let { dispatch } = self.$store

                    analyticsService.sb_unitedEvent('Registration', 'Phone Verified');
                    if(!!userId){
                        analyticsService.sb_unitedEvent('Registration', 'User Id', userId.data.id);
                    }

                    if(self.presetRouting()) return

					dispatch('userStatus').then(user => {
                        // when user is register and pick teacher, redirect him to his profile page
                        if(self.teacher) {
                            self.$router.push({
                                name: self.routeNames.Profile,
                                params: {
                                    id: user.id,
                                    name: user.name,
                                },
                                query: {
                                    dialog: 'becomeTutor'
                                }
                            })
                            return
                        }
                        self.$router.push({name: self.routeNames.LoginRedirect})
                    })
				}).catch(error => {
                    self.errors.code = self.$t('loginRegister_invalid_code')
                    self.$appInsights.trackException({exception: new Error(error)});
                })
        },
        presetRouting() {
            this.setStatusLogin()

            if(this.isFromTutorReuqest) {
                this.fromTutorReuqest()
                return true
            }

            if(this.needRedirect()) return true

            this.fromStudyRoom()

            return false
        },
        needRedirect() {
            let pathToRedirect = ['/','/learn'];
            if (pathToRedirect.indexOf(this.$route.path) > -1 && !this.teacher) {
                this.$router.push({name: this.routeNames.LoginRedirect})
                return true
            }
            return false
        },
        sendSms(){
            let childComp = this.$refs.childComponent
            let smsObj = {
                countryCode: childComp.localCode,
                phoneNumber: childComp.phoneNumber
            }

            let self = this
            registrationService.smsRegistration(smsObj)
                .then(function (){
                    let { dispatch } = self.$store

                    dispatch('updateToasterParams',{
                        toasterText: self.$t("login_verification_code_sent_to_phone"),
                        showToaster: true,
                    });
                    analyticsService.sb_unitedEvent('Registration', 'Phone Submitted');
                    self.component = 'verifyPhone'
                }).catch(error => {
                    let { response: { data } } = error
                    
                    self.errors.phone = data && data["PhoneNumber"] ? data["PhoneNumber"][0] : ''
                    self.$appInsights.trackException({exception: new Error(error)});
                })
        },
        phoneCall(){
			let self = this
			registrationService.voiceConfirmation()
            	.then(() => {
					self.$store.dispatch('updateToasterParams',{
						toasterText: self.$t("login_call_code"),
						showToaster: true,
					});
				}).catch(error => {
                    self.$appInsights.trackException({exception: new Error(error)});
                })
        },
        fromTutorReuqest() {
            this.$store.dispatch('userStatus')
            this.needRedirect()

            this.$store.dispatch('updateRequestDialog', true);
            this.$store.dispatch('updateTutorReqStep', 'tutorRequestSuccess')
            this.$store.dispatch('toggleProfileFollower', true)
        },
        fromStudyRoom() {
            if(this.$route.name === this.routeNames.StudyRoom){
                global.location.reload();
            }
        },
        setStatusLogin(){
            this.$store.commit('setComponent', '')
            this.$store.commit('changeLoginStatus', true)
        },
        updatePhone(phone) {
            this.phoneNumber = phone
        },
        updateCode(code) {
            this.localCode = code
        },
        goStep(step) {
            this.component = step
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