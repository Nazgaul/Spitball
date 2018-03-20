<template>
    <v-app>
        <router-view name="header"></router-view>
        <router-view></router-view>
    </v-app>
</template>

<script>
    export default {
        name: "app-server"
    }
</script>

<style scoped>

</style>