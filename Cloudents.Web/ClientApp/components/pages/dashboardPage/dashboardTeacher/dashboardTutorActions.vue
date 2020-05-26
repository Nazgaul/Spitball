<template>
    <div class="dashboardTutorActions pa-5 pb-0 mb-2 mb-sm-4">

        <div class="tutorInfo d-flex align-center justify-space-between">
            <div class="d-flex align-center">
                <userAvatar
                    size="74"
                    :userId="userId"
                    :userName="userName"
                    :userImageUrl="userImage"
                />
                <div class="mx-5">
                    <div class="tutorName mb-2">{{userName}}</div>
                    <button class="tutorUrl">{{userUrl}}</button>
                </div>
                <v-btn class="btn align-self-end" rounded outlined depressed color="#4c59ff" width="120" dense height="34">Edit</v-btn>
            </div>
            <div class="d-none d-sm-block">
                <video src="" width="250" height="150" poster="./images/bitmap.png"></video>
            </div>
        </div>

        <div class="tutorLinks">
            <div class="linkWrap d-flex align-center justify-space-between py-4" v-for="(action, index) in tutorActions" :key="index">
                <div class="linkBorded"></div>
                <div class="link d-flex align-center">
                    <circleArrow class="arrowIcon" width="23" :stroke="action.color || 'blue'" />
                    <div class="ml-4">{{action.text}}</div>
                </div>
                <v-btn class="btn" rounded outlined depressed color="#4c59ff" width="120" :to="action.routeName" v-t="'dashboardTeacher_btn_text'"></v-btn>
            </div>
        </div>

        <analyticOverview></analyticOverview>

    </div>
</template>

<script>
const analyticOverview = () => import(/* webpackChunkName: "analyticsOverview" */'../../global/analyticOverview/analyticOverview.vue');
import circleArrow from './images/circle-arrow.svg';

import constants from '../../../../store/constants/dashboardConstants';

export default {
    name: 'dashboardTutorActions',
    components: {
        analyticOverview,
        circleArrow
    },
    data() {
        return {
            items: {
                [constants.UPLOAD]: {
                    color: '#4c59ff',
                    text: 'dashboardTeacher_link',
                    routeName: 'my-content'
                },
                [constants.CALENDAR]: {
                    color: '#4c59ff',
                    text: 'dashboardTeacher_link',
                    routeName: 'my-calendar'
                },
                [constants.TEACH]: {
                    color: '#4c59ff',
                    text: 'dashboardTeacher_link',
                    routeName: ''
                },
                [constants.SESSIONS]: {
                    color: '#41c4bc',
                    text: 'dashboardTeacher_link',
                    routeName: ''
                },
                [constants.MARKETING]: {
                    color: '#41c4bc',
                    text: 'dashboardTeacher_link',
                    routeName: ''
                },
                [constants.BOOK]: {
                    color: '#eac569',
                    text: 'dashboardTeacher_link',
                    routeName: ''
                },
            }
        }
    },
    computed: {
        tutorActions() {
            let list = this.$store.getters.getTutorListActions
    
            return list.map(l => {
                debugger
                return {
                    ...items[l]
                }
            })

            console.log(list);
            // return list
        },
        userId() {
            return this.$store.getters.getAccountId
        },
        userName() {
            return this.$store.getters.getAccountName
        },
        userImage() {
            return this.$store.getters.getAccountImage
        },
        userUrl() {
            return `${window.location.origin}/${this.userName}`
        }
    },
    created() {
        this.$store.dispatch('updateTutorLinks')
    }
}
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    @import '../../../../styles/colors.less';

    .dashboardTutorActions {
        background: #fff;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
        border-radius: 8px;
        margin: 0 auto;
        @media (max-width: @screen-xs) {
            box-shadow: none;
            padding: 14px;
            border-radius: 0;
        }
        .tutorInfo {
            border-bottom: 2px solid #f5f5f5;
            font-weight: 600;
            .tutorName {
                font-size: 22px;
                color: @global-purple;
            }
            .tutorUrl {
                color: @global-auth-text;
                outline: none;
            }
        }
        .tutorLinks {

            .linkWrap {
                position: relative;
                border-bottom: 2px solid #f5f5f5;
                .linkBorded {
                    position: absolute;
                    left: -20px;
                    width: 5px;
                    height: 45px;
                    background: blue;
                    border-radius: 0 6px 6px 0;
                }
                .link {
                    font-size: 15px;
                    color:#69687d;
                    font-weight: 600;

                    .arrowIcon {
                        transform: none /*rtl:scaleX(-1)*/;
                    }
                }
            }
        }
        .btn {
            font-weight: 600;
        }
    }
</style>