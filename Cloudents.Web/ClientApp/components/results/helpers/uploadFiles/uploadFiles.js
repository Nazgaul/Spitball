import { mapGetters, mapActions } from 'vuex';
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';
import Vue from 'vue';
import FileUpload from 'vue-upload-component/src';
import sbInput from "../../../question/helpers/sbInput/sbInput";
import referralDialog from "../../../question/helpers/referralDialog/referral-dialog.vue";
import uploadService from "../../../../services/uploadService";
import documentService from "../../../../services/documentService";
import { documentTypes, currencyValidator } from "./consts";
import sblCurrency from "./sbl-currency.vue"
// var VueUploadComponent = import('vue-upload-component');
Vue.component('file-upload', FileUpload);

export default {
    components: {
        sbDialog,
        sbInput,
        sblCurrency,
        FileUpload,
        referralDialog
    },
    name: "uploadFiles",
    data() {
        return {
            uploadUrl: '/api/upload/file',
            showUploadDialog: false,
            uploadUrl: "/api/upload/file",
            counter: 0,
            dbReady: false,
            progressDone: false,
            files: [],
            generatedFileName: '',
            steps: 8,
            currentStep: 1,
            step: 1,
            stepsProgress: 100 / 8,
            schoolName: '',
            classesList: ['Social Psych', 'behaviourl psych', 'Biology 2', 'Biology 3', 'behaviourl psych2'],
            selectedClass: '',
            documentTypes: documentTypes,
            selectedDoctype: {},
            documentTitle: '',
            proffesorName: '',
            selectedTags: [],
            tagsOptions: [, 'behaviourl', 'Biology', 'Math', 'History'],
            uploadPrice: null,
            legalCheck: false,
            gotoAsk: false,
            transitionAnimation: 'slide-y-transition'
        }
    },
    props: {},

    computed: {
        ...mapGetters({
            accountUser: 'accountUser',

        }),
        progressShow() {
            return this.progress > 0 < 100 && !this.progressDone
        },
        progress() {
            console.log(this.files[0] ? this.files[0].progress : 0);
            return this.files[0] ? this.files[0].progress : 0
        },
        isFirstStep() {
            return this.currentStep === 1
        },
        // button disabled for each step and enabled once everything filled
        isDisabled() {
            if (this.currentStep === 2 && !this.schoolName || !this.selectedClass) {
                return true
            }
            else if (this.currentStep === 3 && Object.keys(this.selectedDoctype).length === 0) {
                return true
            } else if (this.currentStep === 4 && (!this.documentTitle || !this.proffesorName)) {
                return true
            } else if (this.currentStep === 5 && this.selectedTags.length < 1) {
                return true
            } else if (this.currentStep === 6 && !this.uploadPrice) {
                return true
            } else if (this.currentStep === 7 && !this.legalCheck) {
                return true
            } else {
                return false
            }

        }

    },

    methods: {
        ...mapActions(["updateLoginDialogState",  'updateNewQuestionDialogState']),

        openUploaderDialog() {
            if (this.accountUser == null) {
                this.updateLoginDialogState(true);
            } else {
                this.loadDropBoxSrc(); // load Drop box script
                this.showUploadDialog = true;
            }
        },

        // update data methods
        updateClass(singleClass) {
            this.selectedClass = singleClass;
        },
        updateDocumentType(docType) {
            this.selectedDoctype = docType;
            console.log(this.selectedDoctype)
        },
        removeTag(item) {
            this.selectedTags.splice(this.selectedTags.indexOf(item), 1)
            this.selectedTags = [...this.selectedTags]
        },
        sendDocumentData(step) {
            console.log('sending data');
            //documentTitle if exists replace with custom before send
            let file = this.files[0];
            console.log('FILE:: SEND::', file)
            documentService.sendDocumentData(file);
            this.nextStep(step)
        },
        updateLegal() {
            console.log('legal check', this.legalCheck)
        },
        closeAndOpenAsk(){
            this.gotoAsk= true;
            this.showUploadDialog = false;
        },

        loadDropBoxSrc() {
            // if exists prevent duplicate loading
            let isDbExists = !!document.getElementById('dropboxjs');
            if (isDbExists) {
                this.dbReady = true;
                return
            }
            //if didnt exist before
            let dbjs = document.createElement('script');
            dbjs.id = "dropboxjs";
            dbjs.async = false;
            dbjs.setAttribute('data-app-key', 'mii3jtxg6616y9g');
            dbjs.src = "https://www.dropbox.com/static/api/1/dropins.js";
            document.getElementsByTagName('head')[0].appendChild(dbjs);
            dbjs.onload = () => {
                this.dbReady = true; // enable dropbox upload btn when script is ready
            }
        },

        DbFilesList() {
            var self = this;
            let options = {
                success: (files) => {
                    let type = 'dropBox';
                    let singleFile;
                    //clean if was trying to upload from desktop before
                    files.forEach((file) => {
                        singleFile = {
                            name: file.name,
                            link: file.link,
                            size: file.bytes,
                            type: type
                        };
                        // add to array or replace
                    });
                    this.documentTitle = singleFile.name ?  singleFile.name : '';
                    uploadService.uploadDropbox(singleFile)
                        .then((response) => {
                                console.log("success responce ulpoad drop box api call", response);
                                this.files.splice(0, 1, singleFile);
                                this.progressDone = true;
                                this.generatedFileName = response.data.fileName ? response.data.fileName : '';
                                this.nextStep(1)
                            },
                            error => {
                                console.log('error drop box api call', error)
                            })
                },
                cancel: function () {
                    //optional
                },
                linkType: "direct", // "preview" or "direct"
                multiselect: false, // true or false
                extensions: ['.doc', '.pdf', '.png', '.jpg', '.docx', '.xls', '.xlsx', '.ppt', '.jpeg'],
            };
            global.Dropbox.choose(options);
        },

        // regular upload methods
        inputFile(newFile, oldFile) {
            //happnes once file is added and upload starts
            if (newFile && !oldFile) {
                // Add file
                this.nextStep(1)
            }
            // Upload progress
            if (newFile && newFile.progress) {
                this.progressDone = newFile.progress === 100;
                console.log('progress', newFile.progress, newFile)
            }
            // Upload error
            if (newFile && oldFile && newFile.error !== oldFile.error) {
                console.log('error', newFile.error, newFile)
            }
            // Get response data
            if (newFile && oldFile && !newFile.active && oldFile.active) {
                // Get response data
                console.log('response', newFile.response);
                if (newFile.xhr) {
                    //  Get the response status code
                    console.log('status', newFile.xhr.status)
                    if (newFile.xhr.status === 200) {
                        console.log('Succesfully uploadede')
                    } else {
                        console.log('error, not uploaded')
                    }
                }
                if (newFile && newFile.response && newFile.response.status === 'success') {
                    //add or replace
                    let name = newFile.response.fileName;
                    this.generatedFileName = `${name}`
                }
            }
            if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
                if (!this.$refs.upload.active) {
                    this.$refs.upload.active = true
                }
            }
        },
        inputFilter(newFile, oldFile, prevent) {
            if (newFile && !oldFile) {
                // Add file
                // Filter non-image file remove for docs
                // Will not be added to files
                if (/\.(js|html|php|webp|exe)$/i.test(newFile.name)) {
                   return prevent()
                }

                // Create the 'blob' field for thumbnail preview
                // create file object  in filter before upload starts
                newFile.blob = '';
                let URL = window.URL || window.webkitURL;
                let type = 'fromDisk';
                if (URL && URL.createObjectURL) {
                    let singleFile = {
                        name: newFile.name,
                        link: URL.createObjectURL(newFile.file),
                        type: type
                    };
                    //add or replace
                    this.documentTitle = singleFile.name ?  singleFile.name : '';
                    this.files.splice(0, 1, singleFile)
                }
            }
            if (newFile && oldFile) {
                // Update file
                // Increase the version number
                if (!newFile.version) {
                    newFile.version = 0
                }
                newFile.version++
            }
            if (!newFile && oldFile) {
                // Remove file
                // Refused to remove the file
                // return prevent()
            }
        },
        nextStep(step) {
            console.log('files', this.files)
            if (this.currentStep === this.steps) {
                this.currentStep = 1
            } else {
                this.currentStep = this.currentStep + 1;
                this.stepsProgress = ((100 / 8) * this.currentStep);

            }
            console.log('step', this.stepsProgress, this.currentStep);

        },
        previousStep(step) {
            if (this.currentStep === 1) {
                return this.currentStep = 1;

            } else {
                this.currentStep = this.currentStep - 1;
                this.stepsProgress = ((100 / 8) * this.currentStep);
            }

        },
        changeStep(step) {
            //clean up everytnig for new doc upload
            if (step === 1) {

            }
            this.currentStep = step;
        }
    },
    beforeDestroy(){
        if(this.gotoAsk){
            this.updateNewQuestionDialogState(true);
        }

    },
    created() {
    }

}