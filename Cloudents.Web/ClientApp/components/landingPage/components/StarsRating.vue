<template>
    <div class="starts-box">
        <div class="star" v-for="n in stars">
        </div>
    </div>
</template>

<script>
    export default {
        name: "StarsRating",
        props:['stars'],
    }
</script>

<style>

</style>