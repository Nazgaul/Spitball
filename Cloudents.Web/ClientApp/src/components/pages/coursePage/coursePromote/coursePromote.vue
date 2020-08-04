<template>
    <div class="promoteSection pt-3 pb-4 px-5">
        <div class="promoteTitle text--disabled mb-5" v-t="'promote'"></div>
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
</template>

<script>
import couponSvg from './coupon.svg'
import socialSvg from './social.svg'
import inviteSvg from './invite.svg'
import emailSvg from './email.svg'
export default {
    name: 'coursePromote',
    components: {
        couponSvg,
        socialSvg,
        inviteSvg,
        emailSvg
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.promoteSection {
        background: #fff;
        height: max-content;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
        border-radius: 6px;
    .promoteTitle {
        font-size: 20px;
        font-weight: 600;
        color: @global-purple;
    }
    .promoteWrap {
        display: flex;
        align-items: center;
        opacity: 0.38;
        .promoteText {
            font-size: 16px;
            color: @global-purple;
        }
    }
}
</style>