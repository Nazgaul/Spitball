<template v-once>
    <button type="button" class="d-block pa-2 place-cell" @click="showMap=!showMap">
        <v-container class="pa-0">
            <v-layout row>
                <div class="img-wrap">
                    <img :src="item.image" alt="" v-if="item.image">
                    <food-default v-else class="defaultImage spitball-bg-food"></food-default>
                </div>
                <v-flex class="pa-0 ml-2">
                    <v-container class="pa-0 full-height">
                        <v-layout wrap column justify-content-space-between align-item-stretch class="full-height ma-0">
                            <v-flex class="pa-0">
                                <div class="cell-title" >{{item.name}}</div>
                                <div class="rate mb-2">
                                    <span>{{item.rating}}</span>
                                    
                                    <star-rating :inline="true" :star-size="12" 
                                                 :read-only="true" :show-rating="false" 
                                                 active-color="#f6a623"
                                                 inactive-color="#ddd"
                                                 :increment="0.1"
                                                 :rating="item.rating"></star-rating>
                                </div>
                                <div class="address">{{item.address}}</div>
                            </v-flex>
                            <div class="pa-0 bottom" v-if="item.open">Open Now</div>
                        </v-layout>
                    </v-container>
                </v-flex>
                <!--<gmap-map style="width: 100%; height: 100%; position: absolute; left:0; top:0"
                          :center="myLocation"
                          :zoom="12">
                    <gmap-marker :position="myLocation"
                                 :clickable="true"></gmap-marker>

                </gmap-map>-->
                <!--<gmap-map v-show="showMap" :center="pos"
                          :zoom="20"
                          style="width: 500px; height: 300px">
                    <gmap-marker :position="pos"
                                 :clickable="true"></gmap-marker>

                </gmap-map>-->
            </v-layout>
        </v-container>
    </button>
</template>
<script>
    import StarRating from 'vue-star-rating'
    import FoodDefault from './../navbar/images/food.svg'
    export default {
        props: { item: { type: Object, required: true } },
        components: { StarRating,FoodDefault },
        data() { return { showMap: false } },
        computed: {
            myLocation: function () {
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(({ coords }) => {
                        return { lat: Number(coords.latitude), lng: Number(coords.longitude) }
                    })
                }
                return { lat: this.item.location.latitude, lng: this.item.location.longitude }
            },
            //pos: function () { return { lat: this.item.location.latitude, lng: this.item.location.longitude } }
            pos: function () { return { lat: 10.0, lng: 10.0 }}
        }
    }
</script>
<style src="./ResultFood.less" lang="less"></style>
