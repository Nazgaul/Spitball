<template v-once>
    <button type="button" class="d-block pa-2" @click="$_clickItem">
        <food-cell :item="item"></food-cell>
    </button>
</template>
<script>
    import StarRating from 'vue-star-rating'
    import FoodDefault from './svg/food.svg'
    import foodCell from "./foodCell.vue"
    import hookedLogo from "./svg/hooked-logo.svg"
    export default {
        props: { item: { type: Object, required: true },index:{Number} },
        components: { StarRating, FoodDefault, foodCell, hookedLogo },
        data() { return { showMap: false } },
        methods: {
            $_clickItem() {
            this.$ga.event('Search_Results', 'food',`#${this.index+1}_${this.item.placeId}`);
                if (!this.$vuetify.breakpoint.xsOnly) {
                    this.showMap = !this.showMap;
                } else {
                    this.$router.push({ name: "foodDetails", params: { item: this.item, id: this.item.placeId } })
                }
            }
        }
    }
</script>
<!--<style src="./ResultFood.less" lang="less"></style>-->
