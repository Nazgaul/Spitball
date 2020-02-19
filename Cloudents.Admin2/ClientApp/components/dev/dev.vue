<template>
    <div>
            <div class="dev-content">
                <form v-if="!isDev" @submit.prevent="tryYourLuck()" class="password-container" >
                <h1>Developer Access Required to enter this Area</h1>
                <!-- <div class="dev-error" v-if="wrongPass">Wrong Password</div> -->
                    <v-text-field   solo v-model="password" class="user-pass-input" type="password" placeholder="Password..."/>
            </form>
            <div v-else>
                <v-tabs centered  class="dev-nav"  color="transparent">
                    <v-tabs-slider color="#3532d5"></v-tabs-slider>

                    <v-tab to="/dev/change-country">Change User Country</v-tab>
                    <v-tab to="/dev/delete-user">Delete User</v-tab>
                </v-tabs>
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

<style lang="less" scoped>
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
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        .user-pass-input{
            border: none;
            outline: none;
            border-radius: 25px;
            height: 15px;
            margin-top: 5px;
            padding: 10px;
            width: 345px;
        }
        .dev-error{
            margin-top:5px;
            font-weight: 600;
            color:#ca5d5d;
        }
    }
</style>
