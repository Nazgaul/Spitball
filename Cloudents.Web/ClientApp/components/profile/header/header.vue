<template>
    <regular-header v-if="!$vuetify.breakpoint.xsOnly"
                    currentSelection="profile"
                    submitRoute="/profile"></regular-header>
</template>

<script>
    import regularHeader from "../../header/header.vue"

    export default {
        components: {regularHeader},
    }
</script>
