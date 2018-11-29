import { mapGetters, mapActions } from 'vuex';
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';

import documentService from "../../../../services/documentService";
import uploadStep_1 from "./uploadSteps/uploadFileStart.vue";
import uploadStep_2 from "./uploadSteps/uploadDocSchoolClass.vue";
import uploadStep_3 from "./uploadSteps/documentType.vue";
import uploadStep_4 from "./uploadSteps/documTitleProfessor.vue";
import uploadStep_5 from "./uploadSteps/documentTags.vue";
// import uploadStep_6 from "./uploadSteps/documentPrice.vue";
// import uploadStep_7 from "./uploadSteps/finalDocumentScreen.vue";
// import uploadStep_8 from "./uploadSteps/documentReferral.vue";
 import uploadStep_6 from "./uploadSteps/finalDocumentScreen.vue";
import uploadStep_7 from "./uploadSteps/documentReferral.vue";


export default {
    components: {
        uploadStep_1,
        uploadStep_2,
        uploadStep_3,
        uploadStep_4,
        uploadStep_5,
        uploadStep_6,
        uploadStep_7,
        // uploadStep_8,
        sbDialog,
    },
    name: "uploadFiles",
    data() {
        return {
            confirmationDialog: false,
            progressDone: false,
            steps: 7,
            currentStep: 1,
            step: 1,
            stepsProgress: 100 / 6,
            gotoAsk: false,
            transitionAnimation: 'slide-y-transition',
            callBackmethods: {
                next: this.nextStep,
                changeStep: this.changeStep,
                stopProgress: this.stopProgress,
                // closeAndOpenAsk: this.closeAndOpenAsk
            },
            clearChildrenData: false,
            docReferral: ''
        }
    },
    props: {},

    computed: {
        ...mapGetters({
            accountUser: 'accountUser',
            getSchoolName: 'getSchoolName',
            getSelectedClasses: 'getSelectedClasses',
            getFileData: 'getFileData',
            getLegal: 'getLegal',
            getUploadProgress: 'getUploadProgress',
            getDialogState: 'getDialogState',
            getCustomFileName: "getCustomFileName"
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
            if (this.currentStep === 2 && !this.getFileData.course) {
                return true
            }
            else if (this.currentStep === 3 && Object.keys(this.getFileData.type).length === 0) {
                return true
            } else if (this.currentStep === 4 && (!this.getFileData.name)) {
                return true
            }
            else if (this.currentStep === 5) {
                return false
            }
            // else if (this.currentStep === 6 && !this.getFileData.price) {
            //     return true
            // }
            else if (this.currentStep === 6 && !this.getLegal) {
                return true
            }
            else if (this.currentStep === 7 && !this.getLegal) {
                return true
            } else {
                return false
            }
        }
    },

    methods: {
        ...mapActions([
            'updateFile',
            'updateNewQuestionDialogState',
            'changeSelectPopUpUniState',
            'updateUploadFullMobile',
            'updateDialogState',
            'resetUploadData'
        ]),

        stopProgress(val) {
            return this.progressDone = val;
        },
        sendDocumentData(step) {
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
                        self.nextStep(step)
                    },
                    (error) => {
                        console.log('doc data error', error)
                    });

        },
        confirmCloseOpen() {
            if (this.currentStep === this.steps) {
                this.closeUpload()
            } else {
                this.confirmationDialog = true;
            }

        },
        closeUpload() {
            this.resetFirstStepMobile();
            this.resetUploadData({});
            this.updateDialogState(false);
            this.confirmationDialog = false;

        },
        nextStep(step) {
            if (this.currentStep === this.steps) {
                this.currentStep = 1
            } else {
                this.currentStep = this.currentStep + 1;
                this.stepsProgress = ((100 / 6) * this.currentStep);
            }
            console.log('step', this.stepsProgress, this.currentStep);

        },
        previousStep(step) {
            if (this.currentStep === 1) {
                return this.currentStep = 1;
            } else {
                this.currentStep = this.currentStep - 1;
                this.stepsProgress = ((100 / 6) * this.currentStep);
            }

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
}