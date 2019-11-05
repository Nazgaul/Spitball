<template>
    <div class="chat-upload-wrap">
        <div class="chat-input-container align-center justify-center column" v-show="!typing">
            <template>

                <label for="photo" class="chat-camera" v-if="$vuetify.breakpoint.xsOnly">
                    <input id="photo" type="file" accept="image/*" capture="camera" @change="capturePhoto" />
                    <photo-camera />
                </label>

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
                        :input-id="componentUniqueIdImage"
                        ref="uploadImage"
                        :drop="false"
                        v-model="uploadedImages"
                        :multiple="false"
                        :post-action="uploadUrl"
                        @input-file="inputFile"
                        @input-filter="inputFilter"
                        accept="image/*"
                    ></file-upload>
                </label>
            </template>
            
            <label for="chat-file" class="chat-upload-file">
                <v-icon class="chat-attach-file">sbf-attach</v-icon>
                <file-upload  chunk-enabled
                    :chunk="{
                        action: uploadUrl,
                        minSize: 1,
                        maxRetries: 5,
                        finishBody : {
                        OtherUser: otherUserId
                    }}"
                    id="file-input"
                    :input-id="componentUniqueIdFile"
                    ref="uploadFile"
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
    import chatImage from '../../images/outline-insert-photo.svg';
    import FileUpload from 'vue-upload-component/src';
    import photoCamera from '../../images/photo-camera.svg';

    export default {
        name: "chatUploadFile",
        components: { 
            FileUpload, 
            chatImage, 
            photoCamera },
        props: {
            typing: {
                type: Boolean,
                default: false
            }
        },
        data() {
            return {
                componentUniqueIdFile: `instance-${this._uid}`,
                componentUniqueIdImage: `instance-${this._uid}-image`,
                uploadUrl: "/api/chat/upload",
                uploadedFiles: [],
                uploadedImages: [],
            }
        },
        methods: {
            ...mapActions(['updateChatUploadLoading','updateFileError','uploadCapturedImage']),
            inputFile: function (newFile, oldFile) {
                let self = this;
                if (self.uploadedFiles && self.uploadedFiles.length > 1 || self.uploadedImages && self.uploadedImages.length > 1) {
                    return
                }
                if (newFile && oldFile && !newFile.active && oldFile.active) {
                    console.log('upload Complete');
                    self.uploadedFiles.length = 0;
                    self.uploadedImages.length = 0;
                    this.updateChatUploadLoading(false);
                }
                // Uploaded successfully
                if (newFile && !!newFile.success) {
                    console.log('success upload', newFile, newFile);
                }
                if (newFile && newFile.error && !oldFile.error) {
                    this.updateFileError(true)
                    this.updateChatUploadLoading(false);
                    return;
                }
                if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
                    if (this.$refs.uploadFile && !this.$refs.uploadFile.active) {
                        this.$refs.uploadFile.active = true;
                    }
                    if (this.$refs.uploadImage && !this.$refs.uploadImage.active) {
                        this.$refs.uploadImage.active = true;
                    }
                    this.updateChatUploadLoading(true);
                }
            },
            inputFilter: function (newFile, oldFile, prevent) {
                if (newFile && !oldFile) {
                    //prevent adding new files if maximum reached
                    if (this.uploadedFiles.length >= 1) {
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
            },
            capturePhoto(e) {
                this.updateChatUploadLoading(true);
                let file = e.target.files[0];
                let formData = new FormData();

                formData.append("file", file);
                formData.append('otherUser', this.otherUserId)

                this.uploadCapturedImage(formData).then(()=> {
                    
                }).catch(ex => {
                    this.updateFileError(true)
                }).finally(() => {
                    this.updateChatUploadLoading(false)
                })
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
                input {
                    display: none;
                    outline: 0;
                    width: 22px;
                }
            }
            .chat-upload-image {
                position: absolute;
                width: 22px;
                cursor: pointer;
                .responsive-property(right, 32px, null, 40px);
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
                .chat-attach-file{
                    color: #848bbc;
                }
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