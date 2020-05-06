<template>
    <v-dialog :value="true" persistent :maxWidth="'718'" :fullscreen="$vuetify.breakpoint.xsOnly" :content-class="'upload-dialog'">
        <v-flex xs12>
            <v-icon v-closeDialog class="uf-close" v-html="'sbf-close'" />
            <v-card class="uf-main elevation-0">
                <v-stepper class="uf-mStepper elevation-0" v-model="currentStep">
                    
                    <v-stepper-header class="uf-mHeader elevation-0 mb-2" :class="[isMobile ? 'pt-2' : 'pl-4']">
                        <template>
                            <img v-if="!isMobile" class="uf-mImg" :src="userImage" alt="image" />
                            <h2 class="uf-mTitle" :class="{'ml-4': !isMobile}" v-t="'upload_uf_mTitle'"></h2>
                        </template>
                    </v-stepper-header>

                    <v-stepper-items class="uf-items">
                        <div v-if="errorFile && errorFile.name" class="px-4">
                            <fileCardError :fileItem="errorFile" :singleFileIndex="0" />
                        </div>
                        <v-stepper-content
                            :class="['uf-mStepper-content', `step-${n}`]"
                            v-for="n in steps"
                            :key="`${n}-content`"
                            :step="n"
                        >
                            <upload-files-start 
                                v-show="n===1" 
                                :curStep="1" 
                                :callBackmethods="callBackmethods">
                            </upload-files-start>
                                <transition name="slide">
                                    <uploadStep_2
                                        :chackValidation="chackValidation"
                                        v-show="n===2"
                                        :curStep="2"
                                        :callBackmethods="callBackmethods"
                                    >
                                    </uploadStep_2>
                                </transition>
                        </v-stepper-content>
                    </v-stepper-items>
                </v-stepper>
                <div class="uf-sEdit-bottm pb-4 pt-4" v-if="currentStep == 2">
                    <v-btn :loading="loading" @click="send()" class="uf-sEdit-bottm-btn" depressed rounded color="#4452fc">
                        <span v-t="'upload_uf_sEdit_bottm_btn'"></span>
                    </v-btn>             
                    <span class="uf-sEdit-terms">
                        <span v-t="'upload_uf_sEdit_terms_by'"></span>
                        <a :href="termsLink" target="_blank">  
                            <span class="uf-sEdit-terms-link" v-t="'upload_uf_sEdit_terms_link'"></span>
                        </a>
                    </span>
                </div>
            </v-card>  
        </v-flex>
    </v-dialog>
</template>
<script>
import { mapGetters, mapActions } from 'vuex';

import documentService from "../../services/documentService";
import analyticsService from "../../services/analytics.service";
import uploadService from "../../services/uploadService";

import uploadFilesStart from "./components/uploadMultipleFileStart.vue";
import uploadStep_2 from "./components/filesDetails.vue";
import fileCardError from './components/fileCardError.vue';

import satelliteServie from "../../services/satelliteService";

export default {
    name: "uploadMultipleFiles",
    components: {
        uploadFilesStart,
        uploadStep_2,
        fileCardError
    },
    data() {
        return {
            step: 1,
            steps: 2,
            currentStep: 1,
            callBackmethods: {
                next: this.nextStep,
                changeStep: this.changeStep,
                send: this.sendDocumentData,
            },
            courseSelected: '',
            nextStepCalled: false,
            loading: false,
            lock: false,
            chackValidation: false,
            termsLink: satelliteServie.getSatelliteUrlByName('terms')
        }
    },
    computed: {
        ...mapGetters(['accountUser', 'getSelectedClasses', 'getFileData']),

        isError(){
            return this.getFileData.every(item=>item.error)
        },
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        userImage(){
            return this.accountUser.image;
        },
        errorFile(){
            if(this.getFileData && this.getFileData.length && this.isError && this.getFileData[0].error){
                return this.getFileData[0]
            }else{
                return null;
            }
        }
    },
    methods: {
        ...mapActions(['resetUploadData', 'updateToasterParams']),

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
                    .then(() => {
                        analyticsService.sb_unitedEvent('STUDY_DOCS', 'DOC_UPLOAD_COMPLETE');
                        analyticsService.sb_unitedEvent('Action Box', 'Upload_D', `USER_ID:${self.accountUser.id}, DOC_COURSE${self.courseSelected}`);
                        self.loading = false;
                        this.updateToasterParams({
                            toasterText: self.$t("upload_CreateOk"),
                            showToaster: true
                        });
                        this.closeUpload()
                    },
                        () => {
                            fileObj.error = true;
                            self.loading = false;
                            fileObj.errorText = self.$t("upload_multiple_error_upload_something_wrong");
                        }).finally(()=>{
                            this.lock = false;
                        });
                })
            }
        },
        closeUpload() {
            this.resetUploadData();
            this.$closeDialog()
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

        .v-btn{
            min-width: 150px;
            height: 40px !important;
            margin-left: 0;
            margin-right: 0;
        }
        .uf-sEdit-bottm-btn{
            margin: 6px 8px;
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
                    .v-stepper__wrapper {
                        @media (max-width: @screen-xs) {
                            height: 100%;
                        }
                    }
                    @media (max-width: @screen-xs) {
                        height: 100%;
                    }
                    padding: 0 !important;
                }
                .sbf-menu-down {
                    margin-top: 8px; //global for all inputs arrow causing input custom height
                    font-size: 30px;
                }
            }
        }
    }
}
</style>