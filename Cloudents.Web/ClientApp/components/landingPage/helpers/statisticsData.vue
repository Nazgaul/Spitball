<template>
  <div class="stats-wrap">
    <v-layout v-if="isMobileView" text-xs-center class="stats-container">
      <v-flex v-if="singleStat.id > 1" class="stats-single-item" v-for="(singleStat, index) in statsData" :key="`3${index}`">
        <v-card class="stat-card">
          <v-card-text class="stat-text-title px-0">{{singleStat.title}}</v-card-text>
          <v-card-text class="stat-text-data px-0">{{singleStat.data}}</v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
    <v-layout v-else text-xs-center class="stats-container">
      <v-flex class="stats-single-item" v-for="(singleStat, index) in statsData" :key="`3${index}`">
        <v-card class="stat-card">
          <v-card-text class="stat-text-title px-0">{{singleStat.title}}</v-card-text>
          <v-card-text class="stat-text-data px-0">{{singleStat.data}}</v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
  </div>
</template>

<script>
export default {
  name: "statisticsData",
  data() {
    return {};
  },
  computed: {

    isMobileView() {
      return this.$vuetify.breakpoint.width < 1024;
    }
  },
  props: {
    statsData: {
      required: false,
      type: Array,
      default: () => {
        return [
          {
            id: 0,
            title: "Total Answers",
            data: "162,332"
          },
          {
            id: 1,
            title: "SBL Token Traded",
            data: "1,183,158"
          },
          {
            id: 2,
            title: "Money Earned",
            data: "$262,993"
          },
          {
            id: 3,
            title: "Happy Students",
            data: "162,332"
          }
        ];
      }
    }
  }
};
</script>

<style scoped lang="less">
@import "../../../styles/mixin.less";
.stats-wrap {
  width: 100%;
  padding: 26px 100px;
  @media (max-width: @screen-mds) {
    padding: 0;
  }
}

.stats-container {
  flex-direction: row;
  flex-wrap: nowrap;
  @media (max-width: @screen-mds) {
    /*flex-wrap: wrap;*/
  }
  .stats-single-item {
    @media (max-width: @screen-mds) {
      flex-basis: 50%;
    }
    .stat-card {
      background: transparent;
      border: none;
      box-shadow: none;
      @media (max-width: @screen-mds) {
        flex-basis: 50%;
      }
      .stat-text-title {
        padding-bottom: 2px;
        font-size: 16px;
        text-align: center;
        color: @color-white;
      }
      .stat-text-data {
        padding-bottom: 4px;
        font-family: "Patua One", sans-serif;
        font-size: 32px;
        letter-spacing: 2.4px;
        text-align: center;
        color: @color-white;
      }
    }
  }
}
</style>