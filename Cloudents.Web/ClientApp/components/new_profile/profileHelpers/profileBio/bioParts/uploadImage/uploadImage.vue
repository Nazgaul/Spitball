<template>
    <div class="profile-upload-wrap align-start justify-center">
        <!--Upload Image-->
        <h4 v-if="loading">loading...</h4>
        <div class="profile-image-input-container align-center justify-center column">
            <input class="profile-upload"
                   type="file" name="File Upload"
                   @change="uploadProfilePicture"
                   id="profile-picture"
                   accept="image/*"
                   ref="profileImage" v-show="false"/>
            <label for="profile-picture" >
                <v-icon class="attach-icon">sbf-camera</v-icon>
                <span class="image-edit-text">Edit</span>
            </label>
        </div>
    </div>
</template>

<script>
    import { mapActions } from 'vuex';

    export default {
        name: "uploadImage",
        data() {
            return {
                loading: false
            }
        },
        methods: {
            ...mapActions(['uploadAccountImage']),
            uploadProfilePicture() {
                let self = this;
                let formData = new FormData();
                let file = self.$refs.profileImage.files[0];
                formData.append("file", file);
                self.uploadAccountImage(formData);
            }
        },
    }
</script>

<style lang="less">
    @import '../../../../../../styles/mixin.less';
    .profile-upload-wrap {
        display: flex;
        padding: 4px 4px;
        cursor: pointer;
        label[for=profile-picture]{
            display: flex;
            flex-direction: column;
        }
        .image-edit-text{
            font-family: @fontOpenSans;
            font-size: 11px;
            color: @textColor;
        }
        .profile-image-input-container {
            display: flex;
            .sbf {
                cursor: pointer;
            }
        }
    }

</style>