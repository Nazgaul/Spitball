import {
    mapActions,
    mapGetters,
    mapMutations
} from 'vuex'
import UserAvatar from '../../helpers/UserAvatar/UserAvatar.vue'
import questionService from '../../../services/questionService'
// import FileUpload from 'vue-upload-component/src'; //docs here https://lian-yue.github.io/vue-upload-component
import addQuestionUtilities from './addQuestionUtilities'
// import QuestionRegular from './helpers/question-regular.vue'
import {LanguageService} from "../../../services/language/languageService";

export default {
    components: {
        UserAvatar,
        // QuestionRegular,
        // FileUpload
    },
    data() {
        return {
            addQuestionButtonLoading: false,
            questionMessage: '',
            questionSubjct: '',
            questionClass: '',
            subjectList: [],
            addQuestionValidtionObj: {
                errors: {
                    textArea: {},
                    class: {},
                    server: {}
                }
            },
            currentComponentselected: {
                name: "regular",
                callback: this.handleResult,
                returnedObj: {},
                showError: false
            },
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
            },
            dictionary:{
                askPlaceholder: LanguageService.getValueByKey('addQuestion_ask_your_question_placeholder'),
                selectSubjectPlaceholder: LanguageService.getValueByKey('addQuestion_select_subject_placeholder'),
                classPlaceholder: LanguageService.getValueByKey('addQuestion_class_placeholder')
            }
        };
    },
    computed: {
        ...mapGetters(['accountUser', 'getSelectedClasses', 'newQuestionDialogSate']),
        hasTextAreaError() {
            return !!this.addQuestionValidtionObj.errors['textArea'] && this.addQuestionValidtionObj.errors['textArea'].hasError;
        },
        hasClassError() {
            return !!this.addQuestionValidtionObj.errors['class'] && this.addQuestionValidtionObj.errors['class'].hasError;
        },
        hasExternalError() {
            return !!this.currentComponentselected.showError;
        },
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        userImageUrl(){
            if(this.accountUser.image.length > 1){
                return `${this.accountUser.image}`;
            }
            return '';
        }
    },
    watch: {
        questionMessage() {
            this.addQuestionValidtionObj.errors['textArea'] = {};
        },
        questionClass() {
            this.addQuestionValidtionObj.errors['class'] = {};
        },
        newQuestionDialogSate: {
            immediate: true,
            handler(val) {
                if (val) {
                    // get subject if questionDialog state is true(happens only if accountUser is true)
                    questionService.getSubjects().then((response) => {
                        this.subjectList = response.data;
                    });
                }
            }
        }
    },
    methods: {
        ...mapActions(['updateNewQuestionDialogState','updateAnalytics_unitedEvent']),
        ...mapMutations(['UPDATE_LOADING']),
        requestNewQuestionDialogClose() {
            this.updateNewQuestionDialogState(false);
        },
        openUploadInterface() {
            let uploadFileElement = document.querySelector(`#${this.uploadProp.componentUniqueId}`);
            uploadFileElement.click();
        },
        resetErrorObject() {
            //should be the same as the data object
            this.addQuestionValidtionObj.errors = {
                textArea: {},
                class: {},
                server: {}
            };
        },
        canAddQuestion() {
            let canAddQuestion = true;
            this.resetErrorObject();
            let trimmedMessage = this.questionMessage.trim();
            let externalComponent = this.currentComponentselected.returnedObj;
            if (trimmedMessage.length < 15) {
                const message = LanguageService.getValueByKey('addQuestion_error_minimum_chars');
                let errorObj = addQuestionUtilities.createErrorObj(true, message);
                this.addQuestionValidtionObj.errors['textArea'] = errorObj;
                canAddQuestion = false;

            }
            if (!this.questionClass) {
                const message = LanguageService.getValueByKey('addQuestion_error_select_class');
                let errorObj = addQuestionUtilities.createErrorObj(true, message);
                this.addQuestionValidtionObj.errors['class'] = errorObj;
                canAddQuestion = false;
            }
            if (!!externalComponent.hasError) {
                this.currentComponentselected.showError = true;
                canAddQuestion = false;
            }
            return canAddQuestion;
        },
        addQuestion() {
            if (this.canAddQuestion()) {
                this.addQuestionButtonLoading = true;
                console.log('add question');
                this.UPDATE_LOADING(true);
                let serverQuestionObj = {
                    text:this.questionMessage,
                    subjectId:this.questionSubjct,
                    course : this.questionClass,
                    files:this.uploadProp.uploadedFileNames
                };
                this.updateAnalytics_unitedEvent(['Action Box', 'Ask_Q', `USER_ID:${this.accountUser.id}, Q_COURSE:${serverQuestionObj.course}`]);
                questionService.postQuestion(serverQuestionObj).then(() => {
                    // let val = self.selectedPrice || this.price;
                    // this.updateUserBalance(-val);
                    //close dialog after question submitted
                    this.requestNewQuestionDialogClose(false);
                    this.$router.push({
                        path: '/ask'
                        // query: {
                        //     term: ''
                        // }
                    });
                    this.UPDATE_LOADING(false);
                }, (error) => {
                    let errorMessage = LanguageService.getValueByKey('addQuestion_error_general');
                    if (error && error.response && error.response.data && error.response.data[""] && error.response.data[""][0]) {
                        errorMessage = error.response.data[""][0];
                    }
                    this.UPDATE_LOADING(false);
                    this.addQuestionValidtionObj.errors.server = addQuestionUtilities.createErrorObj(true, errorMessage);
                    console.error(error);
                }).finally(()=>{
                    this.addQuestionButtonLoading = false;
                });
            }
        },
        handleResult(obj) {
            /*
                obj = {
                    hasError: boolean,
                    message: string,
                    result: number
                }
            */
            this.currentComponentselected.showError = false;
            this.currentComponentselected.returnedObj = obj;
        },
        removeImage(img) {
            img.populated = false;
            //remove file
            this.uploadProp.uploadedFiles.forEach((file, index) => {
                if (file.blob == img.src) {
                    this.uploadProp.uploadedFiles.splice(index, 1);
                }
            });
            //remove file from filenames array
            let filenameIndex = this.uploadProp.uploadedFileNames.indexOf(img.fileName);
            if (filenameIndex > -1) {
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
            if (self.uploadProp.uploadedFiles && self.uploadProp.uploadedFiles.length > this.uploadProp.MAX_FILES_AMOUNT) {
                return;
            }
            if (newFile && oldFile && !newFile.active && oldFile.active) {
                // Get response data
                // console.log('response', newFile.response);
                if (newFile.xhr) {
                    //  Get the response status code
                    // console.log('status', newFile.xhr.status)
                    if (newFile.xhr.status === 200) {
                        // console.log('Succesfully uploadede')
                        //on after successful loading done, emit to parent to add to list
                        if (newFile.response && newFile.response.files) {
                            //self.$emit('addFile', newFile.response.files);
                            let filename = newFile.response.files;
                            //if max reached replace
                            if (this.uploadProp.uploadedFileNames && this.uploadProp.uploadedFileNames.length > this.uploadProp.MAX_FILES_AMOUNT) {
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
            if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
                if (!this.$refs.upload.active) {
                    this.$refs.upload.active = true;
                }
            }
        },
        inputFilter: function (newFile, oldFile, prevent) {
            if (newFile && !oldFile) {
                //prevent adding new files if maximum reached
                if (this.uploadProp.uploadedFiles.length >= this.uploadProp.MAX_FILES_AMOUNT) {
                    return prevent();
                }
                // Filter non-supported extensions  both lower and upper case
                let patt1 = /\.([0-9a-z]+)(?:[\?#]|$)/i;
                let ext = (`${newFile.name}`.toLowerCase()).match(patt1)[1];
                let isSupported = this.uploadProp.extensions.includes(ext);
                if (!isSupported) {
                    return prevent();
                }
                if (newFile.size === 0) {
                    return prevent();
                }
            }
            if (newFile && (!oldFile || newFile.file !== oldFile.file)) {
                // Create a blob field
                newFile.blob = '';
                let url = window.URL || window.webkitURL;
                if (url && url.createObjectURL) {
                    newFile.blob = url.createObjectURL(newFile.file);
                }
            }
        }
    }
}