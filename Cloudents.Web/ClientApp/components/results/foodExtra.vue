<template>
    <!--<gmap-map style="width: 100%; height: 100%; position: absolute; left:0; top:0"
                          :center="myLocation"
                          :zoom="12">
        <template v-for="item in items">
            <gmap-marker :position="{lat:item.location.latitude,lng:item.location.longitude}"
                         :clickable="true"></gmap-marker>
        </template>
    </gmap-map>-->
    <!--<iframe width="600"
            height="450"
            frameborder="0" style="border:0"
            :src="url" allowfullscreen></iframe>-->

</template>
<script>
    export default {
        //props: { term: { type: String } },
        //computed: {
        //    url: function () {
        //        var places="place_id:%5B%5D=ChIJwQY8o4Wj6IgRMng3R_e78Vk&place_id:%5B%5D=ChIJS2sniyWj6IgRjQgxMUqJ1NU"
        //        return "https://www.google.com/maps/embed/v1/place?key=AIzaSyCzONvUXR9mUr9xtVKWJV2eWHA010hCSTg&q=" + places;
        //    }
        //}
        //data() {
        //    if (navigator.geolocation) {
        //        navigator.geolocation.getCurrentPosition(({ coords }) => {
        //            this.myLocation = { lat: coords.latitude, lng: coords.longitude }
        //        })
        //            }
           
        //    return { myLocation: this.items[0].location  }
        //},
        //created() {
        //    if (navigator.geolocation) {
        //        navigator.geolocation.getCurrentPosition(({ coords }) => {
        //            this.myLocation = { lat: coords.latitude, lng: coords.longitude }
        //        })
        //    }
        //}
    }
</script>