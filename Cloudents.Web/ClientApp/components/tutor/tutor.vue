<template>
    <v-layout row class="tutoring-page">
        <v-flex>
            <white-board></white-board>
        </v-flex>
        <v-layout column align-start style="position: fixed; right: 0; top: 0;">
            <v-flex xs6 sm6 md6>
                <video-stream :id="id"></video-stream>
            </v-flex>
        </v-layout>
        <v-layout column align-end style="position: fixed; right: 24px; bottom: 12px;">
            <v-flex xs6 sm6 md6>
                <chat v-show="isRoomCreated" :id="id"></chat>
            </v-flex>
        </v-layout>
    </v-layout>


</template>
<script>
    import {mapGetters} from 'vuex';
    import videoStream from './videoStream/videoStream.vue';
    import whiteBoard from './whiteboard/WhiteBoard.vue';
    import chat from './chat/chat.vue';
    export default {
        components:{videoStream, whiteBoard, chat},
        name: "tutor",
        props: {
            id: ''
        },
        computed: {
            ...mapGetters(['isRoomCreated']),
        },

        created() {
            console.log('ID Tutor!!',this.id);
            global.onbeforeunload = function(){
                return "Are you sure you want to close the window?";
            }
        }
    }
</script>

<style lang="less">
    .tutoring-page{
        /*rtl:ignore*/
        direction: ltr;
    }

</style>