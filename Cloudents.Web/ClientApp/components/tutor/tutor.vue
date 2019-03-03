<template>
    <v-layout row>
        <v-flex>
            <white-board></white-board>
        </v-flex>
      <v-flex>
          <video-stream :id="id"></video-stream>
      </v-flex>

    </v-layout>


</template>
<script>
    import videoStream from './videoStream/videoStream.vue'
    import whiteBoard from './whiteboard/WhiteBoard.vue'
    export default {
        components:{videoStream, whiteBoard},
        name: "tutor",
        props: {
            id: ''
        }, 
        created() {
            console.log('ID Tutor!!',this.id)
        }
    }
</script>

<style scoped>

</style>