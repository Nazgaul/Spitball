import SetSchoolLanding from './steps/set_school_landing.vue'
import SetSchool from './steps/set_school.vue'
import SetClass from './steps/set_class.vue'
import { mapGetters, mapActions } from 'vuex'
import noWorries from './popups/noWorries/noWorries.vue'
import sbDialog from '../../wrappers/sb-dialog/sb-dialog.vue';

export default {
    components:{
        SetSchoolLanding,
        SetSchool,
        SetClass,
        noWorries,
        sbDialog
    },

    data(){
        return{
            enumSteps: this.getAllSteps(),
            classes: [],
            fnMethods: {
                changeStep: this.changeStep,
                changeSchoolName: this.changeSchoolName
            },
            beforeLeave: false,
        }
    },

    computed:{
        currentStep: function(){
            return this.getCurrentStep();
        }
    },

    methods:{
        ...mapActions(['changeSelectUniState', 'updateCurrentStep', 'setSelectUniState', 'setUniversityPopStorage_session']),
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
        }
    }
}