<template>
    <v-row class="text-center marketingTools mb-2 mb-sm-4 px-0 py-0 px-sm-4 py-sm-3" align="end" dense>
        <v-col cols="12" class="marketingCol marketingTitle d-none d-sm-block pa-0 mb-4">
            Marketing Tools
        </v-col>
        <v-col cols="12" sm="6" class="marketingCol social mb-2 mb-sm-0 pb-sm-0">
            <shareIcon class="mb-3 icon" />
            <div class="bottom">
                <div class="text1">Share a social post</div>
                <div class="text2 mb-3">Stand out on Facebook & Instagram</div>
                <v-btn class="marketingbtn" color="#4c59ff" outlined rounded>Let’s go</v-btn>
            </div>
        </v-col>
        <v-col cols="12" sm="6" class="marketingCol pb-sm-0">
          <couponIcon class="mb-3 icon" />
          <div class="bottom">
              <div class="text1">Create a coupon</div>
              <div class="text2 mb-3">Create your own costume coupon</div>
              <v-btn class="marketingbtn" color="#4c59ff" outlined rounded>Get started</v-btn>
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
  },
  data: () => ({

  })
}
</script>
<style lang="less">
@color: #43425d;

.marketingTools {
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  border-radius: 8px;
  background: white;

  @media (max-width: 599px) {
    box-shadow: none;
    background: none;
  }
  .marketingCol {
    padding: 12px 16px 16px;
    @media (max-width: 599px) {
      background: white;
    }
    .icon {
      width: 144px;
    }
    &.marketingTitle {
      font-size: 18px;
      font-weight: 600;
      color: @color;
    }
    &.social {
      border-right: 1px solid #dddddd;
      @media (max-width: 599px) {
        border-right: none;
      }
    }
    .bottom {
      color: @color;
      .marketingbtn {
        text-transform: initial;
        font-weight: 600;
        min-width: 120px;
        letter-spacing: normal;
      }
      .text1 {
        font-size: 16px;
        font-weight: 600;
      }
      .text2 {
        font-size: 12px;
      }
    }
  }
}
</style>
