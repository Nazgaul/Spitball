<template>
    <v-container class="study-rooms-container">
        <v-layout pb-4 class="study-rooms-header-container">
            <v-flex class="study-rooms-header-text">
                My Study Rooms
            </v-flex>
        </v-layout>
        <v-layout mt-3 class="study-rooms-cards-container">
            <study-card v-for="(card, index) in 4" :key="index"></study-card>
        </v-layout>
    </v-container>

</template>

<script>
import studyCard from './studyRoomCard/studyRoomCard.vue'
export default {
    components:{
        studyCard
    }
}
</script>

<style lang="less">
.study-rooms-container{
    height: 100%;
    width: 100%;
    background-color: rgba(60, 59, 83, 0.81);
    padding-left: 80px;
    color:#fff;
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
