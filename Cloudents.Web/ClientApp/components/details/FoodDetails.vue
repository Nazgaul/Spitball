<template>
    <div>
        <food-map :place="id">

        </food-map>
       <footer v-if="item"> <result-food :item="item"></result-food></footer>
    </div>
</template>

<script>
    import ResultFood from '../results/ResultFood.vue'
    import FoodMap from '../results/foodExtra.vue'
    export default {
        name: "food-details",
        components:{ResultFood,FoodMap},
        props:{id:{type:String},item:{type:Object}}
    }
</script>

<style scoped>

</style>