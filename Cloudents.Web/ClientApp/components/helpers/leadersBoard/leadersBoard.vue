<template>
    <div class="leaderbox-component">
        <div class="heading" v-show="$vuetify.breakpoint.xsOnly">
            <span class="heading-text" v-language:inner>leadersBoard_title</span>
        </div>
        <v-card class="main-leaders-content">
            <div class="heading" v-show="$vuetify.breakpoint.smAndUp">
                <span class="heading-text" v-language:inner>leadersBoard_title</span>
            </div>

            <v-list two-line class="leaders-list">
                <div class="icon-rounded" v-if="$vuetify.breakpoint.smAndUp">
                    <v-icon class="earn-icon">sbf-icon-earners</v-icon>
                </div>

                <template>
                    <v-list-tile v-for="(leader, index) in leadersList"
                                 v-show="index < leadersLimit"
                                 :key="leader.name"
                                 avatar
                                 @click=""
                                 class="leader-tile">
                        <v-list-tile-avatar class="leader-avatar">
                            <user-avatar :user-name="leader.name" :user-id="leader.userId" :userImageUrl="leader.img"></user-avatar>
                        </v-list-tile-avatar>
                        <v-list-tile-content>
                            <v-list-tile-title class="leader-rank">
                                <user-rank :score="leader.score"></user-rank>

                            </v-list-tile-title>
                            <v-list-tile-sub-title class="leader-university">
                                {{ leader.university}}
                            </v-list-tile-sub-title>
                        </v-list-tile-content>
                        <v-list-tile-action class="leader-ammount">
                            <bdi>
                                <span>${{(parseInt(leader.score)/ 100).toFixed(0) |  commasFilter}}</span>
                            </bdi>
                        </v-list-tile-action>
                    </v-list-tile>
                </template>
            </v-list>
            <div class="total-data">
                    <div class="ml-2">
                        <span class="total-label" :class="$vuetify.breakpoint.xsOnly ? 'mr-5' : 'mr-2'" v-language:inner>leadersBoard_total</span>
                    </div>
                    <div>
                        <span class="total-sum">${{(parseInt(total)/ 100).toFixed(0)  |  commasFilter}}</span>
                    </div>

            </div>

        </v-card>
    </div>

</template>

<script>
    import { mapActions, mapGetters } from "vuex";
    import userAvatar from "../../helpers/UserAvatar/UserAvatar.vue";
    import UserRank from "../../helpers/UserRank/UserRank.vue";

    export default {
        name: "leadersBoard",
        components: {
            userAvatar,
            UserRank

        },
        data() {
            return {
                leaders: {}
            }
        },
        props: {
            propName: {
                type: Number,
                default: 0
            },
        },
        computed: {
            ...mapGetters(["getLeaderBoardState", "LeaderBoardData"]),
            leadersList() {
                return this.LeaderBoardData.leaders
            },
            total() {
                return this.LeaderBoardData.total
            },
            leadersLimit() {
                return this.$vuetify.breakpoint.smAndUp ? 5 : 10
            }
        },
        methods: {
            ...mapActions(["getLeadeBoardData"])
        },
        created() {
            if(this.LeaderBoardData && this.LeaderBoardData.leaders.length === 0 ){
                this.getLeadeBoardData();
            }
        }
    }
</script>

<style lang="less" src="./leadersBoard.less">

</style>