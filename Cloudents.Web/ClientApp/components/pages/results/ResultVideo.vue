<template>
    <iframe width="500" height="283" :src="item.url" frameborder="0" allowfullscreen></iframe>
</template>


<script>
    export default {
        props: { item: {type:Object,required:true}}
    }
</script>