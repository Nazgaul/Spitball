<template>
    <question-card :cardData="item" :key="index"></question-card>
</template>

<script>
import questionCard from '../question/helpers/new-question-card/new-question-card.vue';
export default {    
    props:{
         item: {type: Object, required: true},
         index: {Number}
    },
    components:{
        questionCard
    }
}
</script>

<style>

</style>
