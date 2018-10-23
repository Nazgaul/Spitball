import SetSchoolLanding from './steps/set_school_landing.vue'
import SetSchool from './steps/set_school.vue'
import SetClass from './steps/set_class.vue'

const stepsEnum = {
    set_school_landing: 'SetSchoolLanding',
    set_school: 'SetSchool',
    set_class: 'SetClass',
}

export default {
    components:{
        SetSchoolLanding,
        SetSchool,
        SetClass
    },

    data(){
        return{
            enumSteps: stepsEnum,
            actualStep: stepsEnum.set_school_landing,
            classes: [],
            fnMethods: {
                changeStep: this.changeStep,
                changeSchoolName: this.changeSchoolName
            }
        }
    },

    computed:{
        currentStep: function(){
            return this.actualStep;
        }
    },

    methods:{
        changeStep(step){
            console.log(`step changed to ${step}`)
            this.actualStep = step;
        },
        goBack(){
            this.actualStep = 'SetSchoolLanding'
        }
    }
}