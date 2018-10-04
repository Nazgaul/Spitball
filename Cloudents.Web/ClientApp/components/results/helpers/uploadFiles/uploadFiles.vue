<template>
    <v-flex xs12>
        <a class="upload-files" @click="openUploaderDialog()">Upload Documents</a>
        <sb-dialog :showDialog="showUploadDialog" :popUpType="'uploadDialog'" :content-class="'upload-dialog'">
            <v-card style="height: 250px;">
                <div :style="{width: dynamicProgress, backgroundColor: 'red', height: '30px'}"></div>
                <label for="file-input" class="attach-file">
                    <v-icon style="cursor: pointer;" >sbf-attach</v-icon>
                </label>
                <input id="file-input" type="file" multiple accept="*"/>
                <v-btn block color="primary" @click="DbFilesList()" :disabled="!dbReady"

                       class="ask_btn">Dropbox {{dbReady}}</v-btn>
                <div class="files pt-3 pb-2" v-if="uploadsPreviewList.length">
                    <ul class="preview-list" v-if="uploadsPreviewList.length">
                        <li v-if="uploadsPreviewList.length" v-for="(image, index) in uploadsPreviewList" :key="index">
                            <button class="hover-trash" @click="">
                                <v-icon>sbf-close</v-icon>
                            </button>
                            <img :src="image"/>
                        </li>
                        <li class="add-file" v-show="uploadsPreviewList.length">
                            <label for="file-input">
                                <v-icon>sbf-close</v-icon>
                            </label>
                        </li>
                    </ul>
                </div>
            </v-card>
        </sb-dialog>
    </v-flex>
</template>
<script src="./uploadFiles.js"></script>
<style scoped lang="less" src="./uploadFiles.less">

</style>