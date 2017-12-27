<template>
    <v-toolbar app fixed height="48" class="elevation-1">
        <v-btn icon @click="$router.go(-1)">
            <v-icon>sbf-arrow-button</v-icon>
        </v-btn>
        <v-toolbar-title>Food & Deals</v-toolbar-title>
    </v-toolbar>
</template>
<style scoped>
    .sbf-arrow-button {
        color: #9B9B9B;
        font-size: 12px;
        transform: rotate(-90deg);
    }

    .toolbar__title {
        font-size: 18px;
        letter-spacing: -0.4px;
        color: #0ead91;
    }

    .toolbar {
        background-color: white;
    }
</style>
