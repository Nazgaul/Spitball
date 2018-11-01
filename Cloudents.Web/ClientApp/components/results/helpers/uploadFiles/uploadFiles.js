import { mapGetters, mapActions } from 'vuex';
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';

import documentService from "../../../../services/documentService";
import uploadStep_1 from "./uploadSteps/uploadStep_1.vue";
import uploadStep_2 from "./uploadSteps/uploadStep_2.vue";
import uploadStep_3 from "./uploadSteps/uploadStep_3.vue";
import uploadStep_4 from "./uploadSteps/uploadStep_4.vue";
import uploadStep_5 from "./uploadSteps/uploadStep_5.vue";
import uploadStep_6 from "./uploadSteps/uploadStep_6.vue";
import uploadStep_7 from "./uploadSteps/uploadStep_7.vue";


export default {
    components: {
        uploadStep_1,
        uploadStep_2,
        uploadStep_3,
        uploadStep_4,
        uploadStep_5,
        uploadStep_6,
        uploadStep_7,
        sbDialog,
    },
    name: "uploadFiles",
    data() {
        return {

            progressDone: false,
            steps: 7,
            currentStep: 1,
            step: 1,
            stepsProgress: 100 / 7,
            gotoAsk: false,
            transitionAnimation: 'slide-y-transition',
            callBackmethods: {
                next: this.nextStep,
                changeStep: this.changeStep,
                stopProgress: this.stopProgress,
                // closeAndOpenAsk: this.closeAndOpenAsk
            }
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
            getDialogState: 'getDialogState'
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
        // button disabled for each step and enabled once everything filled
        isDisabled() {
            if (this.currentStep === 2 && !this.getFileData.courses) {
                return true
            }
            else if (this.currentStep === 3 && Object.keys(this.getFileData.type).length === 0) {
                return true
            } else if (this.currentStep === 4 && (!this.getFileData.name || !this.getFileData.proffesorName)) {
                return true
            } else if (this.currentStep === 5 && this.getFileData.tags.length < 1) {
                return true
            }
            // else if (this.currentStep === 6 && !this.uploadPrice) {
            //     return true
            // }
            else if (this.currentStep === 6 && !this.getLegal) {
                return true
            } else {
                return false
            }
        }
    },

    methods: {
        ...mapActions([
            'updateFile',
            'updateLoginDialogState',
            'updateNewQuestionDialogState',
            'changeSelectPopUpUniState',
            'syncUniData',
            'updateDialogState'
        ]),

        stopProgress(val) {
            return this.progressDone = val;
        },
        sendDocumentData(step) {
            let docData = this.getFileData;
            //post all doc data
            documentService.sendDocumentData(docData)
                .then((resp) => {
                        console.log('doc data success', resp);
                        this.nextStep(step)
                    },
                    (error) => {
                        console.log('doc data error', error)
                    });

        },
        closeDialog() {
            this.updateDialogState(false)
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
        changeStep(step) {
            //clean up everytnig for new doc upload
            if (step === 1) {
            }
            this.currentStep = step;
        }
    },
    created() {
        this.syncUniData()

    }

}