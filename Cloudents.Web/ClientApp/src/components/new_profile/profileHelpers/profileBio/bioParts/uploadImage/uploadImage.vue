<template>
    <div class="profile-upload-wrap align-start justify-center">
        <!--Upload Image-->
        <div class="profile-image-input-container align-center justify-center column">
            <input
                class="profile-upload"
                type="file" name="File Upload"
                @change="uploadProfilePicture"
                id="profile-picture"
                accept="image/*"
                ref="profileImage" v-show="false"
            />
            <label for="profile-picture">
                <v-icon class="attach-icon" color="#fff">sbf-cameraNew</v-icon>
                <!-- <span class="image-edit-text" v-t="'profile_edit_image_text'"></span> -->
            </label>
        </div>
    </div>
</template>

<script>
    import { mapActions } from 'vuex';

    export default {
        name: "uploadImage",
        props: {
            fromLiveSession: {
                type: Boolean,
                required: false
            },
            fromProfile: {
                type: Boolean,
                required: false
            },
        },
        methods: {
            ...mapActions(['uploadAccountImage', 'updateToasterParams']),
            uploadProfilePicture() {
                if(this.fromLiveSession) {
                    this.$emit('setLiveImage', this.$refs.profileImage.files)
                    return
                }
                let self = this;
                // will trigger in tutorInfoEdit and userInfoEdit skeleton loader
                this.$emit('setProfileAvatarLoading', false)
                // will trigger in header component to make skeleton loader
                this.$root.$emit('avatarUpdate', false)
                let formData = new FormData();
                let file = self.$refs.profileImage.files[0];
                formData.append("file", file);
                self.uploadAccountImage(formData).then((res) => {
                    if(self.fromProfile) {
                        self.$emit('setLiveImage', file)
                    }
                    if(!res) {
                        this.updateToasterParams({
                            toasterText: this.$t("chat_file_error"),
                            showToaster: true
                        });
                    }
                })
                document.querySelector('#profile-picture').value = ''
            }
        },
    }
</script>

<style lang="less">
    @import '../../../../../../styles/mixin.less';
    .profile-upload-wrap {
        display: flex;
        padding: 4px 4px;
        letter-spacing: normal;
        cursor: pointer;
        label[for=profile-picture]{
            display: flex;
            flex-direction: column;
        }
        .image-edit-text{
            font-size: 12px;
            color: @color-black;
        }
        .profile-image-input-container {
            display: flex;
            .sbf {
                cursor: pointer;
                color: @color-black;
            }
        }
    }

</style>