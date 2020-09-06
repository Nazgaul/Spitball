<template>
    <div v-if="broadcastSessions.length">
        <div class="profileBroadcast pa-4 pb-0 pa-sm-0">
            <div class="mainTitle text-sm-center mb-3 mb-sm-5">{{broadCastTitle}}</div>
            <v-divider style="min-height:3px" color="#ff6f30"></v-divider>
            <div v-for="session in sessionsList"
                class="broadcastList"
                :key="session.id">
                <div class="d-sm-flex listWrapper pt-4 pb-2 mb-7 mb-sm-0 py-sm-7 cursor-pointer" @click="goCourseUrl(session)">
                    <div class="leftSide d-sm-flex me-sm-8">
                        <v-skeleton-loader v-if="!isLoaded" width="290" height="192" type="image"></v-skeleton-loader>
                        <img v-show="isLoaded" class="cursor-pointer" @load="loaded" :src="liveImage(session)" alt="" width="290" height="192">
                    </div>
                    <div class="rightSide d-flex flex-column justify-space-between flex-grow-1 px-3 pa-3 pa-sm-0 pt-2 pt-sm-0 pe-0 ps-0 pe-sm-4 pb-0">
                        <div>
                            <div class="occurrenceWrap mb-3">
                                <div class="sessionTitle mb-2 mb-sm-3">{{session.name}}</div>
                                
                                <div v-if="session.studyRoomCount" class="d-flex align-center flex-wrap flex-sm-nowrap">
                                    <div class="d-flex align-center  flex-grow-1 flex-sm-grow-0">
                                        <div class="d-flex align-center">
                                            <div class="startTime">{{$moment(session.startTime).format('ddd, DD MMM')}}</div>
                                        </div>
                                        <div class="d-flex align-center">
                                            <div class="orangeDot"></div>
                                            <div class="startTime">{{$tc('sessionCount', session.studyRoomCount)}}</div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="center" dir="auto">
                                <div class="description pe-4">
                                    <span>{{session.description}}</span>
                                </div>
                            </div>
                        </div>
                        <div class="bottom justify-space-between justify-sm-start d-flex text-center mt-4 mt-sm-5">
                            <div class="subscription">
                                <span v-t="'regular'"></span>
                                <span class="number text-left ms-2 ms-sm-1">{{$price(session.price.amount, session.price.currency, true)}}</span>
                            </div>
                            <div class="subscription ms-0 ms-sm-9" v-if="isTutorSubscription">
                                <span v-t="'subscriber'"></span>
                                <span class="number text-left ms-2 ms-sm-1">{{$price(session.subscriptionPrice.amount, session.subscriptionPrice.currency, true)}}</span>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
        <!-- <div class="showMore text-center pb-10 pb-sm-0 mt-n4 mt-sm-7"  v-if="broadcastSessions.length > 2">
            <v-btn class="showBtn" color="#fff" fab depressed small dark @click="isExpand = !isExpand">
                <arrowDownIcon class="arrowIcon" :class="{'exapnd': isExpand}" width="22" />
            </v-btn>
        </div> -->
        <stripe ref="stripe"></stripe>
    </div>
</template>

<script>
import * as routeNames from '../../../../routes/routeNames';
// import arrowDownIcon from './group-3-copy-16.svg'
import stripe from "../../../pages/global/stripe.vue";

export default {
    name: 'profileLiveClasses',
    components: {
        // arrowDownIcon,
        stripe
    },
    props: {
        userId: {
            required: true
        }
    },
    data() {
        return {
            isLoaded: false,
            // isExpand: false,
        }
    },
    // watch: {
    //     isExpand() {
    //         this.$nextTick(() => {
    //             this.$vuetify.goTo(this.$parent.$refs.profileLiveClassesElement)
    //         })
    //     }
    // },
    computed: {
        broadCastTitle() {
            return this.isMobile ? this.$t('my_live_classes_mobile') : this.$t('my_live_classes')
        },
        isTutorSubscription() {
            return this.$store.getters.getProfileTutorSubscription
        },
        sessionsList() {
            return this.broadcastSessions.map((session, index) => {
                return {
                    ...session,
                    index,
                    isOpen: false
                }
            })
        },
        broadcastSessions() {
            return this.$store.getters.getProfileCourses;
        },
        // liveSessionsList() {
        //     let liveList = this.broadcastSessions
        //     if(this.isExpand) {
        //         return liveList
        //     }
        //     return liveList.slice(0, 2)
        // },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        },
    },
    methods: {
        loaded() {
            this.isLoaded = true
        },
        goCourseUrl(session){
            this.$router.push(
                {
                    name: routeNames.CoursePage,
                    params: {
                        id:session.id,
                        name:session.name
                    }
                }
            )
        },
        liveImage(session) {
            return this.$proccessImageUrl(
                session.image,
                {
                    width:290,
                    height:192,
                })

        },
    },
    created() {
        this.$store.dispatch('updateProfileCourses', this.userId)
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.profileBroadcast {
    max-width: 960px;
    background: #fff;
    margin: 54px auto 0;

    @media(max-width: @screen-xs) {
        margin: 8px auto;
        background: transparent;
    }
    
    .mainTitle {
        font-size: 36px;
        margin: 0 auto;
        font-weight: 600;
        color: #363637;
        
        @media(max-width: @screen-xs) {
            padding: 0;
            font-size: 23px;
            text-align: left;
            max-width: 350px;
            margin: 0;
        }
    }
    .broadcastList {
        .listWrapper{
            text-decoration: initial;
            color: initial;
            // border-top: 2px solid #ff6f30;
            border-bottom: 2px solid #ebecef;
            .leftSide {
                height: 100%;
                img {
                    object-fit: contain;
                    @media(max-width: @screen-xs) {
                        width: 100%;
                        height: 100%;
                    }
                }
            }
            .occurrenceWrap {
                .responsive-property(font-size, 14px, null, 16px);
                .orangeDot {
                    width: 6px;
                    height: 6px;
                    right: 8px;
                    top: 7px;
                    border-radius: 50%;
                    background-color: #ff6f30;
                    margin: 0 14px;
                }
            }
            .rightSide {
                .center {
                    color: #363637;
                    .description {
                        font-size: 15px;
                        line-height: 1.6;
                        color: #363637;
                        white-space: pre-line;
                        .giveMeEllipsis(3, 22px);
                        @media(max-width: @screen-xs) {
                            font-size: 16px;
                        }
                    }
                }
                .bottom {
                    color: #363637;
                    .number {
                        font-size: 18px;
                        font-weight: 600;
                        @media(max-width: @screen-xs) {
                            font-size: 16px;
                        }
                    }
                    .subscription {
                        font-size: 14px;
                    }
                }
            }
        }
    }
    .sessionTitle {
        font-size: 20px;
        font-weight: 600;
        line-height: 1.4;
        color: #363637;
    }
    .startTime{
        font-size: 14px;
        color: #3b3b3c;
        @media(max-width: @screen-xs) {
            font-size: 16px;
        }

    }
}
// .showMore {
//     margin-bottom: 60px;
//     @media (max-width: @screen-xs) {
//         margin-bottom: unset;
//     }
//     .showBtn {
//         border: 1px solid #d4d6da !important;
//     }
//     .arrowIcon {
//         padding-top: 1px;
//         cursor: pointer;
//         &.exapnd {
//             transform: scaleY(-1);
//         }
//     }
// }
</style>