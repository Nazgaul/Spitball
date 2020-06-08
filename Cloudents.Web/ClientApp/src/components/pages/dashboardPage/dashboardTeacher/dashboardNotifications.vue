<template>
    <div class="dashboardNotifications pa-5 mb-2 mb-sm-4">

        <div class="topHeader d-flex align-center" :class="{'mb-8': tutorNotifications.length}">
            <fillBellIcon class="flex-shrink-0" width="40" />
            <div class="notificationTitle ms-4 text-truncate" v-t="'dashboardTeacher_notification_title'"></div>
        </div>

        <router-link 
            class="center py-4 d-flex align-center justify-space-between"
            v-for="notification in tutorNotifications"
            :key="notification.name"
            :to="{ name: notification.routeName }"
        >
            <div class="notifyWrap d-flex align-center">
                <div class="blueDot"></div>
                <bellIcon class="" width="20px" />
                <div class="notificateText mx-4" v-t="{path: notification.text, args: {0: notification.amount}}"></div>
            </div>
            <arrowRight width="20" class="arrowRight d-sm-none" /> 
        </router-link>

    </div>
</template>

<script>
// import * as routeName from '../../../../routes/routeNames'
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
                [constants.CHAT]: {
                    text: 'chats your did not reply',
                    // text: 'dashboardTeacher_notify_chat',
                    // routeName: routeName.MessageCenter
                },
                [constants.BROADCAST]: {
                    text: 'of students that registered to you live class',
                    // text: 'dashboardTeacher_notify_broadcast',
                    // routeName: routeName.MyFollowers
                },
                [constants.FOLLOWERS]: {
                    text: 'of new followers that do not have a chat ',
                    // text: 'dashboardTeacher_notify_follower',
                    // routeName: routeName.MyFollowers
                },
                [constants.QUESTIONS]: {
                    text: 'questions that you did not answer',
                    // text: 'dashboardTeacher_notify_question',
                    // routeName: routeName.MyContent
                },
                [constants.PAYMENTS]: {
                    text: 'pending payments you did not approve',
                    // text: 'dashboardTeacher_notify_payments',
                    // routeName: routeName.MySales
                }
            }
        }
    },
    computed: {
        // tutorNotificationsFilterList() {
        //     return this.tutorNotifications.filter(notify => notify.value === true) 
        // },
        tutorNotifications() {
            let notifylist = this.$store.getters.getUserNotifications
            return Object.keys(notifylist).map(notify => {
                return {
                    ...this.notifyItems[notify],
                    amount: notifylist[notify],
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
        .center {
            border-bottom: solid 1px #eeeeee;
            position: relative;

            &:last-child {
                border-bottom: none;
                padding-bottom: 0 !important;
            }
            .notifyWrap {
                .blueDot {
                    position: relative;
                    background: #4c59ff;
                    bottom: 8px;
                    left: 20px;
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