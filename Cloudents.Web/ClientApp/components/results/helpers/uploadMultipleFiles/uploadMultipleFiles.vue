<template>
    <v-flex xs12>
        <v-icon class="close-upload-btn-icon" @click="confirmCloseOpen()">sbf-close</v-icon>
        <v-card elevation="0" class="sb-steps-wrap">
            <v-stepper v-model="currentStep" class="sb-stepper">
                <v-stepper-header class="sb-stepper-header px-4">
                    <template>
                        <h2 class="sb-step-title" v-language:inner>upload_files_header_title</h2>
                    </template>
                </v-stepper-header>
                <v-stepper-items class="sb-stepper-item">
                    <v-stepper-content :class="['sb-stepper-content', `step-${n}`]"
                                       v-for="n in 2"
                                       :key="`${n}-content`"
                                       :step="n">
                        <v-layout justify-center column wrap align-center v-if="firstStep" class="mt-4">
                            <v-flex xs12 sm6 d-flex row class="justify-center align-center mb-3 grow-1">
                                <v-icon class="col-blue mr-4">sbf-upload-cloud</v-icon>
                                <span class="upload-subtitle col-blue">Upload</span>
                            </v-flex>
                            <v-flex xs12 sm6 d-flex class="justify-center align-center">
                                <v-select
                                        class="course-select custom-select elevation-0"
                                        :items="['wer','werwer','dfgdfg','435435']"
                                        placeholder="Please select course"
                                        v-model="courseSelected"
                                        @input="updateSelectedCourse()"
                                        solo
                                        :append-icon="'sbf-arrow-down'"
                                ></v-select>
                            </v-flex>
                        </v-layout>
                        <transition name="slide">
                            <upload-files-start
                                    v-show="courseSelected && firstStep"
                                    :curStep="n"
                                    :callBackmethods="callBackmethods"></upload-files-start>
                        </transition>
                        <component
                                v-if="!firstStep"
                                :is="`upload-step_${n}`"
                                :curStep="n"
                                :callBackmethods="callBackmethods"></component>
                    </v-stepper-content>
                </v-stepper-items>
                <v-stepper-header v-show="courseSelected" class="sb-stepper-header footer px-2">
                        <v-flex v-show="!firstStep">
                            <v-btn class="upload-btn" :disabled="!isLoaded">
                                <span class="sb-btn-text">Upload</span>
                            </v-btn>
                        </v-flex>

                </v-stepper-header>
            </v-stepper>
        </v-card>

        <sb-dialog :showDialog="confirmationDialog"
                   :popUpType="'confirmationDialog'"
                   :isPersistent="true"
                   :activateOverlay="true"
                   :content-class="'confirmation-dialog'">
            <v-card class="confirm-card">
                <span class="warning-icon">!</span>
                <v-card-title class="confirm-headline">
                    <span v-language:inner>upload_confirm_stop_title</span>
                </v-card-title>
                <v-card-text class="confirm-text"><span v-language:inner>upload_confirm_text</span></v-card-text>
                <v-card-actions class="card-actions">
                    <v-btn round class="close-upload" @click.native="closeUpload()"><span v-language:inner>upload_confirm_btn_close</span>
                    </v-btn>
                    <v-btn round class="cancel" @click.native="confirmationDialog = false"><span v-language:inner>upload_confirm_btn_cancel</span>
                    </v-btn>
                </v-card-actions>
            </v-card>

        </sb-dialog>
    </v-flex>
</template>
<script>
    import { mapGetters, mapActions } from 'vuex';
    import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';
    import documentService from "../../../../services/documentService";
    import uploadFilesStart from "./helpers/uploadMultipleFileStart.vue";
    import uploadStep_2 from "./helpers/filesDetails.vue";

    export default {
        components: {
            uploadFilesStart,
            uploadStep_2,
            sbDialog,
        },
        name: "uploadMultipleFiles",
        data() {
            return {
                confirmationDialog: false,
                progressDone: false,
                steps: 2,
                currentStep: 1,
                step: 1,
                callBackmethods: {
                    next: this.nextStep,
                    changeStep: this.changeStep,
                    stopProgress: this.stopProgress,
                },
                courseSelected: ''
            }
        },

        computed: {
            ...mapGetters({
                accountUser: 'accountUser',
                getSchoolName: 'getSchoolName',
                getSelectedClasses: 'getSelectedClasses',
                getFileData: 'getFileData',
                getUploadProgress: 'getUploadProgress',
                getDialogState: 'getDialogState',
            }),
            firstStep() {
                return this.currentStep === 1;
            },
            showUploadDialog() {
                return this.getDialogState
            },
            isLoaded() {
                let result = this.getFileData.every((item) => {
                    return item.progress === 100
                });
                return result;
            }

        },
        methods: {
            ...mapActions([
                'updateFile',
                'updateNewQuestionDialogState',
                'changeSelectPopUpUniState',
                'updateDialogState',
                'resetUploadData',
                'setReturnToUpload',
                'updateStep',
                'setCourse'

            ]),
            updateSelectedCourse() {
                this.setCourse(this.courseSelected)
            },
            sendDocumentData(step) {
                this.loading = true;
                let docData = this.getFileData;
                // create Immutable copy, to prevent file name update in UI
                let docDataCopy = Object.assign({}, docData);
                // let docDataCopy = JSON.parse(JSON.stringify(docData));
                //send copy
                docDataCopy.name = this.getCustomFileName;
                let self = this;
                documentService.sendDocumentData(docDataCopy)
                    .then((resp) => {
                            if (resp.data.url) {
                                self.docReferral = resp.data.url
                            }
                            analyticsService.sb_unitedEvent('STUDY_DOCS', 'DOC_UPLOAD_COMPLETE');
                            console.log('DOC_UPLOAD_COMPLETE')
                            self.loading = true;
                            self.nextStep(step)
                        },
                        (error) => {
                            console.log("doc data error", error)
                        });

            },
            confirmCloseOpen() {
                if (this.currentStep === this.steps || this.firstStep) {
                    this.closeUpload()
                } else {
                    this.confirmationDialog = true;
                }

            },
            closeUpload() {
                this.resetUploadData();
                //reset return to upload
                this.setReturnToUpload(false);
                //close
                this.updateDialogState(false);
                this.confirmationDialog = false;
            },
            nextStep() {
                if (this.currentStep === this.steps) {
                    this.currentStep = 1
                } else {
                    this.currentStep = this.currentStep + 1;
                }

            },

            //resets mobile first step to mobile design
            // resetFirstStepMobile() {
            //     if (this.$vuetify.breakpoint.smAndDown) {
            //         this.updateUploadFullMobile(true);
            //     }
            // },

        },
        created() {

        }
    }
</script>

<style lang="less" src="./uploadMultipleFiles.less">

</style>