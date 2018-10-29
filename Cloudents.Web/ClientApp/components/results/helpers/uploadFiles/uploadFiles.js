import { mapGetters, mapActions } from 'vuex';
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';
import Vue from 'vue';
import FileUpload from 'vue-upload-component/src';
import sbInput from "../../../question/helpers/sbInput/sbInput";
import referralDialog from "../../../question/helpers/referralDialog/referral-dialog.vue";

import { documentTypes, currencyValidator } from "./consts";
import sblCurrency from "./sbl-currency.vue"
//var VueUploadComponent = import('vue-upload-component');
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
            showUploadDialog: false,
            uploadUrl: "/api/upload/file",
            counter: 0,
            dbReady: false,
            progressValue: '',
            files: [],
            steps: 8,
            currentStep: 1,
            step: 1,
            stepsProgress: 100/8,
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
        }
    },
    props: {},
    computed: {
        ...mapGetters({
            accountUser: 'accountUser',
        }),
        progressShow() {
            if (this.progressValue === '100.00' && this.progressValue === '') {
                return false
            }

        },
        hideOnFirstStep() {
            this.currentStep === 1
        },
        isFirstStep() {
            return this.currentStep === 1
        },
        isDisabled(){
            if(this.currentStep === 2 && !this.schoolName || !this.selectedClass){
                return true
            }
            else if(this.currentStep === 3 && Object.keys(this.selectedDoctype).length === 0   ){
                return true
            }else if(this.currentStep === 4 && (!this.documentTitle || !this.proffesorName)){
                return true
            }else if(this.currentStep === 5 && this.selectedTags.length < 1){
                return true
            }else if(this.currentStep === 6 && !this.uploadPrice){
                return true
            }else if(this.currentStep === 7 && !this.legalCheck){
                return true
            }else{
                return false
            }

        }

    },

    methods: {
        ...mapActions(["updateLoginDialogState", 'updateUserProfileData']),

        openUploaderDialog() {
            if (this.accountUser == null) {
                this.updateLoginDialogState(true);
                //set user profile
                this.updateUserProfileData('profileHWH')
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
        sendDocumentData(step){
            console.log('sending data');
            //documentTitle if exists replace with custom before send
            this.nextStep(step)
        },
        updateLegal(){
          console.log('legal check',this.legalCheck)
        },
        loadDropBoxSrc() {
            // if exists prevent duplicate loading
            let isDbExists = !!document.getElementById('dropboxjs');
            if (isDbExists) {
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
                    this.files.splice(0, 1, singleFile);
                    this.nextStep(1)
                },
                cancel: function () {
                    //optional
                },
                linkType: "direct", // "preview" or "direct"
                multiselect: false, // true or false
                extensions: ['.png', '.jpg', 'doc', 'pdf'],
            };
            global.Dropbox.choose(options);
        },

        // regular upload methods
        inputFile(newFile, oldFile) {
            console.log('files regular two ');

            if (newFile && !oldFile) {
                // Add file
                this.nextStep(1)

            }
            // Upload progress
            if (newFile && newFile.progress) {
                this.progressValue = newFile.progress;
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
                    }else{
                        console.log('error, not uploaded')
                    }
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

                //if (!/\.(jpeg|jpe|jpg|gif|png|webp)$/i.test(newFile.name)) {
                //    return prevent()
                //}

                // Create the 'blob' field for thumbnail preview
                newFile.blob = ''
                let URL = window.URL || window.webkitURL;
                let type = 'fromDisk';
                if (URL && URL.createObjectURL) {
                    let singleFile = {
                        name: newFile.name,
                        link: URL.createObjectURL(newFile.file),
                        type: type
                    };
                    //add or replace
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
                this.stepsProgress =  ((100/8)* this.currentStep);

            }
            console.log('step', this.stepsProgress, this.currentStep);

        },
        previousStep(step) {
            if (this.currentStep === 1) {
                return this.currentStep = 1;

            } else {
                this.currentStep = this.currentStep - 1;
                this.stepsProgress = ((100/8)* this.currentStep);
            }

        },
        changeStep(step){
            //clean up everytnig for new doc upload
            if(step ===1){

            }
            this.currentStep = step;
        }
    },

    created() {
    }

}