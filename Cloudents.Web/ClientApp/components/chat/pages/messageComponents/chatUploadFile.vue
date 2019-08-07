<template>
    <div class="chat-upload-wrap">
        <!--Upload Image-->
        <div class="chat-input-container align-center justify-center column" v-show="!typing">
            <template>
                <button class="chat-camera" v-if="$vuetify.breakpoint.xsOnly">
                    <photo-camera />
                </button>

                <label for="chat-image" class="chat-upload-image" v-else>
                    <chat-image class="chat-photo"></chat-image>
                    <file-upload  chunk-enabled
                        :chunk="{
                            action: uploadUrl,
                            minSize: 1,
                            maxRetries: 5,
                            finishBody : {
                            OtherUser: otherUserId
                        }}"
                        id="chat-image"
                        :input-id="componentUniqueId"
                        ref="upload"
                        :drop="false"
                        v-model="uploadedFiles"
                        :multiple="false"
                        :post-action="uploadUrl"
                        @input-file="inputFile"
                        @input-filter="inputFilter"
                    ></file-upload>
                </label>
            </template>
            
            <label for="chat-file" class="chat-upload-file">
                <!--<v-icon class="chat-attach-icon">sbf-attach</v-icon>-->
                <add-file-img class="chat-attach-file"></add-file-img>
                <file-upload  chunk-enabled
                    :chunk="{
                        action: uploadUrl,
                        minSize: 1,
                        maxRetries: 5,
                        finishBody : {
                        OtherUser: otherUserId
                    }}"
                    id="file-input"
                    :input-id="componentUniqueId"
                    ref="upload"
                    :drop="false"
                    v-model="uploadedFiles"
                    :multiple="false"
                    :post-action="uploadUrl"
                    @input-file="inputFile"
                    @input-filter="inputFilter"
                ></file-upload>
            </label>
        </div>
    </div>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import addFileImg from '../../../../font-icon/attach.svg';
    import chatImage from './outline-insert-photo.svg';
    import FileUpload from 'vue-upload-component/src';
    import photoCamera from '../messageComponents/photo-camera.svg';

    export default {
        name: "chatUploadFile",
        components: { FileUpload, addFileImg, chatImage, photoCamera },
        props: {
            typing: {
                type: Boolean,
                default: false
            }
        },
        data() {
            return {
                componentUniqueId: `instance-${this._uid}`,
                uploadUrl: "/api/chat/upload",
                uploadedFiles: [],
                uploadedFileNames: [],
            }
        },
        methods: {
            ...mapActions(['uploadChatFile', 'updateChatUploadLoading','updateFileError']),
            inputFile: function (newFile, oldFile) {
                let self = this;
                if (self.uploadedFiles && self.uploadedFiles.length > 1) {
                    return
                }
                if (newFile && oldFile && !newFile.active && oldFile.active) {
                    console.log('upload Complete');
                    self.uploadedFiles.length = 0;
                    this.updateChatUploadLoading(false);
                }
                // Uploaded successfully
                if (newFile && !!newFile.success) {
                    console.log('success upload', newFile, newFile);
                }
                if (newFile && newFile.error && !oldFile.error) {
                    // error
                    //TODO ADD ERROR HANDLER Gaby?
                    //release loader in case of errror
                    this.updateChatUploadLoading(false);
                }
                if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
                    if (!this.$refs.upload.active) {
                        this.$refs.upload.active = true;
                        this.updateChatUploadLoading(true);
                    }
                }
            },
            inputFilter: function (newFile, oldFile, prevent) {
                if (newFile && !oldFile) {
                    //prevent adding new files if maximum reached
                    if (this.uploadedFiles.length >= 1) {
                        return prevent()
                    }
                    if (!/\.(jpeg|jpe|jpg|gif|png|webp|doc|docx|xls|xlsx|PDF|ppt|pptx|tiff|tif|bmp)$/i.test(newFile.name)) {
                        this.updateFileError(true)
                        return prevent()
                    }

                    if (newFile && newFile.size === 0) {
                        return prevent()
                    }
                }
                if (newFile && (!oldFile || newFile.file !== oldFile.file)) {
                    // Create a blob field
                    newFile.blob = '';
                    let URL = window.URL || window.webkitURL;
                    if (URL && URL.createObjectURL) {
                        newFile.blob = URL.createObjectURL(newFile.file);
                    }
                }
            }
        },
        computed:{
            ...mapGetters(['getActiveConversationObj']),
            otherUserId(){
                return this.getActiveConversationObj.userId
            }
        }
    }
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

    .chat-upload-wrap{
        display: flex;
        .chat-input-container{
            display: flex;
            .chat-camera {
                position: absolute;
                .responsive-property(right, 36px, null, 40px);
            }
            .chat-upload-image {
                position: absolute;
                z-index: 5;
                cursor: pointer;
                .responsive-property(right, 36px, null, 40px);
                .file-uploads {
                    position: absolute;
                    top: 0;
                    left: 0;
                    width: 100%;
                    height: 100%;
                    label{
                        cursor:pointer;
                    }
                }
            }
            .chat-upload-file  {
                position: absolute;
                z-index: 5;
                transform: rotate(90deg);
                width: 22px;
                .responsive-property(right, 6px, null, 6px);
                .file-uploads{
                    position: absolute;
                    top: 0;
                    left: 0;
                    width: 100%;
                    height: 100%;
                    label{
                        cursor:pointer;
                    }
                }
                svg {
                    fill: #848bbc;
                }
            }
            svg{
                vertical-align: middle;
            }
        }
    }

</style>