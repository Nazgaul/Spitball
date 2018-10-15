<template>
    <v-flex xs12>
        <a class="upload-files" @click="openUploaderDialog()">Upload Documents</a>
        <sb-dialog :showDialog="showUploadDialog" :popUpType="'uploadDialog'" :content-class="'upload-dialog'">
            <v-card style="height: 250px;">
                <v-progress-linear v-model="progressValue" :active="progressShow"></v-progress-linear>
<h4>{{typeof progressValue}}</h4>
                <v-btn block color="primary" @click="DbFilesList()" :disabled="!dbReady"

                       class="ask_btn">{{files.length >= 1 ?  'D Upload more' : 'Dropbox'}}
                </v-btn>
                <v-btn block color="success" @click="loadGooglePicker()"

                       class="ask_btn">{{files.length >= 1 ?  'G Upload more' : 'Google Drive'}}
                </v-btn>
                <div class="files pt-3 pb-2" v-if="uploadsPreviewList.length">
                    <ul class="preview-list" >
                        <li v-if="uploadsPreviewList.length" v-for="(image, index) in uploadsPreviewList" :key="index">
                            <button class="hover-trash"  @click="deletePreview(index)">
                                <v-icon>sbf-close</v-icon>
                            </button>
                            <img v-if="image.type ==='dropBox' || 'fromDisk' " :src="image.link"   width="50" height="50"/>
                            <img v-else-if="image.type === 'googleDrive' " :src="image.link"   width="50" height="50"/>

                        </li>
                        <!--<li class="add-file" v-show="uploadsPreviewList.length">-->
                            <!--<label>-->
                                <!--<v-icon>sbf-close</v-icon>-->
                            <!--</label>-->
                        <!--</li>-->
                    </ul>
                </div>
                <div id="result"></div>
                <div class="upload">
                    <ul>
                        <li v-for="file in regularUploadFiles">{{file.name}} - Error: {{file.error}}, Success: {{file.success}}
                            <!--<img :src="file.blob" width="50" height="50" />-->
                        </li>
                    </ul>

                    <file-upload class="regular-upload-component"
                            ref="upload"
                            v-model="regularUploadFiles"
                            post-action="/api/upload/ask"
                            chunk-enabled
                            :extensions="['doc', 'pdf', 'png', 'jpg']"
                            :maximum="4"
                            @input-file="inputFile"
                            @input-filter="inputFilter"
                            :chunk="{
                              action: '/upload/chunk',
                              minSize: 1048576,
                              maxActive: 3,
                              maxRetries: 5,
                            }"
                    >Upload file</file-upload>
                    <span v-show="$refs.upload && $refs.upload.uploaded">All files have been uploaded</span>
                    <button v-show="!$refs.upload || !$refs.upload.active" @click.prevent="$refs.upload.active = true"
                            type="button">Start upload
                    </button>
                    <button v-show="$refs.upload && $refs.upload.active" @click.prevent="$refs.upload.active = false"
                            type="button">Stop upload
                    </button>
                </div>
            </v-card>
        </sb-dialog>
    </v-flex>
</template>
<script src="./uploadFiles.js"></script>
<style scoped lang="less" src="./uploadFiles.less">

</style>