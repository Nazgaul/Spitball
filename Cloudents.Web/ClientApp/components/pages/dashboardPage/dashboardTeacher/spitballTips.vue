<template>
    <v-row class="spitballTips">
        <v-col cols="12" class="pa-0 mb-4 d-flex justify-space-between">
            <div class="tipTitle">{{$t('dashboardTeacher_spitball_tips')}}</div>
            <a class="seeAll" :href="blogsLink" target="_blank">{{$t('dashboardTeacher_see_all')}}</a>
        </v-col>
        <template v-if="tips.length">
            <v-col class="tipsList d-flex pa-0" cols="4" v-for="(tip, index) in tips" :key="index">
                <a class="tipsListBox" :href="tip.url" target="_blank">
                    <div class="top">
                        <img :src="tip.image" alt="image" width="200" height="100" />
                    </div>
                    <div class="bottom">
                        <div class="text mb-3">{{tip.title}}</div>
                        <div class="nameDate d-flex justify-space-between">
                            <div class="name text-truncate">{{tip.uploader}}</div>
                            <div class="date">{{$d(new Date(tip.create), 'short')}}</div>
                        </div>
                    </div>
                </a>
            </v-col>
        </template>
        <template v-else>
            <v-col class="" cols="4" v-for="n in 3" :key="n">
                <v-skeleton-loader
                  class=""
                  height="200"
                  type="image"
                ></v-skeleton-loader>
            </v-col>
        </template>
    </v-row>
</template>

<script>
export default {
  name: "spitballTips",
  data: () => ({
    tips: []
  }),
  computed: {
    blogsLink() {
      return global.country === "IL" ? 'https://www.blog.spitball.co/blog/categories/hebrew' : 'https://www.blog.spitball.co/blog/categories/english';
    }
  },
  methods: {
    getSpitballBlogs() {
      let self = this;
      this.$store.dispatch('updateSpitballBlogs').then(blog => {
        self.tips = blog;
      }).catch(ex => {
        self.$appInsights.trackException({exception: new Error(ex)});
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
  padding: 16px;
  background: white;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  border-radius: 8px;
  width: 100%;
  margin: 0 auto;
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
      width: 100%;
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
          min-height: 36px;
          font-size: 13px;
          font-weight: 600;
          .giveMeEllipsis(2, 18);
        }
        .nameDate {
          color: #a0a0a0;
          font-size: 12px;
          .name {
            color: @global-purple;
          }
          .date { 
            flex-grow: 0;
            flex-shrink: 0;
          }
        }
      }
    }
    &:nth-child(2) .tipsListBox {
      margin-right: 8px
    }
    &:nth-child(3) .tipsListBox {
      margin: 0 4px;
    }
    &:nth-child(4) .tipsListBox {
      margin-left: 8px;
    }
  }
}
</style>
