<template>
    <div>
        <nav class="question-nav">
            <span></span>
            <router-link to="/question/mark">Mark As Correct</router-link> 
            <router-link to="/question/delete">Delete Question</router-link>
            <router-link to="/question/add">Add Question</router-link>
            <router-link to="/question/addBulk">Add Multiple Questions</router-link>
            <router-link to="/question/acceptQuestion">Accept pending Question</router-link>
        </nav> 
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
        a{
            margin:2px;
            background-color: #9d9d9d;
            text-decoration: none;
            font-size: 15px;
            border-radius: 25px;
            padding: 5px;
            font-weight: bold;
            color: #5d5d5d;
        }
        .router-link-active{
                color:#97ed82;
            }
    }


    .question-content{
        padding: 5px;
    }
</style>
