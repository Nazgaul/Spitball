<template>
    <cover :fixedHeight="true" @setLoading="loaded">
        <template v-if="loading">
            <div class="coverupload" v-if="$store.getters.getIsMyProfile">
            <input sel="edit_cover_image"
                class="profile-upload"
                type="file"
                name="File Upload"
                @change="uploadCoverPicture"
                accept="image/*"
                ref="profileImage"
                id="profile-cover-upload"
                v-show="false"
            />
            <label for="profile-cover-upload">
                <v-icon class="attach-icon" size="16" color="#fff">sbf-camera</v-icon>
                <span class="image-edit-text" v-t="'profile_edit_image_text'"></span>
            </label>
            </div>
        </template>
    </cover>
</template>

<script>
import cover from "../../components/cover.vue";
export default {
    name: 'profileCover',
    components: {
        cover
    },
    data() {
        return {
            loading: false,
        }
    },
    methods: {
        loaded() {
            this.loading = true
            this.$emit('setLoading')
        },
        uploadCoverPicture() {
        let self = this;
        let formData = new FormData();
        let file = self.$refs.profileImage.files[0];
        formData.append("file", file);
        self.$store.dispatch('uploadCoverImage', formData).then(() => {
            // this.updateToasterParams({
            //    // toasterText: this.$t("chat_file_error"),
            //     showToaster: true
            // });
        });
        this.$refs.profileImage.value = "";
        //document.querySelector('#profile-picture').value = ''
        }
    }
}
</script>