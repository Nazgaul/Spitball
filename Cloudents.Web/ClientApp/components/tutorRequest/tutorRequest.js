import { mapActions, mapGetters, mapMutations } from 'vuex';
import UserAvatar from '../helpers/UserAvatar/UserAvatar.vue';
import FileUpload from 'vue-upload-component/src'; //docs here https://lian-yue.github.io/vue-upload-component
import tutorService from "../../services/tutorService";
import { LanguageService } from "../../services/language/languageService";
import { validationRules } from "../../services/utilities/formValidationRules";
import analyticsService from '../../services/analytics.service';

export default {
    components: {
        UserAvatar,
        FileUpload
    },
    data() {
        return {
            tutorCourse: '',
            tutorRequestText: '',
            btnRequestLoading: false,
            validRequestTutorForm: false,
            guestName: '',
            guestMail: '',
            guestPhone: '',
            rules: {
                required: (value) => validationRules.required(value),
                maximumChars: (value) => validationRules.maximumChars(value, 255)
            },
            coursePlaceholder: LanguageService.getValueByKey("tutorRequest_select_course_placeholder"),
            topicPlaceholder: LanguageService.getValueByKey("tutorRequest_topic_placeholder"),
            guestNamePlaceHolder : LanguageService.getValueByKey("tutorRequest_name"),
            guestEmailPlaceHolder : LanguageService.getValueByKey("tutorRequest_email"),
            guestPhoneNumberPlaceHolder : LanguageService.getValueByKey("tutorRequest_phoneNumber"),
            uploadProp: {
                populatedThumnbailBox: {
                    box_0: {
                        populated: false,
                        src: ''
                    },
                    box_1: {
                        populated: false,
                        src: ''
                    },
                    box_2: {
                        populated: false,
                        src: ''
                    },
                    box_3: {
                        populated: false,
                        src: ''
                    },
                },
                componentUniqueId: `instance-${this._uid}`,
                extensions: ['jpeg', 'jpe', 'jpg', 'gif', 'png', 'webp'],
                uploadUrl: "/api/Tutor/request/upload",
                uploadedFiles: [],
                uploadedFileNames: [],
                MAX_FILES_AMOUNT: 4,
            },
        };
    },
    computed: {
        ...mapGetters(['accountUser', 'getSelectedClasses', 'getRequestTutorDialog']),
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        userImageUrl() {
            if(this.isAuthUser && this.accountUser.image.length > 1) {
                return `${this.accountUser.image}`;
            }
            return '';
        },
        userName() {
            if (this.isAuthUser) {
            return this.accountUser.name
            }
            else {
                return 'JD';
            }
        },
        isAuthUser(){
            return !!this.accountUser;
        }
    },
    methods: {
        ...mapActions(['updateRequestDialog', 'updateToasterParams']),
        ...mapMutations(['UPDATE_LOADING']),
        sendRequest() {
            
            let self = this;
            if(self.$refs.tutorRequestForm.validate()) {
                if(this.isAuthUser){
                    self.btnRequestLoading = true;
                    let serverObj = {
                        course: self.tutorCourse,
                        text: self.tutorRequestText,
                        files: self.uploadProp.uploadedFileNames
                    };
                    let analyticsObject = {
                        userId: self.accountUser.id,
                        course: self.tutorCourse
                    }
                    analyticsService.sb_unitedEvent('Action Box', 'Request_T', `USER_ID:${analyticsObject.userId}, T_Course:${analyticsObject.course}`);
                    tutorService.requestTutor(serverObj)
                                .then(() => {
                                          self.tutorRequestDialogClose();
                                          self.updateToasterParams({
                                            toasterText: LanguageService.getValueByKey("tutorRequest_request_received"),
                                            showToaster: true,
                                          })
                                      },
                                      () => {
                                      }).finally(() => {
                        self.btnRequestLoading = false;
                    });
                }else{
                    self.btnRequestLoading = true;
                    let serverObj = {
                        text: self.tutorRequestText,
                        files: self.uploadProp.uploadedFileNames,
                        name: self.guestName,
                        mail: self.guestMail,
                        phone: self.guestPhone
                    };
                    
                    tutorService.requestTutorAnonymous(serverObj)
                                .then(() => {
                                          self.tutorRequestDialogClose();
                                          self.updateToasterParams({
                                            toasterText: LanguageService.getValueByKey("tutorRequest_request_received"),
                                            showToaster: true,
                                          })
                                      },
                                      () => {
                                      }).finally(() => {
                        self.btnRequestLoading = false;
                    });
                }
            }
        },

        tutorRequestDialogClose() {
            this.updateRequestDialog(false);
        },
        openUploadInterface() {
            let uploadFileElement = document.querySelector(`#${this.uploadProp.componentUniqueId}`);
            uploadFileElement.click();
        },
        removeImage(img) {
            img.populated = false;
            //remove file
            this.uploadProp.uploadedFiles.forEach((file, index) => {
                if(file.blob == img.src) {
                    this.uploadProp.uploadedFiles.splice(index, 1);
                }
            });
            //remove file from filenames array
            let filenameIndex = this.uploadProp.uploadedFileNames.indexOf(img.fileName);
            if(filenameIndex > -1) {
                this.uploadProp.uploadedFileNames.splice(filenameIndex, 1);
            }
        },
        populatThumbnailBoxes() {
            this.uploadProp.uploadedFiles.forEach((file, index) => {
                this.uploadProp.populatedThumnbailBox[`box_${index}`].populated = true;
                this.uploadProp.populatedThumnbailBox[`box_${index}`].src = file.blob;
                this.uploadProp.populatedThumnbailBox[`box_${index}`].fileName = this.uploadProp.uploadedFileNames[index];
            });
        },
        inputFile: function (newFile, oldFile) {
            let self = this;
            if(self.uploadProp.uploadedFiles && self.uploadProp.uploadedFiles.length > this.uploadProp.MAX_FILES_AMOUNT) {
                return;
            }
            if(newFile && oldFile && !newFile.active && oldFile.active) {
                // Get response data
                // console.log('response', newFile.response);
                if(newFile.xhr) {
                    //  Get the response status code
                    // console.log('status', newFile.xhr.status)
                    if(newFile.xhr.status === 200) {
                        // console.log('Succesfully uploadede')
                        //on after successful loading done, emit to parent to add to list
                        if(newFile.response && newFile.response.files) {
                            //self.$emit('addFile', newFile.response.files);
                            let filename = newFile.response.files;
                            //if max reached replace
                            if(this.uploadProp.uploadedFileNames && this.uploadProp.uploadedFileNames.length > this.uploadProp.MAX_FILES_AMOUNT) {
                                this.uploadProp.uploadedFileNames.splice(-1, 1, filename);
                                //add
                            } else {
                                this.uploadProp.uploadedFileNames = this.uploadProp.uploadedFileNames.concat(filename);
                            }
                            this.populatThumbnailBoxes();
                        }
                    }
                }
            }
            if(Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
                if(!this.$refs.upload.active) {
                    this.$refs.upload.active = true;
                }
            }
        },
        inputFilter: function (newFile, oldFile, prevent) {
            if(newFile && !oldFile) {
                //prevent adding new files if maximum reached
                if(this.uploadProp.uploadedFiles.length >= this.uploadProp.MAX_FILES_AMOUNT) {
                    return prevent();
                }
                // Filter non-supported extensions  both lower and upper case
                let patt1 = /\.([0-9a-z]+)(?:[\?#]|$)/i;
                let ext = (`${newFile.name}`.toLowerCase()).match(patt1)[1];
                let isSupported = this.uploadProp.extensions.includes(ext);
                if(!isSupported) {
                    return prevent();
                }
                if(newFile && newFile.size === 0) {
                    return prevent();
                }
            }
            if(newFile && (!oldFile || newFile.file !== oldFile.file)) {
                // Create a blob field
                newFile.blob = '';
                let URL = window.URL || window.webkitURL;
                if(URL && URL.createObjectURL) {
                    newFile.blob = URL.createObjectURL(newFile.file);
                }
            }
        }
    }
};