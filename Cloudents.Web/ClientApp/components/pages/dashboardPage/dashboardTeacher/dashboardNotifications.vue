<template>
    <div class="dashboardNotifications pa-5 mb-2 mb-sm-4">

        <div class="topHeader d-flex align-center mb-10">
            <fillBellIcon class="flex-shrink-0" width="35" />
            <div class="notificationTitle ms-4 text-truncate" v-t="'dashboardTeacher_notification_title'"></div>
        </div>

        <router-link 
            class="center d-flex align-center justify-space-between"
            v-for="notification in tutorNotifications"
            :key="notification.name"
            :to="{ name: notification.routeName }"
        >
            <div class="notifyWrap d-flex align-center">
                <div class="blueDot"></div>
                <bellIcon class="" width="20px" />
                <div class="notificateText text-truncate mx-4" v-t="notification.text"></div>
            </div>
            <arrowRight width="20" class="arrowRight d-sm-none" /> 
        </router-link>

    </div>
</template>

<script>
import * as routeName from '../../../../routes/routeNames'
import constants from '../../../../store/constants/dashboardConstants'

import fillBellIcon from './images/fillBell.svg'
import bellIcon from './images/bell.svg'
import arrowRight from './images/arrow-right-copy-6.svg'

export default {
    name: 'dashboardNotifications',
    components: {
        fillBellIcon,
        bellIcon,
        arrowRight
    },
    data() {
        return {
            notifyItems: {
                [constants.FOLLOWERS]: {
                    text: 'dashboardTeacher_notify_follower',
                    routeName: routeName.MyFollowers
                },
                [constants.QUESTIONS]: {
                    text: 'dashboardTeacher_notify_question',
                    routeName: routeName.MyContent
                },
                [constants.PAYMENTS]: {
                    text: 'dashboardTeacher_notify_payments',
                    routeName: routeName.MySales
                }
            }
        }
    },
    computed: {
        tutorNotifications() {
            let notifylist = this.$store.getters.getUserNotifications
            
            return Object.keys(notifylist).map(notify => {
                return {
                    ...this.notifyItems[notify],
                    name: notify
                }
            })
        }
    },
    created() {
        this.$store.dispatch('updateTutorNotifications')
    }
}
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    @import '../../../../styles/colors.less';

    .dashboardNotifications {
        background: #fff;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
        border-radius: 8px;
        margin: 0 auto;
        @media (max-width: @screen-xs) {
            box-shadow: none;
            padding: 14px;
            border-radius: 0;
        }

        .notificationTitle {
            color: @global-purple;
            font-size: 18px;
            font-weight: 600;
        }

        .topHeader {

        }

        .center {
            border-bottom: solid 1px #eeeeee;
            padding: 16px 0;
            position: relative;

            &:last-child {
                border-bottom: none;
                padding-bottom: 0;
            }
            .notifyWrap {
                min-width: 0;;
                .blueDot {
                    position: absolute;
                    background: #4c59ff;
                    top: 18px;
                    left: 12px;
                    width: 8px;
                    height: 8px;
                    border-radius: 50%;
                }
                .notificateText {
                    color: #69687d;
                    font-size: 15px;
                    font-weight: 600;
                }
            }
            .arrowRight {
                transform: none /*rtl:scaleX(-1)*/;
                vertical-align: bottom;
                width: 10px;
            }
        }
    }
</style>