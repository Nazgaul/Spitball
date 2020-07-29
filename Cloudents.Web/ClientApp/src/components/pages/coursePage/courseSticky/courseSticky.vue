<template>
    <div class="courseSticky ms-6">
        <div class="visibleSection d-flex align-center justify-space-between pa-3 mb-4">
            <div class="courseStickyTitle" v-t="'visible'"></div>
            <v-switch
                v-model="visible"
                class="ma-0 pa-0"
                hide-details
            ></v-switch>
        </div>

        <div class="promoteSection pt-3 pb-4 px-5">
            <div class="promoteTitle mb-5" v-t="'promote'"></div>
            <div class="promoteWrap mb-5">
                <couponSvg width="20" />
                <div class="promoteText ps-5" v-t="'create_coupon'"></div>
            </div>
            <div class="promoteWrap mb-5">
                <socialSvg width="20" />
                <div class="promoteText ps-5" v-t="'share_social'"></div>
            </div>
            <div class="promoteWrap mb-5">
                <inviteSvg width="20" />
                <div class="promoteText ps-5" v-t="'invite_course'"></div>
            </div>
            <div class="promoteWrap">
                <emailSvg width="20" />
                <div class="promoteText ps-5" v-t="'send_email'"></div>
            </div>
        </div>
    </div>
</template>

<script>
import couponSvg from './coupon.svg'
import socialSvg from './social.svg'
import inviteSvg from './invite.svg'
import emailSvg from './email.svg'
export default {
    name: 'courseSticky',
    components: {
        couponSvg,
        socialSvg,
        inviteSvg,
        emailSvg
    },
    data() {
        return {
            visible: false,
        }
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.courseSticky {
    min-width: 296px;
    height: max-content;
    position: sticky;
    top: 170px;
    .visibleSection, .promoteSection{
        background: #fff;
        height: max-content;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
        border-radius: 6px;
    }
    .visibleSection {
        .courseStickyTitle {
            font-size: 16px;
            color: @global-purple;
        }
    }
    .promoteSection {
        .promoteTitle {
            font-size: 20px;
            font-weight: 600;
            color: @global-purple;
        }
        .promoteWrap {
            display: flex;
            align-items: center;
        }
        .promoteText {
            font-size: 16px;
            color: @global-purple;
        }
    }
}
</style>