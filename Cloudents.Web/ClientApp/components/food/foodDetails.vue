<template>
    <div class="food-detail">
        <food-map :place="id">
        </food-map>
        <food-cell class="detail" :item="pageData"></food-cell>
    </div>
</template>

<script>
    //import ResultFood from '../results/ResultFood.vue'
    import foodCell from "./foodCell.vue";
    import FoodMap from '../results/foodExtra.vue'
    import StarRating from 'vue-star-rating';
    import FoodDefault from './../navbar/images/food.svg'
    import { mapMutations } from 'vuex'
    export default {
        name: "food-details",
        components: { foodCell, FoodMap, StarRating, FoodDefault },
        props: { id: { type: String }, item: { type: Object } },
        data() {
            return { pageData: '' }
        },
        methods: {
            ...mapMutations(['UPDATE_LOADING'])
        },
        created() {
            this.UPDATE_LOADING(true);
            this.$store.dispatch("foodDetails", { id: this.id }).then(({ data }) => {
                this.pageData = data;
                this.UPDATE_LOADING(false);
            });
        },
    }
</script>


<style>
    .food-detail {
        height: 100%;
    }

    .food-detail .detail {
        position:fixed;
        background:#fff;
    }
</style>