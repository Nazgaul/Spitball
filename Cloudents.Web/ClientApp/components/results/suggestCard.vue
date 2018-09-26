<template>
    <button type="button" class="suggest" @click="$ga.event('Suggest_Window',$route.path.slice(1),name)">
        <v-container>
            <v-layout class="wrapper hidden-md-and-down" row justify-space-between >
                    <div class="logo" v-language:inner>suggestCard_Spitball</div>
                    <div class="action" v-language:inner>suggestCard_Show_me</div>
            </v-layout>
            
            <v-layout class="icon hidden-lg-and-up">
                <logo></logo>
            </v-layout>
            <div class="text">
                {{suggestList[name]}}
                <!--<img src="./img/smily.png"
                     srcset="./img/smily@2x.png 2x,
                                ./img/smily@3x.png 3x"
                    >-->
            </div>
        </v-container>
    </button>
</template>
<script>
    import logo from "./svg/sb-logo.svg";
    import { suggestList } from './consts';
    export default {
        components: { logo },
        data(){
            return{suggestList}
            },
        props: {
            name: { type: String }
        }
    }
</script>
<style src="./suggestCard.less" lang="less" scoped></style>