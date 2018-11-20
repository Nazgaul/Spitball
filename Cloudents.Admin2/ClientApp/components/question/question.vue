<template>
    <div>
        <v-tabs centered  class="question-nav"  color="transparent">
            <v-tabs-slider color="#3532d5"></v-tabs-slider>
            <v-tab to="/question/mark">Mark As Correct</v-tab>
            <v-tab to="/question/delete">Delete Question</v-tab>
            <v-tab  to="/question/add">Add Question</v-tab>
            <v-tab to="/question/addBulk">Add Multiple Questions</v-tab>
            <v-tab to="/question/pendingQuestions">Pending Questions</v-tab>
            <v-tab to="/question/acceptQuestion">Accept pending Question</v-tab>
        </v-tabs>


        <!--<nav class="question-nav">-->
            <!--<span></span>-->
            <!--<router-link to="/question/mark">Mark As Correct</router-link> -->
            <!--<router-link to="/question/delete">Delete Question</router-link>-->
            <!--<router-link to="/question/add">Add Question</router-link>-->
            <!--<router-link to="/question/addBulk">Add Multiple Questions</router-link>-->
            <!--<router-link to="/question/pendingQuestions">Pending Questions</router-link>-->
            <!--<router-link to="/question/acceptQuestion">Accept pending Question</router-link>-->
        <!--</nav> -->
        <div class="question-content">
            <router-view></router-view>
        </div>
    </div>
</template>

<script>
export default {
    
}
</script>



<style lang="scss" scoped>
    .question-nav{
        display:flex;
        justify-content: center;
        .v-tabs__bar{
            background-color: transparent;
        }
        a{
            margin:2px;
            /*background-color: #9d9d9d;*/
            text-decoration: none;
            font-size: 15px;
            border-radius: 25px;
            padding: 5px;
            font-weight: bold;
            /*color: #5d5d5d;*/
        }
        .router-link-active{
                color: rgba(0,0,0,0.87);
            }
    }


    .question-content{
        padding: 5px;
    }
</style>
