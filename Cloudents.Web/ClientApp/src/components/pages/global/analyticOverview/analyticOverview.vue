<template>
    <v-row class="analyticOverview mt-sm-0 mt-2 mb-2 mb-sm-4 pb-2 pb-sm-0" >
        <v-col class="space pa-0 mb-2 mb-sm-0" cols="7">
            <div class="text">{{$t('dashboardTeacher_analytic_title')}}</div>
        </v-col>
        <v-col class="menuWrap mb-2 mb-sm-7 pa-0" cols="5">
            <v-menu offset-y>
                <template v-slot:activator="{ on }">
                    <div v-on="on" class="menuDropDown">
                        <span class="pe-2 selectedItem">{{$t(`dashboardTeacher_last_days`, [selectedItem.value])}}</span>
                        <div class="arrow-down" sel="analytics_dropdown"></div>
                    </div>
                </template>
                <v-list sel="anayltics_menu">
                    <v-list-item v-for="(item, index) in items" :key="index" @click="changeDays(item)">
                      <v-list-item-title>{{ $t(`dashboardTeacher_last_days`, [item.value]) }}</v-list-item-title>
                    </v-list-item>
                </v-list>
            </v-menu>
        </v-col>

        <v-col cols="12" class="pa-0"></v-col> <!-- keep it empty for make wrap -->

        <template v-if="results.length">
            <v-col
              v-for="(val, key, index) in results[0]"
              :key="index"
              :cols="isMobile ? 6 : 3"
              class="analyticBox pa-0 text-center">
                <router-link :to="{name: navigation[key]}" class="boxWrap d-block mb-0 mb-sm-2 ma-2 ma-sm-0 py-2 py-sm-0" :class="[isMobile ? 'fullBorder' : 'borderSide']">
                  <div class="type">{{ $t(analyticTypeResource(key)) }}</div>
                  <!-- TODO: Currency Change -->
                  <div class="result my-0 my-sm-1" v-if="val > 0">
                    {{ $n(Math.round(val), key === 'revenue' ? {'style':'currency','currency': currencySymbol, minimumFractionDigits: 0, maximumFractionDigits: 0} : '') }}
                  </div>
                  <div class="minus" v-else>-</div>
                  <div class="rate font-weight-bold" v-if="val > 0">
                      <arrowDownIcon class="arrow" v-show="percentage(key)" :class="[showIcon(key) ? 'arrowDown' : 'arrowUp']" />
                      <bdi class="precent me-1" :class="{'down': showIcon(key)}">{{percentage(key)}} <span v-show="percentage(key)">&#37;</span></bdi>
                  </div>
                </router-link>
            </v-col>
        </template>
        <template v-else>
            <v-col
              v-for="n in 4"
              :key="n"
              :cols="isMobile ? 6 : 3"
              class="analyticLoader mb-5">
                <v-skeleton-loader
                  class="mx-auto load "
                  height=""
                  type="image"
                ></v-skeleton-loader>
            </v-col>
        </template>
    </v-row>
</template>
<script>
import arrowDownIcon from '../../layouts/header/images/arrowDownIcon.svg';

