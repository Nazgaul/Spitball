<template>
    <v-row class="spitballTips" dense>
        <v-col cols="12" class="pa-0 mb-4 d-flex justify-space-between">
            <div class="tipTitle">{{$t('dashboardTeacher_spitball_tips')}}</div>
            <router-link class="seeAll" :to="{ name: 'routeName'}">{{$t('dashboardTeacher_see_all')}}</router-link>
        </v-col>
        <v-col class="tipsList d-flex pa-0" cols="12">
            <router-link class="tipsListBox" v-for="(tip, index) in tips" :key="index" :to="tip.url">
                <div class="top">
                    <img :src="$proccessImageUrl(tip.image, 202, 101)" alt="image" />
                </div>
                <div class="bottom">
                    <div class="text mb-3">{{tip.title}}</div>
                    <div class="nameDate d-flex justify-space-between">
                        <div class="name text-truncate">{{tip.uploader}}</div>
                        <div class="date">{{$d(tip.date, 'short')}}</div>
                    </div>
                </div>
            </router-link>
        </v-col>
    </v-row>
</template>
<script>
export default {
  name: "spitballTips",
  data: () => ({
    tips: []
  }),
  methods: {
    getSpitballBlogs() {
      this.$store.dispatch('updateSpitballBlogs').then(blog => {
        this.tips = blog;
      }).catch(ex => {
        console.log(ex);
      })
    }
  },
  created() {
    this.getSpitballBlogs()
  }
}
</script>
<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';
.spitballTips {
  padding: 12px 16px;
  background: white;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  border-radius: 8px;

  @media (max-width: @screen-xs) {
    box-shadow: none;
  }

  .tipTitle {
    color: @global-purple;
    font-weight: 600;
    font-size: 16px;
  }
  .seeAll {
    color: #4c59ff;
  }
  .tipsList {
    .tipsListBox {
      flex: 1;
      margin: 0 10px;
      &:first-child {
        margin-left: 0;
      }
      &:last-child {
        margin-right: 0;
      }
      .top {
        display: flex;
        img {
          width: 100%;
        }
      }
      .bottom {
        border: 1px solid #dddddd;
        border-top: 0;
        padding: 8px 10px;
        .text {
          color: @global-purple;
          font-size: 13px;
          .giveMeEllipsis(2, 18);
        }
        .nameDate {
          color: #a0a0a0;
          font-size: 12px;
          .name {
            color: @global-purple;
          }
        }
      }
    }
  }
}
</style>
