import registrationService from '../../../../../../services/registrationService2';
import analyticsService from '../../../../../../services/analytics.service.js';
import * as routeNames from '../../../../../../routes/routeNames'
import isWebView from  "is-ua-webview";
export default {
    props: {
        teacher: {
            type: Boolean,
            default: false
        }
    },
    data() {
        return {
            gmailBtnLodaing: false,
            studyroomRoute: routeNames.StudyRoom,
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
        isStudyRoomRoute() {
            return this.$route.name === this.studyroomRoute
        },
        cIsWebView() {  
            const retVal = isWebView(navigator.userAgent);
            return !retVal;
        },
        // isVerifyPhone() {
        //     return this.component === 'verifyPhone'
        // },
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
        },
        isLogged() {
            return this.$store.getters.getUserLoggedInStatus
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
                    global.country = data.country;
                    analyticsService.sb_unitedEvent('Login', 'Start');

                    if(self.presetRouting()) return

                    window.location.reload()
                }).catch(error => {
                    let { response: { data } } = error

                    self.errors.password = data["Password"] ? error.response.data["Password"][0] : ''
                    self.$appInsights.trackException(error)
                })
        },
        gmailRegister() {
            this.gmailBtnLodaing = true
            let userType = this.teacher ? 'tutor' : 'student'
            if(this.isFromTutorReuqest) {
                sessionStorage.setItem('hash','#tutorRequest');
                sessionStorage.setItem('tutorRequest', JSON.stringify({
                    text: this.$store.getters.getCourseDescription ,
                    course: this.$store.getters.getSelectedCourse?.text || this.$store.getters.getSelectedCourse, 
                    tutorId: this.$store.getters.getCurrTutor?.userId 
                }))
            }
            window.location.assign(`/External/Google?usertype=${userType}&returnUrl=${encodeURIComponent(window.location.pathname+window.location.search)}`);
        },
        verifyPhone(smsCode){
			let self = this
			registrationService.smsCodeVerification({number: smsCode})
				.then(() => {
                    analyticsService.sb_unitedEvent('Registration', 'Phone Verified');
                    self.$store.commit('setComponent', '');
                    self.$store.commit('setPhoneTaskComplete');
				}).catch(error => {
                    self.errors.code = self.$t('loginRegister_invalid_code')
                    self.$appInsights.trackException(error);
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
        sendSms() {
            let smsObj = {
                countryCode: this.localCode,
                phoneNumber: this.phoneNumber
            }
            return registrationService.smsRegistration(smsObj)
        },
        // phoneCall(){
		// 	let self = this
		// 	registrationService.voiceConfirmation()
        //     	.then(() => {
		// 			self.$store.dispatch('updateToasterParams',{
		// 				toasterText: self.$t("login_call_code"),
		// 				showToaster: true,
		// 			});
		// 		}).catch(error => {
        //             self.$appInsights.trackException(error);
        //         })
        // },
        fromTutorReuqest() {
            this.$store.dispatch('userStatus')
            this.needRedirect()

            this.$store.dispatch('updateRequestDialog', true);
            this.$store.dispatch('updateTutorReqStep', 'tutorRequestSuccess')
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
    }
}