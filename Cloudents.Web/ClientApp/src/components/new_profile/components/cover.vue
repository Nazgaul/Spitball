<template>
  <div class="d-flex">
    <v-skeleton-loader
      v-if="!isLoaded"
      class="skeletonAvatar"
      :class="{'fixedHeight': fixedHeight}"
      type="image"
      :min-width="fixedHeight ? coverImageSize.width : '100%'"
      :height="fixedHeight ? coverImageSize.height : '100%'"
    >
    </v-skeleton-loader>
    <img
      v-show="isLoaded"
      v-resize.quiet="onResize"
      :src="getCoverImage" 
      :width="coverImageSize.width"
      :height="coverImageSize.height"
      @load="loaded"
      class="coverPhoto"
      sel="cover_image"
    />
    <slot></slot>
    <!-- <div class="coverupload" v-if="$store.getters.getIsMyProfile">
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
        {{getProfileCoverImage}}
        <v-icon class="attach-icon" size="16" color="#fff">sbf-camera</v-icon>
        <span class="image-edit-text" v-t="'profile_edit_image_text'"></span>
      </label>
    </div> -->
    <!-- <slot name="linear"></slot> -->
    <div class="imageLinear" :class="{'noImage': !isLoaded}"></div>
  </div>
</template>

<script>
import utilitiesService from "../../../services/utilities/utilitiesService";

var typeingTimer;
export default {
  name: "uploadCover",
  props: {
    fixedHeight: {
      type: Boolean,
      required: false
    }
  },
  data() {
    return {
      isLoaded: false,
      currentTime: Date.now(),
      headerHeight: 60,
      statsHeight: 50,
      // windowWidth: 0
    }
  },
  computed: {
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly
    },
    coverImageSize() {
      let height = 594;
      if(this.isMobile) {
        height = window.innerHeight - this.headerHeight - this.statsHeight
      }
      console.log(this.windowWidth);
      
      return {
        width: window.innerWidth,
        height
      }
    },
    getCoverImage() {
      //https://github.com/vuejs/vue/issues/214
      this.currentTime;
      let profileUser = this.$store.getters.getProfile?.user;
      if (profileUser) {
        let size = this.coverImageSize
        let coverImage = this.$store.getters.getProfileCoverImage
        return utilitiesService.proccessImageURL(
          coverImage,
          size.width,
          size.height,
          'anchorPosition=center',
          'cover'
        );
      }
      return "";
    }
  },
  methods: {
    onResize() {
      clearTimeout(typeingTimer);
      let self = this;
      // this.windowWidth = window.innerWidth
      typeingTimer = setTimeout(() => {
         self.currentTime = Date.now()
        }, 1000);
    },
    loaded() {
      this.isLoaded = true
      this.$emit('setLoading')
    },
    // uploadCoverPicture() {
    //   let self = this;
    //   let formData = new FormData();
    //   let file = self.$refs.profileImage.files[0];
    //   formData.append("file", file);
    //   self.uploadCoverImage(formData).then(() => {
    //     // this.updateToasterParams({
    //     //    // toasterText: this.$t("chat_file_error"),
    //     //     showToaster: true
    //     // });
    //   });
    //   this.$refs.profileImage.value = "";
    //   //document.querySelector('#profile-picture').value = ''
    // }
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
.skeletonAvatar {
  &.fixedHeight {
    .v-skeleton-loader__image {
      height: 594px;
    }
  }
}
</style>