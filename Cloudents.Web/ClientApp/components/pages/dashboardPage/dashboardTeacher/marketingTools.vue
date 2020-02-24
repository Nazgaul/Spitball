<template>
    <div class="marketingTools mb-2 mb-sm-4 pb-0">

        <div class="headerTools pa-0 mb-6">
            <div class="titleTools">{{$t('marketing_title')}}</div>
        </div>

        <div class="mainBlogs d-flex-column d-sm-flex align-center">
            <div class="leftBlogs mr-2 pb-4 pb-sm-0">
                <div class="titleWrap">
                    <div class="title1 mb-3">
                        {{$t('marketing_tools_title1')}}
                    </div>
                    <div class="title2 mb-7 mb-sm-12">
                        {{$t('marketing_tools_title2')}}
                    </div>
                    <v-btn :to="{name: 'marketing'}" class="btn" rounded outlined color="#4c59ff" width="120">{{$t('marketing_lets_go')}}</v-btn>
                </div>
            </div>

            <div class="rightBlogs d-flex-column d-sm-flex pa-0 mr-md-10 mt-sm-12">
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
    
    @media (max-width: @screen-xs) {
      display: flex;
      flex-direction: column;
      text-align: center;
    }
    .leftBlogs {
      .titleWrap {
        max-width: 360px;
        @media (max-width: @screen-sm) {
          &:first-child {
            max-width: 100%;
          }
        }
        .title1 {
           font-size: 24px;
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
          font-weight: 600;
        }
      }
    }
    .rightBlogs {
      margin: 0 0 0 auto;

        @media (max-width: @screen-xs) {
          margin: 0 0 12px 0 !important;
          order: -1;
        }
    }
  }
}
</style>
