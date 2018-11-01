<template>
    <v-card class="mb-5 sb-step-card">
        <div class="upload-row-1">
            <h3 class="upload-cloud-text sb-title">Awesome! get your document ready for
                sale</h3>
            <h4 class="sb-subtitle mt-2">Filling details will increase this doc. chances to get
                sold</h4>
        </div>
        <div class="upload-row-2 paddingTopSm">
            <div class="btn-holder">
                <label :for="'school'" class="steps-form-label school mb-2">
                    <v-icon class="mr-1">sbf-university</v-icon>
                    <span>School</span></label>
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
                <span>Select a Relevant Class</span>
            </label>
            <div class="chip-wrap">
                <v-chip name="sbf-class-chip" class="sbf-class-chip mb-2" outline
                        v-for="(singleClass, index) in classesList"
                        @click="updateClass(singleClass)"
                        :selected="selectedClass ===singleClass"
                        :key="index">{{singleClass}}
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
        components:{sbInput},
        data() {
            return {
                selectedClass: ''
            }
        },
        computed: {
            ...mapGetters({
                getSchoolName: 'getSchoolName',
                getSelectedClasses: 'getSelectedClasses',
            }),
            schoolName() {
                return this.getSchoolName ? this.getSchoolName : ''
            },
            classesList() {
                return this.isClassesSet ? this.getSelectedClasses : ['Class A', 'Class B', 'Class C', 'Class D', 'Class E']
            },
        },
        methods: {
            ...mapActions([ 'updateFile']),
            // update data methods
            updateClass(singleClass) {
                this.updateFile({'courses':singleClass});
                this.selectedClass = singleClass;
            },
        },
        created(){
            console.log('step 2 creadted')

        }
    }
</script>

<style >

</style>