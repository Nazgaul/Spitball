<template>
  <div>
    <img
      class="coverPhoto"
      :src="getCoverImage"
    />
    <div v-if="isCurrentProfileUser">
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
          ...mapGetters(['getProfileCoverImage', 'currentProfileUser', 'accountUser', 'getProfile', 'getUserLoggedInStatus']),
          isCurrentProfileUser(){
            let profileUser = this.getProfile?.user
            if (profileUser && this.getUserLoggedInStatus){
              return profileUser.id == this.accountUser.id;
            } 
            return false;
          },
          getCoverImage() {
            let isMobile = this.$vuetify.breakpoint.xsOnly;
            if (this.getProfileCoverImage) {
              let size = isMobile ? [window.innerWidth, 178] : [window.innerWidth, 430]
              return utilitiesService.proccessImageURL(this.getProfileCoverImage, ...size)
            }
            return `${require('./cover-default.jpg')}`
          },
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
@import '../../../styles/mixin';

.coverPhoto {
  position: absolute;
  left: 0;
  right: 0;
  width: 100%;
  height: 430px;
  @media(max-width: @screen-xs) {
    position: static;
    height: 178px;
  }
}
</style>