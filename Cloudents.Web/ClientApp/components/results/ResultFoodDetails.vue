<template>
    <!--<div>
        {{position}}
        <gmap-map :center="center"
                  :zoom="7"
                  style="width: 500px; height: 300px">
            <gmap-marker :position="position"
                         :clickable="true"
                         :draggable="true"
                        ></gmap-marker>
        </gmap-map>
    </div>-->
    
    <gmap-map :center="position"
              :zoom="20"
              
              style="width: 500px; height: 300px">
        <gmap-marker :position="position"
                     :clickable="true"
                     :draggable="true"></gmap-marker>

    </gmap-map>
</template>
<script>
    export default {
        props: { position: { type: Object } }
        //data: {
        //    position: {
        //        lat: this.$route.query.lat,
        //        lng: this.$route.query.long
        //    }
        //}
        //methods: {
        //    goBack() {
        //        window.history.length > 1
        //            ? this.$router.go(-1)
        //            : this.$router.push('/')
        //    }
        //}
    }
</script>