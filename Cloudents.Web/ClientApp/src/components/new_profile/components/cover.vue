<template>
  <div class="d-flex">
    <!--Should be nice to have quiet attribute-->
    <img v-resize.quiet="onResize" :width="coverImageSize.width" :height="coverImageSize.height" sel="cover_image" class="coverPhoto" :src="getCoverImage" />
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
        <div class="profileEdit text-right pa-3 px-sm-3 px-4" v-if="$store.getters.getIsMyProfile">
          <v-btn @click="openTutorEditInfo" width="122" color="rgba(0,0,0,.6)" height="40" depressed>
            <editSVG class="editIcon" />
            <span class="text ms-2" v-t="'edit'"></span>
          </v-btn>
        </div>
        <!-- <v-icon class="attach-icon" size="16" color="#fff">sbf-camera</v-icon>
        <span class="image-edit-text" v-t="'profile_edit_image_text'"></span> -->
      </label>
    </div>
    <slot></slot>
  </div>
</template>

<script>
import * as componentConsts from '../../pages/global/toasterInjection/componentConsts.js'
import { mapActions, mapGetters } from "vuex";
import utilitiesService from "../../../services/utilities/utilitiesService";
import editSVG from '../images/edit.svg';

var typeingTimer;
export default {
  name: "uploadCover",
  components: {
    editSVG
  },
  computed: {
    ...mapGetters([
      "getProfileCoverImage",
      "getProfile",
    ]),
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly
    },
    coverImageSize() {
      let height = 594
      if(this.isMobile) {
        height = window.innerHeight - this.headerHeight - this.statsHeight
      }
      return {
        width: window.innerWidth,
        height
      }
    },
    getCoverImage() {
      //https://github.com/vuejs/vue/issues/214
      this.currentTime;
      let profileUser = this.getProfile?.user;
      if (profileUser) {
        if (this.getProfileCoverImage) {
          let size = this.coverImageSize
          return utilitiesService.proccessImageURL(
            this.getProfileCoverImage,
            size.width,
            size.height,
            'anchorPosition=center'
          );
        }
        return this.isMobile ? require("./profile-default.jpg") : require("./cover-default.png")
        // return `${require("./cover-default.png")}`;
      }
      return "";
    }
  },
  data() {
    return {
      currentTime: Date.now(),
      headerHeight: 60,
      statsHeight: 50
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
        //    // toasterText: this.$t("chat_file_error"),
        //     showToaster: true
        // });
      });
      this.$refs.profileImage.value = "";
      //document.querySelector('#profile-picture').value = ''
    },
    openTutorEditInfo() {
      this.$store.commit('addComponent', componentConsts.TUTOR_EDIT_PROFILE)
    }
  }
};
</script>

<style lang="less" scoped>
@import "../../../styles/mixin";
.coverPhoto {
  left: 0;
  right: 0;
  width: 100%;
  @media (max-width: @screen-xs) {
    position: static;
  }
}
.coverupload {
  position: absolute;
  // margin: 6px;
  // padding: 4px 8px 6px;
  z-index: 2;
  // color: #fff;
  // border-radius: 6px;
  // background-color: rgba(0, 0, 0, 0.6);
  @media (max-width: @screen-xs) {
    position: absolute; // temporary for mobile version till new design
  }
}
.profileEdit {
  .editIcon {
    //temporary solution till new icon
    path:first-child {
      fill: #fff;
    }
  }
  .text {
    font-size: 16px;
    font-weight: 600;
    color: #fff;
  }
}
</style>