<template>
    <v-container class="study-rooms-container">
        <v-layout pb-4 class="study-rooms-header-container">
            <v-flex class="study-rooms-header-text" v-language:inner>schoolBlock_my_study_rooms</v-flex>
        </v-layout>
        <v-layout mt-3 class="study-rooms-cards-container">
            <study-card v-for="(card, index) in studyRooms" :key="index" :card="card"></study-card>
            <study-card-tutor v-if="!isTutor"></study-card-tutor>
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
            return this.accountUser.isTutor;
        },
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
    height: 100%;
    width: 100%;
    background-image: url('./img/studyRoomBg.jpg');
    background-size: cover;
    background-position-y: bottom;
    padding-left: 48px;
    color:#fff;
    @media (max-width: @screen-xs) {
        padding: 5px;
    }
    .study-rooms-header-container{
        border-bottom: solid 1px rgba(255, 255, 255, 0.24);
        .study-rooms-header-text{
            font-size: 17px;
            line-height: 2.35;
        }
    }
    .study-rooms-cards-container{
        display: flex;
        flex-wrap: wrap;
    }
}
</style>
