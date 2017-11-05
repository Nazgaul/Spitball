<template>
    <iframe width="600" 
            height="450"
            frameborder="0" class="pa-2 pl-5 offset-xs3 flex" style="position:fixed"
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



