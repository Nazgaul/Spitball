<template>
    <div class="stats-wrap">
        <v-layout v-if="isMobileView" text-xs-center class="stats-container">
            <v-flex v-if="singleStat.id > 1" class="stats-single-item" v-for="(singleStat, index) in statsData"
                    :key="`3${index}`">
                <v-card elevation="0" class="stat-card">
                    <v-card-text class="stat-text-title px-0">{{singleStat.title}}</v-card-text>
                    <v-card-text class="stat-text-data px-0">
                        <tween-number :item="singleStat" :startValue="500"></tween-number>
                    </v-card-text>
                </v-card>
            </v-flex>
        </v-layout>
        <v-layout v-else text-xs-center class="stats-container">
            <v-flex class="stats-single-item" v-for="(singleStat, index) in statsData" :key="`3${index}`">
                <v-card elevation="0" class="stat-card">
                    <v-card-text class="stat-text-title px-0">{{singleStat.title}}</v-card-text>
                    <v-card-text class="stat-text-data px-0">
                        <tween-number :item="singleStat" :startValue="500"></tween-number>
                    </v-card-text>
                </v-card>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    import tweenNumber from "../../results/helpers/tweenNumber/tweenNumber.vue"

    export default {
        name: "statisticsData",
        components: {tweenNumber},
        data() {
            return {};
        },
        computed: {
            isMobileView() {
                return this.$vuetify.breakpoint.width < 1024;
            }
        },

        props: {
            statsData: {
                required: false,
                type: Array,
                default: () => {
                    return [];
                }
            }
        },
    };
</script>

<style scoped lang="less">
    @import "../../../styles/mixin.less";

    .stats-wrap {
        align-items: center;
        justify-content: center;
        display: flex;
        width: 115%;
        padding: 26px 100px;
        @media (max-width: @screen-mds) {
            padding: 0;
        }
    }

    .stats-container {
        flex-direction: row;
        flex-wrap: nowrap;
        max-width: 1260px;
        @media (max-width: @screen-mds) {
            /*flex-wrap: wrap;*/
        }
        .stats-single-item {
            @media (max-width: @screen-mds) {
                flex-basis: 50%;
            }
            .stat-card {
                background: transparent;
                border: none;
                box-shadow: none;
                @media (max-width: @screen-mds) {
                    flex-basis: 50%;
                }
                .stat-text-title {
                    padding-bottom: 2px;
                    font-size: 16px;
                    text-align: center;
                    color: @color-white;
                    @media (max-width: @screen-xs) {
                        font-size: 14px;
                    }
                }
                .stat-text-data {
                    padding-bottom: 4px;
                    font-family: @fontFiraSans;
                    font-size: 32px;
                    letter-spacing: 2.4px;
                    text-align: center;
                    color: @color-white;
                    @media (max-width: @screen-xs) {
                        font-size: 22px;
                        padding: 4px 4px 12px 4px;
                    }
                }
            }
        }
    }
</style>