<template>
    <v-flex xs12>         
        <v-icon @click="closeUpload()" class="uf-close" v-html="'sbf-close'" />
        <v-card class="uf-main elevation-0">
            <v-stepper class="uf-mStepper elevation-0" v-model="currentStep" >
                
                <v-stepper-header :class="['uf-mHeader','elevation-0',isMobile?'pt-2' :'pl-3']">
                    <template>
                        <img v-if="!isMobile" class="uf-mImg" :src="userImage" alt="">
                        <h2 :class="['uf-mTitle',{'ml-3':!isMobile}]" v-language:inner="'upload_uf_mTitle'"/>
                    </template>
                </v-stepper-header>

                <v-stepper-items class="uf-items">
                    <div v-if="errorFile && errorFile.name" class="px-3">
                        <fileCardError :fileItem="errorFile" :singleFileIndex="0"/>
                    </div>
                    <v-stepper-content :class="['uf-mStepper-content', `step-${n}`]"
                                       v-for="n in steps"
                                       :key="`${n}-content`"
                                       :step="n">

                        <upload-files-start 
                            v-show="n===1" 
                            :curStep="1" 
                            :callBackmethods="callBackmethods">
                        </upload-files-start>
                            <transition name="slide">
                                <uploadStep_2 :chackValidation="chackValidation" v-show="n===2" :curStep="2" :callBackmethods="callBackmethods"></uploadStep_2>
                            </transition>
                    </v-stepper-content>
                </v-stepper-items>
            </v-stepper>
        <div class="uf-sEdit-bottm pb-3 pt-3" v-if="currentStep == 2">
            <v-btn :loading="loading" @click="send()" class="uf-sEdit-bottm-btn mb-2" depressed round color="#4452fc">
                <span v-language:inner="'upload_uf_sEdit_bottm_btn'"/>
            </v-btn>             
            <span class="uf-sEdit-terms">
                <span v-language:inner="'upload_uf_sEdit_terms_by'"/>
                <a :href="termsLink" target="_blank">  
                    <span class="uf-sEdit-terms-link" v-language:inner="'upload_uf_sEdit_terms_link'"/>
                </a>
            </span>
        </div>
        </v-card>  
    </v-flex>
</template>
<script>
import { mapGetters, mapActions } from 'vuex';
import Base62 from "base62"

import documentService from "../../services/documentService";
import analyticsService from "../../services/analytics.service";
import uploadService from "../../services/uploadService";
import { LanguageService } from "../../services/language/languageService";

import uploadFilesStart from "./components/uploadMultipleFileStart.vue";
import uploadStep_2 from "./components/filesDetails.vue";
import fileCardError from './components/fileCardError.vue';

import satelliteServie from "../../services/satelliteService";

