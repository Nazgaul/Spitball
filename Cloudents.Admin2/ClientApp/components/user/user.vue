<template>
    <div>
        <nav class="user-nav">
            <span></span>
            <router-link to="/user/token">Send Tokens</router-link> 
            <router-link to="/user/cashout">Cash Out List</router-link>
            <router-link to="/user/suspend">Suspend User</router-link>
        </nav> 
        <div class="user-content">
            <router-view></router-view>
        </div>
    </div>
</template>

<script>
export default {
    
}
</script>



<style lang="scss" scoped>
    .user-nav{
        display:flex;
        justify-content: center;
        line-height: 40px;
        background-color: #b6b6b6;
        a{
            margin:2px;
            background-color: #9d9d9d;
            text-decoration: none;
            font-size: 15px;
            border-radius: 4px;
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
    }
</style>
