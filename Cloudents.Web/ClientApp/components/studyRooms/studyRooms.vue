<template>
    <v-container class="study-rooms-container">
        <v-layout pb-4 class="study-rooms-header-container">
            <v-flex class="study-rooms-header-text" v-language:inner>schoolBlock_my_study_rooms</v-flex>
        </v-layout>
        <v-layout mt-4 class="study-rooms-cards-container">
            <study-card xs6 v-for="(card, index) in studyRooms" :key="index" :card="card"></study-card>
            <v-spacer style="max-width:170px;" xs6 v-show="studyRooms.length % 2 !== 0 && $vuetify.breakpoint.xsOnly"></v-spacer>
            <study-card-tutor v-if="!isTutor && isTutorPending"></study-card-tutor>
        </v-layout>
    </v-container>

</template>

<script>
import studyCard from './studyRoomCard/studyRoomCard.vue'
import studyCardTutor from './studyRoomCard/studyRoomCardTutor.vue'
import {mapActions, mapGetters} from 'vuex'
export default {
    components:{
        studyCard,
        studyCardTutor
    },
    data(){
        return {
        }
    },
    computed:{
        ...mapGetters(['getStudyRooms', 'accountUser']),
        studyRooms(){
            return this.getStudyRooms;
        },
        isTutor(){
            if(this.accountUser) {
                return this.accountUser.isTutor;
            }
        },
        isTutorPending() {
            return this.accountUser.isTutorState !== 'pending' ? true : false;
        }
    },
    methods:{
        ...mapActions(['fetchStudyRooms']),
    },
    created(){
        this.fetchStudyRooms();
    }
}
</script>

<style lang="less">
@import '../../styles/mixin.less';
.study-rooms-container{
    max-width: unset;
    height: 100%;
    width: 100%;
    background-image: url('./img/studyRoomBg.jpg');
    background-size: cover;
    background-position-y: bottom;
    padding: 24px 48px; //padding left somehow the rtl doesnt process it. better solution
    color:#fff;
    @media (max-width: @screen-xs) {
        padding: 6px;
    }
    .study-rooms-header-container{
        border-bottom: solid 1px rgba(255, 255, 255, 0.24);
        @media (max-width: @screen-xs) {
            padding-left: 20px;
            padding-top: 16px;
            padding-bottom: 16px !important;
        }
        
        .study-rooms-header-text{
            font-size: 17px;
            line-height: 2.35;
        }
    }
    .study-rooms-cards-container{
        display: flex;
        flex-wrap: wrap;
        @media (max-width: @screen-xs) {
            justify-content: space-around;
        }
    }
}
</style>
