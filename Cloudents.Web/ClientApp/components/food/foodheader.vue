<template>
    <v-toolbar app fixed height="48" class="elevation-1">
        <v-btn icon @click="$router.go(-1)">
            <v-icon>sbf-chevron-down</v-icon>
        </v-btn>
        <v-toolbar-title>Food & Deals</v-toolbar-title>
    </v-toolbar>
</template>
<style scoped>
    .toolbar .toolbar__content > .btn:first-child {
        margin-left: 2px;
    } 
    .sbf-chevron-down {
        color: #9B9B9B !important;
        font-size: 38px;
        transform: rotate(90deg);
    }

    .toolbar__title {
        margin-left: 0;
        font-size: 18px;
        letter-spacing: -0.4px;
        color: #0ead91;
    }

    .toolbar {
        background-color: white;
    }
</style>
