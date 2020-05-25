<template>
    <div class="dashboardNotifications pa-5 mb-2 mb-sm-4">

        <div class="topHeader d-flex align-center mb-10">
            <fillBellIcon width="35" />
            <div class="notificationTitle ml-4" v-t="'dashboardTeacher_notification_title'"></div>
        </div>

        <div class="center d-flex align-center justify-space-between" v-for="(notification, index) in tutorNotifications" :key="index">
            <div class="d-flex align-center">
                <bellIcon class="" width="20px" />
                <div class="notificateText ml-4">dasdsa</div>
            </div>
            <arrowRight width="20" class="arrowRight d-sm-none"/> 
        </div>

    </div>
</template>

<script>
import fillBellIcon from './images/fillBell.svg';
import bellIcon from './images/bell.svg';
import arrowRight from './images/arrow-right-copy-6.svg';

export default {
    name: 'dashboardNotifications',
    components: {
        fillBellIcon,
        bellIcon,
        arrowRight
    },
    computed: {
        tutorNotifications() {
            return [1,2,3]
            return this.$store.getters.getUserNotifications;
        }
    },
    created() {
        // this.$store.dispatch('updateUserNotifications')
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
            &:last-child {
                border-bottom: none;
                padding-bottom: 0;
            }
            .notificateText {
                color: #69687d;
                font-size: 15px;
                font-weight: 600;
            }
            .arrowRight {
                transform: none /*rtl:scaleX(-1)*/;
                vertical-align: bottom;
                width: 10px;
            }
        }
    }
</style>