import { mapGetters, mapActions } from 'vuex';
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';

import documentService from "../../../../services/documentService";
import analyticsService from '../../../../services/analytics.service';
import uploadStep_1 from "./uploadSteps/uploadFileStart.vue";
import uploadStep_2 from "./uploadSteps/uploadDocSchoolClass.vue";
import uploadStep_3 from "./uploadSteps/documentType.vue";
import uploadStep_4 from "./uploadSteps/documTitleProfessor.vue";
import uploadStep_5 from "./uploadSteps/documentTags.vue";
import uploadStep_6 from "./uploadSteps/documentPrice.vue";
import uploadStep_7 from "./uploadSteps/finalDocumentScreen.vue";
import uploadStep_8 from "./uploadSteps/documentReferral.vue";
//  import uploadStep_6 from "./uploadSteps/finalDocumentScreen.vue";
// import uploadStep_7 from "./uploadSteps/documentReferral.vue";


export default {
    components: {
        uploadStep_1,
        uploadStep_2,
        uploadStep_3,
        uploadStep_4,
        uploadStep_5,
        uploadStep_6,
        uploadStep_7,
        uploadStep_8,
        sbDialog,
    },
    name: "uploadFiles",
    data() {
        return {
            confirmationDialog: false,
            progressDone: false,
            steps: 8,
            currentStep: 1,
            //TODO V13 currentStep: this.$store.state.uploadFiles.uploadStep || 1,
            step: 1,
            stepsProgress: 100 / 7,
            gotoAsk: false,
            transitionAnimation: 'slide-y-transition',
            callBackmethods: {
                next: this.nextStep,
                changeStep: this.changeStep,
                stopProgress: this.stopProgress,
                // closeAndOpenAsk: this.closeAndOpenAsk
            },
            clearChildrenData: false,
            docReferral: '',
            loading: false,
        }
    },
    props: {},

    computed: {
        ...mapGetters({
            accountUser: 'accountUser',
            getSchoolName: 'getSchoolName',
            getSelectedClasses: 'getSelectedClasses',
            getFileData: 'getFileData',
            getUploadProgress: 'getUploadProgress',
            getDialogState: 'getDialogState',
            getCustomFileName: "getCustomFileName",
            uploadStep: 'uploadStep'
        }),
        showUploadDialog() {
            return this.getDialogState
        },
        progressShow() {
            return !this.progressDone
        },

        isFirstStep() {
            return this.currentStep === 1
        },
        isLastStepAndMobile() {
            return this.$vuetify.breakpoint.smAndDown && this.currentStep === this.steps;
        },
        // button disabled for each step and enabled once everything filled
        isDisabled() {
            // TODO V13 !this.isUploadActiveProcess add in check if disabled for all
            if (this.currentStep === 2 && !this.getFileData.course) {
                return true
            }
            else if (this.currentStep === 3 && Object.keys(this.getFileData.type || '').length === 0) {
                return true
            } else if (this.currentStep === 4 && (!this.getFileData.name || this.getCustomFileName.length === 0)) {
                return true
            }
            else if (this.currentStep === 5 ) {
                return false
            }
            // else if (this.currentStep === 6 && !this.getFileData.price) {
            //     return true
            // }
            else if (this.currentStep === 6) {
                return false
            }
            //disable if loading not done yet
            else if (this.currentStep === 7 && !this.progressDone) {
                return true
            } else {
                return false
            }
        }
    },
    watch: {
        //TODO V13 update step in store to take user back to upload ifexited to change class
        currentStep(newValue, oldValue) {
            this.updateStepInStore(newValue);
        }
    },
    methods: {
        ...mapActions([
            'updateFile',
            'updateNewQuestionDialogState',
            'changeSelectPopUpUniState',
            'updateUploadFullMobile',
            'updateDialogState',
            'resetUploadData',
            'setReturnToUpload',
            'updateStep',
            'isUploadActiveProcess'
        ]),

        stopProgress(val) {
            return this.progressDone = val;
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
            if (this.currentStep === this.steps || this.isFirstStep) {
                this.closeUpload()
            } else {
                this.confirmationDialog = true;
            }

        },
        closeUpload() {
            this.resetFirstStepMobile();
            this.resetUploadData({});
            //reset return to upload
            this.setReturnToUpload(false);
            //close
            this.updateDialogState(false);
            this.confirmationDialog = false;

        },
        nextStep(step) {
            if (this.currentStep === this.steps) {
                this.currentStep = 1
            } else {
                this.currentStep = this.currentStep + 1;
                this.stepsProgress = ((100 / 7) * this.currentStep);
            }
            console.log('step', this.stepsProgress, this.currentStep);

        },
        previousStep(step) {
            if (this.currentStep === 1) {
                return this.currentStep = 1;
            } else {
                this.currentStep = this.currentStep - 1;
                this.stepsProgress = ((100 / 7) * this.currentStep);
            }

        },
        //update step in store to take back if needed to step left from
        updateStepInStore(){
            this.updateStep(this.currentStep)
        },
        //resets mobile first step to mobile design
        resetFirstStepMobile() {
            if (this.$vuetify.breakpoint.smAndDown) {
                this.updateUploadFullMobile(true);
            }
        },
        changeStep(step) {
            //clean up everytnig for new doc upload
            if (step === 1) {
                this.resetUploadData({});
                this.resetFirstStepMobile();
            }
            this.currentStep = step;
        }
    },
    created(){
        //TODO V13 stop progress animation if came from classes set
        // if(!!this.isUploadActiveProcess){
        //     this.progressDone = true;
        // }
        console.log(this.currentStep)
    }
}