<template>
    <div :class="['mx-3','mb-3','uf-sDrop-container',{'uf-sDrop-container-active': isDraggin}]">
            <span v-if="isDraggin" class="uf-sDrop-drop" v-language:inner="'upload_uf_sDrop_drop'"/>
            
            <template v-if="!isDraggin && !isMobile">
                <span class="uf-sDrop-title" v-language:inner="'upload_uf_sDrop_title'"/>
                <span class="uf-sDrop-or" v-language:inner="'upload_uf_sDrop_or'"/>
            </template> 

            <div class="uf-sDrop-btns">   
                <template v-if="!isDraggin">
                    <v-btn :class="['uf-sDrop-btn',{'mr-2':!isMobile},{'mt-4':isMobile}]" color="white" depressed round @click="DbFilesList()" :disabled="!dbReady">
                        <v-icon v-html="'sbf-upload-dropbox'"/>
                        <span v-language:inner="'upload_uf_sDrop_btn_dropbox'"/>
                    </v-btn>
                </template>

                <file-upload
                    style="top: unset;"
                    id="upload-input"
                    class="file-upload-cmp"
                    ref="upload"
                    :drop="true"
                    v-model="files"
                    :post-action="uploadUrl"
                    chunk-enabled
                    :multiple="true"
                    @input-file="inputFile"
                    :chunk="{
                        action: uploadUrl,
                        minSize: 1,
                        maxRetries: 5,}">
                </file-upload>
                <template v-if="!isDraggin">
                    <v-btn class="uf-sDrop-btn" color="white" depressed round>
                        <v-icon v-html="isMobile?'sbf-phone':'sbf-pc'"/>
                        <span v-language:inner="isMobile?'upload_uf_sDrop_btn_local_mobile':'upload_uf_sDrop_btn_local'"/>
                    </v-btn>
                </template>
            </div>
    </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';

import uploadService from "../../../services/uploadService";
import analyticsService from '../../../services/analytics.service';
import { LanguageService } from "../../../services/language/languageService";

import FileUpload from 'vue-upload-component/src';

export default {
    name: "upload-step-1",
    components: {FileUpload},
    props: {
        callBackmethods: {
            type: Object,
            default: {},
            required: false
        }
    },
    data() {
        return {
            isDraggin:false,
            uploadUrl: '/api/Document/upload',
            dbReady: false,
            files: [],
            showErrorUpload: '',
            uploadError: false,
            errorText: '',
            nextStepCalled: false,
            uploadStarted: false,
        }
    },
    computed: {
        ...mapGetters(['getFileData']),
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        isError(){
            return this.getFileData.every(item=>item.error)
        },
        errorFile(){
            if(this.getFileData && this.getFileData.length && this.getFileData[0].error && this.isError){
                this.uploadStarted = false;
                return true;
            }else{
                return false;
            }
        }
    },
    methods: {
        ...mapActions(['updateFile', 'updateFileName', 'stopUploadProgress', 'setFileBlobNameById', 'updateFileErrorById', 'deleteFileByIndex']),
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
            dbjs.setAttribute('data-app-key', 'cfqlue614nyj8k2');
            dbjs.src = "https://www.dropbox.com/static/api/1/dropins.js";
            document.getElementsByTagName('head')[0].appendChild(dbjs);
            dbjs.onload = () => {
                this.dbReady = true; // enable dropbox upload btn when script is ready
            }
        },
        goToNextStep() {
            if (!this.nextStepCalled) {
                this.nextStepCalled = true;
                this.callBackmethods.next(1);
            }
        },
        DbFilesList() {
            var self = this;
            let singleFile;
            let options = {
                success: (files) => {
                    files.forEach((item) => {
                        //create obj for server and send
                        let serverFile = uploadService.createServerFileData(item);
                        uploadService.uploadDropbox(serverFile)
                            .then((response) => {
                                    //get blob name in resp and create client side formatted obj
                                    item.blobName = response.data.fileName ? response.data.fileName : '';
                                    singleFile = uploadService.createFileData(item);
                                    self.updateFile(singleFile);
                                    self.goToNextStep();
                                },
                                error => {
                                    console.log('error drop box api call', error)
                            })
                    });

                },
                cancel: function (error) {
                    console.log('canceled!!!', error)
                },
                linkType: "direct", // "preview" or "direct"
                multiselect: true, // true or false
                extensions: this.DBsupportedExtensions,
            };
            global.Dropbox.choose(options);
        },
        // regular upload methods
        inputFile(newFile, oldFile) {
            this.uploadStarted = true;
            //happens once file is added and upload starts
            if (newFile && !oldFile) {
                this.addFile(newFile, oldFile)
            }
            // Upload progress
            if (newFile && newFile.progress) {
                if (newFile.progress === 100) {
                    this.stopUploadProgress(newFile)
                }
            }
            // Upload error
            if (newFile && oldFile && newFile.error !== oldFile.error) {
                this.uploadingError(newFile, oldFile)
            }
            // Get response data
            if (newFile && oldFile && !newFile.active && oldFile.active) {
                this.getResponse(newFile, oldFile)
            }
            if (newFile && oldFile && newFile.success !== oldFile.success) {
                if (this.$refs.upload.active) {
                    this.goToNextStep()
                }
            }
            if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
                if (!this.$refs.upload.active) {
                    this.$refs.upload.active = true;
                }
            }             
        },
        addFile(newFile, oldFile) {
            // Add file
            newFile.blob = '';
            let URL = window.URL || window.webkitURL;
            if (URL && URL.createObjectURL) {
                let singleFile = uploadService.createFileData(newFile);
                this.updateFile(singleFile);
            }
        },
        uploadingError(newFile, oldFile) {
            let index = this.getFileData.findIndex((file) => file.id === newFile.id);
            let text = LanguageService.getValueByKey("upload_multiple_error_upload_something_wrong");
            this.errorText = newFile.response.Name ? newFile.response.Name["0"] : text;
            let fileErrorObj = {
                errorText: newFile.response.Name ? newFile.response.Name["0"] : text,
                id: newFile.id,
                error: true
            };
            this.uploadError = true;
            this.showErrorUpload = newFile.response.model[0];
            this.updateFileErrorById(fileErrorObj);
            // add this file is not bla bla bla
            if(this.getFileData.length === 1){
                // this.deleteFileByIndex(index);
            }
        },
        getResponse(newFile, oldFile) {
            // Get response data
            if (newFile.status && newFile.status.toLowerCase() === 'success') {
                //  Get the response status code
                console.log('status', newFile.status)
            }
            if (newFile && newFile.response && newFile.response.status === 'success') {
                //generated blob name from server after successful upload
                let name = newFile.response.fileName;
                let fileData = {
                    id: newFile.id,
                    blobName: name
                };
                this.setFileBlobNameById(fileData);
            }
        }
    },
    created() {
        this.loadDropBoxSrc(); // load Drop box script
        analyticsService.sb_unitedEvent('STUDY_DOCS', 'DOC_UPLOAD_START');
    },
    mounted() {
        let uploadArea = document.querySelector('.uf-sDrop-container')
        uploadArea.addEventListener('dragenter',(e)=>{
            this.isDraggin = true
        })
        uploadArea.addEventListener('dragover',(e)=>{
            e.preventDefault()
            this.isDraggin = true
        })
        uploadArea.addEventListener('drop',(e)=>{
            this.isDraggin = false
        })
        uploadArea.addEventListener('dragleave',(e)=>{
            this.isDraggin = false
        })
    },
}
</script>

