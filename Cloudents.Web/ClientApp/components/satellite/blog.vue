<template>
    <general-page>
        <iframe src="https://spitballco.wordpress.com/"></iframe>
    </general-page>
</template>


<script>
    import generalPage from "./general.vue";

    export default {
        components: { generalPage }
         
    }
</script>
<style scoped>
    iframe {
        width: 100%;
        height:100vh;
        border:0;
    }
</style>
