<template>
    <div>
        <v-tabs centered  class="document-nav"  color="transparent">
            <v-tabs-slider color="#3532d5"></v-tabs-slider>
            <v-tab to="/document/approveDelete">Approve or Delete</v-tab>
            <v-tab to="/document/documentDelete">Delete By ID</v-tab>
        </v-tabs>
        <div class="document-content">
            <router-view></router-view>
        </div>
    </div>
</template>

<script>
    export default {

    }
</script>



<style lang="scss" scoped>
    .document-nav{
        display:flex;
        justify-content: center;
        .v-tabs__bar{
            background-color: transparent;
        }
        a{
            margin:2px;
            text-decoration: none;
            font-size: 15px;
            border-radius: 25px;
            padding: 5px;
            font-weight: bold;
        }
        .router-link-active{
            color: rgba(0,0,0,0.87);
        }
    }
    .document-content{
        padding: 5px;
    }
</style>
