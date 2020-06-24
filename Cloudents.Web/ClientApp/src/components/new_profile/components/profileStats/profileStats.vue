<template>
    <div class="profileStats d-flex align-center justify-center pa-2 text-center">
        <div class="stats">
            <div class="number">{{hours}}</div>
            <div class="text">{{$tc('stats_teachs')}}</div>
        </div>
        <div class="stats">
            <div class="number">{{reviews}}</div>
            <div class="text">{{$tc('stats_reviews')}}</div>
        </div>
        <div class="stats">
            <div class="number">{{followers}}</div>
            <div class="text">{{$tc('stats_followers')}}</div>
        </div>
        <div class="stats">
            <div class="number">{{resources}}</div>
            <div class="text">{{$tc('stats_resources')}}</div>
        </div>
    </div>
</template>

<script>
export default {
    name: 'profileStats',
    // data() {
    //     return {

    //     }
    // },
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