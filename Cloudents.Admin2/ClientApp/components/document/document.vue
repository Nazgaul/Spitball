<template>
    <div data-app>
        <div class="document-content">
            <router-view></router-view>
        </div>
    </div>
</template>

<script>
    export default {}
</script>


<style lang="less" scoped>
    .document-nav {
        display: flex;
        justify-content: center;
        .v-tabs__bar {
            background-color: transparent;
        }
        a {
            margin: 2px;
            text-decoration: none;
            font-size: 15px;
            border-radius: 25px;
            padding: 5px;
            font-weight: bold;
        }
        .router-link-active {
            color: rgba(0, 0, 0, 0.87);
        }
    }

    .document-content {
        padding: 5px;
    }
</style>
