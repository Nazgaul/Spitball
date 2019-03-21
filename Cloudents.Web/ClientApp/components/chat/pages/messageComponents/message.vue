<template>
    <div v-html="message.text"></div>
</template>

<script>
export default {
    props:{
        message:{
            type:Object
        }
    },
    computed:{

    }
}
</script>

<style>

</style>
