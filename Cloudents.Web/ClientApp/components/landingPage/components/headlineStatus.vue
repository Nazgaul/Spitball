<template>
  <div class="hd-cont" v-if="!isMobile">
    <v-layout
      class="landing-headelien-headlineStatus-container px-2"
      align-center
      justify-space-between
      row
      wrap
    >
      <template v-if="stats.documents">
        <span class="hidden-sm-and-down">
          <b>{{(stats.documents).toLocaleString('en')}}</b>
          <span v-language:inner="'homePage_hd_stats_docs'" />
        </span>
        <v-flex class="landing-headelien-headlineStatus-stars">
          <span>95%</span>
          <v-layout class="landing-headelien-headlineStatus-startsrating" align-center row wrap>
            <v-flex class="star" v-for="(star, index) in 5" :key="index"></v-flex>
          </v-layout>
          <span v-language:inner="'landingPage_main_stats_reviews'"></span>
        </v-flex>
        <span>
          <b>{{(stats.tutors).toLocaleString('en')}}</b>
          <span v-language:inner="'homePage_hd_stats_tutors'" />
        </span>
        <span>
          <b>{{(stats.students).toLocaleString('en')}}</b>
          <span v-language:inner="'homePage_hd_stats_students'" />
        </span>
      </template>
    </v-layout>
  </div>
</template>

<script>
import { mapGetters, mapActions } from "vuex";
export default {
  name: "headlineStatus",
  computed: {
    ...mapGetters(["getHPStats"]),
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly;
    },
    stats() {
      return this.getHPStats;
    }
  },
  methods: {
    ...mapActions(["updateHPStats"])
  },
  created() {
    this.updateHPStats();
  }
};
</script>

<style lang="less">
@import "../../../styles/mixin.less";
.hd-cont {
  position: absolute;
  bottom: 0;
  width: 100%;
  height: 62px;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  .landing-headelien-headlineStatus-container {
    margin: 0 auto;
    max-width: 1176px;
    @media (max-width: @sbScreen-smallDesktop) {
      max-width: calc(~"100% - 80px");
    }
    span {
      font-size: 18px;
      font-weight: normal;
      color: white;
    }
    .landing-headelien-headlineStatus-stars {
      display: flex;
      flex-grow: 0;
      flex-shrink: 0;
      flex-basis: auto;
      .landing-headelien-headlineStatus-startsrating {
        padding: 0 8px 0 8px;
        .star {
          width: 21px;
          height: 21px;
          margin: 0 2px;
          background-image: url("../images/star.png");
          background-size: 21px;
        }
        @media (max-width: @screen-xss) {
          padding: 0 9px;
        }
      }
    }
  }
}
</style>
