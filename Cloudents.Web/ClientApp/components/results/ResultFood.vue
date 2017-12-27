<template v-once>
    <button type="button" class="d-block pa-2 place-cell" @click="$_clickItem">
        <food-cell :item="item"></food-cell>
    </button>
</template>
<script>
    import StarRating from 'vue-star-rating'
    import FoodDefault from './../navbar/images/food.svg'
    import foodCell from "../food/foodCell.vue"
    export default {
        props: { item: { type: Object, required: true } },
        components: { StarRating, FoodDefault, foodCell },
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
        },
        methods:{
            $_clickItem(){
                if(!this.$vuetify.breakpoint.xsOnly){
                    this.showMap=!this.showMap;
                }else{
                    this.$router.push({name:"foodDetails",params:{item:this.item,id:this.item.placeId}})
                }
            }
        }
    }
</script>
<!--<style src="./ResultFood.less" lang="less"></style>-->
