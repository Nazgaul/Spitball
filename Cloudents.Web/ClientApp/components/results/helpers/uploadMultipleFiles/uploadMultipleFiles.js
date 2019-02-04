import { mapGetters, mapActions } from 'vuex';
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';
import documentService from "../../../../services/documentService";
import uploadFilesStart from "./helpers/uploadMultipleFileStart.vue";
import uploadStep_2 from "./helpers/filesDetails.vue";
import ulpoadStep_3 from"./helpers/documentReferral.vue"
import analyticsService from "../../../../services/analytics.service";
import uploadService from "../../../../services/uploadService";
import Base62 from "base62"

export default {
    components: {
        uploadFilesStart,
        uploadStep_2,
        ulpoadStep_3,
        sbDialog,
    },
    name: "uploadMultipleFiles",
    data() {
        return {
            confirmationDialog: false,
            progressDone: false,
            steps: 3,
            currentStep: 1,
            step: 1,
            callBackmethods: {
                next: this.nextStep,
                changeStep: this.changeStep,
                stopProgress: this.stopProgress,
            },
            docReferral: [],
            courseSelected: '',
            nextStepCalled: false
        }
    },

    computed: {
        ...mapGetters({
            accountUser: 'accountUser',
            getSelectedClasses: 'getSelectedClasses',
            getFileData: 'getFileData',
            getDialogState: 'getDialogState',
        }),
        isClassesSet() {
            return this.getSelectedClasses.length > 0
        },
        classesList() {
            if (this.isClassesSet) {
                return this.getSelectedClasses
            }
        },
        firstStep() {
            return this.currentStep === 1;
        },
        lastStep(){
            return this.currentStep === this.steps;
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
        goToNextStep() {
            if (!this.nextStepCalled) {
                this.nextStepCalled = true;
                this.nextStep(1);
            }
        },
        updateSelectedCourse() {
            this.setCourse(this.courseSelected)
        },
        sendDocumentData(step) {
            this.loading = true;
            let docData = this.getFileData;
            let self = this;
            docData.forEach((fileObj)=>{
                let serverFormattedObj = uploadService.createServerFileData(fileObj);
                documentService.sendDocumentData(serverFormattedObj)
                    .then((resp) => {
                            if (resp.data.url) {
                                self.docReferral.push(`${global.location.origin}` + resp.data.url + "?referral=" +
                                    Base62.encode(self.accountUser.id) + "&promo=referral");
                            }
                            analyticsService.sb_unitedEvent('STUDY_DOCS', 'DOC_UPLOAD_COMPLETE');
                            console.log('DOC_UPLOAD_COMPLETE')
                            self.loading = true;
                            self.goToNextStep()
                        },
                        (error) => {
                            console.log("doc data error", error)
                        });
            })


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

    },
    created() {

    }
}