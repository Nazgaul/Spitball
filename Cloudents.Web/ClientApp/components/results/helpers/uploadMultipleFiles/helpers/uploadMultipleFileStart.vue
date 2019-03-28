<template>
    <v-card elevation="0"
            :class="[ 'upload-component-wrap', {'mb-3': $vuetify.breakpoint.smAndUp} , $refs.upload && $refs.upload.dropActive ? 'drop-card' : '']">
        <div class="error-block" v-show="extensionErrror || uploadError">
            <div class="error-container">
                <h3 class="error-title" v-show="extensionErrror"
                    v-language:inner>upload_multiple_error_extension_title</h3>
                <h3 class="error-title" v-show="uploadError">{{errorText}}</h3>
                <div class="supported-extensions" v-show="extensionErrror">
                    <span v-language:inner>upload_multiple_error_extensions_support</span>
                    <span class="extension" v-for="(extension, index) in supportedExtensions" :key="extension">
                        <span>
                              &nbsp;{{extension}}
                    <span v-if="index+1 !== supportedExtensions.length">,</span>
                        </span>

                </span>
                </div>
            </div>
        </div>
        <v-layout justify-center align-center column class="upload-area mx-3">
            <v-flex class="justify-center align-center d-flex" grow v-show="$vuetify.breakpoint.smAndUp">
                <span v-show="$vuetify.breakpoint.smAndUp"
                      class="col-blue drop-text text-md-center text-xs-center text-sm-center" v-language:inner>upload_multiple_files_dopHere</span>
            </v-flex>
            <v-flex xs12 sm6 md6 row class="justify-center align-center upload-options">
                <div class="btn-holder">
                    <span v-show="$vuetify.breakpoint.smAndUp" class="browse-text" v-language:inner>upload_multiple_or_browse_label</span>
                    <span v-show="$vuetify.breakpoint.xsOnly" class="browse-text" v-language:inner>upload_multiple_or_browse_label_mobile</span>

                </div>
                <div class="btn-holder c-pointer" @click="DbFilesList()" :class="{'ml-3': $vuetify.breakpoint.smAndUp}"
                     :disabled="!dbReady">
                    <v-icon :class="{'mr-2': $vuetify.breakpoint.smAndUp }">sbf-upload-dropbox</v-icon>
                    <span :class="['btn-label', $vuetify.breakpoint.xsOnly ? 'mobile-text' : '' ] " v-language:inner>upload_multiple_files_btn_dropBox</span>
                </div>
                <div class="btn-holder c-pointer" :class="{'ml-3': $vuetify.breakpoint.smAndUp}">
                    <v-icon v-if="$vuetify.breakpoint.smAndUp" class="mr-2">sbf-upload-desktop</v-icon>
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
                            :multiple="true"
                            @input-file="inputFile"
                            @input-filter="inputFilter"
                            :chunk="{
                              action: uploadUrl,
                              minSize: 1,
                              maxRetries: 5,}">
                    </file-upload>
                    <span v-show="$vuetify.breakpoint.xsOnly" class="btn-label mobile-text"
                          v-language:inner>upload_multiple_files_btn_phone</span>
                    <span v-show="$vuetify.breakpoint.smAndUp" class="btn-label"
                          v-language:inner>upload_multiple_files_btn_desktop</span>
                </div>
            </v-flex>
        </v-layout>
    </v-card>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import uploadService from "../../../../../services/uploadService";
    import analyticsService from '../../../../../services/analytics.service';
    import FileUpload from 'vue-upload-component/src';
    import { LanguageService } from "../../../../../services/language/languageService";

    export default {
        name: "upload-step-1",
        data() {
            return {
                uploadUrl: '/api/upload/file',
                dbReady: false,
                files: [],
                filesUploaded: [],
                generatedFileName: '',
                supportedExtensions: ['doc', 'pdf', 'png', 'jpg', 'docx', 'xls', 'xlsx', 'ppt', 'jpeg', 'pptx', 'tiff', 'tif', 'bmp'],
                DBsupportedExtensions: ['.doc', '.pdf', '.png', '.jpg', '.docx', '.xls', '.xlsx', '.ppt', '.jpeg', '.pptx', 'tiff', 'tif', 'bmp'],
                extensionErrror: false,
                uploadError: false,
                errorText: '',
                hovered: false,
                nextStepCalled: false
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
            isDropActive() {
                if (this.$refs) {
                    return this.$refs.upload && this.$refs.upload.dropActive
                }
            },
        },
        methods: {
            ...mapActions(['updateFile', 'updateFileName', 'stopUploadProgress', 'setFileBlobNameById', 'updateFileErrorById']),
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
                //happnes once file is added and upload starts
                if (newFile && !oldFile) {
                    // Add file
                    newFile.blob = '';
                    let URL = window.URL || window.webkitURL;
                    if (URL && URL.createObjectURL) {
                        //overwrite default file type(error on server if not)
                        newFile.type ? newFile.type = '' : '';
                        let singleFile = uploadService.createFileData(newFile);
                        //add or replace
                        this.updateFile(singleFile);
                    }
                    this.goToNextStep();
                }
                // Upload progress
                if (newFile && newFile.progress) {
                    if (newFile.progress === 100) {
                        this.stopUploadProgress(newFile)
                    }
                }
                // Upload error
                if (newFile && oldFile && newFile.error !== oldFile.error) {
                    let text = LanguageService.getValueByKey("upload_multiple_error_upload_something_wrong");
                    this.errorText = newFile.response.Name ? newFile.response.Name["0"] : text;
                    let fileErrorObj = {
                        errorText: newFile.response.Name ? newFile.response.Name["0"] : text,
                        id: newFile.id,
                        error: true
                    };
                    this.uploadError = true;
                    this.updateFileErrorById(fileErrorObj);
                    console.log('error', newFile.error, newFile)
                }
                // Get response data
                if (newFile && oldFile && !newFile.active && oldFile.active) {
                    // Get response data
                    console.log('response', newFile.response);
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
                if (newFile && oldFile && newFile.success !== oldFile.success) {
                    console.log('success', newFile.success, newFile)
                }
                if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
                    if (!this.$refs.upload.active) {
                        this.$refs.upload.active = true
                    }
                }
            },
            inputFilter(newFile, oldFile, prevent) {
                if (newFile && !oldFile) {
                    this.extensionErrror = false;
                    // Add file
                    // Will not be added to files
                    let patt1 = /\.([0-9a-z]+)(?:[\?#]|$)/i;
                    let ext = (`${newFile.name}`.toLowerCase()).match(patt1)[1];
                    let isSupported = this.supportedExtensions.includes(ext);
                    if (!isSupported) {
                        this.extensionErrror = true;
                        return prevent()
                    }
                }
            },
        },
        created() {
            this.loadDropBoxSrc(); // load Drop box script
            analyticsService.sb_unitedEvent('STUDY_DOCS', 'DOC_UPLOAD_START');
            console.log('DOC_UPLOAD_START');
        }
    }
</script>

<style lang="less">
    @import "../../../../../styles/mixin.less";

    @purpleNewColor: #43425d;
    .upload-component-wrap {
        max-height: 200px;
        background-color: transparent;
        min-height: 200px;
        display: flex;
        flex-direction: column;
        justify-content: flex-start;
        box-shadow: none;
        @media (max-width: @screen-xs) {
            max-height: unset;
            min-height: unset;
        }
        .col-blue {
            color: @purpleNewColor;
        }
        .c-pointer {
            cursor: pointer;
        }
        #upload-input {
            position: absolute;
            height: 24px;
            width: 100px;
            transform: rotate(0deg);
            cursor: pointer;
            @media (max-width: @screen-xs) {
                height: 100px;
            }
            label {
                display: none;
            }
            input {
                position: absolute;
                height: 24px;
                left: 0;
                top: 0;
                bottom: 0;
                width: 100px;
                cursor: pointer;
                @media (max-width: @screen-xs) {
                    height: 100px;
                }
            }
        }

        .upload-area {
            background-color: rgba(68, 82, 252, 0.06);
            border: 2px dashed @color-blue-new;
            max-height: 200px;
            @media (max-width: @screen-xs) {
                max-height: unset;
                background-color: @color-white;
                border: none;

            }
        }
        .drop-text {
            font-family: @fontOpenSans;
            font-size: 18px;
            font-weight: 600;
            color: @purpleNewColor;
        }
        .upload-options {
            display: flex;
            background-color: @color-white;
            border-radius: 4px 4px 0 0;
            padding: 12px 16px;
            max-width: 318px;
            max-height: 40px;
            @media (max-width: @screen-xs) {
                flex-direction: column;
                max-height: 100%;
            }
            .btn-holder {
                display: flex;
                flex-direction: row;
                @media (max-width: @screen-xs) {
                    flex-direction: column;
                }

            }
            .browse-text {
                font-family: @fontOpenSans;
                font-size: 14px;
                color: @colorBlackNew;
            }
            .btn-label {
                color: @color-blue-new;
                font-family: @fontOpenSans;
                font-size: 14px;
                @media (max-width: @screen-xs) {
                    font-family: @fontFiraSans;
                    font-size: 16px;
                }

            }
            .v-icon {
                font-size: 12px;
                color: @color-blue-new;
                @media (max-width: @screen-xs) {
                    margin-top: 36px;
                    margin-bottom: 16px;
                    font-size: 32px;
                }
            }
        }
    }
</style>