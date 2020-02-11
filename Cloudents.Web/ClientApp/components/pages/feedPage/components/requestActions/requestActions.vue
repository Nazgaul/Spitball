<template>
  <div class="requestActions">
    <div class="rA_top text-truncate">
      <userAvatar :size="'34'" :userImageUrl="userImageUrl" :user-name="userName" :user-id="userID"/>
      <span @click="openAskQuestion()" class="rA_txt text-truncate" v-html="$Ph('requestActions_title',userName)" />
    </div>
    <v-layout class="rA_bottom">
      <v-flex xs4 class="rA_btn">
        <v-btn :ripple="false" depressed text block @click="openRequestTutor()" sel="request">
          <rTutor class="rA_i" />
          <span v-language:inner="$vuetify.breakpoint.smAndDown ?'requestActions_btn_tutor_mob':'requestActions_btn_tutor'"/>
        </v-btn>
      </v-flex>
      <v-flex xs4 class="rA_btn">
        <!-- <v-btn :ripple="false" text block @click="openUpload()" sel="upload">
          <uStudy class="rA_i mr-1" />
          <span v-language:inner="$vuetify.breakpoint.smAndDown ?'requestActions_btn_upload_mob':'requestActions_btn_upload'"/>
        </v-btn> -->
        <v-btn :ripple="false" text block :to="{query: { dialog: 'upload' }}" sel="upload">
          <uStudy class="rA_i mr-1" />
          <span v-language:inner="$vuetify.breakpoint.smAndDown ?'requestActions_btn_upload_mob':'requestActions_btn_upload'"/>
        </v-btn>
      </v-flex>
      <v-flex xs4 class="rA_btn">
        <v-btn :ripple="false" text block @click="openAskQuestion()" sel="ask">
          <aQuestion class="rA_i" />
          <span v-language:inner="$vuetify.breakpoint.smAndDown ?'requestActions_btn_ask_mob':'requestActions_btn_ask'"/>
        </v-btn>
      </v-flex>
    </v-layout>
  </div>
</template>

<script>
import { mapActions, mapGetters } from "vuex";
import analyticsService from "../../../../../services/analytics.service";

import aQuestion from "./image/aQuestion.svg";
import rTutor from "./image/rTutor.svg";
import uStudy from "./image/uStudy.svg";

export default {
  name: "requestActions",
  components: {uStudy, rTutor, aQuestion },
  computed: {
    ...mapGetters(["accountUser", "getSelectedClasses",'getUserLoggedInStatus']),
    userImageUrl() {
      if(this.getUserLoggedInStatus && this.accountUser.image.length > 1) {
        return `${this.accountUser.image}`;
      }
      return "";
    },
    userName() {
      if(this.getUserLoggedInStatus){
          return `, ${this.accountUser.name}?`;
      }
      return '?';
    },
    userID() {
      if(this.getUserLoggedInStatus){
        return this.accountUser.id;
      }
      return null;
    }
  },
  methods: {
    ...mapActions([
      "updateNewQuestionDialogState",
      "updateLoginDialogState",
      "setReturnToUpload",
      "updateDialogState",
      "updateRequestDialog",
      "setTutorRequestAnalyticsOpenedFrom"
    ]),
    openAskQuestion() {
      if (this.accountUser == null) {
        this.updateLoginDialogState(true);
      } else {
        this.updateNewQuestionDialogState(true);
      }
    },
    openUpload() {
      if (this.accountUser == null) {
        this.updateLoginDialogState(true);
      } 
      else if (!this.getSelectedClasses.length) {
        this.$router.push({ name: "addCourse" });
        this.setReturnToUpload(true);
      } 
      else if (this.getSelectedClasses.length > 0) {
        this.updateDialogState(true);
        this.setReturnToUpload(false);
      }
    },
    openRequestTutor() {
      analyticsService.sb_unitedEvent("Tutor_Engagement", "request_box");
      if (this.accountUser == null) {
        this.setTutorRequestAnalyticsOpenedFrom({
          component: "actionBox",
          path: this.$route.path
        });
        this.updateRequestDialog(true);
      } else {
        this.setTutorRequestAnalyticsOpenedFrom({
          component: "actionBox",
          path: this.$route.path
        });
        this.updateRequestDialog(true);
      }
    }
  }
};
</script>

<style lang="less">
@import '../../../../../styles/mixin.less';
.requestActions {
  width: 100%;
  height: 138px;
  border-radius: 8px;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  background-color: #ffffff;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  @media (max-width: @screen-xs) {
    border-radius: initial;
    height: 118px;
    box-shadow: none;
  }
  .rA_top {
    padding: 10px;
    display: flex;
    align-items: center;
    .rA_txt {
    @media (max-width: @screen-xs) {
      // margin-left: 4px;
      font-size: 14px;
    }

      margin-left: 14px;
      vertical-align: middle;
      font-size: 16px;
      font-weight: 600;
      font-stretch: normal;
      font-style: normal;
      line-height: normal;
      letter-spacing: normal;
      color: #43425d;
    }
  }
  .rA_bottom {
    height: 46px;
    max-height: 46px;
    border-top: 1px solid rgba(221, 221, 221, 0.62);
    .rA_btn {
      text-align: center;
      border-right: 1px solid rgba(221, 221, 221, 0.62);
      .v-btn {
        padding: 0 !important;
        font-size: 14px;
        font-weight: 600;
        color: #595475;
        text-transform: capitalize !important;
        height: 100%;
        margin: 0;
          @media (max-width: @screen-xs) {
            font-size: 12px;
          }
        .rA_i {
          fill: #595475;
          margin-right: 8px;
          @media (max-width: @screen-xs) {
            margin-right: 2px;
          }
        }
      }
    }
    .rA_btn:last-child {
      border-right: none;
    }
  }
}
</style>