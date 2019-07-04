<template>
    <div class="congrats">
        <div class="top">
            <p v-language:inner="'loginRegister_congrats_title'"/>
            <span v-language:inner="'loginRegister_congrats_subtitle'"/>
        </div>
        <img src="../images/group-4.png" alt="">
        <div class="bottom" v-language:inner="'loginRegister_congrats_bottom'"/>
        <v-btn @click="finish" color="#304FFE" large round class="white--text" v-language:inner="'loginRegister_congrats_btn'"/>
    </div>
</template>

<script>
import { mapActions } from 'vuex';
export default {
    name: 'congrats',
    methods: {
        ...mapActions(['finishRegister']),
        finish(){
            this.finishRegister()
        }
    },
}
</script>


<style lang="less">
@import '../../../styles/mixin.less';
.congrats{
    @text-color: #434c5f;
    display: flex;
    flex-direction: column;
    align-items: center;
    .top{
        font-size: 28px;
        font-weight: 600;
        line-height: 1.54;
        text-align: center;
        color: @text-color;
        margin-bottom: 32px;
        p{
            letter-spacing: -0.51px;
            margin: 0 0 5px;
        }
    }
    img{
        width: 336px;
        height: auto;
    }
    .bottom{
        margin: 34px 0 56px;
        font-size: 20px;
        line-height: 1.5;
        letter-spacing: -0.36px;
        text-align: center;
        color: @text-color;
    }
    button{
        margin: 0;
        .responsive-property(width, 100%, null, 90%);
        font-size: 16px;
        font-weight: 600;
        letter-spacing: -0.42px;
        text-align: center;
        text-transform: none !important;
    }
}
</style>

