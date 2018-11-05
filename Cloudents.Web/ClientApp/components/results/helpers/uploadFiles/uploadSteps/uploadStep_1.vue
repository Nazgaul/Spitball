<template>
    <v-card class="sb-step-card">
        <div class="upload-row-1">
            <v-icon>sbf-upload-cloud</v-icon>
            <h3 class="text-blue upload-cloud-text" v-language:inner>upload_files_uploadDoc</h3>
        </div>
        <div class="upload-row-2 paddingTopSm">
            <div class="btn-holder">
                <v-btn fab class="upload-option-btn" @click="DbFilesList()"
                       :disabled="!dbReady">
                    <v-icon>sbf-upload-dropbox</v-icon>
                </v-btn>
                <span  class="btn-label" v-language:inner>upload_files_btn_dropBox</span>

            </div>
            <div class="btn-holder">
                <v-btn fab class="upload-option-btn">
                    <v-icon>sbf-upload-desktop</v-icon>

                </v-btn>
                <file-upload
                        id="upload-input"
                        class="upload-input"
                        ref="upload"
                        :drop="true"
                        v-model="files"
                        :post-action=uploadUrl
                        chunk-enabled
                        :extensions="['doc', 'pdf', 'png', 'jpg', 'docx', 'xls', 'xlsx', 'ppt', 'jpeg']"
                        :maximum="1"
                        @input-file="inputFile"
                        @input-filter="inputFilter"
                        :chunk="{
                              action: uploadUrl,
                              minSize: 2,
                              maxActive: 3,
                              maxRetries: 5,}">
                </file-upload>
                <span class="btn-label" v-language:inner>upload_files_btn_desktop</span>
            </div>
        </div>
        <div class="upload-row-3">
            <div :class="['btn-holder', $refs.upload && $refs.upload.dropActive ? 'drop-active' : '' ]">
                <!--<v-icon>sbf-upload-drag</v-icon>-->
                <span class="btn-label" v-language:inner>upload_files_btn_drop</span>
            </div>
        </div>
    </v-card>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import uploadService from "../../../../../services/uploadService";
    import Vue from 'vue';
    import FileUpload from 'vue-upload-component/src';

    Vue.component('file-upload', FileUpload);

    export default {
        name: "upload-step-1",
        data() {
            return {
                uploadUrl: '/api/upload/file',
                dbReady: false,
                files: [],
                filesUploaded: [],
                generatedFileName: '',
            }
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

        },
        methods: {
            ...mapActions(['updateFile', 'updateUploadProgress', 'updateFileName']),

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
                dbjs.setAttribute('data-app-key', 'mii3jtxg6616y9g');
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
                                    self.updateFile({'name': singleFile.name, 'blobName': self.generatedFileName});
                                    self.updateFileName(singleFile.name);
                                    self.callBackmethods.stopProgress(true);
                                    self.callBackmethods.next(1);
                                },
                                error => {
                                    console.log('error drop box api call', error)
                                })
                    },
                    cancel: function () {
                        //optional
                    },
                    linkType: "direct", // "preview" or "direct"
                    multiselect: false, // true or false
                    extensions: ['.doc', '.pdf', '.png', '.jpg', '.docx', '.xls', '.xlsx', '.ppt', '.jpeg'],
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
                        let documentTitle = singleFile.name ? singleFile.name : '';
                        this.filesUploaded.splice(0, 1, singleFile);
                        this.updateFile({'name': documentTitle});
                    }
                    this.callBackmethods.next(1)
                }
                // Upload progress
                if (newFile && newFile.progress) {
                    if (newFile.progress === 100) {
                        this.callBackmethods.stopProgress(true);
                    }
                    console.log('progress', newFile.progress, newFile)
                }
                // Upload error
                if (newFile && oldFile && newFile.error !== oldFile.error) {
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
                    // Add file
                    // Filter non-image file remove for docs
                    // Will not be added to files
                    if (/\.(js|html|php|webp|exe)$/i.test(newFile.name)) {
                        return prevent()
                    }
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