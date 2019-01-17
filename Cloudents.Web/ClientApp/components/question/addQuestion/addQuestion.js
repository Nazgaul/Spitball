import { mapActions, mapGetters } from 'vuex'
import UserAvatar from '../../helpers/UserAvatar/UserAvatar.vue'
import questionService from '../../../services/questionService'
import FileUpload from 'vue-upload-component/src'; //docs here https://lian-yue.github.io/vue-upload-component
import addQuestionUtilities from './addQuestionUtilities'
import QuestionRegular from './helpers/question-regular.vue'

export default {
    data() {
        return {
            questionMessage: '',
            questionSubjct: '',
            questionClass: '',
            subjectList: [],
            addQuestionValidtionObj: {
                errors:{
                    textArea:{},
                    subject:{}
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
                     },
                },
                componentUniqueId: `instance-${this._uid}`,
                extensions: ['jpeg', 'jpe', 'jpg', 'gif', 'png', 'webp'],
                uploadUrl: "/api/upload/ask",
                uploadedFiles: [],
                uploadedFileNames: [],
                MAX_FILES_AMOUNT: 4,
            }
        }
    },
    components: {
        UserAvatar,
        QuestionRegular,
        FileUpload
    },
    computed: {
        ...mapGetters(['accountUser', 'getSelectedClasses', 'newQuestionDialogSate']),
        hasTextAreaError(){
            return !!this.addQuestionValidtionObj.errors['textArea'] && this.addQuestionValidtionObj.errors['textArea'].hasError
        },
        hasSubjectError(){
            return !!this.addQuestionValidtionObj.errors['subject'] && this.addQuestionValidtionObj.errors['subject'].hasError
        },
        hasExternalError(){
            return !!this.currentComponentselected.showError
        }

    },
    watch: {
        questionMessage(){
            this.addQuestionValidtionObj.errors['textArea'] = {}
        },
        questionSubjct(){
            this.addQuestionValidtionObj.errors['subject'] = {}
        },
        newQuestionDialogSate: {
            immediate: true,
            handler(val) {
                if (val) {
                    // get subject if questionDialog state is true(happens only if accountUser is true)
                    questionService.getSubjects().then((response) => {
                        this.subjectList = response.data
                    });
                }
            },
        },
    },
    methods: {
        ...mapActions(['updateNewQuestionDialogState']),
        requestNewQuestionDialogClose() {
            this.updateNewQuestionDialogState(false)
        },
        openUploadInterface(){
            let uploadFileElement = document.querySelector(`#${this.uploadProp.componentUniqueId}`);
            uploadFileElement.click();
        },
        resetErrorObject(){
            //should be the same as the data object
            this.addQuestionValidtionObj.errors = {
                textArea:{},
                subject:{}
            };
        },
        canAddQuestion(){
            let canAddQuestion = true;
            this.resetErrorObject();
            let trimmedMessage = this.questionMessage.trim();
            let externalComponent = this.currentComponentselected.returnedObj;

            if(trimmedMessage.length < 15){
                const message = 'There is a minimum of 15 characters for a question';
                let errorObj = addQuestionUtilities.createErrorObj(true, message)
                this.addQuestionValidtionObj.errors['textArea'] = errorObj
                canAddQuestion = false;
                
            }if(!this.questionSubjct){
                const message = 'Don’t forget to select the subject for your question';
                let errorObj = addQuestionUtilities.createErrorObj(true, message)
                this.addQuestionValidtionObj.errors['subject'] = errorObj
                canAddQuestion = false;
            }
            if(!!externalComponent.hasError){
                this.currentComponentselected.showError = true;
                canAddQuestion = false;
            }
            return canAddQuestion;
        },
        addQuestion(){
            if(this.canAddQuestion()){
                console.log('add question');
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
        removeImage(img){
            img.populated = false;
            //remove file
            this.uploadProp.uploadedFiles.forEach((file, index)=>{
                if(file.blob == img.src){
                    this.uploadProp.uploadedFiles.splice(index, 1);
                }
            })
            //remove file from filenames array
            let filenameIndex = this.uploadProp.uploadedFileNames.indexOf(img.fileName)
            if(filenameIndex > -1){
                this.uploadProp.uploadedFileNames.splice(filenameIndex, 1);
            }
        },
        populatThumbnailBoxes(){
            this.uploadProp.uploadedFiles.forEach((file, index)=>{
                this.uploadProp.populatedThumnbailBox[`box_${index}`].populated = true;
                this.uploadProp.populatedThumnbailBox[`box_${index}`].src = file.blob;
                this.uploadProp.populatedThumnbailBox[`box_${index}`].fileName = this.uploadProp.uploadedFileNames[index];
            })
        },
        inputFile: function (newFile, oldFile) {
            let self = this;
            if (self.uploadProp.uploadedFiles && self.uploadProp.uploadedFiles.length > this.uploadProp.MAX_FILES_AMOUNT) {
                return
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
                    this.$refs.upload.active = true
                }
            }
        },
        inputFilter: function (newFile, oldFile, prevent) {
            if (newFile && !oldFile) {
                //prevent adding new files if maximum reached
                if (this.uploadProp.uploadedFiles.length >= this.uploadProp.MAX_FILES_AMOUNT) {
                    return prevent()
                }
                // Filter non-supported extensions  both lower and upper case
                let patt1 = /\.([0-9a-z]+)(?:[\?#]|$)/i;
                let ext = (`${newFile.name}`.toLowerCase()).match(patt1)[1];
                let isSupported = this.uploadProp.extensions.includes(ext);
                if (!isSupported) {
                    return prevent()
                }
                if (newFile && newFile.size === 0) {
                    return prevent()
                }
            }
            if (newFile && (!oldFile || newFile.file !== oldFile.file)) {
                // Create a blob field
                newFile.blob = '';
                let URL = window.URL || window.webkitURL;
                if (URL && URL.createObjectURL) {
                    newFile.blob = URL.createObjectURL(newFile.file);
                }
            }
        }
    }
}