<template>
    <div>
        <v-tabs centered class="answer-nav" color="transparent">
            <v-tabs-slider color="#3532d5"></v-tabs-slider>
            <v-tab to="/answer/delete">Delete Answer</v-tab>
            <v-tab to="/answer/pendingAnswers">Pending Answers</v-tab>
            <v-tab to="/answer/acceptAnswer">Accept pending Answer</v-tab>
        </v-tabs>
        <div class="answer-content">
            <router-view></router-view>
        </div>
    </div>
</template>

<script>
export default {

}
</script>



<style lang="scss" scoped>
    .answer-nav {
        display: flex;
        justify-content: center;
        .v-tabs__bar

    {
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
        color: rgba(0,0,0,0.87);
    }

    }

    .answer-content {
        padding: 5px;
    }
</style>
