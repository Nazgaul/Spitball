<template>
    <iframe 
            frameborder="0" class="ml-2 flex"
            :src="url" allowfullscreen></iframe>

</template>

<script>
    export default {
        props: { place: { type: String } },
        computed: {
            url: function () {
                return "//www.google.com/maps/embed/v1/place?key=AIzaSyCV88wIotTJsgAffA3h5x74WzD_np_WqMU&q=place_id:" + this.place}
        }
    }
</script>
<style scoped>
    iframe {
        width:100%;
        height:50vh; /*TODO: this is a temp solution*/
    }
</style>



