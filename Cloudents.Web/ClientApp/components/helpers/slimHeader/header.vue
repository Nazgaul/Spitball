<template>
    <regular-header v-if="!$vuetify.breakpoint.smAndDown" ref="mainHeader" :toolbarHeight="64" :submitRoute="submitRoute"></regular-header>
</template>

<script>
    import regularHeader from "../header.vue"

    export default {
        components: {regularHeader},
        props:{submitRoute:{type:String,default:'/result'}}
    }
</script>
