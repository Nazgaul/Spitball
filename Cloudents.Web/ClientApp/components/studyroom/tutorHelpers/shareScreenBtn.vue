<template>
    <div class="share-screen-btn-wrap">
        <v-flex class="text-center">
            <div v-if="!getIsShareScreen" >
                <!-- <v-tooltip top >
                    <template v-slot:activator="{on}">
                        <div v-on="on"> -->
                            <button :disabled="!isRoomActive" @click="showScreen" class="outline-btn-share">
                                <castIcon class="cast-icon"></castIcon>
                                <span v-language:inner="'tutor_btn_share_screen'"></span>
                            </button>
                        <!-- </div>
                    </template>
                    <span v-language:inner="'tutor_start_to_share'"/>
                </v-tooltip> -->
            </div>
            <button class="outline-btn-share" v-else @click="stopSharing">
                <span v-language:inner="'tutor_btn_stop_sharing'"></span>
            </button>
        </v-flex>
    </div>
</template>

<script>
    import {mapGetters} from "vuex";
    import castIcon from "../images/cast.svg";
    import insightService from '../../../services/insightService';
    export default {
        name: "shareScreenBtn",
        components: {castIcon},
        computed: {
            ...mapGetters(['getIsShareScreen']),
            isRoomActive(){
                return this.$store.getters.getRoomIsActive;
            }
        },
        methods: {
            showScreen() {
                this.$ga.event("tutoringRoom", 'screen share start');
                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_ShareScreenBtn_Click', {id: this.$route.params.id}, null);
                this.$store.dispatch('updateShareScreen',true)
            },
            stopSharing() {
                this.$ga.event("tutoringRoom", 'screen stopSharing');
                this.$store.dispatch('updateShareScreen',false)
            }
        },
    };
</script>

<style lang="less">
    .share-screen-btn-wrap {
        .outline-btn-share {
            display: inline-flex;
            align-items: center;
            justify-content: space-between;
            padding: 8px 12px;
            font-size: 12px;
            line-height: 1.27;
            letter-spacing: 0.5px;
            color: #2d2d2d;
            .cast-icon {
                fill: #2d2d2d;
                margin-right: 4px;
            }
            &[disabled]{
                color: lighten(#2d2d2d, 40%);
                .cast-icon{
                    fill: lighten(#2d2d2d, 40%);
                }
            }
        }

    }
</style>