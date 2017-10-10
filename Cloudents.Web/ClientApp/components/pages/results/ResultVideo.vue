<template>
    <iframe width="420" height="315" :src="item.url" frameborder="0" allowfullscreen></iframe>
</template>


<script>
    export default {
        props: { item: {type:Object,required:true}}
    }
</script>