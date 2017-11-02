<template>
    <div> <div id="map" ref="map"></div></div>
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
        mounted() {
            var map;


                console.log('map')
                var broadway = {
                    info: '<strong>Chipotle on Broadway</strong><br>\
					    5224 N Broadway St<br> Chicago, IL 60640<br>\
					    <a href="https://goo.gl/maps/jKNEDz4SyyH2">Get Directions</a>',
                    lat: 41.976816,
                    long: -87.659916
                };

                var belmont = {
                    info: '<strong>Chipotle on Belmont</strong><br>\
					    1025 W Belmont Ave<br> Chicago, IL 60657<br>\
					    <a href="https://goo.gl/maps/PHfsWTvgKa92">Get Directions</a>',
                    lat: 41.939670,
                    long: -87.655167
                };

                var sheridan = {
                    info: '<strong>Chipotle on Sheridan</strong><br>\r\
					    6600 N Sheridan Rd<br> Chicago, IL 60626<br>\
					    <a href="https://goo.gl/maps/QGUrqZPsYp92">Get Directions</a>',
                    lat: 42.002707,
                    long: -87.661236
                };

                var locations = [
                    [broadway.info, broadway.lat, broadway.long, 0],
                    [belmont.info, belmont.lat, belmont.long, 1],
                    [sheridan.info, sheridan.lat, sheridan.long, 2],
                ];

                var map = new google.maps.Map(document.getElementById('map'), {
                    zoom: 13,
                    center: new google.maps.LatLng(41.976816, -87.659916)
                });

                var infowindow = new google.maps.InfoWindow({});

                var marker, i;

                for (i = 0; i < locations.length; i++) {
                    marker = new google.maps.Marker({
                        position: new google.maps.LatLng(locations[i][1], locations[i][2]),
                        map: map,
                        mapTypeId: google.maps.MapTypeId.ROADMAP
                    });

                    google.maps.event.addListener(marker, 'click', (function (marker, i) {
                        return function () {
                            infowindow.setContent(locations[i][0]);
                            infowindow.open(map, marker);
                        }
                    })(marker, i));
                }
            }

    }
    function initMap() {
        var map;


        console.log('map')
        var broadway = {
            info: '<strong>Chipotle on Broadway</strong><br>\
					    5224 N Broadway St<br> Chicago, IL 60640<br>\
					    <a href="https://goo.gl/maps/jKNEDz4SyyH2">Get Directions</a>',
            lat: 41.976816,
            long: -87.659916
        };

        var belmont = {
            info: '<strong>Chipotle on Belmont</strong><br>\
					    1025 W Belmont Ave<br> Chicago, IL 60657<br>\
					    <a href="https://goo.gl/maps/PHfsWTvgKa92">Get Directions</a>',
            lat: 41.939670,
            long: -87.655167
        };

        var sheridan = {
            info: '<strong>Chipotle on Sheridan</strong><br>\r\
					    6600 N Sheridan Rd<br> Chicago, IL 60626<br>\
					    <a href="https://goo.gl/maps/QGUrqZPsYp92">Get Directions</a>',
            lat: 42.002707,
            long: -87.661236
        };

        var locations = [
            [broadway.info, broadway.lat, broadway.long, 0],
            [belmont.info, belmont.lat, belmont.long, 1],
            [sheridan.info, sheridan.lat, sheridan.long, 2],
        ];

        var map = new google.maps.Map(document.getElementById('map'), {
            zoom: 13,
            center: new google.maps.LatLng(41.976816, -87.659916)
        });

        var infowindow = new google.maps.InfoWindow({});

        var marker, i;

        for (i = 0; i < locations.length; i++) {
            marker = new google.maps.Marker({
                position: new google.maps.LatLng(locations[i][1], locations[i][2]),
                map: map,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            });

            google.maps.event.addListener(marker, 'click', (function (marker, i) {
                return function () {
                    infowindow.setContent(locations[i][0]);
                    infowindow.open(map, marker);
                }
            })(marker, i));
        }
    }

   
</script>



