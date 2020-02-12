<template>
    <v-row class="text-center marketingTools mb-2 mb-sm-4 px-0 py-0 px-sm-4 py-sm-4" align="end" dense>
        <v-col cols="12" class="marketingCol marketingTitle d-none d-sm-block pa-0 mb-4">
            {{$t('dashboardTeacher_marketing_title')}}
        </v-col>
        <v-col cols="12" sm="6" class="marketingCol social mb-2 mb-sm-0 pb-sm-0 pt-4 pt-sm-0">
            <shareIcon class="mb-3 icon" />
            <div class="bottom">
                <div class="text1">{{$t('dashboardTeacher_share_post')}}</div>
                <div class="text2 mb-3">{{$t('dashboardTeacher_stand_out')}}</div>
                <!-- <v-btn disabled class="marketingbtn" color="#4c59ff" outlined rounded>{{$t('dashboardTeacher_lets_go')}}</v-btn> -->
                <v-btn disabled class="marketingbtn" color="#4c59ff" outlined rounded>{{$t('dashboardTeacher_coming_soon')}}</v-btn>
            </div>
        </v-col>
        <v-col cols="12" sm="6" class="marketingCol pb-sm-0 pt-0 pt-4 pt-sm-0">
          <couponIcon class="mb-3 icon" />
          <div class="bottom">
              <div class="text1">{{$t('dashboardTeacher_create_coupon')}}</div>
              <div class="text2 mb-3">{{$t('dashboardTeacher_costume_coupon')}}</div>
              <!-- <v-btn disabled class="marketingbtn" color="#4c59ff" outlined rounded>{{$t('dashboardTeacher_get_started')}}</v-btn> -->
              <v-btn disabled class="marketingbtn" color="#4c59ff" outlined rounded>{{$t('dashboardTeacher_coming_soon')}}</v-btn>
          </div>
        </v-col>
    </v-row>
</template>
<script>

import shareIcon from './images/shareIcon.svg';
import couponIcon from './images/couponIcon.svg';
export default {
  name: "marketingTools",
  components: {
    shareIcon,
    couponIcon
  }
}
</script>
<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.marketingTools {
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  border-radius: 8px;
  background: white;

  @media (max-width: @screen-xs) {
    box-shadow: none;
    background: none;
  }
  .marketingCol {
    padding: 12px 16px 16px;
    @media (max-width: @screen-xs) {
      background: white;
    }
    .icon {
      width: 144px;
    }
    &.marketingTitle {
      font-size: 18px;
      font-weight: 600;
      color: @global-purple;
    }
    &.social {
      border-right: 1px solid #dddddd;
      @media (max-width: @screen-xs) {
        border-right: none;
      }
    }
    .bottom {
      color: @global-purple;
      .marketingbtn {
        text-transform: initial;
        font-weight: 600;
        min-width: 120px;
        letter-spacing: normal;
      }
      .text1 {
        font-size: 16px;
        font-weight: 600;
        margin-bottom: 2px;
      }
      .text2 {
        font-size: 12px;
      }
    }
  }
}
</style>
