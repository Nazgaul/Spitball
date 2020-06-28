<template>
    <div class="profileStats d-flex align-center justify-center pa-2 text-center">
        <div class="stats">
            <div class="number">{{hours}}</div>
            <div class="text">{{$tc('stats_teachs', hours)}}</div>
        </div>
        <div class="stats">
            <div class="number">{{reviews}}</div>
            <div class="text">{{$tc('stats_reviews', reviews)}}</div>
        </div>
        <div class="stats">
            <div class="number">{{followers}}</div>
            <div class="text">{{$tc('stats_followers', followers)}}</div>
        </div>
        <div class="stats">
            <div class="number">{{resources}}</div>
            <div class="text">{{$tc('stats_resources', resources)}}</div>
        </div>
    </div>
</template>

<script>
export default {
    name: 'profileStats',
    computed: {
        hours() {
            return this.$store.getters.getProfileStatsHours
        },
        reviews() {
            return this.$store.getters.getProfileStatsReviews
        },
        followers() {
            return this.$store.getters.getProfileStatsFollowers
        },
        resources() {
            return this.$store.getters.getProfileStatsResources
        },
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
// @import '../../../../styles/colors.less';

.profileStats {
    position: relative;
    color: #363637;
    &::after {
        content: '';
        position: absolute;
        opacity: 0.4;
        border-bottom: 1px solid #a4a7ab;
        bottom: 0;
        left: 0;
        right: 0;
    }
    .stats {
        position: relative;
        padding: 0 90px;
        font-weight: 600;
        @media(max-width: @screen-xs) {
            padding: 0;
            flex: 1;
        }
        &::after {
            content: '';
            position: absolute;
            border-right: 1px solid #a4a7ab;
            opacity: 0.4;
            border-width: 2px;
            height: 40px;
            right: 0;
            top: 8px;
        }
        &:last-child {
            &::after {
                border-right: none;
            }
            // max-width: 374 by shiran design
            @media(max-width: @screen-xss - 1) {
                display: none;
            }
        }
        &:nth-child(3) {
            &::after {
                @media(max-width: @screen-xss - 1) {
                    border-right: none;
                }                
            }
        }
        .number {
            font-size: 26px;
        }
        .text {
            font-size: 14px;
        }
    }
}
</style>