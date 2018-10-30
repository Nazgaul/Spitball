import SetSchoolLanding from './steps/set_school_landing.vue'
import SetSchool from './steps/set_school.vue'
import SetClass from './steps/set_class.vue'
import { mapGetters, mapActions } from 'vuex'
import noWorries from './popups/noWorries/noWorries.vue'
import changingSchool from "./popups/changingSchool/changingSchool.vue"

export default {
    components:{
        SetSchoolLanding,
        SetSchool,
        SetClass,
        noWorries,
        changingSchool
    },

    data(){
        return{
            enumSteps: this.getAllSteps(),
            classes: [],
            fnMethods: {
                changeStep: this.changeStep,
                changeSchoolName: this.changeSchoolName,
                openNoWorriesPopup: this.openNoWorriesPopup,
                openAreYouSurePopup: this.openAreYouSurePopup
            },
            beforeLeave: false,
            areYouSurePopup: {
                show: false,
                continueActionFunction: null,
                closeFunction: null
            },
            
        }
    },

    computed:{
        currentStep: function(){
            return this.getCurrentStep();
        }
    },

    methods:{
        ...mapActions(['changeSelectUniState', 'updateCurrentStep', 'setUniversityPopStorage_session']),
        ...mapGetters(['getAllSteps','getCurrentStep']),
        changeStep(step){
            if(step === this.enumSteps.done){
                this.changeSelectUniState(false);
            }else{
                console.log(`step changed to ${step}`)
                this.updateCurrentStep(step);
            }
        },
        closeInterface(){
            this.setUniversityPopStorage_session();
            this.changeSelectUniState(false);
        },
        closeNoWorriesPopup(){
            this.beforeLeave = false;
        },
        openNoWorriesPopup(){
            this.beforeLeave = true;
        },
        openAreYouSurePopup(continueActionFunction){
            this.areYouSurePopup.show = true;
            this.areYouSurePopup.continueActionFunction = continueActionFunction;
            this.areYouSurePopup.closeFunction = this.closeAreYouSurePopup;
        },
        closeAreYouSurePopup(){
            this.areYouSurePopup.show = false;
        }
    },
    created(){
        console.log("unicreated")
    }
}