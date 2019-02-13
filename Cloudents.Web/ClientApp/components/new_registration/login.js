import stepTemplate from './helpers/stepTemplate.vue'
import codesJson from './helpers/CountryCallingCodes';
import { mapActions, mapGetters, mapMutations } from 'vuex'
import VueRecaptcha from 'vue-recaptcha';

import analyticsService from '../../services/analytics.service';
import SbInput from "../question/helpers/sbInput/sbInput.vue";
import step_1 from "./steps/firstStep.vue";
import step_2 from "./steps/registerStep.vue";
import step_3 from "./steps/emailSent.vue";
import step_4 from "./steps/phoneEnter.vue";
import step_5 from "./steps/confirmationCode.vue";
import step_6 from "./steps/congrats.vue";
import step_7 from "./steps/loginStep.vue";
import step_8 from "./steps/emailedPassReset.vue";
import step_9 from "./steps/newPassword.vue";
import step_10 from "./steps/resetPassEmailInput.vue";
import step_11 from "./steps/validateEmail.vue";

import { LanguageService } from "../../services/language/languageService";


const defaultSubmitRoute = { path: '/' };
const initialPointsNum = 100;
const signInStr = '/signin';

var auth2;
export default {
    components: {
        stepTemplate,
        SbInput,
        VueRecaptcha,
        step_1,
        step_2,
        step_3,
        step_4,
        step_5,
        step_6,
        step_7,
        step_8,
        step_9,
        step_10,
        step_11
    },
    props: {
        default: false,
    },
    data() {
        return {
            resource: {
                mobile: this.getKey("sure_exit_mobile")

            },
            passScoreObj: {
                0: {
                    name: this.getKey("password_indication_weak"),
                    className: "bad"
                },
                1: {
                    name: this.getKey("password_indication_weak"),
                    className: "bad"
                },
                2: {
                    name: this.getKey("password_indication_strong"),
                    className: "good"
                },
                3: {
                    name: this.getKey("password_indication_strong"),
                    className: "good"
                },
                4: {
                    name: this.getKey("password_indication_strongest"),
                    className: "best"
                }
            },
            siteKey: '6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1',
            gaCategory: '',
            marketingData: {},
            agreeTerms: false,
            agreeError: false,
            loader: null,
            loading: false,
            countryCodesList: codesJson.sort((a, b) => a.name.localeCompare(b.name)),
            toUrl: '',
            confirmationCode: '',
            initialPointsNum,
            phone: {
                phoneNum: '',
                countryCode: ''
            },
            password: '',
            confirmPassword: '',
            passResetCode: '',
            ID: '',
            isPass: false,
            isSignIn: false,
            errorMessage: {
                phone: '',
                code: '',
                password: '',
                confirmPassword: ''
            },
            isNewUser: false,
            camefromCreate: false,
            showDialog: false,
            passDialog: false,
            toasterTimeout: 5000,
            stepNumber: 1,
            lastStep: [],
            userEmail: this.$store.getters.getEmail || '',
            recaptcha: '',
            stepsEnum: {
                "termandstart": 1,
                "startstep": 2,
                "emailconfirmed": 3,
                "enterphone": 4,
                "verifyphone": 5,
                "congrats": 6,
                "loginstep": 7,
                "emailconfirmedpass": 8,
                "createpassword": 9,
                "emailpassword": 10,
                "validateemail": 11
            }
        }
    },
    watch: {
        getShowToaster: function (val) {
            if (val) {
                var self = this;
                setTimeout(function () {
                    self.updateToasterParams({
                        showToaster: false
                    })
                }, this.toasterTimeout)
            }
        },

        '$route': function (from, to, next) {
            if (this.$route.path === signInStr) {
                return this.isSignIn = true;

            } else {
                return this.isSignIn = false;
            }
        }
    },
    computed: {
        ...mapGetters({
            getShowToaster: 'getShowToaster',
            getToasterText: 'getToasterText',
            lastActiveRoute: 'lastActiveRoute',
            campaignName: 'getCampaignName',
            campaignData: 'getCampaignData',
            profileData: 'getProfileData',
            isCampaignOn: 'isCampaignOn'
        }),
        isSignInComputed() {
            return this.isSignIn
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        },
        //profile data relevant for each stepNumber
        meta() {
            return this.profileData.register[this.stepNumber];
        }
    },
    methods: {
        ...mapMutations({ updateLoading: "UPDATE_LOADING" }),
        ...mapActions({ updateToasterParams: 'updateToasterParams', updateCampaign: 'updateCampaign' }),
        getKey(s) {
            return LanguageService.getValueByKey("login_" + s);
        },
        //do not change step, only from here
        changeStepNumber(param, skipPushState) {
            let step = param.toLowerCase();
            if (this.stepsEnum.hasOwnProperty(step)) {
                this.lastStep.push(this.stepNumber);
                //must insert a step to the history otherwise it will return to the previous route
                let fakeObj = {};
                if (!skipPushState) {
                    history.pushState(fakeObj, null);
                }
                this.stepNumber = this.stepsEnum[step];
            }
            console.log(this.stepNumber)
        },
        goBackStep(stepNumber) {
            let lastStepPoint = this.lastStep.pop();
            if (!lastStepPoint) {
                lastStepPoint = 1;
            }
            this.stepNumber = parseInt(lastStepPoint);
        },
        goToResetPassword() {
            this.passDialog = false;
            this.changeStepNumber('emailpassword');
        },
        $_back() {
            let url = this.toUrl || defaultSubmitRoute;
            this.$router.push({ path: `${url.path}` });
        },
        showDialogFunc() {
            this.showDialog = true
        },
        hideDialog() {
            this.showDialog = false
        },
    },

    mounted() {
        this.$nextTick(function () {
            gapi.load('auth2', function () {
                auth2 = gapi.auth2.init({
                    client_id: '341737442078-ajaf5f42pajkosgu9p3i1bcvgibvicbq.apps.googleusercontent.com',
                })
            })
        })
    },

    created() {
        let self = this;
        //history update event, fires when back btn clicked
        global.onpopstate = (event) => {
            self.goBackStep()
        };

        //event liseners for all steps
        self.$on('changeStep', (stepName) => {
            self.changeStepNumber(stepName);
        });
        self.$on('updateEmail', (email) => {
            self.userEmail = email;
        });
        self.$on('updatePhone', (phone) => {
            self.phone = phone;
        });
        self.$on('updateIsNewUser', (isNew) => {
            self.isNewUser = isNew;
        });
        self.$on('fromCreate', (create) => {
            if (create === 'create') {
                self.camefromCreate = true
            } else if (create === 'forgot') {
                self.camefromCreate = false
            }
        });
        self.$on('updateCountryCodeList', (countryCodes) => {
            self.phone.countryCode = countryCodes;
        });
        
        let path = self.$route.path.toLowerCase();
        //check if returnUrl exists
        if (!!self.$route.query.returnUrl) {
            self.toUrl = { path: `${self.$route.query.returnUrl}`, query: { term: '' } };
        }
        if (self.$route.query && self.$route.query.step) {
            let step = self.$route.query.step;
            self.changeStepNumber(step);
        } else if (self.$route.path === signInStr) {
            self.isSignIn = true;
            self.changeStepNumber('termandstart', true);
        } else if (path === '/resetpassword') {

            //v8Fix - please use js convension this.$route.query['id'] || ''
            self.passResetCode = self.$route.query['code'] ? self.$route.query['code'] : '';
            self.ID = self.$route.query['Id'] ? self.$route.query['Id'] : '';
            self.changeStepNumber('createpassword', true);
        }
        //check if new user param exists in email url
        self.isNewUser = self.$route.query['isNew'] !== undefined;
        if (self.isNewUser && self.stepNumber === 4) {
            analyticsService.sb_unitedEvent('Registration', 'Email Verified');

        }
        self.$loadScript("https://unpkg.com/zxcvbn@4.4.2/dist/zxcvbn.js").then(
            (data) => {
                //global.zxcvbn = data;
            }
        );
    },
}