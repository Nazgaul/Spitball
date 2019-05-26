<template>
    <div class="item-wrap" data-app>
   
        <v-card v-for="(conversation, index) in conversations" :key="index">
            <div class="conversation-toolbar mt-4 back-color-purple" :style="{cursor: 'pointer'}" @click="openItem(conversation)">
                <div class="conversation-text-title">
                    UserName: {{conversation.userName}} &nbsp;&nbsp; TutorName: {{conversation.tutorName}}
                 
                </div>
               <div class="conversation-text-title">LastMessage: {{conversation.lastMessage.toLocaleString()}}</div>
            </div>      
        </v-card>
    </div>
</template>

<script>
    
   
    export default {
        name: "conversationIitem",
        data () {
      return {
   
      }
    },
        props: {
            conversations: {
                type: Array,
                required: false
            },
            
        },

         methods: {
            openItem(item) {
                
                this.$router.push({ path: `/conversation/conversationDetail/${item.id}` })
            }
        },
    }


</script>

<style  lang="scss">
    .item-wrap{
        .conversation-toolbar{
            max-width: 100%;
            background-color: transparent!important;
            box-shadow: none;
            border-bottom: 1px solid grey;
        }
        .v-card{
            max-width: 100%;
        }
    }
    .conversation-text-title{
        font-size: 15px;
        text-align: left;
    }
   

</style>