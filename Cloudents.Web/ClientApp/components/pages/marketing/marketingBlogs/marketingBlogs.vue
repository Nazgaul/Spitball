<template>
    <div class="spitballBlogs fullWidth">

        <div class="headerBlogs pa-0 mb-4 d-flex justify-space-between">
          <div class="titleBlog">{{$t('dashboardTeacher_spitball_tips')}}</div>
          <a  class="seeAllBlog d-sm-block d-none" href="https://www.blog.spitball.co/blog-1/categories/english" target="_blank">{{$t('dashboardTeacher_see_all')}}</a>
        </div>

        <div class="mainBlogs d-flex-column d-sm-flex">
            <div class="leftBlogs pt-sm-5 pt-2 mr-2">
                <div class="titleWrap">
                    <div class="title1 mb-2">
                      Why Marketing Through Social Media is a Must
                    </div>
                    <div class="title2">
                      10 Reasons Why Marketing Through Social Media is a Must For Every Small Business
                    </div>
                </div>
            </div>

            <div class="rightBlogs d-flex-column d-sm-flex pa-0 mt-6 mt-sm-0">
                <div class="mb-3 d-block text-right" v-if="$vuetify.breakpoint.xsOnly">
                  <a class="seeAllBlog text-right" href="https://www.blog.spitball.co/blog-1/categories/english" target="_blank">{{$t('dashboardTeacher_see_all')}}</a>
                </div>
                <a class="linkBlog d-flex d-sm-block" :href="blog.url" target="_blank" v-for="(blog, index) in blogs" :key="index">
                    <div class="top mr-2 mr-sm-0">
                        <img :src="blog.image" alt="image" />
                    </div>
                    <div class="bottom">
                        <div class="text mb-3">{{blog.title}}</div>
                        <div class="nameDate d-flex justify-space-between">
                            <div class="name text-truncate">{{blog.uploader}}</div>
                            <div class="date">{{$d(new Date(blog.create), 'short')}}</div>
                        </div>
                    </div>
                </a>
            </div>
        </div>
        
    </div>
</template>

<script>
export default {
    data: () => ({
        blogs: []
    }),
    methods: {
        getBlogs() {
            let self = this;
            self.$store.dispatch('updateMarketingBlogs').then(blogs => {
                self.blogs = blogs
            }).catch(ex => {
                self.$appInsights.trackException({exception: new Error(ex)});
            })
        }
    },
    created() {
        this.getBlogs()
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';
.spitballBlogs {
  padding: 16px;
  background: white;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  border-radius: 8px;

  @media (max-width: @screen-xs) {
    box-shadow: none;
    border-radius: 0;
  }
  .headerBlogs {
      .titleBlog {
        color: @global-purple;
        font-weight: 600;
        font-size: 16px;
      }
   }
  .seeAllBlog {
    color: #4c59ff;
    font-weight: 600;
  }
  .mainBlogs {

    .leftBlogs {
      .titleWrap {
        max-width: 320px;
        @media (max-width: @screen-xs) {
          &:first-child {
            max-width: 100%;
          }
        }
        .title1 {
           font-size: 22px;
           font-weight: 600;
           letter-spacing: normal;
           color: @global-purple;
        }
        .title2 {
            line-height: 1.6;
        }
      }
    }
    .rightBlogs {
      margin: 0 0 0 auto;
      .linkBlog {
        width: 200px;
        margin: 0 8px;
        @media (max-width: @screen-xs) {
          border-top: 1px solid #dddddd;
          padding: 12px 0;
          margin: 0;
          width: 100%;
        }
        &:first-child {
          margin-left: 0;
        }
        &:last-child {
          margin-right: 0;
        }
        @media (max-width: @screen-sm) {
          &:first-child {
            display: none !important;
          }
        }
        @media (max-width: @screen-xs) {
          &:first-child {
            display: flex !important;
          }
          &:last-child {
            padding-bottom: 0;
          }
        }
        .top {
          display: flex;
          img {
            width: 100%;
          }
          @media (max-width: @screen-xs) {
            img {
              width: 100px;
            }
          }
        }
        .bottom {
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            border: 1px solid #dddddd;
            padding: 8px 10px;
            font-size: 13px;
            color: @global-purple;
            min-width: 0;
            @media (max-width: @screen-xs) {
                width: 100%;
                border: none;
                padding: 0;
            }
            .text {
                .giveMeEllipsis(2, 18);
                min-height: 36px;
                @media (max-width: @screen-xs) {
                    font-size: 14px;
                    font-weight: 600;
                }
            }
            .nameDate {
              opacity: 0.6;
              font-size: 12px;
              .date {
                  flex-shrink: 0;
                  margin-left: 4px;
              }
            }
        }
      }
    }
  }
}
</style>