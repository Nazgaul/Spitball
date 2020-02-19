<template>
    <v-row class="marketingBlogs pa-4" dense>
        <v-col cols="12" class="top pa-0 mb-4 d-flex justify-space-between">
            <div class="mainTitle">{{$t('marketing_blog_title')}}</div>
            <a class="seeAll" href="https://www.blog.spitball.co/blog-1/categories/english" target="_blank">{{$t('dashboardTeacher_see_all')}}</a>
        </v-col>

        <v-col cols="6" class="leftSide  pa-0">
            <div class="wrap">
                <!-- <div>{{$t('marketing_blog_title1')}}</div>
                <div>{{$t('marketing_blog_title2')}}</div> -->
                <div class="text1 mb-2">Why Marketing Through Social Media is a Must </div>
                <div class="text2">10 Reasons Why Marketing Through Social Media is a Must For Every Small Business</div>
            </div>
        </v-col>
        <v-col cols="6" class="d-flex rightSide pa-0">
            <template v-if="!blogs.length">
                <v-col class="tipsList d-flex pa-0" cols="6" v-for="(blog, index) in blogs" :key="index">
                    <a class="tipsListBox" :href="blog.url" target="_blank">
                        <div class="top">
                            <img :src="blog.image" alt="image" width="200" height="100" />
                        </div>
                        <div class="bottom">
                            <div class="text mb-3">{{blog.title}}</div>
                            <div class="nameDate d-flex justify-space-between">
                                <div class="name text-truncate">{{blog.uploader}}</div>
                                <div class="date">{{$d(blog.date, 'short')}}</div>
                            </div>
                        </div>
                    </a>
                </v-col>
            </template>
            
            <template v-else>
                <v-col class="pa-0 skeletonLoader" cols="6" v-for="n in 2" :key="n">
                    <v-skeleton-loader
                    class=""
                    height="200"
                    type="image"
                    ></v-skeleton-loader>
                </v-col>
            </template>
        </v-col>
    </v-row>
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
    .marketingBlogs {
        background: white;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
        border-radius: 8px;
        color: @global-purple;
        @media (max-width: @screen-xs) {
            box-shadow: none;
        }
        .top {
            .mainTitle {
                font-size: 16px;
                font-weight: 600;
            }
              .seeAll {
                color: #4c59ff;
                font-weight: 600;
            }
        }
        .leftSide {
            .wrap {
                display: flex;
                flex-direction: column;
                align-items: center;
                justify-content: center;
                height: 100%;
                max-width: 300px;
                width: 100%;
                .text1 {
                    font-size: 22px;
                    font-weight: 600;
                }
            }
        }
        .rightSide {
            &:not(:last-child) {
                margin-right: 10px;
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
            }
            &:nth-child(3) .tipsListBox:last-child {
                margin-left: 16px;
            }
            .skeletonLoader {
                &:nth-child(1) {
                    margin-right: 8px;
                }
                &:nth-child(2) {
                    margin-left: 8px;
                }
            }
        }
    }
</style>