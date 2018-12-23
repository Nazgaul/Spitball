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
        },
        currentStepCssClass(){
            let className = '';
            if(this.currentStep  && this.currentStep.toLowerCase() === 'setschool'){
                className = 'set-school'
            }else if(this.currentStep && this.currentStep.toLowerCase()  === 'setclass'){
                className = 'set-class'
            }
            return className
        }

    },

    methods:{
        ...mapActions(['changeSelectUniState', 'updateCurrentStep', 'setUniversityPopStorage_session', 'updateDialogState']),
        ...mapGetters(['getAllSteps','getCurrentStep', 'getReturnToUpload', 'getSelectedClasses']),
        changeStep(step){
            if(step === this.enumSteps.done){
                this.changeSelectUniState(false);
                let isReturnToUpload = this.getReturnToUpload();
                let isClasses = this.getSelectedClasses();
                //if class was set and return to upload is true, open upload component
                if(isReturnToUpload && isClasses.length > 0){
                    this.updateDialogState(true);
                }

            }else{
                console.log(`step changed to ${step}`);
                this.updateCurrentStep(step);
            }
        },
        closeInterface(){
            this.closeNoWorriesPopup();
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
        console.log(this.$route)
    },

}