export default {
  name: "analyticOverview",
  components: {
    arrowDownIcon
  },
  data: () => ({
    selectedItem: { title: 'Last 7 days', value: 7 },
    results: [],
    navigation: {
      revenue: 'mySales',
      sales: 'mySales',
      views: 'myCourses',
      followers: 'myFollowers'
    },
    items: [
      { title: 'Last 7 days', value: 7 },
      { title: 'Last 30 Day', value: 30 },
      { title: 'Last 90 Day', value: 90 },
    ],
  }),
  computed: {
    currencySymbol() {
      return this.$store.getters.accountUser?.currencySymbol
    },
    isMobile() {
      return this.$vuetify.breakpoint.width < 600;
    },
  },
  methods: {
    changeDays(item) {
      if(this.selectedItem.value === item.value) return
      // this.results = []; if we want to activate the skeleton loader on each call
      this.selectedItem = item;
      this.getData();
    },
    getData() {
      let self = this;
      this.$store.dispatch('updateUserStats', this.selectedItem.value).then((data) => {
        self.results = data;
      }).catch(ex => {
        self.$appInsights.trackException(ex);
      })
    },
    deltaCalc(key) {
      let stats1 = this.results[0][key];
      let stats2 = this.results[1][key];
      if(!stats2) return 0;
      return stats1 / stats2 * 100 - 100;
    },
    percentage(key) {
      let delta = this.deltaCalc(key) ? this.deltaCalc(key).toFixed(1) : ''
      return delta;
    },
    showIcon(key) {
      let delta = this.deltaCalc(key);
      return (isNaN(delta) || delta <= 0);
    },
    analyticTypeResource(key) {
      let resource = {
        revenue: this.$t('dashboardTeacher_revenue'),
        sales: this.$t('dashboardTeacher_sales'),
        views: this.$t('dashboardTeacher_views'),
        followers: this.$t('dashboardTeacher_followers')
      }
      return resource[key]
    }
  },
  created() {
    this.getData();
  }
}
</script>
<style lang="less">
  @import '../../../../styles/mixin.less';
  @import '../../../../styles/colors.less';

  .analyticOverview {
    padding: 16px;
    background: white;
    border-radius: 8px;
    width: 100%;
    margin: 0 auto;
   
    @media (max-width: @screen-xs) {
      box-shadow: none;
      padding: 14px;
      border-radius: 0;
    }
    .space {
      .text {
        color: @global-purple;
        font-weight: 600;
        font-size: 18px;
        @media (max-width: @screen-xs) {
          font-size: 16px;
        }
      }
      @media (max-width: @screen-xss) {
        flex-basis: 100%;
        max-width: 100%;
      }
    }
    .menuWrap {
      text-align: right;
      @media (max-width: @screen-xss) {
        text-align: left;
      }
      .selectedItem {
        vertical-align: middle;
        cursor: pointer;
      }
      .arrowDown {
        cursor: pointer;
        width: 12px;
        color: @global-purple;
      }
      .menuDropDown {
        display: flex;
        align-items: center;
        justify-content: flex-end;
        @media (max-width: @screen-xss) {
          justify-content: normal;
        }
        .arrow-down {
          width: 0;
          height: 0;
          border-left: 6px solid transparent;
          border-right: 6px solid transparent;
          border-top: 6px solid @global-purple;
        }
      }
    }
    .analyticBox {
      .boxWrap {
        &.borderSide {
          border-right: 1px solid #dddddd;
        }

        &.fullBorder {
          border: 1px solid #e6e6e6;
        }
        .type {
          color: @global-purple;
          @media (max-width: @screen-xs) {
            font-size: 12px;
          }
        }
        .result {
          font-size: 28px;
          color: @global-purple;
          font-weight: 600 /*rtl:500*/;
          @media (max-width: @screen-md) {
            font-size: 24px;
          }
          @media (max-width: @screen-xs) {
            font-size: 26px;
          }
        }
        .rate {
          display: inline-flex;
          align-items: center;
          .precent {
            font-size: 12px;
            color: #42bc46;

            &.down {
              color: #eb3134;
            }
          }
          .arrow {
            width: 10px;
            margin: 2px 4px 0;
            &.arrowDown {
              fill: #eb3134;
            }
            &.arrowUp {
              fill: #42bc46;
              transform: scaleY(-1);
            }
          }
        }
        .minus {
          min-height: 66px;
          font-size: 40px;
          color: @global-purple;
          @media (max-width: @screen-xs) {
          min-height: unset;
          }
        }
      }
      &:last-child .borderSide {
        border-right: none;
      }
      &:nth-child(odd) .boxWrap {
        margin-right: 0 !important;
      }
      &:nth-child(even) .boxWrap {
        margin-left: 0 !important;
      }
      &:nth-last-child(-n+3) {
        margin-bottom: 8px;
      }
    }
    .analyticLoader {
      height: 100px;
      .load {
        height: 100px;
      }
    }
  }
</style>
