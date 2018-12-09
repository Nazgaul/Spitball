<template>
    <div class="question-container">
        <router-link :to="{path:'/question/'+cardData.id}">
            <h1>{{cardData}}</h1>
        </router-link>
    </div>
</template>

<script>
export default {
props:{
        cardData:{
            required:true
        }
    }   
}
</script>

<style land="less">
    .question-container{
        width: 500px;
    }
</style>
