<template>
    <div class="profileStats">
        <div class="statsWrapper d-flex align-center justify-center pt-2 pb-3 text-center">
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
    color: #363637;
    border-bottom: 1px solid rgba(164, 167, 171, .4);
    .statsWrapper {
        max-width: 844px;
        margin: 0 auto;
        .stats {
            position: relative;
            flex: 1;
            @media(max-width: @screen-xs) {
                padding: 0;
            }
            &::after {
                content: '';
                position: absolute;
                border-right: 1px solid #a4a7ab;
                opacity: 0.2;
                border-width: 2px;
                height: 40px;
                right: 0;
                top: 8px;
            }
            &:last-child {
                &::after {
                    border-right: none;
                }
                @media(max-width: @screen-xs) {
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
                font-weight: 600;
            }
            .text {
                font-weight: 500;
                font-size: 14px;
                @media(max-width: @screen-xs) {
                    font-size: 13px;
                }   
            }
        }
    }
}
</style>