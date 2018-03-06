<template>
    <iframe class="cell-video" height="283" :src="item.url" frameborder="0" allowfullscreen></iframe>
</template>


<script>
    export default {
        props: { item: {type:Object,required:true}}
    }
</script>
<style scoped>
    iframe {
        width: 504px;
        height:283px;
    }
    @media only screen and (max-device-width : 600px) {
        iframe {
            width: 360px;
            height: 202px;
        }
    }
</style>