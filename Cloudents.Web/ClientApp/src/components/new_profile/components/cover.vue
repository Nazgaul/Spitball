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

    <slot>
      <div class="coverupload" v-if="$store.getters.getIsMyProfile && isLoaded">
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
          <v-icon class="attach-icon" color="#fff">sbf-cameraNew</v-icon>
        </label>
      </div>
    </slot>
    
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
      windowWidth: window.innerWidth
    }
  },
  computed: {
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly
    },
    coverImageSize() {
      return {
        width: this.windowWidth,
        height: this.isMobile ? 420 : 594
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
      typeingTimer = setTimeout(() => {
        // prevent duplicate request on switch to mobile, coverImageSize computed is already listen to isMobile changes
        if(!self.isMobile) {
          self.windowWidth = window.innerWidth
        } else {
          self.windowWidth = self.$el.clientWidth
        }
        self.currentTime = Date.now()
      }, 1000);
    },
    loaded() {
      this.isLoaded = true
      this.$store.commit('setProfileCoverLoading', true)
    },
    unLoaded() {
      this.isLoaded = false
      this.$store.commit('setProfileCoverLoading', false)
    },
    uploadCoverPicture() {
      let self = this;
      this.unLoaded()
      let formData = new FormData();
      let file = self.$refs.profileImage.files[0];
      formData.append("file", file);
      this.$store.dispatch('uploadCoverImage', formData).then(() => {
        // this.updateToasterParams({
        //    // toasterText: this.$t("chat_file_error"),
        //     showToaster: true
        // });
      });
      this.$refs.profileImage.value = "";
      //document.querySelector('#profile-picture').value = ''
    }
  },
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
  padding: 6px;
  z-index: 2;
  color: #fff;
  border-radius: 6px;
  .attach-icon {
    cursor: pointer;
  }
}
.imageLinear {
  position: absolute;
  bottom: 0;
  right: 0;
  left: 0;
  height: 100%;
  background-image: linear-gradient(to bottom, rgba(255, 255, 255, 0) 17%, rgba(0, 0, 0, 0.41) 50%, #000000);
  @media (max-width: @screen-xs) {
    // background-image: linear-gradient(to bottom, rgba(0, 0, 0, 0) 97%, rgba(0, 0, 0, 0.65) 50%, rgba(0, 0, 0, 0.94) 18%);
  }
  &.noImage {
    background-image: none
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