<template>
    <div class="uf-sDrop-container mb-4" :class="{'uf-sDrop-container-active': isDraggin, 'd-flex align-center justify-center': uploadStarted}">
        <div v-if="isDraggin" class="uf-sDrop-drop d-flex align-center justify-center" v-t="'upload_uf_sDrop_drop'"></div>
        <div v-if="uploadStarted" class="uf-uploading-container">
            <div class="uf-uploading-text">
                <span class="uf-bold" v-t="'upload_uf-uploading'"></span>
                <span v-t="'upload_uf-take-time'"></span>
                <span>{{files[0].progress}}&#37;</span>
            </div>
            <v-progress-linear color="success" v-model="files[0].progress"></v-progress-linear>
        </div>
        <div class="uf-upload-screen-container" :class="{'dragActive': isDraggin}" v-show="!uploadStarted">
            
            <template v-if="!isDraggin && !isMobile">
                <span class="uf-sDrop-title" v-t="'upload_uf_sDrop_title'"></span>
                <span class="uf-sDrop-or" v-t="'upload_uf_sDrop_or'"></span>
            </template> 

            <div class="uf-sDrop-btns">   
                <template v-if="!isDraggin">
                    <v-btn
                        @click="DbFilesList()"
                        class="uf-sDrop-btn me-sm-2 mt-4 mt-sm-0"
                        :disabled="!dbReady"
                        color="#fff"
                        sel="dropbox"
                        depressed
                        rounded
                    >
                        <v-icon v-html="'sbf-upload-dropbox'"></v-icon>
                        <span v-t="'upload_uf_sDrop_btn_dropbox'"></span>
                    </v-btn>
                </template>
                
                <file-upload
                    v-model="files"
                    ref="upload"
                    @input-file="inputFile"
                    id="upload-input"
                    class="file-upload-cmp"
                    :drop="true"
                    :post-action="uploadUrl"
                    style="top: unset;"
                    :multiple="true"
                    :timeout="600 * 10000"
                    :chunk-enabled="true"
                    
                    :chunk="{
                        action: uploadUrl,
                        minSize: 1,
                        maxRetries: 5,
                    }">
                </file-upload>
                <v-btn v-if="!isDraggin" class="uf-sDrop-btn" color="white" depressed rounded sel="my_computer">
                    <v-icon v-html="isMobile ? 'sbf-phone' : 'sbf-pc'"></v-icon>
                    <span> {{isMobile? $t('upload_uf_sDrop_btn_local_mobile') : $t('upload_uf_sDrop_btn_local')}} </span>
                </v-btn>
            </div>
        </div>
    </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';

import uploadService from "../../../services/uploadService";
import analyticsService from '../../../services/analytics.service';

import FileUpload from 'vue-upload-component';

export default {
    name: "upload-step-1",
    components: {FileUpload},
    data() {
        return {
            isDraggin: false,
            uploadUrl: '/api/Document/upload',
            dbReady: false,
            files: [],
            showErrorUpload: '',
            errorText: '',
            uploadStarted: false,
            progress: 0,
        }
    },
    computed: {
        ...mapGetters(['getFileData']),
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        isError(){
            return this.getFileData.every(item => item.error)
        },
        // errorFile(){
        //     return !!(this.getFileData && this.getFileData.length && this.getFileData[0].error && this.isError);
        // }
    },
    watch:{
        isError(hasError){
            if(this.getFileData && this.getFileData.length && this.getFileData[0].error && hasError) {
                this.uploadStarted = false;
            }
        }
    },
    methods: {
        ...mapActions(['updateFile', 'stopUploadProgress', 'setFileBlobNameById', 'updateFileErrorById']),
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
        DbFilesList() {
            const self = this;
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
                                    this.uploadStarted = false
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
            if(newFile) {
                // Upload progress
                if (newFile.progress === 100) {
                    this.stopUploadProgress(newFile)
                    this.uploadStarted = false
                }
                // happens once file is added and upload starts
                if (!oldFile) {
                    this.addFile(newFile, oldFile)
                } else {
                    // Upload error
                    if (newFile.error !== oldFile.error) {
                        this.uploadingError(newFile, oldFile)
                        this.uploadStarted = false
                    }
                    // Get response data
                    if (!newFile.active && oldFile.active) {
                        this.getResponse(newFile, oldFile)
                    }
                    if (newFile.success !== oldFile.success) {
                        if (this.$refs.upload.active) {
                            //TODO
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
        addFile(newFile) {
            // Add file
            newFile.blob = '';
            let URL = window.URL || window.webkitURL;
            if (URL && URL.createObjectURL) {
                let singleFile = uploadService.createFileData(newFile);
                this.updateFile(singleFile);
            }
        },
        uploadingError(newFile) {
            let text = this.$t("upload_multiple_error_upload_something_wrong");
            this.errorText = newFile.response.Name ? newFile.response.Name["0"] : text;
            let fileErrorObj = {
                errorText: newFile.response.Name ? newFile.response.Name["0"] : text,
                id: newFile.id,
                error: true
            };
            this.showErrorUpload = newFile.response.model[0];
            this.updateFileErrorById(fileErrorObj);
            // add this file is not bla bla bla
            if(this.getFileData.length === 1){
                // this.deleteFileByIndex(index);
            }
        },
        getResponse(newFile) {
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
        uploadArea.addEventListener('dragenter',()=>{
            this.isDraggin = true
        })
        uploadArea.addEventListener('dragover',(e)=>{
            e.preventDefault()
            this.isDraggin = true
        })
        uploadArea.addEventListener('drop',()=>{
            this.isDraggin = false
        })
        uploadArea.addEventListener('dragleave',()=>{
            this.isDraggin = false
        })
    },
}
</script>

<style lang="less">
    @import "../../../styles/mixin.less";
.uf-sDrop-container {
    padding: 12px;
    border-radius: 6px;
    border: dashed 2px #d8d8df;
    height: 194px;
    // display: flex;
    // flex-direction: column;
    // justify-content: center;
    // align-items: center;
    // color: @global-blue;
    // position: relative;
    @media (max-width: @screen-xs) {
        height: unset;
        border: none;
    }
    &.uf-sDrop-container-active{
        border: dashed 2px @global-blue;
    }
    .uf-uploading-container{
        .uf-uploading-text{
            color: #000;
            display: flex;
            flex-direction: column;
            align-items: center;
            .uf-bold{
                font-weight: bold;
                font-size:18px;
            }
        }
        .v-progress-linear{
            margin: 10px 0;
            border-radius: 4px;
            // width: 350px;
            // @media (max-width: @screen-xs) {
            //     width: 250px;
            // }
        }
    }
    
    .file-upload-cmp-area{
        position: absolute;
        width: 100%;
        height: 100%;
    }
    .uf-sDrop-drop{
        font-size: 24px;
        font-weight: 600;
        height: 100%;
        line-height: 1.17;
        color: #848bbc;
    }
    .uf-upload-screen-container {
        padding: 10px;
        // padding-bottom: 30px;
        background: #f0f4f8;
        border-radius: 6px;
        display: flex;
        flex-direction: column;
        align-items: center;
        line-height: normal;
        height: 100%;
        &.dragActive {
            display: none;
        }
    .uf-sDrop-title{
        font-size: 18px;
        margin-top: 16px;
        color: rgba(67, 66, 93, 0.56);
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
            // border: 1px solid @global-blue !important;
            @media (max-width: @screen-xs) {
                font-size: 16px;
            }
            font-size: 14px;
            font-weight: 600;
            letter-spacing: -0.26px;
        }
    }
    
    }
}
</style>