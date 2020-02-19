<template>
    <div>
        <div class="user-content" data-app>
            <router-view></router-view>
        </div>
    </div>
</template>

<script>
export default {
    
}
</script>



<style lang="less" scoped>
    .user-nav{
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


    .user-content{
        padding: 5px;
        width: 100%;
    }
</style>
