<template>
    <v-flex xs12>

        <a class="upload-files" @click="openUploaderDialog()">Upload Documents</a>
        <sb-dialog :showDialog="showUploadDialog"  :popUpType="'uploadDialog'" :fullWidth="true" :content-class="'upload-dialog'">
            <v-card class="sb-steps-wrap">
                <!--<v-progress-linear v-model="progressValue" :active="progressShow"></v-progress-linear>-->
                <v-stepper v-model="e1" class="sb-stepper">
                    <v-stepper-header class="sb-stepper-header" v-if="e1 ===1">
                        <template>
                            <h2 class="sb-step-title">Ready, Set, Sale!</h2>
                            <h4 class="sb-step-subtitle">Make money of your study documents.</h4>
                        </template>
                    </v-stepper-header>
                    <v-stepper-items class="sb-stepper-item">
                        <!--step 1-->
                        <v-stepper-content class="sb-stepper-content step-one"
                                :key="`${1}-content`"
                                :step="1">
                            <v-card class="sb-step-card" color="" >
                                <!--dropshadow for drag and drop-->
                                 <div class="upload-row-1">
                                    <v-icon>sbf-upload-cloud</v-icon>
                                    <h3 class="text-blue upload-cloud-text">Upload a Document</h3>
                                </div>

                                <div class="upload-row-2">
                                    <div class="btn-holder">
                                    <v-btn fab class="upload-option-btn">
                                        <v-icon>sbf-upload-dropbox</v-icon>

                                    </v-btn>
                                        <span  class="btn-label">DropBox</span>
                                    </div>
                                    <div class="btn-holder">
                                        <!--<div class="desktop-upload-btn">-->
                                        <v-btn fab class="upload-option-btn">
                                        <v-icon>sbf-upload-desktop</v-icon>
                                        <file-upload
                                                    class="upload-input"
                                                     ref="upload"
                                                     :drop="true"
                                                     v-model="regularUploadFiles"
                                                     post-action="/api/upload/ask"
                                                     chunk-enabled
                                                     :extensions="['doc', 'pdf', 'png', 'jpg']"
                                                     :maximum="1"
                                                     @input-file="inputFile"
                                                     @input-filter="inputFilter"
                                                     :chunk="{
                              action: '/upload/chunk',
                              minSize: 1048576,
                              maxActive: 3,
                              maxRetries: 5,}">
                                        </file-upload>
                                </v-btn>
                                        <span class="btn-label">Your Dekstop</span>
                                    </div>
                                </div>
                                <!--</div>-->
                                <div class="upload-row-3">
                                    <div :class="['btn-holder', $refs.upload && $refs.upload.dropActive ? 'drop-active' : '' ]" >
                                            <v-icon>sbf-upload-drag</v-icon>
                                        <span  class="btn-label">Or just drop your file here</span>
                                    </div>
                                </div>
                            </v-card>
                        </v-stepper-content>
                        <!--step 2-->
                        <v-stepper-content class="sb-stepper-content step-two"
                                           :key="`${2}-content`"
                                           :step="2">
                            <v-card class="mb-5 sb-step-card" color="grey lighten-1" >
                                <div class="upload-row-1">
                                    <h3 class="upload-cloud-text sb-title">Awesome! get your document ready for sale</h3>
                                    <h4 class="sb-subtitle mt-2">Filling details will increase this doc. chances to get sold</h4>
                                </div>
                                <div class="upload-row-2">
                                    <div class="btn-holder">
                                        <sb-input :bottomError="true"
                                                  v-model="schoolName" placeholder="Your School" name="password"
                                                  type="text"
                                                  :autofocus="true" @keyup.enter.native="">

                                        </sb-input>
                                    </div>
                                    <div class="btn-holder">

                                    </div>
                                </div>
                                <div class="upload-row-3">

                                </div>

                            </v-card>
                            <!--<v-btn color="primary" @click="nextStep(n)">Continue</v-btn>-->

                            <!--<v-btn flat>Cancel</v-btn>-->
                        </v-stepper-content>

                    </v-stepper-items>
                </v-stepper>
                <v-progress-linear v-model="stepsProgress" :active="true"></v-progress-linear>
                <!--<v-btn block color="primary" @click="DbFilesList()" :disabled="!dbReady"-->

                <!--class="ask_btn">{{files.length >= 1 ? 'D Upload more' : 'Dropbox'}}-->
                <!--</v-btn>-->
                <div id="result"></div>
                <div class="upload">
                    <ul>
                        <li v-for="file in regularUploadFiles">{{file.name}} - Error: {{file.error}}, Success:
                            {{file.success}}
                            <!--<img :src="file.blob" width="50" height="50" />-->
                        </li>
                    </ul>

                     <span v-show="$refs.upload && $refs.upload.uploaded">All files have been uploaded</span>
                    <button v-show="!$refs.upload || !$refs.upload.active" @click.prevent="$refs.upload.active = true"
                            type="button">Start upload
                    </button>
                    <button v-show="$refs.upload && $refs.upload.active" @click.prevent="$refs.upload.active = false"
                            type="button">Stop upload
                    </button>
                </div>
                <div class="step-controls">
                    <v-btn color="primary" @click="nextStep(step)">Continue</v-btn>

                    <v-btn flat>Cancel</v-btn>
                </div>

            </v-card>
        </sb-dialog>

    </v-flex>
</template>
<script src="./uploadFiles.js"></script>
<style scoped lang="less" src="./uploadFiles.less">

</style>