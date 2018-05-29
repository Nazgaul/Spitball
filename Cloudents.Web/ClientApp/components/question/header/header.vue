<template>
    <regular-header v-if="!$vuetify.breakpoint.xsOnly" ref="mainHeader" :toolbarHeight="72"></regular-header>
</template>

<script>
    import regularHeader from "../../helpers/header.vue"

    export default {
        components: {regularHeader},
    }
</script>
