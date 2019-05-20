<template>
    <v-layout row wrap>
        <v-flex xs12 sm12 md12>
            <userAvatar :user-name="user.name"  :userImageUrl="user.image" :user-id="user.id" ></userAvatar>
        </v-flex>
        <v-flex xs12 sm12 md12></v-flex>
        <v-flex xs12 sm12 md12></v-flex>
        <v-flex xs12 sm12 md12></v-flex>
    </v-layout>
</template>

<script>
    import userAvatar from '../../../helpers/UserAvatar/UserAvatar.vue'
    export default {
        name: "startSession-popUp-student",
        components: {userAvatar},
        data() {
            return {

            };
        },
    };
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
</style>