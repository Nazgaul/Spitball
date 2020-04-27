<template>
  <div class="d-flex">
    <!--Should be nice to have quiet attribute-->
    <img v-resize="onResize" sel="cover_image" class="coverPhoto" :src="getCoverImage" />
    <div class="coverupload" v-if="isCurrentProfileUser">
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
        <v-icon class="attach-icon">sbf-camera</v-icon>
        <span class="image-edit-text" v-t="'profile_edit_image_text'"></span>
      </label>
    </div>
  </div>
</template>

<script>
import { mapActions, mapGetters } from "vuex";
import utilitiesService from "../../../services/utilities/utilitiesService";
var typeingTimer;
export default {
  name: "uploadCover",
  computed: {
    ...mapGetters([
      "getProfileCoverImage",
      "currentProfileUser",
      "accountUser",
      "getProfile",
      "getUserLoggedInStatus"
    ]),
    isCurrentProfileUser() {
      let profileUser = this.getProfile?.user;
      if (profileUser && this.getUserLoggedInStatus) {
        return profileUser.id == this.accountUser.id;
      }
      return false;
    },
    getCoverImage() {
      //https://github.com/vuejs/vue/issues/214
      this.currentTime;
      let isMobile = this.$vuetify.breakpoint.xsOnly;
      let profileUser = this.getProfile?.user;
      if (profileUser) {
        if (this.getProfileCoverImage) {
          let size = isMobile
            ? [window.innerWidth, 178]
            : [window.innerWidth, 430];
          return utilitiesService.proccessImageURL(
            this.getProfileCoverImage,
            ...size
          );
        }
        return `${require("./cover-default.png")}`;
      }
      return "";
    }
  },
  data() {
    return {
      currentTime: Date.now()
    }
  },
  methods: {
    onResize() {
      clearTimeout(typeingTimer);
      let self = this;
      typeingTimer = setTimeout(() => {
         self.currentTime = Date.now()
        }, 1000);
    },
    ...mapActions(["uploadCoverImage", "updateToasterParams"]),
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
      });
      this.$refs.profileImage.value = "";
      //document.querySelector('#profile-picture').value = ''
    }
  }
};
</script>

<style lang="less" scoped>
@import "../../../styles/mixin";

.coverPhoto {
  position: absolute;
  left: 0;
  right: 0;
  width: 100%;
  height: 350px;
  @media (max-width: @screen-xs) {
    position: static;
    height: 178px;
  }
}
.coverupload {
  padding: 4px;
  z-index: 2;
  border: 1px solid black;
  background-color: #fff;
  @media (max-width: @screen-xs) {
    position: absolute; // temporary for mobile version till new design
  }
}
</style>