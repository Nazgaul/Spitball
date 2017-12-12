<template>
     <v-dialog ref="person" v-model="showDialog" max-width="518" :fullscreen="isSearch" :content-class="isSearch?'dialog-choose':''" :overlay="!isSearch">
        <v-card v-show="!isSearch">
            <v-card-title class="">Personalize Results</v-card-title>
            <v-card-text>You can tailor the results to you by adding your school and classes.</v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn flat="flat" @click.native="showDialog = false">Not now</v-btn>
                <v-btn flat="flat" @click.native="$_personalize">Personalize</v-btn>
            </v-card-actions>
        </v-card>
         <search-item v-model="showDialog" v-show="isSearch" :type="type" :keep="keep" :isFirst="isfirst"></search-item>
    </v-dialog>
</template>
<style lang="less" scoped>
    @import "../../mixin.less";
    .card{
        border-radius:6px;
    } 
    .card__title {
        font-size: 36px;
        font-weight: 300;
        letter-spacing: -0.5px;
        text-align: center;
        color: @Spitball-Text;
        display: block; //override
    }
    .card__text {
        font-size: 16px;
        letter-spacing: -0.3px;
        text-align: center;
        max-width: 360px;
        margin: 0 auto;
        line-height: 1.38;
        color: @Feed-Like-count;
    }
    .btn {
        text-transform: capitalize;
        font-size: 16px;
        letter-spacing: -0.3px;
        color: @Spitball-Main-color;
    }
    .overlay {
        &:before

    {
        background-color: rgba(0,0,0,.6);
    }
    /*&:after {
        filter: blur(20px);
    }*/
    }
</style>
<script>
    import searchItem from './searchItem.vue'
    import { mapGetters,mapActions } from 'vuex'
    export default {
        components:{searchItem},
        data() {
            return { showDialog: false,isSearch:false,type:"",keep:true,isfirst:false}
        },

        computed:{
            ...mapGetters(['isFirst'])
        },

        watch:{
            showDialog(val){
                !val&&this.isfirst?this.isfirst=false:"";
            }
        },
        created(){
            if(this.isFirst){
                this.isfirst=true;
                this.updateFirstTime("isFirst");
                this.showDialog=true;
            }
        },
        methods: {
            ...mapActions(["updateFirstTime"]),
            $_personalize() {
                this.type="university";
                this.isSearch=true;
            }
        },
    }
</script>