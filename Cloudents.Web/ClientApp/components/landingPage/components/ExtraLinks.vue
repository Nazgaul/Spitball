<template>
    <div class="extra-links-box">
        <a v-for="(link, index) in links" :key="index" href="#" class="extra-link">{{link.title}}</a>
    </div>
</template>

<script>
    export default {
        name: "ExtraLinks",
        props:['links']
    }
</script>

<style>

</style>