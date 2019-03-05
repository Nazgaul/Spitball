<template>
    <v-layout row>
        <v-flex>
            <white-board></white-board>
        </v-flex>
        <v-layout column align-end style="position: fixed; right: 0; top: 0; bottom: 0;">
            <v-flex xs6 sm6 md6>
                <video-stream :id="id"></video-stream>
            </v-flex>
            <v-flex xs6 sm6 md6>
                <chat :id="id"></chat>
            </v-flex>
        </v-layout>
    </v-layout>


</template>
<script>
    import videoStream from './videoStream/videoStream.vue';
    import whiteBoard from './whiteboard/WhiteBoard.vue';
    import chat from './chat/chat.vue'
    export default {
        components:{videoStream, whiteBoard, chat},
        name: "tutor",
        props: {
            id: ''
        },

        created() {
            console.log('ID Tutor!!',this.id)
            global.onbeforeunload = function(){
                return "Are you sure you want to close the window?";
            }
        }
    }
</script>

<style scoped>

</style>