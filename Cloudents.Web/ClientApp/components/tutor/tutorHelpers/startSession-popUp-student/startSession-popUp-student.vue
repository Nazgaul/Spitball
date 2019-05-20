<template>
    <v-layout row wrap>
        <v-flex xs12 sm12 md12>
            <v-icon>sbf-close</v-icon>
        </v-flex>
        <v-flex xs12 sm12 md12>
            <userAvatar :user-name="user.name"  :userImageUrl="user.image" :user-id="user.id" ></userAvatar>
        </v-flex>
        <v-flex xs12 sm12 md12>
            <span class="subheading font-weight-bold">Your tutor, Tony Stark</span>
            <span class="subheading">has entered the room</span>
        </v-flex>
        <v-flex xs12 sm12 md12>
            <v-btn class="start-session-btn">
                <timerIcon class="timer-icon"></timerIcon>
                <span class="text-uppercase">Accept and start</span>
            </v-btn>
        </v-flex>
    </v-layout>
</template>

<script>
    import userAvatar from '../../../helpers/UserAvatar/UserAvatar.vue';
    import timerIcon from '../../images/timer.svg';
    export default {
        name: "startSession-popUp-student",
        components: {userAvatar, timerIcon},
        data() {
            return {

            };
        },
    };
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
</style>