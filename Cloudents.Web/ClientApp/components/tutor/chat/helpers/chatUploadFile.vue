<template>
    <div class="chat-upload-wrap">
        <!--Upload Image-->
        <div class="chat-input-container align-center justify-center column">
            <input class="chat-upload"
                   type="file" name="File Upload"
                   @change="uploadChatFiles"
                   id="chat-file"
                   ref="chatFiles" v-show="false"/>
            <label for="chat-file"><add-file-img></add-file-img></label>
        </div>
    </div>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import addFileImg from '../images/add-file.svg';
    export default {
        name: "chatUploadFile",
        components: { addFileImg },
        data() {
            return {
            }
        },
        methods: {
            ...mapActions(['uploadChatFile']),
            uploadChatFiles() {
                let self = this;
                let formData = new FormData();
                let file = self.$refs.chatFiles.files[0];
                console.log(file);
                formData.append("file", file);
                let objData ={
                    formData: formData,
                    isImageType :  file.type && file.type.includes('image')
                };
                self.uploadChatFile(objData);
            }
        },
    }
</script>

<style scoped lang="less">
    .chat-upload-wrap{
        display: flex;
        .chat-input-container{
            display: flex;
        }
    }

</style>