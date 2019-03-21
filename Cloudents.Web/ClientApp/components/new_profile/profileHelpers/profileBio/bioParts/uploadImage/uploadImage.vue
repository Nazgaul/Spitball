<template>
    <div class="profile-upload-wrap align-start justify-center">
        <!--Upload Image-->
        <div class="profile-image-input-container align-center justify-center column">
            <input class="profile-upload"
                   type="file" name="File Upload"
                   @change="uploadProfilePicture"
                   id="profile-picture"
                   accept="image/*"
                   ref="profileImage" v-show="false"/>
            <label for="profile-picture">
                <v-icon class="attach-icon">sbf-edit-icon</v-icon>

            </label>
        </div>
    </div>
</template>

<script>
    import { mapActions } from 'vuex';

    export default {
        name: "uploadImage",
        data() {
            return {}
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
    .profile-upload-wrap {
        display: flex;
        .profile-image-input-container {
            display: flex;
            .sbf {
                cursor: pointer;
            }
        }
    }

</style>