<template>
   <v-flex class="right-sidebar">
        <v-flex xs12>
            <router-link class="ask-question"  :to="{path:'/askquestion/'}">Ask Your Question</router-link>
        </v-flex>

        <v-flex xs12 class="card-block">
            <div class="header">Spitball FAQ</div>
            <div class="content">
                <ul class="list">                               
                    <li v-for="item in faqList"><a href="javascript:void(0)">{{item.text}}</a></li>
                </ul>

            </div>
            <div class="footer">
                <button>More</button>
            </div>                         
        </v-flex>

        <v-flex xs12 class="card-block">
            <div class="header">Spitball</div>
            <div class="content">
                <p>I found some sweet quizzes and flashcards for you. You can thank me later</p>
            </div>
            <div class="footer">
                <button>Show me</button>
            </div>                        
        </v-flex>
        
    </v-flex>
</template>


<script>
    import {faqList} from "./../../consts"
    export default {
        data() {
            return {
                faqList               
            }
        }
     }
</script>

<style src="./faq-block.less" lang="less"></style>