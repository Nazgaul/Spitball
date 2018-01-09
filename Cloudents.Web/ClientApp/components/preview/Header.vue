<template>
    <v-toolbar app fixed :height="60" class="header">
        <v-layout column class="header-elements">
            <main-header></main-header>
        </v-layout>
    </v-toolbar>
</template>

<script>
    import mainHeader from '../helpers/header.vue'
    export default {
        components:{mainHeader}
    }
</script>
