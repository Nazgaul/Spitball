<template>
    <v-card class="mb-5 sb-step-card">
        <div class="upload-row-1">
            <h3 class="upload-cloud-text sb-title" ><span v-language:inner>upload_first_step2_title_partOne</span><br/>
            <span v-language:inner>upload_first_step2_title_partTwo</span>
            </h3>
            <h4 class="sb-subtitle mt-2" v-language:inner>upload_files_step2_subtitle</h4>
        </div>
        <div class="upload-row-2 paddingTopSm">
            <div class="btn-holder">
                <label :for="'school'" class="steps-form-label school mb-2">
                    <v-icon class="mr-1">sbf-university</v-icon>
                    <span v-language:inner>upload_files_label_school</span></label>
                <sb-input :bottomError="true"
                          v-model="schoolName" placeholder="Your School" name="school"
                          type="text" :disabled="true"
                          :autofocus="true" @keyup.enter.native="">
                </sb-input>
            </div>
            <div class="btn-holder">
            </div>
        </div>
        <div class="upload-row-3 chip-classes-row">
            <label :for="'class-chip'" class="steps-form-label mb-2">
                <v-icon class="mr-1">sbf-classes</v-icon>
                <span v-language:inner>upload_files_label_class</span>
            </label>
            <div class="chip-wrap">
                <v-chip name="sbf-class-chip" class="sbf-class-chip mb-2" outline
                        v-for="(singleClass, index) in classesList"
                        @click="updateClass(singleClass.text)"
                        :selected="selectedClass ===singleClass.text"
                        :key="index">{{singleClass.text}}
                </v-chip>
            </div>
        </div>
    </v-card>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import sbInput from "../../../../question/helpers/sbInput/sbInput";

    export default {
        name: "uploadStep_2",
        components: {sbInput},
        data() {
            return {
                // selectedClass: '',
            }
        },
        computed: {
            ...mapGetters({
                getSchoolName: 'getSchoolName',
                getSelectedClasses: 'getSelectedClasses',
                getFileData: 'getFileData'
            }),
            schoolName() {
                return this.getSchoolName ? this.getSchoolName : ''
            },
            isClassesSet() {
                return this.getSelectedClasses.length > 0
            },
            classesList() {
                let result = []
                if(this.isClassesSet){
                    this.getSelectedClasses.forEach(chip=>{
                        if(chip.text){
                            result.push(chip)
                        }else{
                            let newChip = {
                                text: chip
                            }
                            result.push(newChip)
                        } 
                    })
                }
                return result;
            },
            selectedClass() {
                return this.getFileData.course;
            }
        },

        methods: {
            ...mapActions(['updateFile']),
            // update data methods
            updateClass(singleClass) {
                this.updateFile({'course': singleClass});
            },
        },
        beforeDestroy() {
            console.log('step 2 destroyed')
        },
        created() {
            console.log('step 2 creadted')

        }
    }
</script>

<style>

</style>