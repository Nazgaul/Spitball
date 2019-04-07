import SetSchoolLanding from './steps/set_school_landing.vue'
import SetSchool from './steps/set_school.vue'
import SetClass from './steps/set_class.vue'
import { mapGetters, mapActions, mapMutations } from 'vuex'
import noWorries from './popups/noWorries/noWorries.vue'
import changingSchool from "./popups/changingSchool/changingSchool.vue"
import addSchoolOrClass from "./popups/addSchoolorClass/addSchoolorClass.vue"
import { LanguageService } from "../../../services/language/languageService";

export default {
    components:{
        SetSchoolLanding,
        SetSchool,
        SetClass,
        noWorries,
        changingSchool,
        addSchoolOrClass
    },

    data(){
        return{
            enumSteps: this.getAllSteps(),
            classes: [],
            schoolPlaceholder: LanguageService.getValueByKey("uniSelect_type_school_name_placeholder"),
            classesPlaceholder:  LanguageService.getValueByKey("uniSelect_type_class_name_placeholder"),
            fnMethods: {
                changeStep: this.changeStep,
                changeSchoolName: this.changeSchoolName,
                openNoWorriesPopup: this.openNoWorriesPopup,
                // openAreYouSurePopup: this.openAreYouSurePopup,
                openAddSchoolOrClass: this.openAddSchoolOrClass
            },
            beforeLeave: false,
            // areYouSurePopup: {
            //     show: false,
            //     continueActionFunction: null,
            //     closeFunction: null
            // },
            openAddSchoolOrClassData:{
                show: false,
                continueActionFunction: null,
                closeFunction: null,
                isSchool: null
            },
            returnPath: '/note'
        }
    },
    props:{
        step:{
            type:String
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
        ...mapActions(['changeSelectUniState', 'updateCurrentStep', 'setUniversityPopStorage_session', 'updateDialogState', 'assignSelectedClassesCache']),
        ...mapGetters(['getAllSteps','getCurrentStep', 'getReturnToUpload', 'getSelectedClasses']),
        ...mapMutations(['UPDATE_SEARCH_LOADING']),
        changeStep(step){
            if(step === this.enumSteps.done){
                this.assignSelectedClassesCache();
                // this.changeSelectUniState(false);
                this.UPDATE_SEARCH_LOADING(true);
                this.$router.push({
                    path:this.returnPath,
                })
                let isReturnToUpload = this.getReturnToUpload();
                let isClasses = this.getSelectedClasses();
                //if class was set and return to upload is true, open upload component
                if(isReturnToUpload && isClasses.length > 0){
                    this.updateDialogState(true);
                }
                
            }else if(step === this.enumSteps.set_class){
                this.$router.push({name: 'editCourse'});
            }
            else{
                console.log(`step changed to ${step}`);
                this.updateCurrentStep(step);
            }
        },
        closeInterface(){
            this.closeNoWorriesPopup();
            this.setUniversityPopStorage_session();
            // this.changeSelectUniState(false);
            this.UPDATE_SEARCH_LOADING(true);
            this.$router.push({
                path:this.returnPath,
            })
        },
        closeNoWorriesPopup(){
            this.beforeLeave = false;
        },
        openNoWorriesPopup(){
            this.beforeLeave = true;
        },
        // openAreYouSurePopup(continueActionFunction){
        //     this.areYouSurePopup.show = true;
        //     this.areYouSurePopup.continueActionFunction = continueActionFunction;
        //     this.areYouSurePopup.closeFunction = this.closeAreYouSurePopup;
        // },
        // closeAreYouSurePopup(){
        //     this.areYouSurePopup.show = false;
        // },
        openAddSchoolOrClass(addSchool, continueActionFunction){
            this.openAddSchoolOrClassData.show = true;
            this.openAddSchoolOrClassData.continueActionFunction = continueActionFunction;
            this.openAddSchoolOrClassData.closeFunction = this.closeOpenAddSchoolOrClassPopup;
            this.openAddSchoolOrClassData.isSchool = addSchool;
        },
        closeOpenAddSchoolOrClassPopup(){
            this.openAddSchoolOrClassData.show = false;
        }
    },
    created(){
        this.updateCurrentStep(this.step);
    },

}