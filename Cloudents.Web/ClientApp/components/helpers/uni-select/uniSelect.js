import SetSchoolLanding from './steps/set_school_landing.vue'
import SetSchool from './steps/set_school.vue'
import SetClass from './steps/set_class.vue'
import { mapGetters, mapActions } from 'vuex';

export default {
    components:{
        SetSchoolLanding,
        SetSchool,
        SetClass
    },

    data(){
        return{
            enumSteps: this.getAllSteps(),
            classes: [],
            fnMethods: {
                changeStep: this.changeStep,
                changeSchoolName: this.changeSchoolName
            }
        }
    },

    computed:{
        currentStep: function(){
            return this.getCurrentStep();
        }
    },

    methods:{
        ...mapActions(['changeSelectUniState', 'updateCurrentStep']),
        ...mapGetters(['getAllSteps','getCurrentStep']),
        changeStep(step){
            if(step === this.enumSteps.done){
                this.changeSelectUniState(false);
            }else{
                console.log(`step changed to ${step}`)
                this.updateCurrentStep(step);
            }
        },
        goBack(){
            this.actualStep = 'SetSchoolLanding'
        }
    }
}