<template>
   <v-flex class="right-sidebar">
        <v-flex xs12 v-if="isAsk">
            <router-link class="ask-question"  :to="{path:'/newquestion/'}">Ask Your Question</router-link>
        </v-flex>

        <v-flex xs12 class="card-block">
            <div class="header">Spitball FAQ</div>
            <div class="content">
                <ul class="list">                               
                    <li v-for="item in faqList"><a href="javascript:void(0)">{{item.text}}</a></li>
                </ul>

            </div>
            <div class="footer">
                <router-link tag="button" to="/faq">More</router-link>
            </div>                         
        </v-flex>

        <v-flex xs12 class="card-block">
            <div class="header">Spitball</div>
            <div class="content">
                <p>{{suggestList[name]}}</p>
            </div>
            <div class="footer">
                <router-link tag="button" :to="{path:'/'+name,query:{q:text}}">Show me</router-link>
            </div>                        
        </v-flex>
        
    </v-flex>
</template>


<script>
    import {faqList,suggestList} from "./../../consts"
    export default {
        data() {
            return {
                faqList,suggestList
            }
        },
        props:{name:{},text:{},isAsk:Boolean}
     }
</script>

<style src="./faq-block.less" lang="less"></style>