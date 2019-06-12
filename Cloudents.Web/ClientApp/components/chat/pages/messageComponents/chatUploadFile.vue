<template>
    <div class="chat-upload-wrap">
        <!--Upload Image-->
        <div class="chat-input-container align-center justify-center column">
            <label for="chat-file">
                <!--<v-icon class="chat-attach-icon">sbf-attach</v-icon>-->
                    <add-file-img style="opacity: 0.38;"></add-file-img>
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
                    accept="image/*"
                    :extensions="extensions"
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
    import FileUpload from 'vue-upload-component/src';
    export default {
        name: "chatUploadFile",
        components: { FileUpload, addFileImg },
        data() {
            return {
                componentUniqueId: `instance-${this._uid}`,
                extensions: ['jpeg', 'jpe', 'jpg', 'gif', 'png', 'webp'],
                uploadUrl: "/api/chat/upload",
                uploadedFiles: [],
                uploadedFileNames: [],
            }
        },
        methods: {
            ...mapActions(['uploadChatFile', 'updateChatUploadLoading']),
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
                // Filter non-supported extensions  both lower and upper case
                let patt1 = /\.([0-9a-z]+)(?:[\?#]|$)/i;
                let ext = (`${newFile.name}`.toLowerCase()).match(patt1)[1];
                let isSupported = this.extensions.includes(ext);
                if (!isSupported) {
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
            label{
                position: absolute;
                z-index: 5;
                .responsive-property(right, 15px, null, 65px);
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
            }
            svg{
                vertical-align: middle;
            }
        }
    }

</style>