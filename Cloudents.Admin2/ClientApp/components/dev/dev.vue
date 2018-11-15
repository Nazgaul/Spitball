<template>
    <div>
            <div class="dev-content">
                <form v-if="!isDev" @submit.prevent="tryYourLuck()" class="password-container" >
                <h1>Developer Access Required to enter this Area</h1>
                <!-- <div class="dev-error" v-if="wrongPass">Wrong Password</div> -->
                <input v-model="password" class="user-pass-input" type="password" placeholder="Password...">
            </form>
            <div v-else>
                <nav class="dev-nav">
                    <router-link to="/dev/change-country">Change User Country</router-link>
                    <router-link to="/dev/delete-user">Delete User</router-link>
                </nav> 
                <router-view></router-view>
            </div>
        </div>
    </div>
</template>

<script>
import {mapActions, mapGetters} from 'vuex';

export default {
    data(){
        return{
            password: "",
            wrongPass: false
        }
    },
    computed:{
        ...mapGetters(['devStore_getIsDev']),
        isDev(){
            return this.devStore_getIsDev;
        }

    },
    methods:{
        ...mapActions(['devStore_updateIsDev']),
        tryYourLuck(){
            this.devStore_updateIsDev(this.password);
        }
    }
}
</script>

<style lang="scss" scoped>
    .dev-nav{
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


    .dev-content{
        padding: 5px;
    }
    .password-container{
        .user-pass-input{
            border: none;
            outline: none;
            border-radius: 25px;
            height: 15px;
            margin-top: 5px;
            padding: 10px;
            width: 200px;
        }
        .dev-error{
            margin-top:5px;
            font-weight: 600;
            color:#ca5d5d;
        }
    }
</style>
