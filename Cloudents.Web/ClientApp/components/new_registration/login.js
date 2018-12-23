import stepTemplate from './helpers/stepTemplate.vue'
import codesJson from './helpers/CountryCallingCodes';
import { mapActions, mapGetters, mapMutations } from 'vuex'
import VueRecaptcha from 'vue-recaptcha';

import analyticsService from '../../services/analytics.service';
import SbInput from "../question/helpers/sbInput/sbInput.vue";
import step_1 from "./steps/step_1.vue";
import step_2 from "./steps/step_2.vue";
import step_3 from "./steps/step_3.vue";
import step_4 from "./steps/step_4.vue";
import step_5 from "./steps/step_5.vue";
import step_6 from "./steps/step_6.vue";
import step_7 from "./steps/step_7.vue";
import step_8 from "./steps/step_8.vue";
import step_9 from "./steps/step_9.vue";
import step_10 from "./steps/step_10.vue";
import step_11 from "./steps/step_11.vue";

import { LanguageService } from "../../services/language/languageService";


const defaultSubmitRoute = { path: '/' };
const initialPointsNum = 100;


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
            progressSteps: 5,
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
        '$route': function (form, to) {
            //V8Fix - you should use $route.name and not path - what happen if we change the path?
            //Also why not const and in one place
            if (this.$route.path === '/signin') {
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
        isShowProgress() {
            //V8Fix  - look at the code below
            //let steps = [7, 8, 9, 10];
            //steps.indexOf(this.stepNumber) !== -1
            //learn : https://frontstuff.io/a-better-way-to-perform-multiple-comparisons-in-javascript

            let filteredSteps = this.stepNumber !== 7 && this.stepNumber !== 8 && this.stepNumber !== 9 && this.stepNumber !== 10;
            return filteredSteps
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
        //history update event, fires when back btn clicked
        global.onpopstate = (event) => {
            this.goBackStep()
        };
        //v8Fix -var self = this and use self. this way we mimify the code
        //event liseners for all steps
        this.$on('changeStep', (stepName) => {
            this.changeStepNumber(stepName);
        });
        this.$on('updateEmail', (email) => {
            this.userEmail = email;
        });
        this.$on('updatePhone', (phone) => {
            this.phone = phone;
        });
        this.$on('updateIsNewUser', (isNew) => {
            this.isNewUser = isNew;
        });
        this.$on('fromCreate', (create) => {
            if (create === 'create') {
                this.camefromCreate = true
            } else if (create === 'forgot') {
                this.camefromCreate = false
            }
        });
        this.$on('updateCountryCodeList', (countryCodes) => {
            this.phone.countryCode = countryCodes;
        })
        
        let path = this.$route.path.toLowerCase();
        //check if returnUrl exists
        if (!!this.$route.query.returnUrl) {
            this.toUrl = { path: `${this.$route.query.returnUrl}`, query: { term: '' } };
        }
        if (this.$route.query && this.$route.query.step) {
            let step = this.$route.query.step;
            this.changeStepNumber(step);
              //V8Fix - you should use $route.name and not path - what happen if we change the path?
            //why noe use var in line 263?
        } else if (this.$route.path === '/signin') {
            this.isSignIn = true;
            this.changeStepNumber('termandstart', true);
  //V8Fix - you should use $route.name and not path - what happen if we change the path?
        } else if (path === '/resetpassword') {

            //v8Fix - please use js convension this.$route.query['id'] || ''
            this.passResetCode = this.$route.query['code'] ? this.$route.query['code'] : '';
            this.ID = this.$route.query['Id'] ? this.$route.query['Id'] : '';
            this.changeStepNumber('createpassword', true);
        }
        //check if new user param exists in email url
        this.isNewUser = this.$route.query['isNew'] !== undefined;
        if (this.isNewUser && this.stepNumber === 4) {
            analyticsService.sb_unitedEvent('Registration', 'Email Verified');

        }
    },
}