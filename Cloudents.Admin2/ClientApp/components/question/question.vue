<template>
    <div>
        <div class="question-content" data-app>
            <router-view></router-view>
        </div>
    </div>
</template>

<script>
export default {
    
}
</script>



<style lang="less" scoped>
    .question-content{
        padding: 5px;
        display: flex;
        width:100%;
    }
</style>
