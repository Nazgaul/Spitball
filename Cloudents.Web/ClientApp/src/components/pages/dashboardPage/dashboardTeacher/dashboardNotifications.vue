<template>
    <div class="dashboardNotifications pa-5 mb-2 mb-sm-4">

        <div class="topHeader d-flex align-center mb-8">
            <fillBellIcon class="flex-shrink-0" width="40" />
            <div class="notificationTitle ms-4 text-truncate" v-t="'dashboardTeacher_notification_title'"></div>
        </div>

        <template v-if="Object.keys(notifications).length">
            <router-link 
                class="center py-4 d-flex align-center justify-space-between"
                v-for="notification in notifyFilter"
                :key="notification.id"
                :to="{ name: notification.routeName }"
            >
                <div class="notifyWrap d-flex align-center" :class="{'ms-2': !notification.amount}">
                    <div class="blueDot" v-if="notification.amount > 0"></div>
                    <bellIcon class="" width="20px" />
                    <div class="notificateText mx-4">{{notification.text}}</div>
                </div>
                <!-- <arrowRight width="20" class="arrowRight d-sm-none" />  -->
            </router-link>
        </template>

        <div class="notificationEmpty" v-else v-t="'notification_empty'"></div>
    </div>
</template>

<script>
import { MessageCenter, MyCourses, MySales } from '../../../../routes/routeNames'
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
        notifications() {
            return this.$store.getters.getUserNotifications
        },
        notifyFilter() {
            return Object.keys(this.notifyItems).filter((n) => {
                if(this.notifyItems[n].amount !== 0) {
                    console.log(this.notifyItems[n].amount);
                    return this.notifyItems[n]
                }
            }).reduce((obj, key) => {
                console.log(key);
                return {
                    ...obj,
                    [key]: this.notifyItems[key]
                };
            }, {});
        },
        notifyItems() {
            let notifylist = this.notifications

            return {
                [constants.CHAT]: {
                    id: 1,
                    text: this.$tc('dashboardTeacher_notify_chat', notifylist[constants.CHAT]),
                    amount: notifylist[constants.CHAT],
                    routeName: MessageCenter
                },
                [constants.BROADCAST]: {
                    id: 2,
                    text: this.$tc('dashboardTeacher_notify_broadcast', notifylist[constants.BROADCAST]),
                    amount: notifylist[constants.BROADCAST],
                    routeName: MyCourses
                },
                [constants.FOLLOWERS]: {
                    id: 3,
                    text: this.$tc('dashboardTeacher_notify_follower', notifylist[constants.FOLLOWERS]),
                    amount: notifylist[constants.FOLLOWERS],
                    routeName: MessageCenter
                },
             
                [constants.PAYMENTS]: {
                    id: 4,
                    text: this.$tc('dashboardTeacher_notify_payments', notifylist[constants.PAYMENTS]),
                    amount: notifylist[constants.PAYMENTS],
                    routeName: MySales
                }
            }
        },
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
        .notificationEmpty {
            font-size: 16px;
            color: @global-purple;
        }
    }
</style>