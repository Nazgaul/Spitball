<template>
  <div>
    <img
      class="coverPhoto"
      :src="getCoverImage"
    />
    <div>
      <input
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
        <v-icon class="attach-icon">sbf-camera</v-icon>
        <span class="image-edit-text" v-t="'profile_edit_image_text'"></span>
      </label>
    </div>
  </div>
</template>

<script>
    import { mapActions,mapGetters } from 'vuex';
    import utilitiesService from '../../../services/utilities/utilitiesService';

    export default {
        name: "uploadCover",
        computed : {
          getCoverImage() {
            if (this.getProfileCoverImage) {
              return utilitiesService.proccessImageURL(this.getProfileCoverImage, 1920, 430)
            }
            return `${require('./cover-default.jpg')}`
          },
          ...mapGetters(['getProfileCoverImage'])
        },
        methods: {
            ...mapActions(['uploadCoverImage', 'updateToasterParams']),
            uploadCoverPicture() {
                let self = this;
                let formData = new FormData();
                let file = self.$refs.profileImage.files[0];
                formData.append("file", file);
                self.uploadCoverImage(formData).then(() => {
                        // this.updateToasterParams({
                        //    // toasterText: LanguageService.getValueByKey("chat_file_error"),
                        //     showToaster: true
                        // });
                })
                this.$refs.profileImage.value = '';
                //document.querySelector('#profile-picture').value = ''
            }
        },
    }
</script>

<style lang="less" scoped>
.coverPhoto {
  position: absolute;
  left: 0;
  right: 0;
  width: 100%;
  height: 430px;
}
</style>