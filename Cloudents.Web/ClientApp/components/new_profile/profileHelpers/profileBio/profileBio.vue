<template>
  <v-layout class="profile-bio" align-center>
    <v-flex xs12>
      <v-card
        class="profile-bio-card"
        :class="[$vuetify.breakpoint.smAndUp ?  'pl-4 px-3 py-4' : 'px-1 mt-2 transparent elevation-0 py-1']"
      >
        <v-layout
          v-bind="xsColumn"
          :class="[$vuetify.breakpoint.smAndUp ? 'align-start' : 'align-center' ]"
        >
          <v-flex order-xs2 order-sm1>
            <user-image :isMyProfile="isMyProfile"></user-image>
          </v-flex>
          <v-flex
            xs12
            order-xs1
            order-sm2
            :class="[$vuetify.breakpoint.smAndUp ?  'pl-4' : 'mb-4']"          >
            <v-layout class="name-price-wrap" justify-space-between>
              <v-flex>
                <div class="user-name mb-2">
                  <div class="align-start d-flex">
                    <v-icon
                      v-if="$vuetify.breakpoint.xsOnly && isTutorProfile"
                      class="face-icon mr-2"
                    >sbf-face-icon</v-icon>
                    <h1 
                      class="subheading font-weight-bold lineClamp"
                      :style="[{wordBreak: 'break-all'},{maxWidth: $vuetify.breakpoint.xsOnly? '180px':'inherit'}]"
                    >{{userName}}</h1>
                    <v-icon
                      @click="openEditInfo()"
                      v-if="$vuetify.breakpoint.xsOnly && isMyProfile"
                      class="edit-profile-action ml-2 "
                    >sbf-edit-icon</v-icon>
                  </div>
                </div>
                <h2
                  class="text-xs-center text-sm-left  user-university caption text-capitalize"
                >{{university}}</h2>
              </v-flex>
              <div class="tutor-price mr-3">
                <span class="tutor-price" v-if="$vuetify.breakpoint.smAndUp && isTutorProfile">
                  {{isDiscount ? isDiscount : tutorPrice}}
                  <span class="tutor-price small-text">
                    /
                    <span v-language:inner>profile_points_hour</span>
                  </span>
                </span>
                <span class="mt-0 ml-2" v-if="$vuetify.breakpoint.smAndUp && isMyProfile">
                  <v-icon
                    @click="openEditInfo()"
                    class="edit-profile-action subheading"
                  >sbf-edit-icon</v-icon>
                </span>
              </div>
              <div v-if="isDiscount && $vuetify.breakpoint.smAndUp" class="tutor-price strike-through mr-3">
                <span class="tutor-price" v-if="$vuetify.breakpoint.smAndUp && isTutorProfile">
                 {{tutorPrice}}
                  <span class="tutor-price small-text">/<span v-language:inner>profile_points_hour</span>
                  </span>
                </span>
              </div>
            </v-layout>
            <div class="mt-5" v-if="$vuetify.breakpoint.smAndUp">
              <userAboutMessage></userAboutMessage>
            </div>
          </v-flex>
        </v-layout>
        <v-flex style="position:relative;">
          <div
            class="tutor-price text-xs-center"
            v-if="$vuetify.breakpoint.xsOnly && isTutorProfile"
          >
            <span class="tutor-price">
              {{isDiscount ? isDiscount : tutorPrice}}
              <span class="tutor-price small-text">
                <span>/</span>
                <span v-language:inner>profile_points_hour</span>
              </span>
            </span>
            <span
              class="divider mt-4"
              style="height: 2px; width: 44px; background-color: rgba(67, 66, 93, 0.2); margin: 0 auto; display: block"
            ></span>
          </div>
          <div
            class="tutor-price strike-through text-xs-center"
            v-if="$vuetify.breakpoint.xsOnly && isTutorProfile && showStriked"
          >
            <span class="tutor-price">
              {{tutorPrice}}
              <span class="tutor-price small-text">
                <span v-language:inner>profile_points_hour</span>
              </span>
            </span>
          </div>
        </v-flex>
        <v-flex>
          <div class="mt-4" v-if="$vuetify.breakpoint.xsOnly">
            <userAboutMessage></userAboutMessage>
          </div>
        </v-flex>
      </v-card>
    </v-flex>
    <sb-dialog
      :onclosefn="closeEditDialog"
      :activateOverlay="false"
      :showDialog="getShowEditDataDialog"
      :maxWidth="'760px'"
      :popUpType="'editUserInfo'"
      :content-class="'edit-dialog'"
      :isPersistent="true"
    >
      <tutorInfoEdit v-if="isTutorProfile" :closeCallback="closeEditDialog"></tutorInfoEdit>
      <userInfoEdit v-else :closeCallback="closeEditDialog"></userInfoEdit>
    </sb-dialog>
  </v-layout>
