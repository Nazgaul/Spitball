<template>
    <v-card :class="['sb-step-card', $refs.upload && $refs.upload.dropActive ? 'drop-card' : '']">
        <div class="error-block" v-show="extensionErrror || uploadError">
            <div class="error-container">
            <h3 class="error-title" v-show="extensionErrror" v-language:inner>upload_error_extension_title</h3>
                <h3 class="error-title" v-show="uploadError">{{errorText}}</h3>
                <div class="supported-extensions" v-show="extensionErrror">
                <span v-language:inner>upload_error_extensions_support</span>
                <span class="extension" v-for="(extension, index) in supportedExtensions" :key="extension">&nbsp;{{extension}}
                    <span v-if="index+1 !== supportedExtensions.length">,&nbsp;</span>
                </span>
            </div>
            </div>
        </div>
        <div class="upload-row-1">
            <v-icon>sbf-upload-cloud</v-icon>
            <h3 v-show="$vuetify.breakpoint.smAndUp" class="text-blue upload-cloud-text" v-language:inner>upload_files_dopHere</h3>
            <h3 v-show="$vuetify.breakpoint.xsOnly" class="text-blue upload-cloud-text" v-language:inner>upload_files_uploadDoc</h3>

        </div>
        <div class="or-upload"> <span class="options-upload-from" v-language:inner>upload_files_options_text</span></div>
        <div class="upload-row-2 paddingTopSm">
            <div class="btn-holder" >
                <v-btn fab class="upload-option-btn" @click="DbFilesList()"
                       :disabled="!dbReady">
                    <v-icon>sbf-upload-dropbox</v-icon>
                </v-btn>
                <span :class="['btn-label', $vuetify.breakpoint.xsOnly ? 'mobile-text' : '' ] " v-language:inner>upload_files_btn_dropBox</span>
            </div>
            <div class="btn-holder">
                <div fab :class="['upload-option-btn', 'desktop-file']">
                    <v-icon v-if="$vuetify.breakpoint.smAndUp">sbf-upload-desktop</v-icon>
                    <v-icon v-else>sbf-phone</v-icon>
                    <file-upload
                            style="top: unset;"
                            id="upload-input"
                            class="upload-input"
                            ref="upload"
                            :drop="true"
                            v-model="files"
                            :post-action=uploadUrl
                            chunk-enabled
                            :extensions="supportedExtensions"
                            :maximum="1"
                            @input-file="inputFile"
                            @input-filter="inputFilter"
                            :chunk="{
                              action: uploadUrl,
                              minSize: 2,
                              maxActive: 3,
                              maxRetries: 5,}">
                    </file-upload>
                </div>
                <span v-show="$vuetify.breakpoint.xsOnly" class="btn-label mobile-text"
                      v-language:inner>upload_files_btn_phone</span>
                <span v-show="$vuetify.breakpoint.smAndUp" class="btn-label"
                      v-language:inner>upload_files_btn_desktop</span>
            </div>
        </div>
        <!--<div class="upload-row-3">-->
            <!--<div  :class="['btn-holder', {isDropActive : 'drop-active'}]">-->
                <!--<span class="btn-label" v-language:inner>upload_files_btn_drop</span>-->
            <!--</div>-->
        <!--</div>-->
    </v-card>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import uploadService from "../../../../../services/uploadService";
    import Vue from 'vue';
    import FileUpload from 'vue-upload-component/src';
    import { LanguageService } from "../../../../../services/language/languageService";

    //Vue.component('file-upload', FileUpload);

    export default {
        name: "upload-step-1",
        data() {
            return {
                uploadUrl: '/api/upload/file',
                dbReady: false,
                files: [],
                filesUploaded: [],
                generatedFileName: '',
                supportedExtensions: ['doc', 'pdf', 'png',  'jpg', 'docx', 'xls', 'xlsx', 'ppt', 'jpeg', 'pptx'],
                DBsupportedExtensions: ['.doc', '.pdf', '.png', '.jpg', '.docx', '.xls', '.xlsx', '.ppt', '.jpeg', '.pptx'],
                extensionErrror: false,
                uploadError: false,
                errorText: '',
                hovered: false
            }
        },
        components: {
            FileUpload
        },
        props: {
            callBackmethods: {
                type: Object,
                default: {},
                required: false
            }
        },
        computed: {
            ...mapGetters({
                getFileData: 'getFileData'
            }),
            isDropActive(){
                if(this.$refs){
                    return this.$refs.upload && this.$refs.upload.dropActive
                }
            },
        },
        methods: {
            ...mapActions(['updateFile', 'updateUploadProgress', 'updateFileName', 'updateUploadFullMobile']),

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
                var self = this;
                let options = {
                    success: (files) => {
                        let singleFile;
                        //clean if was trying to upload from desktop before
                        files.forEach((file) => {
                            singleFile = {
                                name: file.name,
                                link: file.link,
                                size: file.bytes

                            };
                            // add to array or replace
                        });
                        // this.documentTitle = singleFile.name ? singleFile.name : '';
                        uploadService.uploadDropbox(singleFile)
                            .then((response) => {
                                    self.filesUploaded.splice(0, 1, singleFile);
                                    self.progressDone = true;
                                    // generated blob name from server
                                    self.generatedFileName = response.data.fileName ? response.data.fileName : '';
                                    let fileName = singleFile.name.replace(/\.[^/.]+$/, "");
                                    self.updateFile({'name': fileName, 'blobName': self.generatedFileName});
                                    self.updateFileName(fileName);
                                    self.callBackmethods.stopProgress(true);
                                    self.updateUploadFullMobile(false);
                                    self.callBackmethods.next(1);
                                },
                                error => {
                                    console.log('error drop box api call', error)
                                })
                    },
                    cancel: function (error) {
                        console.log('canceled!!!', error)
                    },
                    linkType: "direct", // "preview" or "direct"
                    multiselect: false, // true or false
                    extensions: this.DBsupportedExtensions,
                };
                global.Dropbox.choose(options);
            },

            // regular upload methods
            inputFile(newFile, oldFile) {
                //happnes once file is added and upload starts
                if (newFile && !oldFile) {
                    // Add file
                    newFile.blob = '';
                    let URL = window.URL || window.webkitURL;
                    if (URL && URL.createObjectURL) {
                        let singleFile = {
                            name: newFile.name,
                            link: URL.createObjectURL(newFile.file),
                        };
                        //add or replace
                        let documentTitle = singleFile.name ? singleFile.name.replace(/\.[^/.]+$/, "") : '';
                        this.filesUploaded.splice(0, 1, singleFile);
                        this.updateFile({'name': documentTitle});
                        this.updateFileName(documentTitle);
                    }
                    this.updateUploadFullMobile(false)
                    this.callBackmethods.next(1)
                }
                // Upload progress
                if (newFile && newFile.progress) {
                    if (newFile.progress === 100) {
                        this.callBackmethods.stopProgress(true);
                    }
                }
                // Upload error
                if (newFile && oldFile && newFile.error !== oldFile.error) {
                    this.errorText = newFile.response.Name ?  newFile.response.Name["0"] : LanguageService.getValueByKey("upload_error_upload_something_wrong");
                    this.uploadError = true;
                    this.callBackmethods.stopProgress(true);
                    this.callBackmethods.changeStep(1);
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
                        //generated blob name from server after successful upload
                        let name = newFile.response.fileName;
                        this.updateFile({'blobName': name});

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
                    this.extensionErrror= false;
                    // Add file
                    // Will not be added to files
                    let patt1 = /\.([0-9a-z]+)(?:[\?#]|$)/i;
                    let ext = (`${newFile.name}`.toLowerCase()).match(patt1)[1];
                    let isSupported = this.supportedExtensions.includes(ext);
                    if (!isSupported) {
                        this.extensionErrror = true;
                        return prevent()
                    }
                    // if (/\.(js|html|php|exe)$/i.test(newFile.name)) {
                    //     return prevent()
                    // }
                }
            },
        },
        created() {
            this.loadDropBoxSrc(); // load Drop box script

        }
    }
</script>

<style>

</style>