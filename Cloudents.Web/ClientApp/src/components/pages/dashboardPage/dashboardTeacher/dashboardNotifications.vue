<template>
    <div class="dashboardNotifications pa-5 mb-2 mb-sm-4">

        <div class="topHeader d-flex align-center mb-8">
            <fillBellIcon class="flex-shrink-0" width="40" />
            <div class="notificationTitle ms-4 text-truncate" v-t="'dashboardTeacher_notification_title'"></div>
        </div>

        <router-link 
            class="center py-4 d-flex align-center justify-space-between"
            tag="div"
            v-for="notification in notifyItems"
            :key="notification.name"
            :to="{ name: notification.routeName }"
        >
            <div class="notifyWrap d-flex align-center" :class="{'ms-2': !notification.amount}">
                <div class="blueDot" v-if="notification.amount > 0"></div>
                <bellIcon class="" width="20px" />
                <div class="notificateText mx-4">{{notification.text}}</div>
            </div>
            <!-- <arrowRight width="20" class="arrowRight d-sm-none" />  -->
        </router-link>

    </div>
</template>

<script>
// import * as routeName from '../../../../routes/routeNames'
import constants from '../../../../store/constants/dashboardConstants'

import fillBellIcon from './images/fillBell.svg'
import bellIcon from './images/bell.svg'
// import arrowRight from './images/arrow-right-copy-6.svg'

export default {
    name: 'dashboardNotifications',
    components: {
        fillBellIcon,
        bellIcon,
        // arrowRight
    },
    data() {
        return {

        }
    },
    computed: {
        notifyItems() {
            let notifylist = this.$store.getters.getUserNotifications

            return {
                [constants.CHAT]: {
                    text: this.$tc('dashboardTeacher_notify_chat', notifylist[constants.CHAT]),
                    amount: notifylist[constants.CHAT]
                    // routeName: routeName.MessageCenter
                },
                [constants.BROADCAST]: {
                    text: this.$tc('dashboardTeacher_notify_broadcast', notifylist[constants.BROADCAST]),
                    amount: notifylist[constants.BROADCAST]
                    // routeName: routeName.MyFollowers
                },
                [constants.FOLLOWERS]: {
                    text: this.$tc('dashboardTeacher_notify_follower', notifylist[constants.FOLLOWERS]),
                    amount: notifylist[constants.FOLLOWERS]
                    // routeName: routeName.MyFollowers
                },
             
                [constants.PAYMENTS]: {
                    text: this.$tc('dashboardTeacher_notify_payments', notifylist[constants.PAYMENTS]),
                    amount: notifylist[constants.PAYMENTS]
                    // routeName: routeName.MySales
                }
            }
        },
        // tutorNotificationsFilterList() {
        //     return this.tutorNotifications.filter(notify => notify.value === true) 
        // },
        // tutorNotifications() {
        //     let notifylist = this.$store.getters.getUserNotifications
        //     return Object.keys(notifylist).map(notify => {
        //         return {
        //             ...this.notifyItems[notify],
        //             amount: notifylist[notify],
        //             name: notify
        //         }
        //     })
        // }
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
            // .arrowRight {
            //     transform: none /*rtl:scaleX(-1)*/;
            //     vertical-align: bottom;
            //     width: 10px;
            // }
        }
    }
</style>