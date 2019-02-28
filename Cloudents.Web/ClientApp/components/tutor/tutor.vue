<template>
    <video-stream></video-stream>
</template>
<script>
    import videoStream from './videoStream/videoStream.vue'
    export default {
        components:{videoStream},
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