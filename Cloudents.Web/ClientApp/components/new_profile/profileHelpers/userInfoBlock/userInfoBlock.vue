<template>
  <v-layout
    class="user-info-section"
    column
    align-center
    v-if="!tutorPending"
  >
    <v-card
      class="px-3 user-info-card"
      :class="[$vuetify.breakpoint.xsOnly ? 'justify-space-betweeen transparent elevation-0 px-0 py-2': 'py-4']"
    >
      <v-flex class="hidden-sm-and-down">
        <div>
          <div class="mb-2 text-xs-center">
            <h2 class="box-title" v-language:inner>profile_user_sidebar_title</h2>
            <span
              class="tutoring-info-heading caption d-block font-weight-bold font-italic"
              v-language:inner
            >profile_user_sidebar_text</span>
          </div>
        </div>
      </v-flex>
      <v-flex
        xs12
        :class="[$vuetify.breakpoint.xsOnly ? 'mobile-btn-fixed-bottom py-0 mb-0' : 'py-4 mb-3']"
      >
        <becomeTutorBtn></becomeTutorBtn>
      </v-flex>
      <!-- <div class="bottom-section px-3" :class="{'mobile-view': $vuetify.breakpoint.xsOnly}" v-if="false">
                    <v-flex xs6 sm12 class="info-item mb-2 text-xs-center" v-for="one in 2">
                    <div>
                        <span class="tutoring-info-label">Learning Hours</span>
                    </div>
                    <div>
                        <span class="tutoring-info-value">10 hours</span>
                    </div>
                    </v-flex>

      </div>-->
    </v-card>
  </v-layout>
</template>

<script>
import { mapGetters } from "vuex";
import becomeTutorBtn from "../../profileHelpers/becomeTutorBtn/becomeTutorBtn.vue";
export default {
  name: "userInfoBlock",
  components: { becomeTutorBtn },
  computed: {
    ...mapGetters(["getIsTutorState"]),
    tutorPending() {
      return this.getIsTutorState && this.getIsTutorState === "pending";
    }
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
.user-info-section {
  min-width: 260px;
   @media (max-width: @screen-xs) {
    background-color: transparent;
    flex-direction: row;
  }
  .mobile-btn-fixed-bottom {
    align-items: baseline;
    position: fixed;
    bottom: 66px;
    right: 0;
    width: 100%;
    z-index: 9;
    margin: 0;
    padding: 0;
    .ct-btn {
      width: 98%;
      border-radius: 4px;
      box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.22);
      margin: 0 auto;
    }
  }

  .box-title {
    font-size: 30px;
    color: @global-purple;
  }
  .user-info-card {
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
    min-width: 260px;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.18);
    &.mobile-view {
      width: 100%;
      padding-top: 12px;
    }
  }
  .bottom-section {
    display: flex;
    flex-direction: column;
    width: 100%;
    justify-content: space-between;
     @media (max-width: @screen-xs) {
    //&.mobile-view {
      flex-direction: row;
      padding: 0 !important;
    }
  }
  .info-item {
    display: flex;
    justify-content: space-between;
    @media (max-width: @screen-xs) {
      flex-direction: column;
      box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.16);
      background-color: @color-white;
      border-radius: 4px;
      min-height: 76px;
      min-width: 116px;
      padding: 12px;
      margin-bottom: 0;
      margin-left: 8px;
      &:first-child {
        margin-left: 0;
      }
    }
  }
  .tutoring-info-heading {
    margin-top: 12px;
    font-style: italic;
    color: @global-purple;
  }
  .tutoring-info-label {
    font-size: 14px;
    line-height: 1.46;
    color: @textColor;
    @media (max-width: @screen-xs) {
      color: rgba(0, 0, 0, 0.54);
    }
  }
  .tutoring-info-value {
    font-size: 14px;
    font-weight: 600;
    line-height: 1.46;
    color: @global-purple;
  }
}
</style>