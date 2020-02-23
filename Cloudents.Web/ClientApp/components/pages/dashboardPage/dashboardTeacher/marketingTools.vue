<template>
    <div class="marketingTools mb-2 mb-sm-4 pb-0">

        <div class="headerTools pa-0 mb-8">
            <div class="titleTools">{{$t('dashboardTeacher_spitball_tips')}}</div>
        </div>

        <div class="mainBlogs d-flex-column d-sm-flex align-center">
            <div class="leftBlogs mr-2 pb-4">
                <div class="titleWrap">
                    <div class="title1">
                        {{$t('marketing_title1')}}
                    </div>
                    <div class="title2 mb-7">
                        {{$t('marketing_title2')}}
                    </div>
                    <!-- TODO: switch when merge with marketing branch -->
                    <!-- <v-btn :to="{name: routeNames.marketing}" class="btn" rounded outlined color="#4c59ff" width="120">{{$t('marketing_btn_lets_go')}}</v-btn> -->
                    <v-btn :to="{name: 'marketing'}" class="btn" rounded outlined color="#4c59ff" width="120">{{$t('marketing_btn_lets_go')}}</v-btn>
                </div>
            </div>

            <div class="rightBlogs d-flex-column d-sm-flex pa-0 mr-10">
                <img src="./images/marketing.png" alt="">
            </div>

        </div>
    </div>
</template>
<script>
import * as routeNames from '../../../../routes/routeNames';

export default {
  name: "marketingTools",
  data() {
    return {
      routeNames
    }
  }
}
</script>
<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.marketingTools {
  padding: 16px;
  background: white;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  border-radius: 8px;

  @media (max-width: @screen-xs) {
    box-shadow: none;
  }
  .headerTools {
      .titleTools {
        color: @global-purple;
        font-weight: 600;
        font-size: 16px;
      }
      .seeAllBlog {
        color: #4c59ff;
      }
   }
  .mainBlogs {

    .leftBlogs {
      .titleWrap {
        max-width: 320px;
        @media (max-width: 599px) {
          &:first-child {
            max-width: 100%;
          }
        }
        .title1 {
           font-size: 22px;
           font-weight: 600;
           letter-spacing: normal;
           color: #43425d;
        }
        .title {
           font-weight: 600;
           letter-spacing: normal;
           color: #43425d;
        }
        .btn {
          text-transform: initial;
        }
      }
    }
    .rightBlogs {
      margin: 0 0 0 auto;
    }
  }
}
</style>
