import { mapActions, mapGetters, mapMutations } from 'vuex';
import UserAvatar from '../../helpers/UserAvatar/UserAvatar.vue';
import FileUpload from 'vue-upload-component/src'; //docs here https://lian-yue.github.io/vue-upload-component
import { LanguageService } from "../../../services/language/languageService";
import { validationRules } from "../../../services/utilities/formValidationRules";
import questionService from "../../../services/questionService";
import analyticsService from "../../../services/analytics.service";

export default {
    components: {
        UserAvatar,
        FileUpload
    },
    data() {
        return {
            questionCourse: '',
            questionText: '',
            btnQuestionLoading: false,
            validQuestionForm: false,
            errorMessage: '',
            rules: {
                required: (value) => validationRules.required(value),
                maximumChars: (value) => validationRules.maximumChars(value, 255),
                minimumChars: (value) => validationRules.minimumChars(value, 15)
            },
            coursePlaceholder: LanguageService.getValueByKey("addQuestion_class_placeholder"),
            topicPlaceholder: LanguageService.getValueByKey("addQuestion_ask_your_question_placeholder"),
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
                    }
                },
                componentUniqueId: `instance-${this._uid}`,
                extensions: ['jpeg', 'jpe', 'jpg', 'gif', 'png', 'webp'],
                uploadUrl: "/api/Question/ask",
                uploadedFiles: [],
                uploadedFileNames: [],
                MAX_FILES_AMOUNT: 4
            }
        };
    },
    computed: {
        ...mapGetters(['accountUser', 'getSelectedClasses', 'newQuestionDialogSate']),
        userImageUrl() {
            if(this.accountUser.image.length > 1) {
                return `${this.accountUser.image}`;
            }
            return '';
        }
    },
    methods: {
        ...mapActions(['updateNewQuestionDialogState']),
        ...mapMutations(['UPDATE_LOADING']),
        requestAskClose() {
            this.updateNewQuestionDialogState(false);
        },
        submitQuestion() {
            let self = this;
            if(self.$refs.questionForm.validate()) {
                self.btnQuestionLoading =true;
                self.UPDATE_LOADING(true);
                let serverQuestionObj = {
                    text:self.questionText,
                    course : self.questionCourse,
                    files:self.uploadProp.uploadedFileNames
                };
                questionService.postQuestion(serverQuestionObj).then(() => {
                    analyticsService.sb_unitedEvent("Submit_question", "Homework help");
                    self.btnQuestionLoading =false;
                    //close dialog after question submitted
                    self.requestAskClose(false);
                    ;
                    self.$router.push({
                                          path: '/ask'
                                        //   query: {
                                        //       term: ''
                                        //   }
                                      });
                    self.UPDATE_LOADING(false);
                }, (error) => {
                    let errorMessage = LanguageService.getValueByKey('addQuestion_error_general');
                    if (error && error.response && error.response.data && error.response.data[""] && error.response.data[""][0]) {
                        errorMessage = error.response.data[""][0];
                    }
                    self.errorMessage = errorMessage;
                    self.UPDATE_LOADING(false);
                    console.error(error);
                }).finally(()=>{
                    self.btnQuestionLoading =false;
                });
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
                if(newFile.size === 0) {
                    return prevent();
                }
            }
            if(newFile && (!oldFile || newFile.file !== oldFile.file)) {
                // Create a blob field
                newFile.blob = '';
                let url = window.URL || window.webkitURL;
                if(url && url.createObjectURL) {
                    newFile.blob = url.createObjectURL(newFile.file);
                }
            }
        }
    },
    created() {
        if(this.$route.query && this.$route.query.Course){
            this.questionCourse = this.$route.query.Course;
        }
    }
};