<style lang="less">
    @import "../../../styles/mixin.less";
.uf-sDrop-container{
    height: 174px;
    border-radius: 6px;
    border: dashed 2px #d8d8df;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    color: @global-blue;
    position: relative;
    @media (max-width: @screen-xs) {
        height: unset;
        border: none;
    }
    &.uf-sDrop-container-active{
        border: dashed 2px @global-blue;
    }
    .file-upload-cmp-area{
        position: absolute;
        width: 100%;
        height: 100%;
    }
    .uf-sDrop-drop{
        font-size: 24px;
        font-weight: 600;
        font-style: normal;
        font-stretch: normal;
        line-height: 1.17;
        color: #848bbc;
    }
    .uf-sDrop-title{
        font-size: 20px;
        font-weight: 600;
        margin-top: 16px;
        color: @global-purple;
    }
    .uf-sDrop-or{
        padding-top: 18px;
        padding-right: 10px;
        font-size: 16px;
        font-weight: 600;
        font-style: normal;
        font-stretch: normal;
        line-height: 1.75;
        letter-spacing: -0.3px;
        color: @global-purple;
    }
    .uf-sDrop-btns{
        position: relative;
        @media (max-width: @screen-xs) {
            display: flex;
            flex-direction: column-reverse;
            align-items: center;
            margin-top: 30px;
        }
        .v-btn{
            &:before {
            background-color: transparent !important;
            transition: none !important;
            }
            min-width: 150px;
            @media (max-width: @screen-xs) {
              min-width: 244px;
              height: 54px !important;
            }
            height: 40px !important;
            text-transform: capitalize !important;
            margin-left: 0;
            margin-right: 0;
            .v-btn__content{
                align-items: normal;
                @media (max-width: @screen-xs) {
                    align-items: center;
                }
                .v-icon{
                    margin-right: 10px !important;
                    font-size: 20px;
                    @media (max-width: @screen-xs) {
                        margin-right: 14px !important;
                        font-size: 30px;
                    }
                }
            }
        }
        .file-upload-cmp {
            position: absolute;
            width: 50%;
            height: 100%;
            right: 0;
            bottom: 0!important;
            label {
                cursor: pointer;
            }
            @media (max-width: @screen-xs) {
                width: 100%;
                height: 50%;
                top: 0 !important;
            }
        }
        .uf-sDrop-btn{
            color: @global-blue;
            border: 1px solid @global-blue !important;
            @media (max-width: @screen-xs) {
                font-size: 16px;
            }
            font-size: 14px;
            font-weight: 600;
            letter-spacing: -0.26px;
        }
    }
    .redgg{
        width:150px;
        height:150px;
        background:red;
    }
}
</style>