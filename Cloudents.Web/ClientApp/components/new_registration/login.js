import stepTemplate from './helpers/stepTemplate.vue'
import codesJson from './helpers/CountryCallingCodes';
import { mapActions, mapGetters, mapMutations } from 'vuex'
import VueRecaptcha from 'vue-recaptcha';
import registrationService from '../../services/registrationService'
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

const defaultSubmitRoute = {path: '/ask'};
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
        step_10
    },
    props: {
        default: false,
    },
    data() {
        return {
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
            lastStep:[],
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
                "emailpassword": 10
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
        isShowProgress() {
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
        ...mapMutations({updateLoading: "UPDATE_LOADING"}),
        ...mapActions({updateToasterParams: 'updateToasterParams', updateCampaign: 'updateCampaign'}),

        //do not change step, only from here
        changeStepNumber(param, skipPushState) {
            let step = param.toLowerCase();
            if (this.stepsEnum.hasOwnProperty(step)) {
                this.lastStep.push(this.stepNumber);
                //must insert a step to the history otherwise it will return to the previous route
                let fakeObj = {};
                if(!skipPushState){
                    history.pushState(fakeObj, null);
                }
                this.stepNumber = this.stepsEnum[step];
            }
            console.log(this.stepNumber)
        },
        goBackStep(stepNumber){
            let lastStep = this.lastStep.pop();
            this.stepNumber = parseInt(lastStep);
        },
        goToResetPassword() {
            this.passDialog = false;
            this.changeStepNumber('emailpassword');
        },
        $_back() {
            let url = this.toUrl || defaultSubmitRoute;
            this.$router.push({path: `${url.path }`});
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
        global.onpopstate = (event)=> {
                this.goBackStep()
        };
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
        this.$on('fromCreate', (create) => {
            if (create === 'create') {
                this.camefromCreate = true
            }else if(create === 'forgot'){
                this.camefromCreate = false
            }
        });

        let path = this.$route.path.toLowerCase();
        //check if returnUrl exists
        if (!!this.$route.query.returnUrl) {
            this.toUrl = {path: `${this.$route.query.returnUrl}`, query: {q: ''}};
        }
        if (this.$route.query && this.$route.query.step) {
            let step = this.$route.query.step;
            this.changeStepNumber(step);
        } else if (this.$route.path === '/signin') {
            this.changeStepNumber('termandstart', true);
            this.isSignIn = true;
        } else if (path === '/resetpassword') {
            this.passResetCode = this.$route.query['code'] ? this.$route.query['code'] : '';
            this.ID = this.$route.query['Id'] ? this.$route.query['Id'] : '';
            this.changeStepNumber('createpassword', true);
        }
        registrationService.getLocalCode().then(({data}) => {
            this.phone.countryCode = data.code;
        });
        //check if new user param exists in email url
        this.isNewUser = this.$route.query['newUser'] !== undefined;
        if (this.isNewUser && this.stepNumber === 3) {
            analyticsService.sb_unitedEvent('Registration', 'Email Verified');

        }
    },
    //value = String; query = ['String', 'String','String'] || []
    // filters: {
    //     bolder: function (value, query) {
    //         if (query.length) {
    //             query.map((item) => {
    //                 value = value.replace(item, '<span class="bolder">' + item + '</span>')
    //             });
    //         }
    //         return value
    //     }
    // }
}