export default {
    components: {
        uploadFilesStart,
        uploadStep_2,
        fileCardError
    },
    name: "uploadMultipleFiles",
    data() {
        return {
            selectCoursePlaceholder: LanguageService.getValueByKey("upload_multiple_select_course_placeholder"),
            progressDone: false,
            steps: 2,
            currentStep: 1,
            step: 1,
            callBackmethods: {
                next: this.nextStep,
                changeStep: this.changeStep,
                stopProgress: this.stopProgress,
                send: this.sendDocumentData,
            },
            courseSelected: '',
            nextStepCalled: false,
            loading: false,
            disableBtn: false,
            isEdge : global.isEdge,
            lock: false,
            chackValidation: false,
            termsLink: satelliteServie.getSatelliteUrlByName('terms')
        }
    },
    computed: {
        ...mapGetters({
            getIsValid: 'getIsValid',
            accountUser: 'accountUser',
            getSelectedClasses: 'getSelectedClasses',
            getFileData: 'getFileData',
            getDialogState: 'getDialogState',
        }),
        isError(){
            return this.getFileData.every(item=>item.error)
        },
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        userImage(){
            return this.accountUser.image;
        },
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
        lastStep() {
            return this.currentStep === this.steps;
        },
        showUploadDialog() {
            return this.getDialogState
        },
        isNameExists(){
            let result = this.getFileData.every((item) => {
                return item.name && item.name.length > 0
            });
            return result;
        },
        errorFile(){
            if(this.getFileData && this.getFileData.length && this.isError && this.getFileData[0].error){
                return this.getFileData[0]
            }
        }
    },
    methods: {
        ...mapActions([
            'changeSelectPopUpUniState',
            'updateDialogState',
            'resetUploadData',
            'setReturnToUpload',
            'updateStep',
            'setCourse',
            'updateToasterParams'
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
        sendDocumentData() {
            if(!this.lock){
                this.lock = true;
                this.loading = true;
            let docData = this.getFileData;
            let self = this;
            docData.forEach((fileObj) => {
                if(fileObj.error)return;
                let serverFormattedObj = uploadService.createServerFileData(fileObj);
                documentService.sendDocumentData(serverFormattedObj)
                    .then((resp) => {
                        analyticsService.sb_unitedEvent('STUDY_DOCS', 'DOC_UPLOAD_COMPLETE');
                        analyticsService.sb_unitedEvent('Action Box', 'Upload_D', `USER_ID:${self.accountUser.id}, DOC_COURSE${self.courseSelected}`);
                        self.loading = false;
                        this.updateToasterParams({
                            toasterText: LanguageService.getValueByKey("upload_CreateOk"),
                            showToaster: true
                        });
                        this.closeUpload()
                        // self.goToNextStep()
                    },
                        (error) => {
                            fileObj.error = true;
                            self.loading = false;
                            fileObj.errorText = LanguageService.getValueByKey("upload_multiple_error_upload_something_wrong");
                            self.disableBtn = false;
                        }).finally(()=>{
                            this.lock = false;
                        });
                })
            }
        },
        closeUpload() {
            this.resetUploadData();
            //reset return to upload
            this.setReturnToUpload(false);
            //close
            this.updateDialogState(false);
        },
        nextStep() {
            if (this.currentStep === this.steps) {
                this.currentStep = 1
            } else {
                this.currentStep = this.currentStep + 1;
            }
        },
        changeStep(stepNumber){
            this.currentStep = stepNumber
            if(stepNumber == 1){
                this.resetUploadData();
            }
        },
        send(){
            this.chackValidation = !this.chackValidation;
        }
    },
    created() {
        // if(this.$route.query && this.$route.query.Course){
        //     this.courseSelected = this.$route.query.Course
        //     this.updateSelectedCourse()
        // }
    }
}
</script>

<style lang="less">
@import "../../styles/mixin.less";

.upload-dialog {
    border-radius: 6px;
    position: relative;
    background-color: #fff;

    .uf-close {
        position: absolute;
        right: 10px;
        top: 10px;
        z-index: 99;
        font-size: 12px;
        cursor: pointer;
        color: #adadba;
        @media (max-width: @screen-xs) {
            right: 14px;
            top: 14px;
        }
    }

    .uf-main {
        height: 100%;
        @media (max-width: @screen-xs) {
            display: flex;
            justify-content: center;
            flex-direction: column;
            justify-content: space-between;
            align-items: center;
        }
   .uf-sEdit-bottm{
        display: flex;
        flex-direction: column;
        align-items: center;
        // border-top: 1px solid #e2e2e4;

        .v-btn{
            min-width: 150px;
            height: 40px !important;
            text-transform: capitalize !important;
            margin-left: 0;
            margin-right: 0;
        }
        .uf-sEdit-bottm-btn{
            color: white;
            border: 1px solid @global-blue !important;
            font-size: 14px;
            font-weight: 600;
            letter-spacing: -0.26px;
        }
       .uf-sEdit-terms{
           @media (max-width: @screen-xs) {
            padding: 0 40px;
            text-align: center;
            line-height: 1.83;
           }
            font-size: 12px;
            font-weight: 600;
            font-style: normal;
            font-stretch: normal;
            color: @global-purple;
            .uf-sEdit-terms-link{
                color: @global-blue;
                cursor: pointer;
            }
       }
   }
        .uf-mStepper {
            @media (max-width: @screen-xs) {
                height: 100%;
            }
            .uf-mHeader {
                display: flex;
                align-items: center;
                justify-content: flex-start;
                margin-bottom: 6px;
                @media (max-width: @screen-xs) {
                    margin: 0;
                    display: block;
                    text-align: center;
                }

                .uf-mImg {
                    border-radius: 50%;
                    width: 40px;
                    height: 40px;
                    object-fit: contain;
                }

                .uf-mTitle {
                    font-size: 18px;
                    font-weight: 600;
                    font-style: normal;
                    font-stretch: normal;
                    line-height: normal;
                    letter-spacing: -0.34px;
                    color: @global-purple;
                    @media (max-width: @screen-xs) {
                        letter-spacing: 0.34px;
                        font-size: 16px;
                    }
                }
            }

            .uf-items {
            @media (max-width: @screen-xs) {
                height: calc(~"100% - 42px");
                min-height: calc(~"100% - 42px");
                max-height: calc(~"100% - 42px");
                margin-top: -30px;
            }
                .uf-mStepper-content {
                    .v-stepper__wrapper{
                        @media (max-width: @screen-xs) {
                            height: 100%;
                        }
                    }
                    @media (max-width: @screen-xs) {
                        height: 100%;
                    }
                    padding: 0 !important;
                }
            }
        }
    }
}
</style>