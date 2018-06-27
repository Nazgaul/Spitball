<template>
    <v-flex class="right-sidebar">
        <v-flex xs12 v-if="isAsk">
            <router-link class="ask-question" :to="{path:'/newquestion/'}">Ask Your Question</router-link>
        </v-flex>

        <v-flex xs12 class="card-block">
            <div class="header">Spitball FAQ</div>
            <div class="content">
                <ul class="list">
                    <li v-for="(item,id) in faqList" v-if="id<5">
                        <router-link :to="{name:'faqSelect',params:{id:id}}" target='_blank'>{{item.question}}</router-link>
                    </li>
                </ul>

            </div>
            <div class="footer">
                <router-link tag="button" to="/faq">More</router-link>
            </div>
        </v-flex>

        <v-flex xs12 class="card-block mt-3">
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
    import {suggestList} from "./../../consts"
    import help from "../../../../services/satelliteService";

    export default {
        data() {
            return {
                faqList: null,
                suggestList
            }
        },
        props: {name: {}, text: {}, isAsk: Boolean},

        created() {
            var self = this;
            help.getFaq().then(function (response) {
                self.faqList = response.data;
            })
        }
    }
</script>

<style src="./faq-block.less" lang="less"></style>