</template>

<script>
import { mapGetters, mapActions } from "vuex";
import userImage from "./bioParts/userImage/userImage.vue";
import userAboutMessage from "./bioParts/userAboutMessage.vue";
import userInfoEdit from "../../profileHelpers/userInfoEdit/userInfoEdit.vue";
import tutorInfoEdit from "../../profileHelpers/userInfoEdit/tutorInfoEdit.vue";
import sbDialog from "../../../wrappers/sb-dialog/sb-dialog.vue";
export default {
  name: "profileBio",
  components: {
    userImage,
    userAboutMessage,
    userInfoEdit,
    tutorInfoEdit,
    sbDialog
  },
  data() {
    return {
      discountAmount:70,
      minimumPrice:55
    };
  },
  props: {
    isMyProfile: {
      type: Boolean,
      default: false
    }
  },
  computed: {
    ...mapGetters(["getProfile", "isTutorProfile", "getShowEditDataDialog", "getActivateTutorDiscounts"]),
    xsColumn() {
      const xsColumn = {};
      if (this.$vuetify.breakpoint.xsOnly) {
        xsColumn.column = true;
      }
      return xsColumn;
    },
    tutorPrice() {
      if (
        this.getProfile &&
        this.getProfile.user &&
        this.getProfile.user.tutorData
      ) {
        return this.getProfile.user.tutorData.price;
      }
      return 0;
    },
    isDiscount() {
      return this.getProfile && this.getProfile.user.tutorData.discountPrice ? this.getProfile.user.tutorData.discountPrice : null;
    },
    showStriked(){
      if(!this.getActivateTutorDiscounts) return false;
      let price = this.tutorPrice;
      return price > this.minimumPrice;
    },
    discountedPrice(){
      let price = this.tutorPrice;
      let discountedAmount = price - this.discountAmount;
      return discountedAmount > this.minimumPrice ? discountedAmount.toFixed(0) : this.minimumPrice.toFixed(0);
    },
    university() {
      if (this.getProfile && this.getProfile.user) {
        return this.getProfile.user.universityName;
      }
    },
    userName() {
      if (this.isTutorProfile) {
        if (
          this.getProfile &&
          this.getProfile.user &&
          this.getProfile.user.tutorData
        ) {
          return `${this.getProfile.user.tutorData.firstName} ${this.getProfile.user.tutorData.lastName}`;
        }
      } else {
        if (this.getProfile && this.getProfile.user) {
          return this.getProfile.user.name;
        }
      }
    },
    userScore() {
      if (this.getProfile && this.getProfile.user) {
        return this.getProfile.user.score;
      }
    }
  },
  methods: {
      ...mapActions(['updateEditDialog']),
    openEditInfo() {
      this.updateEditDialog(true);
    },
    closeEditDialog() {
        this.updateEditDialog(false);
    }
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

.profile-bio {
  max-width: 760px;
  .profile-bio-card {
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.18);
    border-radius: 4px;
  }
  .user-name {
    display: flex;
    flex-direction: row;
    align-items: center;
    font-size: 18px;
    font-weight: bold;
    letter-spacing: -0.4px;
    color: @global-purple;
    @media (max-width: @screen-xs) {
      justify-content: center;
    }
    .face-icon {
      font-size: 18px;
      color: @global-purple;
    }
    .lineClamp{
      .lineClamp()
    }
  }
  .tutor-price {
    font-weight: bold;
    font-size: 20px;
    flex-shrink: 0;
    color: @global-purple;
    @media (max-width: @screen-xs) {
      font-size: 26px;
    }
    .small-text {
      font-size: 13px;
    }
    &.strike-through{
      position: absolute;
      top: 50px; 
      right: 15px;
        @media (max-width: @screen-xs) {
          top: 20px;
          right: 0px;
          left: 0;
          width: 25%;
          margin: 0 auto;
        }
      
      .tutor-price{
        font-size: 18px;
        color: @colorBlackNew;
        .small-text{
          font-size: 11px;
        }
      }
      &:after {
        content: '';
        width: 100%;
        border-bottom: solid 1px @colorBlackNew;
        position: absolute;
        left: 0;
        top: 50%;
        z-index: 1;
        @media (max-width: @screen-xs) {
          top: 30%
        }
      }
    }
  .edit-profile-action {
    color: @purpleLight;
    opacity: 0.41;
    font-size: 20px;
    cursor: pointer;
    vertical-align: baseline;
    @media (max-width: @screen-xs) {
      color: @purpleDark;
      font-size: 16px;
    }
  }
  .line-height-1 {
    line-height: 1;
  }
  .user-university {
    font-size: 14px;
    letter-spacing: -0.3px;
    color: @textColor;
  }
}
}
</style>