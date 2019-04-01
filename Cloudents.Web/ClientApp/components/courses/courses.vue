<template>
    <v-card class="edit-courses-card">
        <router-view></router-view>
    <!--<component :is="'step_'+currentStep" :callbackFunc="callBackmethods"> </component>-->
        <!--<router-view></router-view>-->
    </v-card>
</template>

<script>
    import { mapActions, mapGetters } from "vuex";
    import { LanguageService } from "../../services/language/languageService";
    import step_1 from './addCourses/addCourses.vue';
    import step_2 from './editCourses/editCourses.vue';


    export default {
        components: {
            step_1,
            step_2

        },
        data() {
            return {
                search: "",
                classNamePlaceholder: LanguageService.getValueByKey(
                    "uniSelect_type_class_name_placeholder"
                ),
                isRtl: global.isRtl,
                global: global,
                steps : 2,
                currentStep: 1,
                callBackmethods: {
                    next: this.nextStep,
                    changeStep: this.changeStep,
                },
            };
        },

        computed: {
            ...mapGetters(["getSelectedClasses"]),

        },
        methods: {
            ...mapActions(["updateClasses", "updateSelectedClasses", "assignClasses", "pushClassToSelectedClasses"]),
            ...mapGetters(["getSchoolName", "getClasses"]),

            lastStep() {
            this.$router.go(-1)
                // this.fnMethods.changeStep(this.enumSteps.set_school);
            },
            nextStep() {
                if (this.currentStep === this.steps) {
                    this.currentStep = 1
                } else {
                    this.currentStep = this.currentStep + 1;
                }
            },
            changeStep(step){
                this.currentStep = step
            }
        }
    };
</script>


<style lang="less" src="./courses.less">

</style>