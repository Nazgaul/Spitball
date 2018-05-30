<template>
    <regular-header v-if="!$vuetify.breakpoint.smAndDown" ref="mainHeader" :toolbarHeight="72"></regular-header>
</template>

<script>
    import regularHeader from "../header.vue"

    export default {
        components: {regularHeader},
    }
</script>
