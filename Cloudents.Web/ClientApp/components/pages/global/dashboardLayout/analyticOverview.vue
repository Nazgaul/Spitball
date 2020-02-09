<template>
    <v-row class="analyticOverview mt-sm-0 mt-2 mb-2 mb-sm-4" dense>
        <v-col class="space" cols="6">
            <div class="text">{{$t('dashboard_analytic_title')}}</div>
        </v-col>
        <v-col class="menuWrap mb-6" cols="6">
            <v-menu offset-y>
                <template v-slot:activator="{ on }">
                    <div v-on="on">
                        <span class="pr-2 selectedItem">{{$t(`dashboard_${selectedItem.key}`)}}</span>
                        <arrowDownIcon class="arrowDown" />
                    </div>
                </template>
                <v-list dense>
                    <v-list-item v-for="(item, index) in items" :key="index" @click="getData(item)">
                      <v-list-item-title>{{ $t(`dashboard_${item.key}`) }}</v-list-item-title>
                    </v-list-item>
                </v-list>
            </v-menu>
        </v-col>

        <v-col cols="12" class="pa-0"></v-col> <!-- keep it empty for make wrap -->

        <v-col
          v-for="(data, index) in results"
          :key="index"
          :cols="isMobile ? 6 : 3"
          class="box pa-0 text-center">
            <div class="boxWrap ma-2 ma-sm-0 py-2 py-sm-0" :class="[isMobile ? 'fullBorder' : 'borderSide']">
              <div class="type">{{ $t(`dashboard_${data.type}`) }}</div>
              <div class="result my-0 my-sm-1">{{data.result}}</div>
              <div class="rate font-weight-bold">
                <arrowDownIcon class="arrow" :class="[data.down ? 'arrowDown' : 'arrowUp']" />
                <div class="precent" :class="{'down': data.down}">{{data.precent}}</div>
              </div>
            </div>
        </v-col>
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
    selectedItem: {title: 'Last 7 days', key: '7days'},
    items: [
      { title: 'Last 7 days', key: '7days' },
      { title: '2 Day', key: '2days' },
      { title: '3 Day', key: '3days' },
      { title: '4 Day', key: '4days' }
    ],
    results: [
      {
        type: 'Revenue',
        result:  '1500$',
        precent: '7.5 %',
        down: false
      },
      {
        type: 'Sales',
        result: '658',
        precent: '13.5 %',
        down: false
      },
      {
        type: 'Visitors',
        result: '500',
        precent: '1.3 %',
        down: true
      },
      {
        type: 'Followers',
        result: '56',
        precent: '2.4 %',
        down: false
      },
    ]
  }),
  computed: {
    textSelectedItem() {
      return this.$t(`dashboard_${this.selectedItem.key}`);
    },
    isMobile() {
      return this.$vuetify.breakpoint.width < 600;
    },
    isXSMobile() {
      return this.$vuetify.breakpoint.width < 375;
    }
  },
  methods: {
    getData(item) {
      this.selectedItem = item
    }
  },
}
</script>
<style lang="less">
  @import '../../../../styles/mixin.less';
  @import '../../../../styles/colors.less';

  .analyticOverview {
    padding: 10px 14px;
    background: white;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    border-radius: 8px;

    @media (max-width: @screen-xs) {
      box-shadow: none;
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
      }
    }
    .box {
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
          font-size: 30px;
          color: @global-purple;
          font-weight: 600;
          @media (max-width: @screen-xs) {
            font-size: 24px;
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
      }
      &:last-child .borderSide {
        border-right: none;
      }
    }
  }
</style>
