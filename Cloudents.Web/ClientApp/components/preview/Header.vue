<template>
    <v-toolbar app fixed :height="$vuetify.breakpoint.mdAndUp ? 60 : 115" class="header">
            <main-header></main-header>
    </v-toolbar>
</template>

<script>
    import mainHeader from '../helpers/header.vue'
    export default {
        components:{mainHeader}
    }
</script>
